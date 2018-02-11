#include <Arduino.h>
#include <OneWire.h>
#include <DallasTemperature.h>
#include <SoftwareSerial.h>
 
// Data wire is plugged into pin 2 on the Arduino
#define ONE_WIRE_BUS 2

// Setup a oneWire instance to communicate with any OneWire devices 
// (not just Maxim/Dallas temperature ICs)
OneWire oneWire(ONE_WIRE_BUS);
 
// Pass our oneWire reference to Dallas Temperature.
DallasTemperature sensors(&oneWire);
 

 /*
Measuring Current Using ACS712
*/
const int legOneIn = A0;
const int legTwoIn = A1;

int mVperAmp = 66; // use 66 for 30A, use 100 for 20A Module and 185 for 5A Module
int ACSoffset = 2500; 
double LegOneAmps = 0;
double LegTwoAmps = 0;
double WattMinutes = 0;
int MinutesTracked = -1;
double ThisMinuteWatts = 0;
double ThisMinuteMinutes = 0;
bool InWattMinute = false;

//Relays
const int RelaysOut = 8;

//Our variables
double CurrentTemp = 0; //Default to turn it on!
double TargetTemp = 150; //Water heater max (default to water heater setting)
double LowerLimit = 145; //Water shoudl maintain between lower limit and target. When lower limit is reached, it will turn on.

bool PowerOn = false; //Turn on relay's in setup

//Misc
unsigned long LastTempTicks = 0;
unsigned long LastWattsTicks = 0;

//For debugging we use pins 10 and 11 instead of the serial port
bool IsDebug = true;

SoftwareSerial mySerial(10, 11); // RX, TX

//methods
void initServerDebug();
void handleComm();
void initServer();
bool waitFor(String goodResponse, String badResponse);
void doTemperature(void);
float getVPP(int sensorPin);
void doPower(void);
void setRelay(bool turnOn);
void sendMsg(String message);
String buildReport();

void setup(void)
{
    //setup pins
    pinMode(RelaysOut, OUTPUT);
    pinMode(LED_BUILTIN, OUTPUT);
    
    //init relay's to off
    setRelay(false);

    // Start up the library
    sensors.begin();

    if (IsDebug)
        initServerDebug();
    else
        initServer();
}
 
void loop(void)
{
    //sample power every 10 seconds
    if (millis() >= LastWattsTicks + 10000){
        doPower();
    }

    //Take temp every 30 seconds
    if (millis() >= LastTempTicks + 30000){
        doTemperature();
        LastTempTicks = millis();
    }

    handleComm();
}

void handleComm(){

    bool avail = IsDebug ? mySerial.available() : Serial.available();
    String cmd;

    if (!avail) return;

    cmd = IsDebug ? mySerial.readString() : Serial.readString();

    int start = cmd.indexOf(":GO");

    if (start >= 0){
        sendMsg(buildReport());
        if (IsDebug) Serial.write("REPORT SENT");
    }

    start = cmd.indexOf(":SET");

    if (start >= 0){

        String newTemp = cmd.substring(start+5,start+8);

        TargetTemp = newTemp.toDouble();
        LowerLimit = TargetTemp - 1;

        sendMsg(buildReport());
        if (IsDebug) Serial.write("TEMP SET");
    }

    start = cmd.indexOf(":ZERO");

    if (start >= 0){
        LastTempTicks = 0;
        LastWattsTicks = 0;
        WattMinutes = 0;
        MinutesTracked = -1;
        ThisMinuteWatts = 0;
        ThisMinuteMinutes = 0;
        InWattMinute = false;

        sendMsg(buildReport());
        if (IsDebug) Serial.write("RESET STATS");
    }
}

void sendMsg(String message){

    String cmd = "AT+CIPSEND=0,";
    cmd += message.length();
    cmd += "\r\n";

    const char * cmdBuff = cmd.c_str();
    const char * msgBuff = message.c_str();

    if (IsDebug){
        mySerial.write(cmdBuff);
        waitFor("OK", "ERROR");
        mySerial.write(msgBuff);
    } else{
        Serial.write(cmdBuff);
        waitFor("OK", "ERROR");
        Serial.write(msgBuff);
    }
}

bool waitFor(String goodResponse, String badResponse){
    bool found = false;
    bool ret = false;
    String buff = "";

    if (IsDebug){
        while (!found){
            if (mySerial.available()) {
                buff += mySerial.readString();
            
                if (buff.indexOf(goodResponse) >= 0) {
                    found = true;
                    ret = true;
                }
                if (buff.indexOf(badResponse) >= 0) {
                    found = true;
                    ret = false;
                }

                const char * msgBuff = buff.c_str();
                Serial.write(msgBuff);
            }
        }
    }
    else {
        while (!found){
            if (Serial.available()) {
                buff += Serial.readString();
            
                if (buff.indexOf(goodResponse) >= 0) {
                    found = true;
                    ret = true;
                }
                if (buff.indexOf(badResponse) >= 0) {
                    found = true;
                    ret = false;
                }
            }
        }
    }

    return ret;
}

void doTemperature(void){

    // call sensors.requestTemperatures() to issue a global temperature
    // request to all devices on the bus
    sensors.requestTemperatures(); // Send the command to get temperatures

    CurrentTemp = sensors.getTempFByIndex(0);// Why "byIndex"? 
        // You can have more than one IC on the same bus. 
        // 0 refers to the first IC on the wire

    if (CurrentTemp < LowerLimit && !PowerOn){
        setRelay(true);
    }
    if (CurrentTemp >= TargetTemp && PowerOn){
        setRelay(false);
    }
}

void doPower(void){
    double legOneVoltage;
    double legTwoVoltage;
    double legOneVoltageRMS;
    double legTwoVoltageRMS;
    double minutes;
    int watts;

    if (PowerOn){
        legOneVoltage = getVPP(legOneIn);
        legOneVoltageRMS = (legOneVoltage/2.0) *0.707;
        LegOneAmps = ((legOneVoltageRMS * 1000) / mVperAmp);

        legTwoVoltage = getVPP(legTwoIn);
        legTwoVoltageRMS = (legTwoVoltage/2.0) *0.707;
        LegTwoAmps = ((legTwoVoltageRMS * 1000) / mVperAmp);

        minutes = (millis() - LastWattsTicks) / 60000.0;
        LastWattsTicks = millis();

        //Watts = Amps * Volts
        watts = (LegOneAmps * 120) + (LegTwoAmps * 120);
    } else{
        LegOneAmps = 0;
        LegTwoAmps = 0;
        minutes = (millis() - LastWattsTicks) / 60000.0;
        LastWattsTicks = millis();
        watts = 0;
    }


    if (InWattMinute){
        ThisMinuteMinutes += minutes;
        ThisMinuteWatts += watts * minutes;
        if (ThisMinuteMinutes >= 1)
            InWattMinute = false;
    } else{
        WattMinutes += ThisMinuteWatts;
        MinutesTracked++;
        InWattMinute = true;
        ThisMinuteMinutes = minutes;
        ThisMinuteWatts = watts * minutes;
    }
    /*
    String out = "A1: ";
    out += LegOneAmps;
    out += "A2: ";
    out += LegTwoAmps;
    out += "\n";

    Serial.write(out.c_str());
    */
}

float getVPP(int sensorPin)
{
    float result;

    int readValue;             //value read from the sensor
    int maxValue = 0;          // store max value here
    int minValue = 1024;          // store min value here

    uint32_t start_time = millis();
    while((millis()-start_time) < 1000) //sample for 1 Sec
    {
        readValue = analogRead(sensorPin);
        // see if you have a new maxValue
        if (readValue > maxValue) 
        {
            /*record the maximum sensor value*/
            maxValue = readValue;
        }
        if (readValue < minValue) 
        {
            /*record the maximum sensor value*/
            minValue = readValue;
        }
    }

    // Subtract min from max
    result = ((maxValue - minValue) * 5.0)/1024.0;
        
    return result;
}
void setRelay(bool turnOn){
    if (turnOn) {
        digitalWrite(RelaysOut, HIGH);
        PowerOn = true;
    }
    else {
        digitalWrite(RelaysOut, LOW);
        PowerOn = false;
    }
}

void initServerDebug(){
    digitalWrite(LED_BUILTIN, LOW);
    
    // start serial port
    Serial.begin(9600);  
  
    while (!Serial) {
        ; // wait for serial port to connect. Needed for native USB port only
    }

    Serial.println("Dallas Temperature IC Control Library Demo");
    // set the data rate for the SoftwareSerial port
    mySerial.begin(115200);
    mySerial.println("AT+CIOBAUD=9600");

    delay(2000);

    mySerial.end();

    mySerial.begin(9600);
    
    mySerial.println("AT+CIPMUX=1");
    waitFor("OK", "ERROR");

    mySerial.println("AT+CIPSERVER=1,23");
    waitFor("OK", "ERROR");

    digitalWrite(LED_BUILTIN, HIGH);
}

void initServer(){
    digitalWrite(LED_BUILTIN, LOW);
    
    // set the data rate for the SoftwareSerial port
    Serial.begin(115200);
    Serial.println("AT+CIOBAUD=9600");

    delay(2000);

    Serial.end();

    Serial.begin(9600);
    
    Serial.println("AT+CIPMUX=1");
    waitFor("OK", "ERROR");

    Serial.println("AT+CIPSERVER=1,23");
    waitFor("OK", "ERROR");

    digitalWrite(LED_BUILTIN, HIGH);
}

String buildReport(){
    String ret = "{\"Current\": ";
    ret += CurrentTemp;
    ret += ", \"Target\": ";
    ret += TargetTemp;
    ret += ", \"Lower\": ";
    ret += LowerLimit;
    ret += ", \"TrackedMinutes\": ";
    ret += MinutesTracked;
    ret += ", \"Leg1Amps\": ";
    ret += LegOneAmps;
    ret += ", \"WattMinutes\": ";
    ret += WattMinutes;
    ret += ", \"Leg2Amps\": ";
    ret += LegTwoAmps;
    ret += "}";

    return ret;
}

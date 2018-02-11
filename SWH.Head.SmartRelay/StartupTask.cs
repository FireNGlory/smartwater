using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using SWH.Api.Contracts;
using SWH.SmartWaterRelay;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace SWH.Head.SmartRelay
{
    public sealed class StartupTask : IBackgroundTask
    {
        private CancellationTokenSource _tSource;
        private DisplayI2C _display;
        private SmartSensorManager _commMgr;
        private int? _attemptingUpdate;
        private bool _attemptingZero;
        private SmartWaterApi _api;

        private const string I2C_CONTROLLER_NAME = "I2C1"; //use for RPI2
        private const byte DEVICE_I2C_ADDRESS = 0x27; // 7-bit I2C address of the port expander
        private const byte EN = 0x02;
        private const byte RW = 0x01;
        private const byte RS = 0x00;
        private const byte D4 = 0x04;
        private const byte D5 = 0x05;
        private const byte D6 = 0x06;
        private const byte D7 = 0x07;
        private const byte BL = 0x03;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _tSource = new CancellationTokenSource();

            var deferral = taskInstance.GetDeferral();

            //BODY GOES HERE
            _display = InitLcd();

            var bus = new BusManager();

            _commMgr = new SmartSensorManager();
            //_api = new SmartWaterApi("http://192.168.25.181:15151");
            _api = new SmartWaterApi("https://smartwater.azurewebsites.net");

            _commMgr.StatusUpdate += CommMgrOnStatusUpdate;

            bus.SetRequest += BusOnSetRequest;
            bus.ZeroRequest += BusOnZeroRequest;

            //END BODY
            while (!_tSource.Token.IsCancellationRequested)
            {
                await _commMgr.GetStatus();
                await Task.Delay(TimeSpan.FromMinutes(1), _tSource.Token);
            }

            //CLOSING DOWN

            deferral.Complete();
        }

        private void CommMgrOnStatusUpdate(object sender, StatusUpdateEventArgs args)
        {
            var newReport = new SmartSensorReport(args.Report.Current, (int)args.Report.Target, args.Report.WattMinutes, args.Report.TrackedMinutes, args.Report.Current < args.Report.Target);

            _display.clrscr();
            _display.gotoxy(0, 0);
            _display.prints($"F: {args.Report.Current} Set: {args.Report.Target}");
            _display.gotoxy(0, 1);
            _display.prints($"A1: {args.Report.Leg1Amps} A2: {args.Report.Leg2Amps}");
            _display.gotoxy(0, 2);
            _display.prints($"{args.Report.WattMinutes}wM over {args.Report.TrackedMinutes}");

            _api.PostSensorReport("accesstoken", newReport).Wait();
        }

        private async Task BusOnSetRequest(object sender, SetRequestEventArgs args)
        {
            if (_attemptingUpdate != null)
            {
                _attemptingUpdate = args.NewTemp;
                return;
            }

            _attemptingUpdate = args.NewTemp;

            var success = await _commMgr.SetTemp(args.NewTemp);

            while (!success)
            {
                _display.gotoxy(0, 3);
                _display.prints("Noncomm...");
                Task.Delay(1000).Wait(_tSource.Token);

                try
                {
                    success = await _commMgr.SetTemp(args.NewTemp);
                }
                catch
                {
                    success = false;
                }
            }
            _attemptingUpdate = null;

            _display.gotoxy(0, 3);
            _display.prints("          ");
        }

        private async Task BusOnZeroRequest(object sender, EventArgs args)
        {
            if (_attemptingZero) return;

            _attemptingZero = true;

            var success = await _commMgr.ZeroStats();

            while (!success)
            {
                _display.gotoxy(0, 3);
                _display.prints("Noncomm...");
                Task.Delay(1000).Wait(_tSource.Token);

                try
                {
                    success = await _commMgr.ZeroStats();
                }
                catch
                {
                    success = false;
                }
            }
            _attemptingZero = false;

            _display.gotoxy(0, 3);
            _display.prints("          ");
        }

        private static DisplayI2C InitLcd()
        {
            // Here is I2C bus and Display itself initialized.
            //
            //  I2C bus is initialized by library constructor. There is also defined PCF8574 pins 
            //  Default `DEVICE_I2C_ADDRESS` is `0x27` (you can change it by A0-2 pins on PCF8574 - for more info please read datasheet)
            //  `I2C_CONTROLLER_NAME` for Raspberry Pi 2 is `"I2C1"`
            //  For Arduino it should be `"I2C5"`, but I did't test it.
            //  Other arguments should be: RS = 0, RW = 1, EN = 2, D4 = 4, D5 = 5, D6 = 6, D7 = 7, BL = 3
            //  But it depends on your PCF8574.
            var lcd = new DisplayI2C(DEVICE_I2C_ADDRESS, I2C_CONTROLLER_NAME, RS, RW, EN, D4, D5, D6, D7, BL);

            //Initialization of HD44780 display do by init method.
            //By arguments you can turnOnDisplay, turnOnCursor, blinkCursor, cursorDirection and textShift (in thius order)
            lcd.init();
            //lcd.createSymbol(new byte[] { 0x00, 0x00, 0x0A, 0x00, 0x11, 0x0E, 0x00, 0x00 }, 0x00);

            // Here is printed string
            lcd.prints("Good morning,");

            // Navigation to second line
            lcd.gotoxy(0, 1);
            
            // Here is printed string
            lcd.prints("gentlemen");

            // Here is printed our new symbol (emoticon)
            //lcd.printSymbol(0x00);

            return lcd;

        }
        
    }
}

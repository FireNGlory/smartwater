namespace SWH.Api.Contracts
{
    public class SmartSensorReport
    {
        public SmartSensorReport(decimal currentTemp, int currentTarget, decimal wattMinutes, int minutesMeasured, bool isOn)
        {
            CurrentTemp = currentTemp;
            CurrentTarget = currentTarget;
            WattMinutes = wattMinutes;
            MinutesMeasured = minutesMeasured;
            IsOn = isOn;
        }

        public decimal CurrentTemp { get; set; }
        public int CurrentTarget { get; set; }
        public decimal WattMinutes { get; set; }
        public int MinutesMeasured { get; set; }
        public bool IsOn { get; set; }
    }
}

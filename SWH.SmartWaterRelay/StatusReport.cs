namespace SWH.SmartWaterRelay
{
    public class StatusReport
    {
        public decimal Current { get; set; }
        public decimal Target { get; set; }
        public decimal Limit { get; set; }
        public decimal Leg1Amps { get; set; }
        public decimal Leg2Amps { get; set; }
        public int TrackedMinutes { get; set; }
        public decimal WattMinutes { get; set; }
    }
}

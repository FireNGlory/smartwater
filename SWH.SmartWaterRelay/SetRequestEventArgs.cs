namespace SWH.SmartWaterRelay
{
    public class SetRequestEventArgs : SmartWaterCommandArgs
    {
        public SetRequestEventArgs(int newTemp, string token) : base(token)
        {
            NewTemp = newTemp;
        }

        public int NewTemp { get; set; }
    }
}
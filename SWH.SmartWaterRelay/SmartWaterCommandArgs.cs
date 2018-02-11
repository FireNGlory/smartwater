using System;

namespace SWH.SmartWaterRelay
{
    public class SmartWaterCommandArgs : EventArgs
    {
        public SmartWaterCommandArgs(string token)
        {
            Token = token;
        }
        public string Token { get; set; }
    }
}
using System;

namespace SWH.SmartWaterRelay
{
    public delegate void StatusUpdateEvent(object sender, StatusUpdateEventArgs args);

    public class StatusUpdateEventArgs : EventArgs
    {
        public StatusUpdateEventArgs(StatusReport report)
        {
            Report = report;
        }

        public StatusReport Report { get; set; }
    }
}
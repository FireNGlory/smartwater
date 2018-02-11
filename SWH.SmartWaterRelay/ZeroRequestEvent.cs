using System;
using System.Threading.Tasks;

namespace SWH.SmartWaterRelay
{
    public delegate Task ZeroRequestEvent(object sender, SmartWaterCommandArgs args);
}
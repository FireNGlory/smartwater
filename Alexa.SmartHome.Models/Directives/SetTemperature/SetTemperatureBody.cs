using Alexa.SmartHome.Models.ValueTypes;

namespace Alexa.SmartHome.Models.Directives.SetTemperature
{
    public class SetTemperatureBody
    {
        public Header Header { get; set; }
        public CommandEndpoint Endpoint { get; set; }
        public TargetSetpointPayload Payload { get; set; }
    }
}
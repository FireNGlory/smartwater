using Alexa.SmartHome.Models.ValueTypes;

namespace Alexa.SmartHome.Models.Directives.SetTemperature
{
    public class SetDeltaBody
    {
        public Header Header { get; set; }
        public CommandEndpoint Endpoint { get; set; }
        public TargetSetpointDeltaPayload Payload { get; set; }
    }
}
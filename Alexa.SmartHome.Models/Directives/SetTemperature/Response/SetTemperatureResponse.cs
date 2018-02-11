using Alexa.SmartHome.Models.ValueTypes;

namespace Alexa.SmartHome.Models.Directives.SetTemperature.Response
{
    public class SetTemperatureResponse
    {
        public EndpointContext Context { get; set; }
        public ResponseEvent Event { get; set; }
    }
    
    public class ResponseEvent
    {
        public Header Header { get; set; }
        public SyncResponseEndpoint Endpoint { get; set; }
        public EmptyObject Payload { get; set; }
    }

}

using Alexa.SmartHome.Models.ValueTypes;

namespace Alexa.SmartHome.Models.Directives.Discover
{
    public class DiscoverEvent
    {
        public Header Header { get; set; }
        public DiscoverEventPayload Payload { get; set; }

    }
}
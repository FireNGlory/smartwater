
using Alexa.SmartHome.Models.ValueTypes;

namespace Alexa.SmartHome.Models.Directives.Discover
{
    public class DiscoverBody
    {
        public Header Header { get; set; }
        public DiscoverScopePayload Payload { get; set; }
    }
}
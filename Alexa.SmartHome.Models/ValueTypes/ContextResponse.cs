

using Alexa.SmartHome.Models.Directives.ReportState;

namespace Alexa.SmartHome.Models.ValueTypes
{
    public class ContextResponse
    {
        public EndpointContext Context { get; set; }
        public ReportStateEvent Event { get; set; }
    }
}
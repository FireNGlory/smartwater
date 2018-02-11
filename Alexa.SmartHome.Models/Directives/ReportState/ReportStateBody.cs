using Alexa.SmartHome.Models.ValueTypes;

namespace Alexa.SmartHome.Models.Directives.ReportState
{
    public class ReportStateBody
    {
        public Header Header { get; set; }
        public DirectiveEndpoint Endpoint { get; set; }
        public EmptyObject Payload { get; set; }
    }
}
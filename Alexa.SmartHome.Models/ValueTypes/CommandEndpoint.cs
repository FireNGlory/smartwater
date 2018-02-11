namespace Alexa.SmartHome.Models.ValueTypes
{
    public class CommandEndpoint
    {
        public string EndpointId { get; set; }
        public Scope Scope { get; set; }
        public EmptyObject Cookie { get; set; }
    }
}
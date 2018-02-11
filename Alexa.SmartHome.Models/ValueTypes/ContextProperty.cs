namespace Alexa.SmartHome.Models.ValueTypes
{
    public class ContextProperty
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public EndpointValue Value { get; set; }
        public string TimeOfSample { get; set; }
        public int UncertaintyInMilliseconds { get; set; }
    }
}
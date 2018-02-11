namespace Alexa.SmartHome.Models.ValueTypes
{
    public class Header
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string MessageId { get; set; }
        public string CorrelationToken { get; set; }
        public string PayloadVersion { get; set; }
    }
}
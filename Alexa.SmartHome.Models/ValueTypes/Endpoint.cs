using System.Collections.Generic;

namespace Alexa.SmartHome.Models.ValueTypes
{
    public class Endpoint
    {
        public string EndpointId { get; set; }
        public string ManufacturerName { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public ICollection<string> DisplayCategories { get; set; }
        public EmptyObject Cookie { get; set; }
        public ICollection<AlexaInterface> Capabilities { get; set; }
    }
}

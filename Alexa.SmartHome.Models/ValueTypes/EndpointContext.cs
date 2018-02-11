using System.Collections.Generic;

namespace Alexa.SmartHome.Models.ValueTypes
{
    public class EndpointContext
    {
        public ICollection<ContextProperty> Properties { get; set; }
    }
}
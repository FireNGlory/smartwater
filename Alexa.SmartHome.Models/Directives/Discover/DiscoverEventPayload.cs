using System.Collections.Generic;
using Alexa.SmartHome.Models.ValueTypes;

namespace Alexa.SmartHome.Models.Directives.Discover
{
    public class DiscoverEventPayload
    {
        public ICollection<Endpoint> Endpoints { get; set; }
    }
}
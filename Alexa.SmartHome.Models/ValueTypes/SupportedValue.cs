
namespace Alexa.SmartHome.Models.ValueTypes
{
    public class SupportedValue
    {
        public SupportedValue()
        {
            
        }

        public SupportedValue(string name)
        {
            Name = name;
        }
        //[JsonProperty("name")]
        public string Name { get; set; }
    }
}
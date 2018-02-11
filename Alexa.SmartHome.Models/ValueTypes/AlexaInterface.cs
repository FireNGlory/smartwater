
namespace Alexa.SmartHome.Models.ValueTypes
{
    public class AlexaInterface
    {
        public string Type => "AlexaInterface";
        public string Interface { get; set; }
        public string Version { get; set; }
        public InterfacePropery Properties { get; set; }
        public bool ProactivelyReported { get; set; }
        public bool Retrievable { get; set; }
    }
}
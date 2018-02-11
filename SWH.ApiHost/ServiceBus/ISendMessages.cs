using System.Threading.Tasks;

namespace SWH.ApiHost.ServiceBus
{
    public interface ISendMessages
    {
        Task SetTemp(int newTemp, string accessToken);
        Task TurnOn(string accessToken);
        Task TurnOff(string accessToken);
        Task ResetStats(string accessToken);
    }
}

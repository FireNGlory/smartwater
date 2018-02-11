using System.Threading.Tasks;

namespace SWH.Api.Contracts
{
    public interface ISmartWaterApi
    {
        Task PostSensorReport(string token, SmartSensorReport report);
    }
}

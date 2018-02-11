using System.Threading.Tasks;
using SWH.Api.Contracts;

namespace SWH.ApiHost.Code
{
    public interface ISmartSensorMaster
    {
        //Reporting
        Task<decimal> GetCurrentTemp();
        Task<int> GetTempTarget();
        Task<int> GetTodaysUsage();
        Task<bool> IsPowerOn();

        //Commands
        Task<bool> SetOnOff(bool turnOn);
        Task<bool> SetNewTemp(int newTarget);

        //Incoming
        Task LogReport(SmartSensorReport report);
    }
}
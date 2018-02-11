using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SWH.Api.Contracts;
using SWH.ApiHost.ServiceBus;

namespace SWH.ApiHost.Code
{
    public class SmartSensorMaster : ISmartSensorMaster
    {
        public DateTime LastReading { get; private set; }

        private readonly ISendMessages _messenger;
        private readonly ILogger<SmartSensorMaster> _logger;
        private SmartSensorReport _lastReport;

        public SmartSensorMaster(ISendMessages messenger, ILogger<SmartSensorMaster> logger)
        {
            _messenger = messenger;
            _logger = logger;
        }
        public Task<decimal> GetCurrentTemp()
        {
            return Task.FromResult(_lastReport.CurrentTemp);
        }

        public Task<int> GetTempTarget()
        {
            return Task.FromResult(_lastReport.CurrentTarget);
        }

        public Task<int> GetTodaysUsage()
        {
            return Task.FromResult((int)_lastReport.WattMinutes / 60000);
        }

        public Task<bool> IsPowerOn()
        {
            return Task.FromResult(_lastReport.IsOn);
        }

        public async Task<bool> SetOnOff(bool turnOn)
        {
            try
            {
                if (turnOn)
                    await _messenger.TurnOn("accessToken");
                else
                    await _messenger.TurnOff("accessToken");

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
        }

        public async Task<bool> SetNewTemp(int newTarget)
        {
            try
            {
                await _messenger.SetTemp(newTarget, "accessToken");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }

        }

        public Task LogReport(SmartSensorReport report)
        {
            _lastReport = report;
            LastReading = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }
}

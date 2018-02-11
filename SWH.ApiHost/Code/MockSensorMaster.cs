using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SWH.Api.Contracts;

namespace SWH.ApiHost.Code
{
    public class MockSensorMaster : ISmartSensorMaster
    {
        public DateTime LastReading { get; private set; }
        
        private readonly ILogger<SmartSensorMaster> _logger;
        private SmartSensorReport _lastReport;

        public MockSensorMaster(ILogger<SmartSensorMaster> logger)
        {
            _logger = logger;

            _lastReport = new SmartSensorReport(105, 120, 120.54m, 60, true);
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

        public Task<bool> SetOnOff(bool turnOn)
        {
            try
            {
                _lastReport.IsOn = turnOn;
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Task.FromResult(false);
            }
        }

        public Task<bool> SetNewTemp(int newTarget)
        {
            try
            {
                _lastReport.CurrentTarget = newTarget;
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Task.FromResult(false);
            }

        }

        public Task LogReport(SmartSensorReport report)
        {
            return Task.CompletedTask;
        }
    }
}
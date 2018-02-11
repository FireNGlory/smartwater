using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace SWH.ApiHost.ServiceBus
{
    public class ServiceBusMessenger : ISendMessages
    {
        private readonly ILogger<ServiceBusMessenger> _logger;
        private const string ServiceBusConnectionString = "Your Connection String Here";

        private const string QueueName = "commands";

        private static IQueueClient _queueClient;

        public ServiceBusMessenger(ILogger<ServiceBusMessenger> logger)
        {
            _logger = logger;
            _queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
        }

        public async Task SetTemp(int newTemp, string accessToken)
        {
            _logger.LogTrace($"Sending command to set temp to {newTemp}");
            try
            {
                var message = new Message(Encoding.UTF8.GetBytes(newTemp.ToString("00#") + accessToken)) {ContentType = "SET"};

                // Send the message to the queue
                await _queueClient.SendAsync(message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }

        public async Task TurnOn(string accessToken)
        {
            _logger.LogTrace("Sending ON command");
            try
            {
                var message = new Message(Encoding.UTF8.GetBytes(accessToken)) {ContentType = "ON"};

                // Send the message to the queue
                await _queueClient.SendAsync(message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }

        public async Task TurnOff(string accessToken)
        {
            _logger.LogTrace("Sending OFF command");
            try
            {
                var message = new Message(Encoding.UTF8.GetBytes(accessToken)) {ContentType = "OFF"};

                // Send the message to the queue
                await _queueClient.SendAsync(message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }

        public async Task ResetStats(string accessToken)
        {
            _logger.LogTrace("Sending reset");
            try
            {
                var message = new Message(Encoding.UTF8.GetBytes(accessToken)) { ContentType = "ZERO" };

                // Send the message to the queue
                await _queueClient.SendAsync(message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }
    }
}
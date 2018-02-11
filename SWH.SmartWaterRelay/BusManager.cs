using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace SWH.SmartWaterRelay
{
    public class BusManager
    {
        public event SetRequestEvent SetRequest;
        public event ZeroRequestEvent ZeroRequest;

        private const string ServiceBusConnectionString = "Your Connection string here";

        private const string QueueName = "commands";

        private static IQueueClient _queueClient;

        public BusManager()
        {
            _queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            // Register QueueClient's MessageHandler and receive messages in a loop
            RegisterOnMessageHandlerAndReceiveMessages();
        }

        public async Task CloseConnection()
        {
            await _queueClient.CloseAsync();
        }

        protected virtual void OnSetRequest(int newTemp, string token)
        {
            SetRequest?.Invoke(this, new SetRequestEventArgs(newTemp, token));
        }

        async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message
            var msgBody = Encoding.UTF8.GetString(message.Body);

            var accessToken = msgBody;
            switch (message.ContentType)
            {
                case "SET":
                    var newTemp = msgBody.Substring(0, 3);
                    accessToken = msgBody.Substring(3);
                    OnSetRequest(int.Parse(newTemp), accessToken);
                    break;
                case "ZERO":
                    OnZeroRequest(accessToken);
                    break;
                case "ON":
                    OnSetRequest(999, accessToken);
                    break;
                case "OFF":
                    OnSetRequest(32, accessToken);
                    break;
                default:
                    //TODO: better handling
                    throw new ArgumentOutOfRangeException($"Unhandled message type {message.ContentType}");
            }

            // Complete the message so that it is not received again.
            // This can be done only if the queueClient is opened in ReceiveMode.PeekLock mode (which is default).
            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        // Use this Handler to look at the exceptions received on the MessagePump
        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            //TODO: Add some exceptioon handling
            return Task.CompletedTask;
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False value below indicates the Complete will be handled by the User Callback as seen in `ProcessMessagesAsync`.
                AutoComplete = false
            };

            // Register the function that will process messages
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }
        /*
                // Sends messages to the queue.
                static async Task SendMessagesAsync(int numberOfMessagesToSend)
                {
                    for (var i = 0; i < numberOfMessagesToSend; i++)
                    {
                        try
                        {
                            // Create a new message to send to the queue
                            string messageBody = $"Message {i}";
                            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                            // Write the body of the message to the console
                            Console.WriteLine($"Sending message: {messageBody}");

                            // Send the message to the queue
                            await queueClient.SendAsync(message);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
                        }
                    }
                }*/
        protected virtual void OnZeroRequest(string token)
        {
            ZeroRequest?.Invoke(this, new SmartWaterCommandArgs(token));
        }
    }
}

using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SbMessageSessionsConsoleApp
{
    public class ServiceBusProcessor : IDisposable
    {
        private const string TopicName = "orders";
        private const string SubscriptionName = "OrderTask";

        private SubscriptionClient subscriptionClient;
        private string serviceBusConnectionString;
        public List<string> MessageList { get; private set; }
        private string filePath;

        public ServiceBusProcessor(string sbConnString, string instanceGuid)
        {
            serviceBusConnectionString = sbConnString;

            var fileDir = filePath = Path.Combine(Environment.CurrentDirectory, "Processed");
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            filePath = Path.Combine(Environment.CurrentDirectory, "Processed", $"{instanceGuid}.txt");
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
        }

        public void Initilaize()
        {
            var sessionHandlerOptions = new SessionHandlerOptions(e => this.LogMessageHandlerException(e))
            {
                MessageWaitTimeout = TimeSpan.FromSeconds(30 * 5),
                MaxConcurrentSessions = 2,
                AutoComplete = false
            };

            subscriptionClient = new SubscriptionClient(serviceBusConnectionString, TopicName, SubscriptionName, ReceiveMode.PeekLock);
            subscriptionClient.RegisterSessionHandler(MessageSessionHandlerAsync, sessionHandlerOptions);
        }

        public async Task MessageSessionHandlerAsync(IMessageSession session, Message message, CancellationToken cancellationToken)
        {
            if (message == null)
            {
                LogMessage("Message is null");

                await session.CompleteAsync(message.SystemProperties.LockToken);
                LogMessages();
                return;
            }

            string messageBody = null;

            if (message.Body != null)
            {
                messageBody = Encoding.UTF8.GetString(message.Body);
            }

            if (string.IsNullOrWhiteSpace(messageBody))
            {
                // If the message was sent using WindowsAzure.ServiceBus Sdk, read it using InteropExtensions
                messageBody = message.GetBody<string>();
            }

            if (string.IsNullOrWhiteSpace(messageBody))
            {
                LogMessage("Message Body is null");
                await session.CompleteAsync(message.SystemProperties.LockToken);

                return;
            }

            OrderTask orderTask;

            try
            {
                LogMessage(messageBody);
                orderTask = JsonConvert.DeserializeObject<OrderTask>(messageBody);

                if (orderTask.Index == "Start")
                {
                    Console.Clear();
                }

                LogMessage($"MessageId - {message.MessageId}; SessionId - {session.SessionId}");
            }
            catch (Exception ex)
            {
                LogMessage($"Invalid Json. {ex.Message}");
                await session.CompleteAsync(message.SystemProperties.LockToken);

                LogMessage($"Message completed with failure due to invalid json");

                return;
            }

            ///TODO - Process Message

            WriteToFile(messageBody);
            await session.CompleteAsync(message.SystemProperties.LockToken);
            LogMessage($"Message completed success");

            var taskStatus = (JobState)Enum.Parse(typeof(JobState), orderTask.JobState);

            if (taskStatus == JobState.COMPLETED || taskStatus == JobState.FAILED)
            {
                if (!session.IsClosedOrClosing)
                {
                    LogMessage($"Message session closed");
                    await session.CloseAsync();
                }
            }
        }

        public void WriteToFile(string contents)
        {
            //File.AppendAllText(filePath, contents);
        }

        private Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            LogMessage($"{nameof(LogMessageHandlerException)} - ClientId: {e.ExceptionReceivedContext.ClientId}; Exception: {e.Exception.Message}");
            return Task.CompletedTask;
        }

        public void LogMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void LogMessages()
        {
            Console.WriteLine("-----------------START-------------------------");
            foreach (var msg in MessageList)
            {
                Console.WriteLine(msg);
            }
            Console.WriteLine("-----------------END-------------------------");
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!subscriptionClient.IsClosedOrClosing)
                    {
                        subscriptionClient.CloseAsync();
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
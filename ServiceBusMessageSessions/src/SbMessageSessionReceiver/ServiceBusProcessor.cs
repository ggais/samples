using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Newtonsoft.Json;

namespace SbMessageSessionsConsoleApp
{
    public class ServiceBusProcessor : IDisposable
    {
        private const string TopicName = "Orders";
        private const string SubscriptionName = "OrderTask";

        private SubscriptionClient subscriptionClient;
        private string serviceBusConnectionString;
        public List<string> MessageList { get; private set; }
        string filePath;

        public ServiceBusProcessor(string sbConnString, string instanceGuid)
        {
            serviceBusConnectionString = sbConnString;
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
                MaxConcurrentSessions = 10,
                AutoComplete = false
            };

            this.subscriptionClient = new SubscriptionClient(serviceBusConnectionString, TopicName, SubscriptionName, ReceiveMode.PeekLock);
            this.subscriptionClient.RegisterSessionHandler(MessageSessionHandlerAsync, sessionHandlerOptions);
        }

        public async Task MessageSessionHandlerAsync(IMessageSession session, Message message, CancellationToken cancellationToken)
        {
            MessageList = new List<string>();

            if (message == null)
            {
                MessageList.Add("Message is null");
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
                messageBody = message.GetBody<string>();
            }

            if (string.IsNullOrWhiteSpace(messageBody))
            {
                MessageList.Add("Message Body is null");
                await session.CompleteAsync(message.SystemProperties.LockToken);
                LogMessages();
                return;
            }

            MessageList.Add($"MessageId - {message.MessageId}; SessionId - {session.SessionId}");

            OrderTask orderTask;

            try
            {
                MessageList.Add(messageBody);
                orderTask = JsonConvert.DeserializeObject<OrderTask>(messageBody);
            }
            catch (Exception ex)
            {
                MessageList.Add($"Invalid Json. {ex.Message}");
                await session.CompleteAsync(message.SystemProperties.LockToken);

                MessageList.Add($"Message completed with failure due to invalid json");
                LogMessages();
                return;
            }

            ///TODO - Process Message
            
            WriteToFile(messageBody);
            await session.CompleteAsync(message.SystemProperties.LockToken);
            MessageList.Add($"Message completed success");

            var taskStatus = (JobState)Enum.Parse(typeof(JobState), orderTask.JobState);

            if (taskStatus == JobState.COMPLETED || taskStatus == JobState.FAILED)
            {
                if (!session.IsClosedOrClosing)
                {
                    MessageList.Add($"Message session closed");
                    await session.CloseAsync();
                }
            }

            LogMessages();
        }

        public void WriteToFile(string contents)
        {
            File.AppendAllText(filePath, contents);
        }

        private Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            MessageList.Add($"{nameof(LogMessageHandlerException)} - ClientId: {e.ExceptionReceivedContext.ClientId}; Exception: {e.Exception.Message}");
            return Task.CompletedTask;
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
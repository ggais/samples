using Microsoft.Azure.ServiceBus;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;

namespace SbMessageSessionsConsoleApp
{
    internal class Program
    {
        private static TopicClient topicClient = null;
        private static string sessionId = null;
        private static bool serviceBusTopicEnabled = true;
        public const string TopicName = "orders";
        public const string SubscriptionName = "OrderTask";

        private static void Main(string[] args)
        {
            SendMessages();

            Console.WriteLine("");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void SetupServiceBusTopic()
        {
            if (!serviceBusTopicEnabled)
            {
                return;
            }
            string connectionString = GetConnectionString();
            topicClient = new TopicClient(connectionString, TopicName);
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.AppSettings["ServiceBusConnectionString"];
        }

        private static void SendMessages()
        {
            SetupServiceBusTopic();

            string taskId = string.Empty;  //ReadStringFromConsole("Enter the TaskId: ");
            int messageStartFrom = 1;  //ReadIntFromConsole("Enter the value for Start Index: ");
            int messageCount = ReadIntFromConsole("Enter the total count of messages to log: ");
            int messageDelayInMilliseconds = ReadIntFromConsole("Enter delay between messages in milliseconds: ");

            Console.WriteLine("Press any key to start processing");
            Console.ReadKey(true);

            char rerun = 'n';
            do
            {
                Console.WriteLine("");
                Console.WriteLine("Messages: Start");
                SendServiceBusMessages(taskId, messageStartFrom, messageCount, messageDelayInMilliseconds);
                Console.WriteLine("Messages: Done");
                Console.WriteLine("Do you want to re-run (y/n)?");
                rerun = Console.ReadKey().KeyChar;
            } while (rerun.ToString().ToLower() == "y");
        }

        private static void SendServiceBusMessages(string taskId, int startCount = 1, int messageCount = 10, double messageTimeSpanInMs = 1)
        {
            sessionId = Guid.NewGuid().ToString();
            Console.WriteLine($"SessionId: {sessionId}");
            var responseMessagePath = Path.Combine(Environment.CurrentDirectory, "Content", "OrderTask.json");
            var responseMessageContent = File.ReadAllText(responseMessagePath);

            SendServiceBusMessage(GetMessage(responseMessageContent, $"Start", taskId, JobState.RUNNING));

            for (int i = startCount; i < startCount + messageCount; i++)
            {
                Console.WriteLine($"Index: {i}");
                SendServiceBusMessage(GetMessage(responseMessageContent, $"{i}", taskId, JobState.RUNNING));

                if (messageTimeSpanInMs > 0)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(messageTimeSpanInMs));
                }
            }

            SendServiceBusMessage(GetMessage(responseMessageContent, "Done", taskId, JobState.COMPLETED));
        }

        private static string GetMessage(string responseMessageContent, string indexText, string taskId, JobState jobState)
        {
            return responseMessageContent.Replace("indexreplace", indexText).Replace("replacetaskid", taskId).Replace("replacejobstate", ((int)jobState).ToString());
        }

        private static void SendServiceBusMessage(string content)
        {
            if (!serviceBusTopicEnabled)
            {
                return;
            }

            var message = new Message(Encoding.UTF8.GetBytes(content));
            message.SessionId = sessionId;
            message.UserProperties["SubscriberName"] = SubscriptionName;

            topicClient.SendAsync(message).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static int ReadIntFromConsole(string message)
        {
            bool isValid = false;
            int value;

            do
            {
                Console.WriteLine(message);
                var sVal = Console.ReadLine();
                isValid = int.TryParse(sVal, out value);

                if (!isValid)
                {
                    Console.WriteLine("Invalid value");
                }
            }
            while (!isValid);

            return value;
        }

        private static string ReadStringFromConsole(string message)
        {
            string value;
            bool isValid;

            do
            {
                Console.WriteLine(message);
                value = Console.ReadLine();
                isValid = !string.IsNullOrWhiteSpace(value);

                if (!isValid)
                {
                    Console.WriteLine("Invalid value");
                }
            }
            while (!isValid);

            return value;
        }
    }
}
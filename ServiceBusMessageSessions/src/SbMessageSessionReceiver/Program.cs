using System;
using System.Configuration;
using System.Threading;

namespace SbMessageSessionsConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var instanceGuid = Guid.NewGuid().ToString();
            Console.WriteLine($"Instance Id: {instanceGuid}");
            SetupServiceBusReceiver(instanceGuid);

            Console.WriteLine("");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void SetupServiceBusReceiver(string instanceGuid)
        {
            Console.WriteLine("Service Bus - Setup");
            var processor = new ServiceBusProcessor(GetConnectionString(), instanceGuid);
            processor.Initilaize();
            KeepRunning();
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.AppSettings["ServiceBusConnectionString"];
        }

        private static void KeepRunning()
        {
            while (1 == 1)
            {
                Thread.Sleep(20000);
            }
        }
    }
}
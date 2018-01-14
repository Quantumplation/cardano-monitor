using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CardanoMonitor
{
    
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length > 1 && (args[1] == "-h" || args[1] == "--help"))
            {
                PrintHelp();
                return;
            }
            Console.WriteLine("Monitoring BTC-ADA price!");
            var configSpecified = args.Length > 1;
            var configFile = configSpecified ? args[1] : "defaults";
            Console.WriteLine($"Loading configuration from {configFile}...");
            var contents = configSpecified ? File.ReadAllText(configFile) : "";
            var options = configSpecified ? JsonConvert.DeserializeObject<ConfigurationOptions>(contents, new MonitorConvert(), new NotifierConvert()) : new ConfigurationOptions();
            Console.WriteLine("Fetching market summaries...");
            var lastRefresh = DateTime.MinValue;
            BittrexAPIResponse<MarketSummary> response = null;
            while (true)
            {
                if (response == null || lastRefresh + options.APIRateLimit < DateTime.Now)
                {
                    lastRefresh = DateTime.Now;
                    response = Bittrex.GetMarketSummaries("USDT-BTC", "USDT-ADA", "BTC-ADA");
                }
                var markets = response.result.ToDictionary(s => s.Key);
                foreach (var monitor in options.Monitors) {
                    var msg = monitor.Check(options.CurrentBalances, markets);
                    if(msg != null)
                    {
                        foreach(var notifier in monitor.Notifications)
                        {
                            options.Notifications[notifier].Notify(monitor, msg);
                        }
                    }
                }
                System.Threading.Thread.Sleep(0);
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("A small tool for continuous monitoring of the ADA market");
            Console.WriteLine("Usage: ada-monitor config.yml");
            Console.WriteLine("       ada-monitor -h");
            Console.WriteLine(" -h   Prints this help message");
        }
    }
}

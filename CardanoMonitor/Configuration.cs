using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardanoMonitor
{
    using MarketKey = Currency;
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Currency
    {
        ADA = 1,
        BTC = 2,
        USDT = 4
    }
    public class ConfigurationOptions
    {
        public TimeSpan APIRateLimit { get; set; } = TimeSpan.FromSeconds(10);
        public Dictionary<Currency, decimal> CurrentBalances { get; set; } = new Dictionary<Currency, decimal>();
        public List<Monitor> Monitors { get; set; } = new List<Monitor>();
        public Dictionary<string, Notifier> Notifications { get; set; } = new Dictionary<string, Notifier>();
    }
}

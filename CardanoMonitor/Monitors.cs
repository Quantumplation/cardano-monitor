using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardanoMonitor
{
    using MarketKey = Currency;
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MonitorType
    {
        Balance,
        Price,
        RelativePrice
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Comparison
    {
        GreaterThan,
        LessThan,
    }
    public abstract class Monitor
    {
        public abstract MonitorType Type { get; }
        public List<string> Notifications { get; set; } = new List<string>();
        public abstract string Check(Dictionary<Currency, decimal> balances, Dictionary<MarketKey, MarketSummary> markets);
    }
    public class BalanceMonitor : Monitor
    {
        public override MonitorType Type => MonitorType.Balance;
        public decimal Threshold { get; set; }
        public Currency SourceCurrency { get; set; }
        public Currency DestinationCurrency { get; set; }
        public Comparison Comparison { get; set; }
        public override string Check(Dictionary<Currency, decimal> balances, Dictionary<MarketKey, MarketSummary> markets)
        {
            var summary = markets[SourceCurrency | DestinationCurrency];
            var conversionRate = summary.Right == SourceCurrency ? summary.Last : (1 / summary.Last);
            balances.TryGetValue(SourceCurrency, out var sourceBalance);
            balances.TryGetValue(DestinationCurrency, out var destinationBalance);
            var convertedBalance = destinationBalance + conversionRate * sourceBalance;
            if (Comparison == Comparison.GreaterThan ? convertedBalance > Threshold : convertedBalance < Threshold)
            {
                var sc = SourceCurrency.ToString();
                var dc = DestinationCurrency.ToString();
                var dir = Comparison == Comparison.GreaterThan ? "greater than" : "less than";
                return $"If you converted {sc} to {dc}, you would have roughly {convertedBalance:G6} {dc}, which is {dir} your threshold of {Threshold} {dc}.";
            }
            return null;
        }
    }
    public class PriceMonitor : Monitor
    {
        public override MonitorType Type => MonitorType.Price;
        public MarketKey Market { get; set; }
        public Currency Currency { get; set; }
        public Comparison Comparison { get; set; }
        public decimal Threshold { get; set; }

        public override string Check(Dictionary<Currency, decimal> balances, Dictionary<MarketKey, MarketSummary> markets)
        {
            var summary = markets[Market];
            var targetLast = summary.Right == Currency ? summary.Last : (1 / summary.Last);
            if (Comparison == Comparison.GreaterThan ? targetLast > Threshold : targetLast < Threshold)
            {
                var curr = Currency.ToString();
                var otherCurr = summary.Right == Currency ? summary.Left : summary.Right;
                var dir = Comparison == Comparison.GreaterThan ? "greater than" : "less than";
                return $"1 {curr} is now worth roughly {targetLast:G6} {otherCurr}, which is {dir} your threshold of {Threshold} {otherCurr}";
            }
            return null;
        }
    }
    public class RelativePriceMonitor : Monitor
    {
        public override MonitorType Type => MonitorType.RelativePrice;
        public MarketKey Market { get; set; }
        public Currency Currency { get; set; }
        public TimeSpan Timespan { get; set; }
        public decimal Percentage { get; set; }
        private SortedList<DateTime, decimal> PriceHistory = new SortedList<DateTime, decimal>();
        public override string Check(Dictionary<Currency, decimal> balances, Dictionary<MarketKey, MarketSummary> markets)
        {
            var summary = markets[Market];
            var targetLast = summary.Right == Currency ? summary.Last : (1 / summary.Last);
            if(PriceHistory.ContainsKey(DateTime.Parse(summary.TimeStamp + "Z"))) { return null; }
            PriceHistory.Add(DateTime.Parse(summary.TimeStamp + "Z"), targetLast);
            var obsoletePrices = PriceHistory.Keys.Where(x => DateTime.Now - x > Timespan).ToList();
            obsoletePrices.ForEach(p => PriceHistory.Remove(p));
            if (PriceHistory.Count == 0) { return null; }
            var oldestPrice = PriceHistory.First();
            var change = Math.Abs(oldestPrice.Value - targetLast);
            var changePercentage = change / oldestPrice.Value;
            if (changePercentage > Percentage)
            {
                var curr = Currency.ToString();
                var otherCurr = summary.Right == Currency ? summary.Left : summary.Right;
                var dir = targetLast < oldestPrice.Value ? "down" : "up";
                var span = DateTime.Now - oldestPrice.Key;
                return $"The price of {curr} relative to {otherCurr} is {dir} roughly {changePercentage * 100:G6} percent since {span.TotalMinutes:G2} minutes ago.";
            }
            return null;
        }
    }

    public class MonitorConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Monitor).IsAssignableFrom(objectType);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var target = Create(jsonObject);
            serializer.Populate(jsonObject.CreateReader(), target);
            return target;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        private Monitor Create(JObject obj)
        {
            switch (obj["Type"].ToString())
            {
                case "Balance": return new BalanceMonitor();
                case "Price": return new PriceMonitor();
                case "RelativePrice": return new RelativePriceMonitor();
                default: throw new Exception("Invalid type");
            }
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoMonitor
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotifierType
    {
        Console,
        Beep,
        Email,
        SMS
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RateLimitType
    {
        Global,
        PerSource
    }

    public abstract class Notifier
    {
        public abstract NotifierType Type { get; }
        public RateLimitType RateLimitType { get; set; } = RateLimitType.PerSource;
        public TimeSpan RateLimit { get; set; }
        private Dictionary<Monitor, DateTime> LastNotifiedBySource { get; set; } = new Dictionary<Monitor, DateTime>();
        private DateTime? LastNotifiedGlobal = null;

        public void Notify(Monitor source, string message)
        {
            DateTime? lastNotified = null;
            if(RateLimitType == RateLimitType.Global)
            {
                lastNotified = LastNotifiedGlobal;
            }
            else if(LastNotifiedBySource.ContainsKey(source))
            {
                lastNotified = LastNotifiedBySource[source];
            }
            if(lastNotified == null || lastNotified + RateLimit < DateTime.Now)
            {
                LastNotifiedGlobal = DateTime.Now;
                LastNotifiedBySource[source] = DateTime.Now;
                InternalNotify(message);
            }
        }
        protected abstract void InternalNotify(string message);
    }

    public class ConsoleNotifier : Notifier
    {
        public override NotifierType Type => NotifierType.Console;
        protected override void InternalNotify(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class BeepNotifier : Notifier
    {
        public override NotifierType Type => NotifierType.Beep;
        protected override void InternalNotify(string message)
        {
            Console.Beep();
        }
    }

    public class EmailNotifier : Notifier
    {
        public override NotifierType Type => NotifierType.Email;
        protected override void InternalNotify(string message)
        {
            // TODO: Implement email notifier
            Console.WriteLine("Emailed: " + message);
        }
    }

    public class SMSNotifier : Notifier
    {
        public override NotifierType Type => NotifierType.SMS;
        protected override void InternalNotify(string message)
        {
            // TODO: Implement SMS notifier
            Console.WriteLine("SMS sent: " + message);
        }
    }

    public class NotifierConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Notifier).IsAssignableFrom(objectType);
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
        private Notifier Create(JObject obj)
        {
            switch (obj["Type"].ToString())
            {
                case "Console": return new ConsoleNotifier();
                case "Beep": return new BeepNotifier();
                case "Email": return new EmailNotifier();
                case "SMS": return new SMSNotifier();
                default: throw new Exception("Invalid type: " + obj["Type"].ToString());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardanoMonitor
{
    using MarketKey = Currency;
    public class Bittrex
    {
        private static RestSharp.RestClient GetClient()
        {
            return new RestSharp.RestClient("https://bittrex.com/api/v1.1/public/");
        }

        public static BittrexAPIResponse<MarketSummary> GetMarketSummary(string marketName)
        {
            var client = GetClient();
            var request = new RestSharp.RestRequest("getmarketsummary", RestSharp.Method.GET);
            request.AddQueryParameter("market", marketName);
            var response = client.Execute<BittrexAPIResponse<MarketSummary>>(request);
            return response.Data;
        }

        public static BittrexAPIResponse<MarketSummary> GetMarketSummaries(params string[] markets)
        {
            var client = GetClient();
            var request = new RestSharp.RestRequest("getmarketsummaries", RestSharp.Method.GET);
            var response = client.Execute<BittrexAPIResponse<MarketSummary>>(request);
            response.Data.result = response.Data.result.Where(ms => markets.Contains(ms.MarketName)).ToList();
            return response.Data;
        }
    }

    public class BittrexAPIResponse<T>
    {
        public BittrexAPIResponse() { }
        public bool success { get; set; }
        public string message { get; set; }
        public List<T> result { get; set; }
    }
    public class MarketSummary
    {
        public MarketSummary() { }
        public string MarketName { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Volumne { get; set; }
        public string TimeStamp { get; set; }
        public decimal Last { get; set; }
        public decimal BaseVolume { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public int OpenBuyOrders { get; set; }
        public int OpenSellOrders { get; set; }
        public decimal PrevDay { get; set; }

        public Currency Left => Enum.Parse<Currency>(MarketName.Split('-')[0]);
        public Currency Right => Enum.Parse<Currency>(MarketName.Split('-')[1]);
        public MarketKey Key => Left | Right;
    }
}

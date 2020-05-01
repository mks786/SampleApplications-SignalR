using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalR.Model
{
    public class Ticker
    {
        public string Symbol { get; set; }
        public decimal Value { get; set; }
        public DateTime SavedOn { get; set; }
    }

    public class Tickers
    {
        public ExchangeTypes Exchange { get; set; }

        public List<Ticker> All { get; set; }

        public decimal? this[string symbol]
        {
            get
            {
                var first = All.FirstOrDefault(x => x.Symbol == symbol);
                return first?.Value;
            }
        }
    }
    public enum ExchangeTypes
    {
        Binance = 0,
        Poloniex = 1
    }
}

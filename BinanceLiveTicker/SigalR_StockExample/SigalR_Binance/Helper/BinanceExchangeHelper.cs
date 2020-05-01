using Binance.Net;
using Microsoft.AspNetCore.SignalR;
using SigalR_Binance.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using SignalR.Model;

namespace SigalR_Binance.Helper
{
    public class BinanceExchangeHelper
    {
        private readonly BinanceSocketClient _client = new BinanceSocketClient();
        private readonly Subject<Ticker> _subject = new Subject<Ticker>();
        private readonly Tickers _ticker = new Tickers();
        private readonly SemaphoreSlim _marketStateLock = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _updateTickerPricesLock = new SemaphoreSlim(1, 1);
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
        private Timer _timer;

        public BinanceExchangeHelper(IHubContext<ExchangeTickerHub> hub)
        {
            Hub = hub;
            LoadDefaultSymbols();
        }
        private IHubContext<ExchangeTickerHub> Hub { get; set; }

        public IEnumerable<Ticker> GetAllTickers()
        {
            Thread.Sleep(5000);
            return _ticker.All;
        }
        public IObservable<Ticker> StreamTicker()
        {
            return _subject;
        }
        private void LoadDefaultSymbols()
        {
            _client.SubscribeToAllSymbolTickerUpdates((data) =>
            {
                var list = data.Select(x => new Ticker() { Value = x.BestAskPrice, SavedOn = DateTime.Now, Symbol = x.Symbol }).ToList();
                _ticker.Exchange = ExchangeTypes.Binance;
                _ticker.All = list;
            });
        }

        public async Task BTicker()
        {
            await _marketStateLock.WaitAsync();
            try
            {
                _timer = new Timer(UpdateTickerPrices, null, _updateInterval, _updateInterval);
            }
            finally
            {
                _marketStateLock.Release();
            }
        }
        public async Task BinanceTicker()
        {
            await _client.SubscribeToAllSymbolTickerUpdatesAsync((data) =>
            {
                var list = data.Select(x => new Ticker() { Value = x.BestAskPrice, SavedOn = DateTime.Now, Symbol = x.Symbol }).ToList();
                _ticker.Exchange = ExchangeTypes.Binance;
                _ticker.All = list;
                Console.WriteLine($"Binance:: Ticker Socket Updated at {DateTime.Now}");
                foreach (var a in list) { Console.Write(a.Symbol.ToString() + " " + a.Value.ToString() + Environment.NewLine); };
            });
        }

        private async void UpdateTickerPrices(object state)
        {
            // This function must be re-entrant as it's running as a timer interval handler
            await _updateTickerPricesLock.WaitAsync();
            try
            {
                await BinanceTicker();
                foreach (var ticker in _ticker.All)
                {
                    _subject.OnNext(ticker);
                }
            }
            finally
            {
                _updateTickerPricesLock.Release();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Binance.Net;
using Microsoft.AspNetCore.SignalR;
using SigalR_Binance.Helper;
using SignalR.Model;

namespace SigalR_Binance.Hubs
{
    public class ExchangeTickerHub : Hub
    {
        private readonly BinanceExchangeHelper _binanceExchangeHelper;

        public ExchangeTickerHub(BinanceExchangeHelper binanceExchangeHelper)
        {
            _binanceExchangeHelper = binanceExchangeHelper;
        }

        public IEnumerable<Ticker> GetAllTicker()
        {
            return _binanceExchangeHelper.GetAllTickers();
        }

        public ChannelReader<Ticker> StreamTicker()
        {
            return _binanceExchangeHelper.StreamTicker().AsChannelReader(10);
        }

        public async Task BinanceTicker()
        {
            await _binanceExchangeHelper.BinanceTicker();
        }

        public async Task BTicker()
        {
            await _binanceExchangeHelper.BTicker();
        }


    }
}

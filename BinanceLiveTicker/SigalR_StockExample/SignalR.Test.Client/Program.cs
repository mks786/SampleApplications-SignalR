using Microsoft.AspNetCore.SignalR.Client;
using SignalR.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.Test.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44382/exchangeTickerHub")
                .Build();

            await connection.StartAsync();

            Console.WriteLine("Starting connection. Press Ctrl-C to close.");
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, a) =>
            {
                a.Cancel = true;
                cts.Cancel();
            };

            connection.Closed += e =>
            {
                Console.WriteLine("Connection closed with error: {0}", e);

                cts.Cancel();
                return Task.CompletedTask;
            };

            connection.On("bTicker", () => { Console.WriteLine("bTicker"); });

            var channel = await connection.StreamAsChannelAsync<Ticker>("StreamTicker", CancellationToken.None);
            while (await channel.WaitToReadAsync() && !cts.IsCancellationRequested)
            {
                while (channel.TryRead(out var stock))
                {
                    Console.WriteLine($"{stock.Symbol} {stock.Value}");
                }
            }
        }
    }
}

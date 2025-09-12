using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using ps_globomantics_signalr.Models;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ps_globomantics_console_client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Delay(5000);
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7241/");
            var response = await httpClient.GetAsync("auctions");
            var actions = await response.Content.ReadFromJsonAsync<List<Auction>>();

            foreach (var auction in actions)
            {
                Console.WriteLine($"Auction: {auction.Id} - {auction.ItemName} - Current Bid: {auction.CurrentBid}");
            }

            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7241/auctionhub", o =>
                {
                    o.Transports = HttpTransportType.WebSockets;
                    o.SkipNegotiation = true;
                })
                .Build();

            connection.On("ReceiveNewBid", (AuctionNotify auctionNotify) => 
            {
                var auction = actions.First(a => a.Id == auctionNotify.AuctionId);
                auction.CurrentBid = auctionNotify.NewBid;
                Console.WriteLine("New bid:");
                Console.WriteLine($"{auction.Id,-3} {auction.ItemName,-20} " + 
                    $"{auction.CurrentBid,10}");
            });

            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                while (true)
                {
                    Console.WriteLine("Auction id?");
                    var id = Console.ReadLine();
                    Console.WriteLine($"New bid for auction {id}?");
                    var bid = Console.ReadLine();
                    await connection.InvokeAsync("NotifyNewBid", 
                        new AuctionNotify
                        {
                            AuctionId = int.Parse(id!),
                            NewBid = int.Parse(bid!)
                        });
                    Console.WriteLine("Bid sent");
                }
            }
            finally
            {
                await connection.StopAsync();
            }
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using ps_globomantics_signalr.Models;

namespace ps_globomantics_signalr.Hubs
{
    public class AuctionHub : Hub
    {
        public async Task NotifyNewBid(AuctionNotify auction)
        {
            //throw new Exception("Simulated exception in NotifyNewBid");
            var groupName = $"auction-{auction.AuctionId}";

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.OthersInGroup(groupName).SendAsync("NotifyOutbid", 
                auction);

            await Clients.All.SendAsync("ReceiveNewBid", auction);
        }
    }
}

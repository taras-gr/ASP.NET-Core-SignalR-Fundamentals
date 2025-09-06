﻿using Microsoft.AspNetCore.SignalR;
using ps_globomantics_signalr.Models;

namespace ps_globomantics_signalr.Hubs
{
    public class AuctionHub : Hub
    {
        public async Task NotifyNewBid(AuctionNotify auctionNotify)
        {
            await Clients.All.SendAsync("ReceiveNewBid", 
                auctionNotify);
        }
    }
}

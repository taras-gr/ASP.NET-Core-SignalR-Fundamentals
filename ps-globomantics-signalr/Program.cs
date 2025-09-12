using Microsoft.AspNetCore.SignalR;
using ps_globomantics_signalr.Hubs;
using ps_globomantics_signalr.Models;
using ps_globomantics_signalr.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = builder.Environment.IsProduction();
    o.StatefulReconnectBufferSize = 100;
}).AddMessagePackProtocol();

builder.Services.AddSingleton<IAuctionRepo, AuctionMemoryRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapPost("auction", async (Auction auction, IAuctionRepo auctionRepo,
    IHubContext<AuctionHub> hubContext) =>
{
    auctionRepo.AddAuction(auction);
    await hubContext.Clients.All.SendAsync("ReceiveNewAuction", 
        auction);
});

app.MapHub<AuctionHub>("/auctionhub", o => o.AllowStatefulReconnects = true);

app.Run();

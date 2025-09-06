﻿const initializeSignalRConnection = () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/auctionhub",
            {
                transport: signalR.HttpTransportType.WebSockets,
                skipNegotiation: true
        })
        .build();

    connection.on("ReceiveNewBid", ({ auctionId, newBid }) => {
        const tr = document.getElementById(auctionId + "-tr");
        const input = document.getElementById(auctionId + "-input");

        // start animation
        tr.classList.add("animate-highlight");
        setTimeout(() => {
            tr.classList.remove("animate-highlight");
        }, 2000);

        const bidElement = document.getElementById(auctionId + "-bidtext");
        bidElement.innerText = newBid;
        input.value = newbid + 1;
    });

    connection.start().catch(err => console.error(err.toString()));
    return connection;
}

const connection = initializeSignalRConnection();

const submitBid = (auctionId) => {
    const bid = document.getElementById(auctionId + "-input").value;
    fetch("/auction/" + auctionId + "/newbid?currentBid=" + bid, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        }
    });

    connection.invoke("NotifyNewBid", {
        auctionId: parseInt(auctionId),
        newBid: parseInt(bid)
    });
}


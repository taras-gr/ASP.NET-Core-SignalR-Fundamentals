const initializeSignalRConnection = () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/auctionhub",
            {
                transport: signalR.HttpTransportType.WebSockets,
                skipNegotiation: true
            })
        .withStatefulReconnect({ bufferSize: 1000 })
        //.withAutomaticReconnect()
        .withHubProtocol(
            new signalR.protocols.msgpack.MessagePackHubProtocol())
        .build();

    connection.on("ReceiveNewBid", ({ AuctionId, NewBid }) => {
        const tr = document.getElementById(AuctionId + "-tr");
        const input = document.getElementById(AuctionId + "-input");

        // start animation
        tr.classList.add("animate-highlight");
        setTimeout(() => {
            tr.classList.remove("animate-highlight");
        }, 2000);

        const bidElement = document.getElementById(AuctionId + "-bidtext");
        bidElement.innerText = NewBid;
        input.value = Newbid + 1;
    });

    connection.on("ReceiveNewAuction", ({ Id, ItemName, CurrentBid }) => {
        var tbody = document.querySelector(".table>tbody");
        tbody.innerHTML += `<tr id="${Id}-tr" class="align-middle">
                                <td>${ItemName}</td >
                                <td id="${Id}-bidtext" class="bid">${CurrentBid}</td >
                                <td class="bid-form-td">
                                    <input id="${Id}-input" class="bid-input" type="number" value="${CurrentBid + 1}" />
                                    <button class="btn btn-primary" type="button" onclick="submitBid(${Id})">Bid</button>
                                </td>
                            </tr>`;
    });

    connection.on("NotifyOutbid", ({ AuctionId }) => {
        const tr = document.getElementById(AuctionId + "-tr");
        if (!tr.classList.contains("outbid"))
            tr.classList.add("outbid");
    });

    connection.start().catch(err => console.error(err.toString()));
    return connection;
}

const connection = initializeSignalRConnection();

const submitBid = (auctionId) => {
    const tr = document.getElementById(auctionId + "-tr");
    tr.classList.remove("outbid");

    const bid = document.getElementById(auctionId + "-input").value;
    fetch("/auction/" + auctionId + "/newbid?currentBid=" + bid, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!connection.state === "Connected")
        location.reload();

    connection.invoke("NotifyNewBid", {
        AuctionId: parseInt(auctionId),
        NewBid: parseInt(bid)
    }).catch(e => console.log(e.message));
}

const submitAuction = () => {
    const itemName = document.getElementById("add-itemname").value;
    const currentBid = document.getElementById("add-currentbid").value;
    fetch("/auction", {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ itemName, currentBid })
    });
}


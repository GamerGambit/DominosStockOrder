const dcId = "74804ede-cfef-41fc-aad6-f5f646fe4508"; // Americold DC ID

let connection;

function bindSignalREndpoints()
{
    connection.on("ReceiveCredentials", function(data) {
        dcId = data.dcId;
        storeId = data.storeId;
    });

    connection.on("PlaceOrder", function(order) {
        fetch("https://purchasing.dominos.com.au/purchaseorders/" + order.purchaseOrderId, {
            method: "POST",
            headers: {
                "Content-Type": "application/json; charset=utf-8"
            },
            body: JSON.stringify(order)
        })
        .catch(err => {
            console.error("PlaceOrder fetch error:", err);
            window.location.reload();
        })
        .then(async response => {
            // When we get logged out, we get a 302 redirect with no response.
            // Do not treat this as an error and do not send a successful order
            if (response == null)
                return false;

            if (!response.ok) {
                return response.text().then((text) => {
                    throw new Error(text)
                });
            }

            // Send the new pending order before sending order confirmation.
            // This lets us shortcut notifications about a successful order submission.
            await sendPendingOrders();
            return true;
        })
        .then(succeed => {
            if (succeed && connection.state == "Connected")
            {
                return connection.invoke("OrderSuccessful");
            }
        })
        .catch(async (error) => {
            console.error("Order failed:", error.toString());
            await connection.invoke("OrderFailed", error.toString());
        });
    });
}

function createSignalRHubConnection()
{
    connection = new signalR.HubConnectionBuilder()
        .withUrl(stockServer + "/purchasinghub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    bindSignalREndpoints();

    connection.onclose(() => {
        console.log("SignalR connection closed, attempted to reconnect");
        setTimeout(startSignalRConnection, 5000)
    });

    startSignalRConnection();
}

function startSignalRConnection() {
    if (connection.state == "Connecting" || connection.state == "Connected")
        return;

    connection.start()
        .then(() => {
            console.log("Connected to SignalR Hub");
        })
        .then(() => main())
        .catch(err => {
            console.error("SignalR Connection Error:", err);
            setTimeout(startSignalRConnection, 5000);
        });
}

function injectSignalR()
{
    let script = document.createElement("script");
    script.type = "text/javascript";
    script.src = stockServer + "/lib/signalr/signalr.js";
    script.onload = function () {
        console.log("SignalR script loaded...attempting connection");
        createSignalRHubConnection();
    };
    script.onerror = function () {
        console.error("Failed to load SignalR, retrying in 5 seconds");
        script.remove();

        setTimeout(injectSignalR, 5000);
    };

    document.body.append(script);
}

async function fetchOrderItems(orderId, bFetchInTransit)
{
    let res = await fetch("https://purchasing.dominos.com.au/purchaseorders/" + orderId);
    let json = await res.json();

    let itemsInTransit = {};

    if (bFetchInTransit)
    {
        itemsInTransit = await getItemsInTransit(json.orderDate);
    }

    return json.purchaseOrderItems.map(function(i) {
        let pulseCode = i.code.replace(/\D+$/, ""); // trim trailing letters which are only used to distinguish substitutions/variations

        return {
            purchaseOrderItemId: i.purchaseOrderItemId,
            code: pulseCode,
            description: i.description,
            packSizeQuantity: i.packSizeQuantity,
            autoIssue: i.autoIssue,
            suggested: i.suggested,
            override: i.override,
            finalOrder: i.finalOrder,
            inTransit: itemsInTransit[pulseCode] || 0,
            isNewInventory: i.isNewInventory,
            isPacksizeupdated: i.isPacksizeupdated,
            isItemEnabledRecently: i.isItemEnabledRecently,
            isItemCodeChangedRecently: i.isItemCodeChangedRecently
        };
    });
}

async function fetchAndProcessPendingOrders()
{
    let res = await fetch("https://purchasing.dominos.com.au/purchaseorders/pending?storeId=" + storeId);
    let json = await res.json();

    if (json.length == 0)
        return [];

    return await Promise.all(json.filter((d) => d.dcId == dcId)
    .map(async (d) => ({
        purchaseOrderId: d.purchaseOrderId,
        orderDate: d.orderDate,
        deliveryDate: d.supplyStartDate,
        items: await fetchOrderItems(d.purchaseOrderId, true)
    })));
}

async function sendPendingOrders()
{
    let pending = await fetchAndProcessPendingOrders();
    await connection.invoke("SetPendingOrders", pending);
}

async function getItemsInTransit(orderDate)
{
    let res = await fetch("https://purchasing.dominos.com.au/purchaseorders/completed?storeId=" + storeId);
    let json = await res.json();
    let filtered = json.filter((d) => d.dcId == dcId && Date.parse(d.supplyStartDate) >= Date.parse(orderDate));

    let itemsInTransit = {};
    for (const delivery of filtered)
    {
        let items = await fetchOrderItems(delivery.purchaseOrderId, false);
        for (const item of items)
        {
            itemsInTransit[item.code] = itemsInTransit[item.code] || 0;
            itemsInTransit[item.code] += item.finalOrder;
        }
    }

    return itemsInTransit;
}

function login()
{
    let usernameElement = document.querySelector("#username");
    usernameElement.value = username;
    usernameElement.dispatchEvent(new Event("input", { bubbles: true }));

    let passwordElement = document.querySelector("#password");
    passwordElement.value = password;
    passwordElement.dispatchEvent(new Event("input", { bubbles: true }));

    let submit = document.querySelector("#cal-login-button");
    submit.click();
}

async function main()
{
    if (window.location.href.includes("login")) {
        console.log("Logging in...");
        login();
    }
    else {
        console.log("Sending pending orders...");
        await sendPendingOrders();
    }
}

(function() {
    // 5 second time out to make sure the page is loaded.
    // When the page is initially visited, a JS router will redirect to login
    setTimeout(injectSignalR, 5000);
})();


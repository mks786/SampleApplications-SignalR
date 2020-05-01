if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}

var binanceTable = document.getElementById('BinanceTicker');
var binanceTableBody = binanceTable.getElementsByTagName('tbody')[0];
var rowTemplate = '<td>{symbol}</td><td>{value}</td>';

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/exchangeTickerHub")
    .build();

connection.start().then(function () {
    connection.invoke("GetAllTicker").then(function (tickers) {
        for (let i = 0; i < tickers.length; i++) {
            displayTicker(tickers[i]);
        };
    });
    connection.invoke("bTicker").then(function () {
        binanceTicker();
        startStreaming();
    });
});

connection.on("binanceTicker", function () {
    binanceTicker();
    startStreaming();
});
connection.on("bTicker", function () {
    binanceTicker();
    startStreaming();
});

connection.on("startAsync", function () {
    startAsync();
    startStreaming();
});

function startStreaming() {
    connection.stream("StreamTicker").subscribe({
        close: false,
        next: displayTicker,
        error: function (err) {
            logger.log(err);
        }
    });
}

var pos = 30;
var tickerInterval;

function moveTicker() {
    pos--;
    if (pos < -600) {
        pos = 500;
    }
}

function binanceTicker() {
    tickerInterval = setInterval(moveTicker, 20);
}

function displayTicker(ticker) {
    var displayTicker = formatTicker(ticker);
    addOrReplaceTicker(binanceTableBody, displayTicker, 'tr', rowTemplate);
}


function addOrReplaceTicker(table, ticker, type, template) {
    var child = createTickerNode(ticker, type, template);
    // try to replace
    var tickerNode = document.querySelector(type + "[data-symbol=" + ticker.symbol + "]");
    if (tickerNode) {
        table.replaceChild(child, tickerNode);
    } else {
        // add new Ticker
        table.appendChild(child);
    }
}

function formatTicker(ticker) {
    ticker.Value = ticker.price;
    return ticker;
}

function createTickerNode(ticker, type, template) {
    var child = document.createElement(type);
    child.setAttribute('data-symbol', ticker.symbol);
    child.setAttribute('class', ticker.symbol);
    child.innerHTML = template.supplant(ticker);
    return child;
}


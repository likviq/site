async function login() {
    let addEmail = document.getElementById('email').value;
    let addPassword = document.getElementById('password').value;

    const item = {
        'email': addEmail,
        'password': addPassword
    };
    const options = {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Accept: "application/json"
        },
        body: JSON.stringify(item)
    };
    const url = "/Login";
    const response = await fetch(url, options);
    if (!response.ok) {
        console.log('No, my friend')
        alert("Uncorrect")
    }
    else {
        let json = await response.json();
        document.cookie = 'token=' + json;
        window.location.replace("/profile.html");
        console.log(document.cookie);
    }
}
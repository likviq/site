
function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

async function createQueue() {
    let token = getCookie('token');
    let title = document.getElementById('title').value.trim();
    let description = document.getElementById('description').value.trim();
    const item = {
        'Title': title,
        'Description': description
    };
    console.log(title)
    console.log(description)
    const options = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        },
        body: JSON.stringify(item)
    };

    const url = '/queue/create/system/create';
    const response = await fetch(url, options);
    if (!response.ok && token == '') {
        console.log('No, my friend')
    }

    else {
        let json = await response.json();
        window.location.replace("/profile.html");
    }
}




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

async function joinQueue() {
    let token = getCookie('token');
    let idqueue = document.getElementById('idqueue').value.trim();
    console.log(idqueue)
    const options = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        }
    };

    const url = `/queue/system/enter/${idqueue}`;
    const response = await fetch(url, options);
    if (!response.ok && token == '') {
        console.log('No, my friend')
    }

    else {
        let json = await response.json();
        window.location.replace("/profile.html");
    }
}



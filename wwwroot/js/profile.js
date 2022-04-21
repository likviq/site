
function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    console.log(decodedCookie);
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

async function profile() {
    let token = getCookie('token');
    let profile_icon = document.getElementById('myprofile');
    let logout_icon = document.getElementById('logout');
    const options = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        }
    };

    const url = '/123';
    const response = await fetch(url, options);
    if (!response.ok && token == '') {
        document.getElementById('myprofilea').innerText = "Profile";
    }
    else {
        username = await response.text();
        console.log(username + 'Yes, my friend');
        document.getElementById('myprofilea').innerText = username;
        //window.location.replace("/swagger/index.html");
    }
}

profile();

function deleteCookies() {
    var cookies = document.cookie.split(";");

    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i];
        var eqPos = cookie.indexOf("=");
        var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
        document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
    }
}
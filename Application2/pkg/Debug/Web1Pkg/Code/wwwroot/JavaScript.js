
    function signUp() {
        var x = document.getElementById("id6").value;
        var z = "&id7=";
        var y = document.getElementById("id7").value;
        // proceed only when user has entered email and password
        if (x === "" || y === "") {
            alert("Please enter all fields");
            return false;
        }
        var http = new XMLHttpRequest();
        http.onreadystatechange = function () {
            if (http.readyState == 4) {
                if (http.responseText === "Username already taken. Please enter a different one") {
                    // if username is taken, send a confirm dialog box saying that and keep the user on
                    // the signup page
                    confirm(http.responseText);
                    window.location.href = 'http://localhost:8490/signup.html';
                }
                else {
                    // if successful, send confirm dialog and redirect user to login page
                    confirm(http.responseText);
                    window.location.href = 'http://localhost:8490/login.html';
                }

            }
        };

        http.open("GET", 'http://localhost:8490/api/signup?id6='.concat(x, z, y), true);
        http.send();

        //window.location = 'http://localhost:8490/api/signup?id6='.concat(x, z, y);
}

    function logOut() {

        var http = new XMLHttpRequest();
        http.onreadystatechange = function () {
            if (http.readyState == 4) {
                confirm(http.responseText);
                window.location.href = 'http://localhost:8490/htmlpage.html';

            }
        };

        http.open("GET", 'http://localhost:8490/api/logout', true);
        http.send();


}

    function logIn() {
        var x = document.getElementById("id4").value;
        var z = "&id5=";
        var y = document.getElementById("id5").value;
        // proceed only when user has entered email and password
        if (x === "" || y === "") {
            alert('Please enter all fields');
            return false;
        }

        var http = new XMLHttpRequest();
        http.onreadystatechange = function () {
            if (http.readyState == 4) {
                if (http.responseText === "Username/Password incorrect. No such username exists" || http.responseText == "Wrong username/password combination") {
                    confirm(http.responseText);
                    window.location.href = 'http://localhost:8490/login.html';
                }
                else {
                    confirm(http.responseText);
                    window.location.href = 'http://localhost:8490/htmlpage.html';
                }

            }
        };

        http.open("GET", 'http://localhost:8490/api/values/login?id4='.concat(x, z, y), true);
        http.send();

        //window.location = 'http://localhost:8490/api/values/login?id4='.concat(x, z, y);
    }
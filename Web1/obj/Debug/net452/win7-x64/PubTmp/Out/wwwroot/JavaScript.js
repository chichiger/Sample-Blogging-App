﻿
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
            if (http.readyState === 4) {
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

        return false;
    }

    function logOut() {

        deleteCookie();

    }
    function setCookie(cname, cvalue) {
        var d = new Date();
        d.setDate(d.getDate() + 1);
        var expires = "expires=" + d.toString();
        document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
    }

    function getCookie(cname) {
        var name = cname + "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(";");
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) === " ") {
                c = c.substring(1);
            }
            if (c.indexOf(name) === 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }

    // this function will check for the specific cookie based on value. The value here will be the username
    // that the user enters. If it's there, it will write a return message, and if it is not, a cookie
    // for the user will be created. The name parameter for setCookie will always be LoggedIn, and the value
    // depends on whoever is logging in. By doing this, if a user logs in from their browser, the value returned
    // will be their username, and if someone else is logging in from their browser, it will return their
    // username and not the other person's. 
    function checkCookie() {
        var user1 = document.getElementById("email").value;
        var user = getCookie(user1);
        if (user !== "") {
            alert("Welcome again " + user);
        } else {
            user = document.getElementById("email").value;
            if (user !== "" && user != null) {
                setCookie("LoggedIn", user, 1);
            }
        }
    }

    // delete a cookie by updating the current username cookie and changing the expiration date to 0
    function deleteCookie() {
        var cookie = getCookie("LoggedIn");
        if (cookie !== "" && cookie != null) {
            setCookie("LoggedIn", cookie, 0);
            alert("You are logged out, " + cookie);

        }
    }

    function logIn() {
            var x = document.getElementById("email").value;
            var z = "&password=";
            var y = document.getElementById("password").value;
            // proceed only when user has entered email and password
            if (x === "" || y === "") {
                alert("Please enter all fields");
                return false;
            }

            var http = new XMLHttpRequest();
            http.onreadystatechange = function() {
                if (http.readyState === 4) {
                    if (http.responseText === "Username/Password incorrect. No such username exists" ||
                        http.responseText === "Wrong username/password combination") {
                        confirm(http.responseText);
                        window.location.href = "http://localhost:8490/login.html";
                    } else {
                        checkCookie(); // create cookie with the username as the value
                        confirm(http.responseText);
                        window.location.href = "http://localhost:8490/htmlpage.html";
                    }

                }
            };

            http.open("GET", "http://localhost:8490/api/values/login?email=".concat(x, z, y), true);
            http.send();
          return false;
    }

        function newPost() {
            var x = document.getElementById("text").value;
            var z = "&hashtags=";
            var y = document.getElementById("hashtags").value;
            var v = "&cookie=";
            // proceed only when user has entered a hashtag
            if (y === "" || x === "") {
                alert("Please fill all required fields");
                return false;
            }
            var cookie = getCookie("LoggedIn");
            if (cookie === "" || cookie == null) {
                return "You must be logged in to post";
            }
            var http = new XMLHttpRequest();
            http.onreadystatechange = function() {
                if (http.readyState === 4) {
                    if (http.responseText === "Unable to post") {
                        confirm(http.responseText);
                        window.location.href = "http://localhost:8490/post.html";
                    } else {
                        confirm(http.responseText);
                        window.location.href = "http://localhost:8490/htmlpage.html";
                    }

                }
            };

            http.open("GET", "http://localhost:8490/api/post?text=".concat(x, z, y, v, cookie), true);
            http.send();
            return false;
        }

        function UrlImage() {
            var x = document.getElementById("id8").value;
            var z = "&id9=";
            var y = document.getElementById("id9").value;
            var v = "&cookie=";
            // proceed only when user has entered email and password
            if (x === "" || y === "") {
                alert("Please enter all fields");
                return false;
            }
            var cookie = getCookie("LoggedIn");
            if (cookie === "" || cookie == null) {
                return "You must be logged in to post";
            }
            var http = new XMLHttpRequest();
            http.onreadystatechange = function() {
                if (http.readyState === 4) {
                    confirm(http.responseText);
                    window.location.href = "http://localhost:8490/htmlpage.html";
                }
            };

            http.open("GET", "http://localhost:8490/api/image?id8=".concat(x, z, y, v, cookie), true);
            http.send();
            return false;
        }

    
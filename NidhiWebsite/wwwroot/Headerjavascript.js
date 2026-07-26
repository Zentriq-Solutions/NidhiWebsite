javascript
async function loadHeader() {

    try {

        const response = await fetch("header.html");

        if (!response.ok) {
            throw new Error("Unable to load header");
        }

        const headerHtml = await response.text();

        document.getElementById("headerContainer")
            .innerHTML = headerHtml;


        // Initialize header functionality
        initializeHeader();

    }
    catch (error) {

        console.error(
            "Header loading error:",
            error
        );

    }

}


function initializeHeader() {

    const profileBtn =
        document.getElementById("profileBtn");

    const popup =
        document.getElementById("profilePopup");


    // Profile button click
    profileBtn.onclick = function () {

        popup.style.display =
            popup.style.display === "block"
                ? "none"
                : "block";

    };


    // Close popup when clicking outside
    window.onclick = function (e) {

        if (!e.target.closest(".profile")) {

            popup.style.display = "none";

        }

    };


    // Get cookies
    const userId =
        getCookie("userId");

    const isAdmin =
        getCookie("isAdmin");


    const loginBtn =
        document.getElementById("loginBtn");

    const registerBtn =
        document.getElementById("registerBtn");

    const logoutBtn =
        document.getElementById("logoutBtn");

    const Additem =
        document.getElementById("Additem");

    const Additemgroup =
        document.getElementById("Additemgroup");

    const AllItemGroup =
        document.getElementById("AllItemGroup");


    // User is logged in
    if (userId) {

        loginBtn.style.display = "none";

        registerBtn.style.display = "none";

        logoutBtn.style.display = "block";


        // Admin user
        if (isAdmin === "true") {

            Additem.style.display = "block";

            Additemgroup.style.display = "block";

            AllItemGroup.style.display = "block";

        }

        // Normal user
        else {

            Additem.style.display = "none";

            Additemgroup.style.display = "none";

            AllItemGroup.style.display = "none";

        }

    }

    // User is not logged in
    else {

        loginBtn.style.display = "block";

        registerBtn.style.display = "block";

        logoutBtn.style.display = "none";

        Additem.style.display = "none";

        Additemgroup.style.display = "none";

        AllItemGroup.style.display = "none";

    }

}


// Navigation functions

function goToAddItem() {

    window.location.href =
        "ProductCreation.html";

}


function goToAddItemGroup() {

    window.location.href =
        "ItemGroupCreation.html";

}


function goToAllItemGroup() {

    window.location.href =
        "AllItemGroup.html";

}


function goToLogin() {

    window.location.href =
        "UserLogin.html";

}


function goToRegister() {

    window.location.href =
        "UserRegister.html";

}


function logout() {

    deleteCookie("userId");

    deleteCookie("isAdmin");

    window.location.href =
        "dashboard.html";

}


function deleteCookie(name) {

    document.cookie =
        `${ name }=; expires = Thu, 01 Jan 1970 00:00:00 UTC; path = /`;

}


function getCookie(name) {

    const cookies =
        document.cookie.split(";");


    for (let cookie of cookies) {

        cookie = cookie.trim();


        if (cookie.startsWith(name + "=")) {

            return decodeURIComponent(
                cookie.substring(
                    name.length + 1
                )
            );

        }

    }


    return null;

}

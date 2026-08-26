function initializeLayout() {

    const profileBtn = document.getElementById("profileBtn");
    const profilePopup = document.getElementById("profilePopup");

    const reportBtn = document.getElementById("reportBtn");
    const reportPopup = document.getElementById("reportPopup");

    const loginBtn = document.getElementById("loginBtn");
    const registerBtn = document.getElementById("registerBtn");
    const logoutBtn = document.getElementById("logoutBtn");

    const addItem = document.getElementById("Additem");
    const addItemGroup = document.getElementById("Additemgroup");
    const allItemGroup = document.getElementById("AllItemGroup");
    const allItem = document.getElementById("AllItem");

    const reportMenu = document.getElementById("reportMenu");


    // =========================
    // PROFILE MENU
    // =========================

    profileBtn.onclick = function (event) {

        event.stopPropagation();

        profilePopup.style.display =
            profilePopup.style.display === "block"
                ? "none"
                : "block";

        if (reportPopup) {
            reportPopup.style.display = "none";
        }
    };


    // =========================
    // REPORT MENU
    // =========================

    if (reportBtn) {

        reportBtn.onclick = function (event) {

            event.stopPropagation();

            reportPopup.style.display =
                reportPopup.style.display === "block"
                    ? "none"
                    : "block";

            profilePopup.style.display = "none";
        };
    }


    // =========================
    // CLOSE POPUPS
    // =========================

    document.addEventListener("click", function (event) {

        if (!event.target.closest(".profile")) {

            profilePopup.style.display = "none";

            if (reportPopup) {
                reportPopup.style.display = "none";
            }
        }

    });


    // =========================
    // LOGIN / ADMIN CHECK
    // =========================

    const userId = getCookie("userId");
    const isAdmin = getCookie("isAdmin");


    if (userId) {

        loginBtn.style.display = "none";
        registerBtn.style.display = "none";

        logoutBtn.style.display = "block";

        ViewAllProducts.style.display = "block";
        if (isAdmin === "true") {

            addItem.style.display = "block";
            addItemGroup.style.display = "block";
            allItemGroup.style.display = "block";
            allItem.style.display = "block";

            reportMenu.style.display = "block";

        }
        else {

            addItem.style.display = "none";
            addItemGroup.style.display = "none";
            allItemGroup.style.display = "none";
            allItem.style.display = "none";

            reportMenu.style.display = "none";
        }

    }
    else {

        loginBtn.style.display = "block";
        registerBtn.style.display = "block";
        ViewAllProducts.style.display = "block";
        logoutBtn.style.display = "none";

        addItem.style.display = "none";
        addItemGroup.style.display = "none";
        allItemGroup.style.display = "none";
        allItem.style.display = "none";

        reportMenu.style.display = "none";
    }
}


// =========================
// COOKIE
// =========================

function getCookie(name) {

    const cookies = document.cookie.split(";");

    for (let cookie of cookies) {

        cookie = cookie.trim();

        if (cookie.startsWith(name + "=")) {

            return decodeURIComponent(
                cookie.substring(name.length + 1)
            );
        }
    }

    return null;
}


// =========================
// NAVIGATION
// =========================

function goToAddItem() {

    window.location.href = "ProductCreation";
}


function goToAddItemGroup() {

    window.location.href = "ItemGroupCreation";
}


function goToAllItemGroup() {

    window.location.href = "AllItemGroup";
}


function goToAllItem() {

    window.location.href = "AllItem";
}


function goToLogin() {

    window.location.href = "UserLogin";
}


function goToRegister() {

    window.location.href = "UserRegister";
}


function goToCart() {

    const userId = getCookie("userId");

    if (userId) {

        window.location.href = "CartItemDetails";

    }
    else {

        LoginPopUp();
    }
}
function GoToHome() {
    window.location.href = "Dashboard";
}
function GoToViewAllProducts() {
    window.location.href = "ViewAllPage"
}

// =========================
// LOGOUT
// =========================

function logout() {

    deleteCookie("userId");

    deleteCookie("isAdmin");

    sessionStorage.clear();

    localStorage.clear();

    window.location.href = "dashboard";
}


function deleteCookie(name) {

    document.cookie =
        `${name}=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/`;
}


// =========================
// LOGIN POPUP
// =========================

function LoginPopUp() {

    const popup =
        document.getElementById("loginPopup");

    popup.style.display = "flex";
}


function closeLoginPopUp() {

    const popup =
        document.getElementById("loginPopup");

    popup.style.display = "none";
}


// =========================
// REPORTS
// =========================

function goToMostWishlistReport() {

    window.location.href = "WishListReport";
}


function goToMostCartReport() {

    window.location.href = "CartListReport";
}
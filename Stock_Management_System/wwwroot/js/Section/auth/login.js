$(function () {
    var resetStatus = sessionStorage.getItem('resetStatus');
    if (resetStatus) {
        toastr.info(resetStatus);
    }
});

var loginAttempts = parseInt(sessionStorage.getItem("loginAttempts")) || 0;
var timerInterval; // Variable to store the interval for the timer
var timerCount = parseInt(sessionStorage.getItem("timerCount")) || 120; // Initial timer count: 2 minutes

// Function to disable the login button and start the timer
function disableLoginButton() {
    var loginButton = document.getElementById("loginButton");
    loginButton.disabled = true;
    loginButton.innerHTML = "Login (" + timerCount + "s)";

    timerInterval = setInterval(function () {
        timerCount--;
        loginButton.innerHTML = "Login (" + timerCount + "s)";
        sessionStorage.setItem("timerCount", timerCount); // Update the timer count in sessionStorage
        if (timerCount <= 0) {
            clearInterval(timerInterval);
            loginButton.innerHTML = "Login";
            loginButton.disabled = false;
            sessionStorage.removeItem("loginAttempts"); // Reset login attempts count
            sessionStorage.removeItem("timerCount"); // Remove the timer count from sessionStorage
        }
    }, 1000);
}

// Function to broadcast the loginAttempts count to other tabs/windows
function broadcastLoginAttempts() {
    var broadcastChannel = new BroadcastChannel('loginAttemptsChannel');
    broadcastChannel.postMessage(loginAttempts);
}

document.getElementById("loginForm").addEventListener("submit", function (event) {
    event.preventDefault(); // Prevent form submission

    var username = document.getElementById("Username").value;
    var password = document.getElementById("Password").value;

    if (username.trim() === "" || password.trim() === "") {
        toastr.error('Please enter your username and password.');
    } else {
        // Increment login attempts
        loginAttempts++;
        sessionStorage.setItem("loginAttempts", loginAttempts);

        if (loginAttempts >= 3) {
            // Disable login button for 2 minutes
            disableLoginButton();
            // Broadcast the loginAttempts count to other tabs/windows
            broadcastLoginAttempts();
        }

        // Perform AJAX login request
        $.ajax({
            url: '/Auth/Login',
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    // Redirect to the dashboard page
                    window.location.href = response.redirectUrl;
                } else {
                    // Show Toastr error message
                    toastr.error(response.errorMessage);
                }
            },
            error: function (xhr, status, error) {
                // Show Toastr error message for server error
                toastr.error('Server error. Please contact administrator.');
            }
        });
    }
});

// Listen for changes in loginAttempts from other tabs/windows
var broadcastChannel = new BroadcastChannel('loginAttemptsChannel');
broadcastChannel.onmessage = function (event) {
    var receivedAttempts = parseInt(event.data);
    if (receivedAttempts >= 3) {
        disableLoginButton();
    }
};

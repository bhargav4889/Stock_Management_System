$(document).ready(function () {
    // Check the initial connection status
    checkInternetConnection();

    // Add event listeners for online and offline events
    window.addEventListener('online', checkInternetConnection);
    window.addEventListener('offline', checkInternetConnection);
});

function checkInternetConnection() {
    if (navigator.onLine) {
        hideNoInternetToast();
    } else {
        showNoInternetToast();
    }
}

function showNoInternetToast() {
    var toastElement = $('#no-internet-toast');
    var toast = new bootstrap.Toast(toastElement);
    toast.show();
}

function hideNoInternetToast() {
    var toastElement = $('#no-internet-toast');
    var toast = new bootstrap.Toast(toastElement);
    toast.hide();
}
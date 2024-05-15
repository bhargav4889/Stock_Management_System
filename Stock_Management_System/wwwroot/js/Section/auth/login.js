$(function () {
    var Reset = sessionStorage.getItem('ResetStatus');
    if (Reset) {
        toastr.success(Reset);
        // Clear the message from session storage so it doesn't reappear on refresh
        sessionStorage.removeItem('ResetStatus');
    }
});

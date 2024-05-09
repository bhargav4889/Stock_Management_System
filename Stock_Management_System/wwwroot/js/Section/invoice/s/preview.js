$(document).ready(function () {
    var UpdateMessage = sessionStorage.getItem('InvoiceStatus');
    if (UpdateMessage) {
        toastr.success(UpdateMessage);
        // Clear the message from session storage so it doesn't reappear on refresh
        sessionStorage.removeItem('InvoiceStatus');
    }
});

function notifyDownloadStart() {
    toastr.success('Download started successfully!', 'Download', {
        closeButton: true,
        progressBar: true,
        positionClass: 'toast-top-center',
        timeOut: 3000, // 3 seconds
    });
}

$(function () {
    var AddMessage = sessionStorage.getItem('AddStatus');
    if (AddMessage) {
        toastr.success(AddMessage);
        // Clear the message from session storage so it doesn't reappear on refresh
        sessionStorage.removeItem('AddStatus');
    }
});

$(function () {
    var DeleteStatus = sessionStorage.getItem('DeleteStatus');
    if (DeleteStatus) {
        toastr.success(DeleteStatus);
        // Clear the message from session storage so it doesn't reappear on refresh
        sessionStorage.removeItem('DeleteStatus');
    }
});


$(function () {
    // Attaching click event to elements with class 'loadSaveInfo'
    $('.loadSaveInfo').on('click', function () {
        // Retrieving the encrypted reminder ID stored in data attribute
        var reminderId = $(this).data('reminder-id');

        // Performing an AJAX GET request to fetch reminder details by ID
        $.ajax({
            url: '/Reminder/GetReminderByID',  // Updated endpoint
            type: 'GET',  // Method type GET
            data: { Reminder_ID: reminderId },  // Data passed to the server
            success: function (response) {
                // On success, load the response into the modal's content area
                $('#Reminder .modal-content').html(response);
                // Show the modal
                $('#Reminder').modal('show');
            },
            error: function (xhr) {
                toastr.error("Somthing Went Wrong !");
            }
        });
    });
});

function confirmDeletion(deleteUrl, reminderDate) {
    Swal.fire({
        title: 'Are You Sure You Want to Delete?',
        text: `Are you sure you want to delete the reminder scheduled for ${reminderDate}? This action cannot be undone.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: deleteUrl,
                type: 'POST',
                success: function (response) {
                    // Set a success message in session storage
                    sessionStorage.setItem('DeleteStatus', 'Delete Reminder Successfully!');
                    // Redirect immediately
                    window.location.href = response.redirectUrl;
                },
                error: function () {
                    // Set an error message in session storage
                    sessionStorage.setItem('DeleteStatus', 'Error. Please try again.');
                    // You might still want to redirect or handle errors differently here
                    window.location.href = response.redirectUrl;
                }
            });
        }
    });
}


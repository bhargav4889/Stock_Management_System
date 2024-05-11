// Document ready function to handle addition and deletion messages using toastr
$(function () {
    // Handle addition message
    var addMessage = sessionStorage.getItem('AddStatus');
    if (addMessage) {
        toastr.success(addMessage);
        sessionStorage.removeItem('AddStatus'); // Clear message to prevent it from appearing again
    }

    // Handle deletion message
    var deleteStatus = sessionStorage.getItem('DeleteStatus');
    if (deleteStatus) {
        toastr.success(deleteStatus);
        sessionStorage.removeItem('DeleteStatus'); // Clear message to prevent it from appearing again
    }

    // Event listener for loading save information via AJAX
    $(document).on('click', '.loadSaveInfo', function () {
        var infoId = $(this).data('information-id');
        $.ajax({
            url: '/Information/GetInformationByID',
            type: 'GET',
            data: { Information_ID: infoId },
            success: function (response) {
                $('#information .modal-content').html(response);
                $('#information').modal('show');
            },
            error: function () {
                toastr.error('Error loading information');
            }
        });
    });
});

// Function to confirm deletion using Swal alerts
function confirmDeletion(deleteUrl) {
    Swal.fire({
        title: 'Are You Sure You Want to Delete?',
        text: "This Information will be permanently deleted.",
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
                    sessionStorage.setItem('DeleteStatus', 'Delete Information Successfully!');
                    window.location.href = response.redirectUrl; // Redirect on success
                },
                error: function () {
                    sessionStorage.setItem('DeleteStatus', 'Error. Please try again.');
                    window.location.href = response.redirectUrl; // Redirect on error
                }
            });
        }
    });
}

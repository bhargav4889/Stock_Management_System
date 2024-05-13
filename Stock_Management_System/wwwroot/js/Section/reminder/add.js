
    $(function () {
        $('#SelectReminder').on('change', function () {
            if ($(this).val() === 'Other') {
                $('#otherReminderType').show();
            } else {
                $('#otherReminderType').hide();
            }
        });
    });
    // Document ready function to handle the dynamic display of the 'Other' reminder type field
    $(function () {
        $('#SelectReminder').on('change', function () {
            var selectedType = $(this).val();
            if (selectedType === 'Other') {
                $('#otherReminderType').show();
                $('#otherTypeReminder').attr('required', true);  // Make input required if 'Other' is selected
            } else {
                $('#otherReminderType').hide();
                $('#otherTypeReminder').removeAttr('required');  // Remove required attribute if 'Other' is not selected
            }
        });
    });

    // Validation function before form submission
    function CheckData() {
        var reminderDate = document.getElementById('datepicker').value;
        var reminderType = document.getElementById('SelectReminder').value;
        var reminderCustomType = document.getElementById('otherTypeReminder').value;
        var reminderDescription = document.getElementById('remainderDescription').value;

        var missingFields = [];

        if (reminderDate === '') missingFields.push('Reminder Date');
        if (reminderType === '') missingFields.push('Reminder Type');
        if (reminderType === 'Other' && reminderCustomType === '') missingFields.push('Custom Reminder Type');
        if (reminderDescription === '') missingFields.push('Reminder Description');

        if (missingFields.length > 0) {
            toastr.error(`Please fill out the following required fields: ${missingFields.join(', ')}`, {
                closeButton: true,
                progressBar: true,
                positionClass: 'toast-bottom-right',
                preventDuplicates: true,
                showDuration: '300',
                hideDuration: '1000',
                timeOut: '5000',
                extendedTimeOut: '1000',
                showEasing: 'swing',
                hideEasing: 'linear',
                showMethod: 'fadeIn',
                hideMethod: 'fadeOut'
            });
            return false;
        } else {
            // Confirmation before adding customer
            confirmAddReminder('/Reminder/InsertReminder');
            return false; // Prevent form submission
        }
    }

    // Function to confirm the addition of a customer
    function confirmAddReminder(redirectUrl) {
        Swal.fire({
            title: 'Confirm Add Reminder',
            text: 'Are you sure you want to add this reminder?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, add it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: redirectUrl,
                    type: 'POST',
                    data: $("#ReminderForm").serialize(),
                    success: function (response) {
                        sessionStorage.setItem('AddStatus', 'Reminder added successfully!');
                        window.location.href = response.redirectUrl;
                    },
                    error: function () {
                        sessionStorage.setItem('ErrorMsg', 'Somthing Went Wrong!');
                        window.location.reload();
                    }
                });
            }
        });
}

$(function () {
    var ErrorMsg = sessionStorage.getItem('ErrorMsg');
    if (ErrorMsg) {
        toastr.error(ErrorMsg);
        // Clear the message from session storage so it doesn't reappear on refresh
        sessionStorage.removeItem('ErrorMsg');
    }
});

function confirmReminderDataReset() {
    Swal.fire({
        title: 'Are you sure Want Reset All values ?',
        text: "You won't be able to revert this!",
        icon: 'info',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, clear it!'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.reload();
        }
    });
}

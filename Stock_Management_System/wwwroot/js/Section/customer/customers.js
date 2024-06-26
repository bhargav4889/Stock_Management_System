
$(function () {
    var table = $('#data').DataTable();

    function applySearches() {
        var Type = $("#searchCTType").val();

        // Clear existing searches
        table.columns().search('');

        // Apply dropdown search if selected
        if (Type) {
            table.column(1).search(Type);
        }

        // Perform a single draw after all search criteria have been applied
        table.draw();

        // Reset the input values after search - Optionally you can keep the value

        $('#searchCTType').val('').trigger('change');
    }

    $('#searchButton').on('click', function () {
        applySearches();
    });
});


function confirmDeletion(deleteUrl, customerName) {
    Swal.fire({
        title: 'Are You Sure Want To Delete?',
        text: `${customerName}"S Account Deleted along with related Stock And payment info.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: deleteUrl,
                type: 'POST',
                success: function (response) {
                    // Set a success message in session storage
                    sessionStorage.setItem('DeleteStatus', 'Delete Account Successfully!');
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

$(function () {
    var AddMessage = sessionStorage.getItem('AddStatus');
    if (AddMessage) {
        toastr.success(AddMessage);
        // Clear the message from session storage so it doesn't reappear on refresh
        sessionStorage.removeItem('AddStatus');
    }
});



$(function () {
    var deletionMessage = sessionStorage.getItem('DeleteStatus');
    if (deletionMessage) {
        toastr.success(deletionMessage);
        // Clear the message from session storage so it doesn't reappear on refresh
        sessionStorage.removeItem('DeleteStatus');
    }
});

function showToastNotification(formId) {
    // Show a success toast notification
    toastr.success('Download started successfully.', 'Success', {
        closeButton: true,
        progressBar: true,
        positionClass: 'toast-top-center',
        timeOut: 3000, // Display duration
    });

    // Use setTimeout to delay form submission for a short period (e.g., 100 ms)
    setTimeout(function () {
        document.getElementById(formId).submit();
    }, 100); // Delay the form submission to allow the toast to render

    return false; // Prevent immediate form submission
}

document.addEventListener("DOMContentLoaded", function () {
    // Function to check and disable/enable the buttons and update their titles
    function updateButtonState() {
        var dataRows = document.querySelectorAll("#data tbody tr");
        var pdfButton = document.querySelector("#pdfExportForm button");
        var excelButton = document.querySelector("#excelExportForm button");
        var printButton = document.querySelector("button[onclick='window.print();']");

        if (dataRows.length === 0) {
            // Disable all buttons and adjust styles and titles
            [pdfButton, excelButton, printButton].forEach(button => {
                button.disabled = true;
                button.style.opacity = 0.5;
                button.style.cursor = "not-allowed";
                // Updating title to indicate no data available
                button.title = "No Data Available";
            });
        } else {
            // Enable all buttons and reset styles and titles
            [pdfButton, excelButton, printButton].forEach(button => {
                button.disabled = false;
                button.style.opacity = 1;
                button.style.cursor = "pointer";
                // Resetting titles based on the function of each button
                if (button === pdfButton) {
                    button.title = "Download PDF";
                } else if (button === excelButton) {
                    button.title = "Download Excel";
                } else if (button === printButton) {
                    button.title = "Print";
                }
            });
        }
    }

    updateButtonState(); // Run this function on page load

    // Optionally, add event listeners or other mechanisms to invoke updateButtonState if the table is dynamically updated.
});

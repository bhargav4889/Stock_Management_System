
function applySearches() {


    var table = $('#data').DataTable();
    var location = $("#searchLocation").val();
    var startdate = $('#startdate').val();
    var selectedGrain = $("#producttype").val();
    var selectedGrainText = $("#producttype option:selected").text(); // Get the selected option text
    var enddate = $('#enddate').val();



    // Clear existing searches
    table.columns().search('');


    // Apply individual column searches from input fields
    table.column(3).search(location);

    if (selectedGrain) {
        table.column(2).search(selectedGrainText);
    }

    // Setup date filtering
    $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
        var start = startdate ? parseDate(startdate) : null;
        var end = enddate ? parseDate(enddate) : new Date();

        var dateString = data[0]; // Assuming the date is in the first column
        var date = parseDate(dateString); // Parse date in format dd/MM/yyyy

        return (!start || date >= start) && (!end || date <= end);
    });

    // Perform a single draw after all search criteria have been applied
    table.draw();

    // Remove the date filter after drawing
    $.fn.dataTable.ext.search.pop();

    $('#producttype').val('').trigger('change');
    $('#searchLocation').val('');


    $('#startdate').val('');
    $('#enddate').val('');
}

$('#searchButton').on('click', function () {
    applySearches();

});

function parseDate(dateString) {
    var parts = dateString.split('-');
    var year = parseInt(parts[0], 10);
    var month = parseInt(parts[1], 10); // Months are 1-based
    var day = parseInt(parts[2], 10);

    return new Date(year, month - 1, day); // Months are 0-based in JavaScript Date objects
}


$(function () {
    // Attach event to document, delegate to .loadPaymentInfoBtn
    $(document).on('click', '.loadPaymentInfoBtn', function () {
        var customerId = $(this).data('customer-id');
        var stockId = $(this).data('stock-id');

        $.ajax({
            url: '/Payment/GetRemainPaymentInfo',
            data: { Customer_ID: customerId, Stock_ID: stockId },
            success: function (response) {
                $('#payment .modal-content').html(response);
                $('#payment').modal('show');
            },
            error: function () {
                alert('Error loading payment information');
            }
        });
    });
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

function confirmRemainDeletion(deleteUrl) {
    Swal.fire({
        title: 'Are You Sure You Want To Delete?',
        text: "This will delete the payment information and Status Wil be Reset !!",
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
                    if (response.success) {
                        sessionStorage.setItem('DeleteStatus', response.message);
                        window.location.href = response.redirectUrl;
                    } else {
                        Swal.fire('Failed!', response.message, 'error');
                    }
                },
                error: function () {
                    toastr.error("Somthing Went Wrong !!");
                }
            });
        }
    });
}

$(function () {
    var deleteStatus = sessionStorage.getItem('DeleteStatus');
    if (deleteStatus) {
        toastr.success(deleteStatus);
        sessionStorage.removeItem('DeleteStatus');
    }
});
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
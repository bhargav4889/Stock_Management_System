
$(function () {
    var table = $('#data').DataTable();

    function applySearches() {
        var startdate = $('#startdate').val();
        var enddate = $('#datepickerend').val();
        var location = $('#searchLocation').val() ? $('#searchLocation').val() : '';
        var brandName = $('#BrandName').val() ? $('#BrandName').val() : '';

        // Clear existing searches
        table.columns().search('');

        // Apply individual column searches from input fields
        if (location) {
            table.column(2).search(location);
        }

        if (brandName) {
            table.column(2).search(brandName);
        }

        // Setup date filtering
        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            var dateColumn = data[0]; // Assuming the date is in the first column
            var date = parseDate(dateColumn); // Parse the date

            var startDate = startdate ? new Date(startdate) : null;
            var endDate = enddate ? new Date(enddate) : new Date();

            return (!startDate || date >= startDate) && (!endDate || date <= endDate);
        });

        // Perform a single draw after all search criteria have been applied
        table.draw();
        $.fn.dataTable.ext.search.pop(); // Remove the date filter after drawing

        // Reset the input values after search
        $('#startdate, #datepickerend, #searchLocation, #BrandName').val('');
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
});


//pending

$(function () {
    // Attach event to document, delegate to .loadPaymentInfoBtn
    $(document).on('click', '.loadPendingPaymentInfoBtn', function () {
        var customerId = $(this).data('customer-id');
        var stockId = $(this).data('stock-id');

        $.ajax({
            url: '/Payment/GetPaymentInfoByCustomerStockID',
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


// remain


$(function () {
    // Attach event to document, delegate to .loadPaymentInfoBtn
    $(document).on('click', '.loadRemainPaymentInfoBtn', function () {
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

//paid


$(function () {
    // Attach event to document, delegate to .loadPaymentInfoBtn
    $(document).on('click', '.loadPaidPaymentInfoBtn', function () {
        var customerId = $(this).data('customer-id');
        var stockId = $(this).data('stock-id');

        $.ajax({
            url: '/Payment/GetPaymentInfo',
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


$(function () {
    // Correct class name for the button
    $(document).on('click', '.loadSalesInfoBtn', function () {
        var saleId = $(this).data('sale-id');

        $.ajax({
            url: '/Sale/GetSaleInfo',
            type: 'GET',
            data: { Sale_ID: saleId },
            success: function (response) {
                $('#showsaleModal .modal-content').html(response);
                $('#showsaleModal').modal('show');
            },
            error: function (xhr, status, error) {
                alert('Error loading sale information: ' + error);
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

function confirmDeletion(deleteUrl, customerName) {
    Swal.fire({
        title: 'Are You Sure Want To Delete?',
        text: `${customerName}"S Delete Entry ?.`,
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
                    sessionStorage.setItem('DeleteStatus', 'Deleted Successfully!');
                    // Redirect immediately
                    window.location.reload();
                },
                error: function () {
                    // Set an error message in session storage
                    sessionStorage.setItem('DeleteStatus', 'Error. Please try again.');
                    // You might still want to redirect or handle errors differently here
                    window.location.reload();
                }
            });
        }
    });
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
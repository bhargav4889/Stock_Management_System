
        $(function () {
            var table = $('#data').DataTable();

            // Extend DataTable search to include date range filtering
            $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
                var startDateInput = $("#startdate").val();
                var endDateInput = $("#datepickerend").val();

                // If only the end date is specified, set the start date to the first day of the current month
                var startDate = startDateInput ? moment(startDateInput, "YYYY-MM-DD") : (endDateInput ? moment().startOf('month') : moment().startOf('month'));
                // If only the start date is specified, set the end date to today's date
                var endDate = endDateInput ? moment(endDateInput, "YYYY-MM-DD").endOf('day') : (startDateInput ? moment() : moment().endOf('day'));

                var dateColumn = data[0]; // Adjust if date is in another column
                var date = moment(dateColumn, "DD-MM-YY");

                if (!date.isValid() || !startDate.isValid() || !endDate.isValid()) {
                    console.error("Invalid date encountered");
                    return false;
                }

                return date.isSameOrAfter(startDate) && date.isSameOrBefore(endDate);
            });

            // Function to apply all search filters and reset date inputs after search
            function applySearches() {
                var Name = $("#searchName").val();
          
                var location = $("#location").val();
                var selectedGrain = $("#graintype").val();
                var selectedGrainText = $("#graintype option:selected").text();

                // Clear existing searches
                table.search('').columns().search('');

                // Apply text input searches
                table.column(1).search(Name, true, false);
                table.column(3).search(location, true, false);

                // Apply dropdown searches
                if (selectedGrain) {
                    table.column(2).search(selectedGrainText, false, false);
                }

               

                // Redraw table to apply all filters
                table.draw();

                // Reset start and end date values after search
                $("#startdate, #datepickerend").val('');
            }

            // Bind the search function to the search button click event
            $('#searchButton').on('click', function () {
                applySearches();
            });
        });
   
        $(function () {
            // Attach event to document, delegate to .loadPaymentInfoBtn
            $(document).on('click', '.loadPaymentInfoBtn', function () {
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



function confirmPendingDeletion(deleteUrl) {
    Swal.fire({
        title: 'Are You Sure Want To Delete?',
        text: `If This Delete Payment Information As Well Stock Details Realeted !!`,
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
                    window.location.href = response.redirectUrl;
                },
                error: function () {
                    // Set an error message in session storage
                    toastr.error("Somthing Went Wrong !!");
                    // You might still want to redirect or handle errors differently here
                    window.location.href = window.location.reload;
                }
            });
        }
    });
}

$(function () {
    var AddMessage = sessionStorage.getItem('DeleteStatus');
    if (AddMessage) {
        toastr.success(AddMessage);
        // Clear the message from session storage so it doesn't reappear on refresh
        sessionStorage.removeItem('DeleteStatus');
    }
});
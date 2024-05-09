
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

$(document).ready(function () {
    var table = $('#data').DataTable();

    function parseDate(str) {
        var parts = str.split('-');
        return new Date(2000 + parseInt(parts[2], 10), parseInt(parts[1], 10) - 1, parseInt(parts[0], 10));
    }

    function applySearches() {
        var Name = $("#searchName").val();
        var VehicleName = $("#vehicletype").val();
        var selectedVehicle = $("#vehicletype option:selected").text();
        var TolatName = $("#searchTolatName").val();
        var selectedGrain = $("#graintype").val();
        var selectedGrainText = $("#graintype option:selected").text();
        var startdate = $('#startdate').val();
        var enddate = $('#datepickerend').val(); // Ensure this matches your HTML

        // Clear existing searches
        table.columns().search('');

        // Apply dropdown search if selected
        if (selectedGrain) {
            table.column(3).search(selectedGrainText);
        }

        if (VehicleName) {
            table.column(10).search(selectedVehicle);
        }

        // Apply individual column searches from input fields
        table.column(1).search(Name);
        table.column(9).search(TolatName);

        // Setup date filtering
        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            var dateColumn = data[0]; // Assuming the date is in the first column
            var date = parseDate(dateColumn); // Parse the DD-MM-YY format

            var start = startdate ? parseDate(startdate.split('-').reverse().join('-')) : null;
            var end = enddate ? parseDate(enddate.split('-').reverse().join('-')) : new Date();

            if (enddate && !startdate) {
                start = new Date(end.getFullYear(), end.getMonth(), 1); // Set to the first of the month
            }

            if (startdate && !enddate) {
                end = new Date(); // Set to current date
            }

            if (!startdate && !enddate) {
                return true; // No filtering on dates
            }

            return (start ? date >= start : true) && (end ? date <= end : true);
        });

        // Perform a single draw after all search criteria have been applied
        table.draw();
        $.fn.dataTable.ext.search.pop(); // Remove the date filter after drawing

        // Reset the input values after search
        $("#searchName, #searchTolatName").val('');
        $('#graintype').val('').trigger('change');
        $('#vehicletype').val('').trigger('change');
    }

    $('#searchButton').on('click', function () {
        applySearches();
    });
});

function confirmDeletion(deleteUrl, customerName) {
    Swal.fire({
        title: 'Are You Sure Want To Delete?',
        text: `${customerName} Stock Entry Deleted along with related payment info.`,
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
                    sessionStorage.setItem('DeleteStatus', 'Error. Please try again.');
                    // You might still want to redirect or handle errors differently here
                    window.location.href = response.redirectUrl;
                }
            });
        }
    });
}

$(document).ready(function () {
    var deletionMessage = sessionStorage.getItem('DeleteStatus');
    if (deletionMessage) {
        toastr.success(deletionMessage);
        // Clear the message from session storage so it doesn't reappear on refresh
        sessionStorage.removeItem('DeleteStatus');
    }
});

$(document).ready(function () {
    var AddMessage = sessionStorage.getItem('AddStatus');
    if (AddMessage) {
        toastr.success(AddMessage);
        // Clear the message from session storage so it doesn't reappear on refresh
        sessionStorage.removeItem('AddStatus');
    }
});

$(document).ready(function () {
    var UpdateMessage = sessionStorage.getItem('UpdateStatus');
    if (UpdateMessage) {
        toastr.success(UpdateMessage);
        // Clear the message from session storage so it doesn't reappear on refresh
        sessionStorage.removeItem('UpdateStatus');
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





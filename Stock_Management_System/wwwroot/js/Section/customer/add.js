

function CheckData() {
    var customerName = document.getElementById('CustomerName').value;
    var customerType = document.getElementById('CustomerType').value;
    var customerCity = document.getElementById('CustomerCity').value;
    var customerContact = document.getElementById('CustomerContact').value;

    var missingFields = [];

    if (customerName === '') missingFields.push('Customer Name');
    if (customerType === '') missingFields.push('Customer Type');
    if (customerCity === '') missingFields.push('City');
    if (customerContact === '') missingFields.push('Contact Number');

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
        // Use SweetAlert2 to confirm with the user before submission
        confirmAddCustomer('/Account/InsertCustomer', customerName);
        return false; // Prevent form submission until confirmation and AJAX call are completed
    }
}


function confirmAddCustomer(redirectUrl, customerName) {
    Swal.fire({
        title: 'Confirm Add Customer',
        text: `Are you sure you want to add ${customerName} Customer?`,
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
                data: $("#CustomerForm").serialize(), // Ensure this form ID matches your form
                success: function (response) {
                    // Assuming the server sends a JSON response with a redirectUrl field
                    sessionStorage.setItem('AddStatus', 'Customer added successfully!');
                    window.location.href = response.redirectUrl; // Redirect using the URL from the response
                },
                error: function () {
                    sessionStorage.setItem('AddStatus', 'Error adding customer. Please try again.');
                    window.location.reload(); // Reload the current page on error
                }
            });
        }
    });
}



function confirmCustomerDataReset() {
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



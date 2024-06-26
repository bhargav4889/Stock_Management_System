
$(function () {
    $('#selectgrain').select2({
        minimumResultsForSearch: Infinity // Disables the search box
    });

    // Listen for the 'select2:select' event to capture when an option is selected
    $('#selectgrain').on('select2:select', function (e) {
        // Get the text of the selected option
        var selectedText = e.params.data.text;

        // Update the #ProductName hidden input field's value with the selected option's text
        $('#ProductName').val(selectedText);

        // Logging the selected text for verification
    });
});

$(function () {
    $('#selectvehicletype').select2({
        minimumResultsForSearch: Infinity // Disables the search box
    });

    // Listen for the 'select2:select' event to capture when an option is selected
    $('#selectvehicletype').on('select2:select', function (e) {
        // Get the text of the selected option
        var selectedText = e.params.data.text;

        // Update the #ProductName hidden input field's value with the selected option's text
        $('#VehicleName').val(selectedText);

        // Logging the selected text for verification
    });
});

function CalculateMethod() {
    var bags = document.getElementById('bags').value.trim();
    var bagsperkg = document.getElementById('bagsperkg').value.trim();
    var weightInput = document.getElementById('weight');
    var productprice = document.getElementById('productprice').value.trim();
    var totalprice = 0;

    // Check if both bags and bagsperkg are provided for multiplication
    var weight = (bags !== '' && bagsperkg !== '') ? bags * bagsperkg : parseFloat(weightInput.value) || 0;

    weightInput.value = weight === 0 ? '' : weight;

    var selectGrain = document.getElementById("selectgrain");
    var selectedValue = selectGrain.value;


    switch (selectedValue) {
        case "1":
        case "2":
        case "4":
        case "5":
        case "6":
        case "7":
        case "8":
            totalprice = weight * productprice / 20;
            break;

        case "3":
            totalprice = weight * productprice / 400;
            break;

        // Add more cases if needed
    }

    document.getElementById('totalprice').value = totalprice === 0 ? '' : totalprice.toFixed(2);
}

// Attach the event listener to the dropdown
document.getElementById("selectgrain").addEventListener("change", CalculateMethod);

// Attach the event listener to the input field for bags
document.getElementById("bags")?.addEventListener("input", function () {
    if (this.value.trim() !== '') {
        CalculateMethod();
    }
});

// Attach the event listener to the input field for bags per kg
document.getElementById("bagsperkg")?.addEventListener("input", function () {
    if (this.value.trim() !== '') {
        CalculateMethod();
    }
});

// Attach the event listener to the input field for product price
document.getElementById("productprice").addEventListener("input", CalculateMethod);


function CheckData() {
    var grainname = document.getElementById("selectgrain").value.trim();
    var customerName = document.getElementById("Customer").value.trim();
    var weight = document.getElementById("weight").value.trim();
    var productprice = document.getElementById("productprice").value.trim();
    var totalprice = document.getElementById("totalprice").value.trim();
    var vehicletype = document.getElementById("selectvehicletype").value.trim();
    var vehicleno = document.getElementById("vehicleno").value.trim();
    var drivername = document.getElementById("drivername").value.trim();

    var missingFields = [];

    if (!grainname) missingFields.push('Grain Type');
    if (!customerName) missingFields.push('Customer Name');
    if (!weight) missingFields.push('Weight');
    if (!productprice) missingFields.push('Product Price');
    if (!totalprice) missingFields.push('Total Price');
    if (!vehicletype) missingFields.push('Vehicle Type');
    if (!vehicleno) missingFields.push('Vehicle No');
    if (!drivername) missingFields.push('Driver Name');

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
        return false; // Ensure this return prevents further code execution in this branch
    } else {
        // Use SweetAlert2 to confirm with the user before submission
        confirmPurchaseInvoiceUpdation("/Invoice/UpdatePurchaseInvoiceDetails", customerName);
        return false; // Prevent form submission until confirmation and AJAX call are completed
    }
}

function confirmPurchaseInvoiceUpdation(redirectUrl, customerName) {
    Swal.fire({
        title: 'Confirm Invoice Update',
        text: `Update an invoice for ${customerName}?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, create it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: redirectUrl,
                type: 'POST',
                data: $("#PurchaseInvoiceForm").serialize(),
                success: function (response) {
                    if (response.success) {
                        sessionStorage.setItem('InvoiceStatus', 'Invoice Update successfully!');
                        window.location.href = response.redirectUrl; // Use the redirect URL from the response
                    } else {
                        // Handle the case where the server responded with success false.
                        toastr.error('Failed to create invoice: ' + response.message);
                        console.error(response.responseContent); // Detailed error for debugging
                    }
                },
                error: function (xhr, status, error) {
                    // General AJAX error handler
                    sessionStorage.setItem('InvoiceStatus', 'Error creating invoice. Please try again.');
                    toastr.error('Ajax error: ' + error);
                }

            });
        }
    });
}





function formatVehicleNo(input) {
    // Remove non-alphanumeric characters and convert to uppercase
    let formattedInput = input.value.replace(/[^a-zA-Z0-9]/g, '').toUpperCase();
    // Ensure the input does not exceed the expected length
    formattedInput = formattedInput.substr(0, 14);

    // Determine the appropriate format based on the number of letters after the first four characters
    if (/^[A-Z]{2}\d{2}[A-Z]{1}\d{4}$/.test(formattedInput)) {
        // Format: GJ-12-A-1234
        formattedInput = formattedInput.replace(/(\w{2})(\d{2})(\w{1})(\d{4})/, '$1-$2-$3-$4');
    } else if (/^[A-Z]{2}\d{2}[A-Z]{2}\d{4}$/.test(formattedInput)) {
        // Format: GJ-12-AB-1234
        formattedInput = formattedInput.replace(/(\w{2})(\d{2})(\w{2})(\d{4})/, '$1-$2-$3-$4');
    }

    // Update the input field with the formatted value
    input.value = formattedInput;

    // Validate the input format
    const regex = /^([A-Z]{2})-(\d{2})-([A-Z]{1,2})-(\d{4})$/;
    const errorMsg = document.getElementById('error-msg');
    errorMsg.textContent = regex.test(formattedInput) ? '' : 'Invalid format. Please follow the formats: GJ-12-A-1234 or GJ-12-AB-1234';
}
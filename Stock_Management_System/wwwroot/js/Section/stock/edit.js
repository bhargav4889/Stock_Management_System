﻿




    document.getElementById('selectgrain').addEventListener('change', function () {
        var selectedOption = this.options[this.selectedIndex];
        var selectedText = selectedOption.text;
        document.querySelector('input[name="ProductName"]').value = selectedText;


    });

    document.getElementById('selectvehicletype').addEventListener('change', function () {
        var selectedOption = this.options[this.selectedIndex];
        var selectedText = selectedOption.text; // This will be "ઘઉં" for the given example
        document.querySelector('input[name="VehicleName"]').value = selectedText;


    });








    window.onload = function () {

        CalculateMethod();


    };



    function CalculateMethod() {
        var bags = document.getElementById('bags').value.trim();
        var bagsperkg = document.getElementById('bagsperkg').value.trim();
        var weightInput = document.getElementById('weight');
        var productprice = document.getElementById('productprice').value.trim();

        var bagsNumeric = parseFloat(bags);
        var bagsPerKgNumeric = parseFloat(bagsperkg);
        var productPriceNumeric = parseFloat(productprice);
        var currentWeight = parseFloat(weightInput.value);

        // Only update weight if both inputs are valid positive numbers
        if (bagsNumeric > 0 && bagsPerKgNumeric > 0) {
            var calculatedWeight = bagsNumeric * bagsPerKgNumeric;
            weightInput.value = calculatedWeight;  // Update the weight input field
        } else if (!currentWeight) {
            // If current weight is NaN or 0, reset it
            weightInput.value = 0;
        }

        // Use the latest weight from the input field for total price calculation
        var weight = parseFloat(weightInput.value) || 0;
        var totalprice = 0;
        var selectedValue = document.getElementById("selectgrain").value;

        // Calculate the total price based on the grain selected
        switch (selectedValue) {
            case "1":
            case "2":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
                totalprice = weight * productPriceNumeric / 20;
                break;
            case "3":
                totalprice = weight * productPriceNumeric / 400;
                break;
            // Add more cases if needed
        }

        // Update the total price input field
        document.getElementById('totalprice').value = totalprice === 0 ? '' : totalprice.toFixed(2);
    }

    // Attach event listeners
    document.getElementById("selectgrain").addEventListener("change", CalculateMethod);
    document.getElementById("bags").addEventListener("input", CalculateMethod);
    document.getElementById("bagsperkg").addEventListener("input", CalculateMethod);
    document.getElementById("weight").addEventListener("input", CalculateMethod); // Ensure changes in weight directly also trigger recalculation
    document.getElementById("productprice").addEventListener("input", CalculateMethod);






function CheckData() {
    var grainname = document.getElementById("selectgrain").value;
    var customerName = document.getElementById("Customer").value;
    var weight = document.getElementById("weight").value;
    var productprice = document.getElementById("productprice").value;
    var totalprice = document.getElementById("totalprice").value;
    var vehicletype = document.getElementById("selectvehicletype").value;
    var vehicleno = document.getElementById("vehicleno").value;
    var drivername = document.getElementById("drivername").value;
    var newCustomerMessage = document.getElementById("newCustomerMessage").innerText.trim();
    var isNewCustomer = document.getElementById("newcustomertype").style.display !== 'none'; // Checks if new customer fields are visible

    var newCustomerType = document.getElementById("cttype").value; // Use .value for input fields
    var newCustomerCity = document.getElementById("customercity").value; // Use .value for input fields
    var newCustomerPhoneNo = document.getElementById("contactno").value; // Use .value for input fields

    var missingFields = [];

    // Always required fields
    if (!grainname) missingFields.push('Grain Type');
    if (!customerName) missingFields.push('Customer Name');
    if (!weight) missingFields.push('Weight');
    if (!productprice) missingFields.push('Product Price');
    if (!totalprice) missingFields.push('Total Price');
    if (!vehicletype) missingFields.push('Vehicle Type');
    if (!vehicleno) missingFields.push('Vehicle No');
    if (!drivername) missingFields.push('Driver Name');

    // Check new customer fields only if it's a new customer
    if (isNewCustomer) {
        if (!newCustomerType) missingFields.push('New Customer Type');
        if (!newCustomerCity) missingFields.push('New Customer City');
        if (!newCustomerPhoneNo) missingFields.push('New Customer Contact No');
    }

    // Display missing fields using toastr
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
        return false; // Prevent form submission
    } else {
        // Use SweetAlert2 to confirm with the user before submission
        confirmStockUpdation("/Stock/UpdateStockAndCustomerDetails", customerName);
        return false; // Prevent form submission until confirmation and AJAX call are completed
    }
}



    function confirmStockUpdation(addUrl, customerName) {
 
        Swal.fire({
            title: 'Are You Sure You Want To Update This Stock?',
            text: `${customerName}'s Stock Update?`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Update it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: addUrl,
                    type: 'POST',
                    data: $("#UpdateForm").serialize(),
                    success: function (response) {
                        sessionStorage.setItem('UpdateStatus', 'Stock Updated successfully!');
                        window.location.href = response.redirectUrl;
                    },
                    error: function () {
                        sessionStorage.setItem('ErrorMsg', 'Somthing Went Wrong !');
                        window.location.href = window.location.reload(); // Consider changing this to handle different errors
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



    $(function () {
        var isSuggestionSelected = false; // Flag to track if a suggestion has been chosen.

        // Function to toggle visibility of new customer fields and messages.
        function toggleNewCustomerFields(show) {
            $("#newCustomerMessage").toggle(show);
            $("#newcustomertype").toggle(show);
            $("#newcustomercity").toggle(show);
            $("#newcustomerphoneno").toggle(show);
        }

        // Hide new customer fields initially.
        toggleNewCustomerFields(false);

        $("#Customer").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "/Stock/GetBuyerCustomerData",
                    type: "POST",
                    dataType: "json",
                    data: { CustomerName: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.customerName + ' (' + item.customerType + ') - ' + item.customerAddress,
                                value: item.customerName,
                                id: item.customerId,
                                type: item.customerType
                            };
                        }));
                    }
                });
            },
            minLength: 1,
            response: function (event, ui) {
                var exactMatch = ui.content.some(item => item.value.toLowerCase() === $("#Customer").val().toLowerCase());
                toggleNewCustomerFields(!exactMatch && !isSuggestionSelected);
            },
            select: function (event, ui) {
                isSuggestionSelected = true;
                event.preventDefault(); // Avoid placing the label in the input.

                $("#Customer").val(ui.item.value);
                $("#CustomerId").val(ui.item.id);
                $("#cttype").val(ui.item.type).trigger('change');

                toggleNewCustomerFields(false);
            }
        });

        $("#Customer").on('input', function () {
            var enteredValue = $(this).val().trim();
            isSuggestionSelected = false; // Reset the flag whenever input changes.

            if (!enteredValue) {
                $("#CustomerId").val(0); // Set CustomerId to 0 if input is empty.
                toggleNewCustomerFields(false);
            } else {
                $("#Customer").autocomplete("search", enteredValue);
                if (!$("#Customer").autocomplete("widget").is(":visible")) {
                    $("#CustomerId").val(0); // Assume new customer if no match found.
                    toggleNewCustomerFields(true);
                }
            }
        });

        $(window).on('beforeunload', function () {
            toggleNewCustomerFields(false); // Reset form state on page unload.
        });
    });




document.getElementById('selectgrain').addEventListener('change', function () {
    var selectedOption = this.options[this.selectedIndex];
    var selectedText = selectedOption.text;
    document.querySelector('input[name="ProductName"]').value = selectedText;
});


$(function () {
    // Handle bank selection change
    $('#bankSelect').change(function () {
        var selectedOption = $(this).find('option:selected');
        var informationId = selectedOption.data('info-id'); // Retrieve the InformationId from data attribute
        $('#informationId').val(informationId); // Set the hidden input value
    });
});


$(function () {
    // Handle change event for the "Remain Bank" select dropdown
    $('#remainbankSelect').change(function () {
        var selectedOption = $(this).find('option:selected');
        var informationId = selectedOption.data('info-id'); // Retrieve the InformationId from data attribute
        $('#remainInformationId').val(informationId); // Update the hidden input field with the retrieved Information ID
    });
});


$(function () {
    function CalculateMethod() {
        var bags = document.getElementById('bags').value.trim();
        var bagsperkg = document.getElementById('bagsperkg').value.trim();
        var weightInput = document.getElementById('weight');
        var rate = parseFloat(document.getElementById('rate').value.trim()) || 0;
        var cgst = parseFloat(document.getElementById('CGST').value.trim()) || 0;
        var sgst = parseFloat(document.getElementById('SGST').value.trim()) || 0;
        var selectGrain = document.getElementById("selectgrain");
        var selectedValue = selectGrain.value;

        // Calculate weight either from bags or directly from the input
        var weight = (bags !== '' && bagsperkg !== '') ? bags * bagsperkg : parseFloat(weightInput.value) || 0;
        weightInput.value = weight === 0 ? '' : weight.toFixed(2);  // Display weight with two decimal places if needed

        // Initialize variables for pricing
        var priceBeforeTax = 0;
        var totalCGST = 0;
        var totalSGST = 0;
        var totalPrice = 0;

        // Only calculate prices if selectedValue matches certain conditions
        switch (selectedValue) {
            case "1":
            case "2":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "3":  // Combined case since all these cases do the same thing
                priceBeforeTax = weight * rate;
                totalCGST = priceBeforeTax * (cgst / 100);
                totalSGST = priceBeforeTax * (sgst / 100);
                totalPrice = priceBeforeTax + totalCGST + totalSGST;
                break;
            default:
                // Optionally handle cases where no valid selection is made
                break;
        }

        // Displaying the calculated prices
        document.getElementById('beforetaxtotal').value = priceBeforeTax.toFixed(2);
        document.getElementById('CGST_Total').value = totalCGST.toFixed(2);
        document.getElementById('SGST_Total').value = totalSGST.toFixed(2);
        document.getElementById('totalprice').value = totalPrice.toFixed(2);
    }

    // Event listeners
    document.getElementById("selectgrain").addEventListener("change", CalculateMethod);
    document.getElementById("bags").addEventListener("input", CalculateMethod);
    document.getElementById("bagsperkg").addEventListener("input", CalculateMethod);
    document.getElementById("weight").addEventListener("input", CalculateMethod);
    document.getElementById("rate").addEventListener("input", CalculateMethod);
    document.getElementById("CGST").addEventListener("input", CalculateMethod);
    document.getElementById("SGST").addEventListener("input", CalculateMethod);




});

$(function () {
    // Function to calculate Deducted Amount
    function calculateDeductedAmount() {
        var totalPrice = parseFloat($('#totalprice').val()) || 0;
        var receivedAmount = parseFloat($('#ReceivedAmount').val()) || 0;
        var discount = parseFloat($('#DiscountAmount').val()) || 0;
        var remainAmount = parseFloat($('#remainAmount').val()) || 0;

        // Ensure Received Amount does not exceed Total Price
        if (receivedAmount > totalPrice) {
            $('#ReceivedAmount').val(totalPrice.toFixed(2));
            receivedAmount = totalPrice;
        }

        // Calculate max Remain Amount as Total Price - Received Amount
        var maxRemainAmount = totalPrice - receivedAmount;

        // Ensure Remain Amount does not exceed max Remain Amount
        if (remainAmount > maxRemainAmount) {
            $('#remainAmount').val(maxRemainAmount.toFixed(2));
            remainAmount = maxRemainAmount;
        }

        // Applying the formula
        var deductedAmount = totalPrice - receivedAmount - discount - remainAmount;

        // Setting the value of Deducted Amount
        $('#DeductedAmount').val(deductedAmount.toFixed(2));
    }

    // Event listeners for input fields to recalculate on change
    $('#totalprice, #ReceivedAmount, #DiscountAmount, #remainAmount').on('input', function () {
        calculateDeductedAmount();
    });

    // Additional checks when user changes the value in Received and Remain Amount fields
    $('#ReceivedAmount').change(function () {
        var receivedAmount = parseFloat($(this).val()) || 0;
        var totalPrice = parseFloat($('#totalprice').val()) || 0;
        if (receivedAmount > totalPrice) {
            $(this).val(totalPrice.toFixed(2));
        }
        calculateDeductedAmount();
    });

    $('#remainAmount').change(function () {
        var remainAmount = parseFloat($(this).val()) || 0;
        var totalPrice = parseFloat($('#totalprice').val()) || 0;
        var receivedAmount = parseFloat($('#ReceivedAmount').val()) || 0;
        var maxRemainAmount = totalPrice - receivedAmount;
        if (remainAmount > maxRemainAmount) {
            $(this).val(maxRemainAmount.toFixed(2));
        }
        calculateDeductedAmount();
    });
});


$(function () {
    $('#selectpaymentmethod').change(function () {
        if ($(this).val() === 'BANK') {
            $('#bankSelection').show();
        } else {
            $('#bankSelection').hide();
        }
    });

    $(function () {
        $('#isFullAmountReceive').change(function () {
            var selectedValue = $(this).val();
            if (selectedValue === "NO") {
                // Show fields when "NO" is selected
                $('#RemainDate').show();
                $('#RemainAmount').show();
                $('#RemainPaymentMethod').show();
            } else {
                // Hide fields when "YES" is selected or nothing is selected
                $('#RemainDate').hide();
                $('#RemainAmount').hide();
                $('#RemainPaymentMethod').hide();
                $('#RemainbankSelection').hide();  // Also hide bank selection when not needed
            }
        });

        // Handle changes in the remain payment method to potentially show bank selection
        $('#selectremainpaymentmethod').change(function () {
            var paymentMethod = $(this).val();
            if (paymentMethod === "BANK") {
                $('#RemainbankSelection').show();
            } else {
                $('#RemainbankSelection').hide();
            }
        });

        // Initialize hidden state based on initial page load values
        $('#isFullAmountReceive').trigger('change');
        $('#selectremainpaymentmethod').trigger('change');
    });




    $(document).ready(function () {
        var isSuggestionSelected = false; // Flag to indicate if a suggestion has been selected.
        // Function to show or hide new customer fields and a message.
        function toggleNewCustomerFields(show) {
            $("#newCustomerMessage").toggle(show);
            $("#newcustomertype").toggle(show);
            $("#newcustomercity").toggle(show);
            $("#newcustomerphoneno").toggle(show);
        }
        // Initially hide the new customer message and fields.
        toggleNewCustomerFields(false);
        $("#Customer").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "/Sale/GetSellerCustomerData", // Correct URL to your controller action.
                    type: "POST",
                    dataType: "json",
                    data: {
                        CustomerName: request.term
                    },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.customerName + ' (' + item.customerType + ') - ' + item.customerAddress,
                                value: item.customerName,
                                id: item.customerId, // Customer ID.
                                type: item.customerType // Customer Type.
                            };
                        }));
                    }
                });
            },
            minLength: 1, // Minimum length before searching.
            response: function (event, ui) {
                var exactMatch = ui.content.some(item => item.value.toLowerCase() === $("#Customer").val().toLowerCase());
                toggleNewCustomerFields(!exactMatch && !isSuggestionSelected);
            },
            select: function (event, ui) {
                isSuggestionSelected = true;
                event.preventDefault(); // Prevent default to avoid placing label in the input.
                $("#Customer").val(ui.item.value); // Set the input to the customer name.
                $("#CustomerId").val(ui.item.id); // Update hidden field with selected customer ID.
                $("#cttype").val(ui.item.type).trigger('change'); // Set the customer type and trigger change for any attached handlers.
                toggleNewCustomerFields(false); // Hide new customer fields as a selection has been made.
            }
        });
        // Reset the flag and potentially hide new customer fields if the input is cleared or changed.
        $("#Customer").on('input', function () {
            var enteredValue = $(this).val().trim();
            if (!enteredValue) {
                toggleNewCustomerFields(false);
                isSuggestionSelected = false;
            } else {
                isSuggestionSelected = false;
                // Since we are still typing, check if any item matches exactly from the last search.
                $("#Customer").autocomplete("search", enteredValue);
            }
        });
        // Optionally, reset the form state when the page is refreshed or navigated away.
        $(window).on('beforeunload', function () {
            toggleNewCustomerFields(false);
        });
    }); 







}); // Correctly closed $(document).ready function

function setDateDefaultValue() {
    var today = new Date();
    var day = String(today.getDate()).padStart(2, '0');
    var month = String(today.getMonth() + 1).padStart(2, '0'); // January is 0!
    var year = today.getFullYear();
    var formattedDate = year + '-' + month + '-' + day;
    // Ensure the datepicker element exists and then set its value
    var datePicker = document.getElementById('datepicker');
    if (datePicker) {
        datePicker.value = formattedDate;
    }
}
// Run the function after the page has loaded to ensure the element is available
document.addEventListener('DOMContentLoaded', function () {
    setDateDefaultValue();

});




function CheckData() {
   
    var grainName = document.getElementById("selectgrain").value;
    var customerName = document.getElementById("Customer").value;
    var paymentMethod = document.getElementById("selectpaymentmethod").value;
    var bankSelect = document.getElementById("bankSelect").value;
    var weight = document.getElementById("weight").value;
    var rate = document.getElementById("rate").value;
    var totalPrice = document.getElementById("totalprice").value;
    var fullAmountReceived = document.getElementById("isFullAmountReceive").value;
    var receiveAmount = document.getElementById('ReceivedAmount').value;
   
    var remainAmount = document.getElementById("remainAmount").value;
    var remainPaymentMethod = document.getElementById("selectremainpaymentmethod").value;
    var remainBankSelect = document.getElementById("remainbankSelect").value;

    var isNewCustomer = document.getElementById("newcustomertype").style.display !== 'none';
    var newCustomerType = document.getElementById("cttype").value;
    var newCustomerCity = document.getElementById("customercity").value;
    var newCustomerPhoneNo = document.getElementById("contactno").value;

    var missingFields = [];

    // Check for missing input in essential fields
   
    if (!grainName) missingFields.push("Grain Name");
    if (!customerName) missingFields.push("Customer Name");
    if (!weight) missingFields.push("Weight");
    if (!rate) missingFields.push("Rate");
    if (!totalPrice) missingFields.push("Total Price");
    if (!receiveAmount) missingFields.push("Received Amount");
    if (!fullAmountReceived) missingFields.push("Select Full Payment Received or Not");
    if (paymentMethod === "BANK" && !bankSelect) missingFields.push("Bank Select for Payment");

    // Check for additional fields when full amount is not received
    if (fullAmountReceived === "NO") {
        
        if (!remainAmount) missingFields.push("Remaining Amount");
        if (!remainPaymentMethod) missingFields.push("Remaining Payment Method");
        if (remainPaymentMethod === "BANK" && !remainBankSelect) missingFields.push("Bank Select for Remaining Payment");
    }

    // Check new customer fields only if it's a new customer
    if (isNewCustomer) {
        if (!newCustomerType) missingFields.push('New Customer Type');
        if (!newCustomerCity) missingFields.push('New Customer City');
        if (!newCustomerPhoneNo) missingFields.push('New Customer Contact No');
    }

  

    // Display a collective error if any fields are missing
    if (missingFields.length > 0) {
        toastr.error(`Please fill out the following required fields: ${missingFields.join(', ')}`, {
            closeButton: true,
            progressBar: true,
            positionClass: 'toast-top-right',
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
    }

    // If all checks pass, optionally confirm before proceeding
    confirmSaleAddition("/Sale/InsertSale", customerName);
    return false; // Prevent form submission
}








function confirmSaleAddition(addUrl, customerName) {
    Swal.fire({
        title: 'Are You Sure You Want To Complete This Sale?',
        text: `${customerName}'s sale completion?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, complete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: addUrl, // Ensure addUrl is correctly defined
                type: 'POST',
                data: $("#SaleForm").serialize(), // Adjust to match your form's ID
                success: function (response) {
                    sessionStorage.setItem('SaleStatus', 'Sale Add successfully!');
                    window.location.href = response.redirectUrl; // Redirect using the URL from the server response
                    $("#SaleForm")[0].reset();  
                },
                error: function () {
                    sessionStorage.setItem('ErrorMsg', 'Something Went Wrong !');
                    window.location.href = window.location.reload(); // Adjust according to server response
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


$(function () {
    function formatBankOption(bank) {
        if (!bank.id || bank.text === '-- Select Bank --') return bank.text;
        var iconUrl = $(bank.element).data('icon');
        var $bank = $('<span><img src="' + iconUrl + '" class="img-flag" /> ' + bank.text + '</span>');
        return $bank;
    }

    $('#bankSelect').select2({
        minimumResultsForSearch: -1,
        templateResult: formatBankOption,
        templateSelection: formatBankOption
    });

    $('#remainbankSelect').select2({
        minimumResultsForSearch: -1,
        templateResult: formatBankOption,
        templateSelection: formatBankOption
    });



        
});
function confirmSaleDataReset() {
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
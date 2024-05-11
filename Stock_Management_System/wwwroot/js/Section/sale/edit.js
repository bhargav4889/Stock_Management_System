
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

        $('button[type="reset"]').click(function () {
            setTimeout(function () {
                $('#bankSelect').val("").trigger('change');
            }, 1);
            setTimeout(function () {
                $('#selectgrain').val("").trigger('change');
            }, 1);
            setTimeout(function () {
                $('#selectpaymentmethod').val("").trigger('change');
            }, 1);
            setTimeout(function () {
                $('#isFullAmountReceive').val("").trigger('change');
            }, 1);
        });
    }); // Correctly closed $(document).ready function


    $(function () {
        // Function to calculate Deducted Amount
        function calculateDeductedAmount() {
            var totalPrice = parseFloat($('#totalprice').val()) || 0;
            var receivedAmount = parseFloat($('#ReceivedAmount').val()) || 0;
            var discount = parseFloat($('#DiscountAmount').val()) || 0;
            var remainAmount = parseFloat($('#remainAmount').val()) || 0;

            // Applying the formula
            var deductedAmount = totalPrice - receivedAmount - discount - remainAmount;

            // Setting the value of Deducted Amount
            $('#DeductedAmount').val(deductedAmount.toFixed(2)); // toFixed(2) to format it to two decimal places
        }

        // Event listeners for the input fields
        $('#totalprice, #ReceivedAmount, #DiscountAmount, #remainAmount').on('input', function () {
            calculateDeductedAmount();
        });
    });

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
        $('#selectpaymentmethod').change(function () {
            var paymentMethod = $(this).val();
            if (paymentMethod == 'BANK') {
                $('#bankSelection').show();
            } else {
                $('#bankSelection').hide();
            }
        });

        if ($('#selectpaymentmethod').val() == 'BANK') {
            $('#bankSelection').show();
        }



        $('#isFullAmountReceive').change(function () {
            if ($(this).val() === "False") {
                $('#RemainDate').show();
                $('#RemainAmount').show();
                $('#RemainPaymentMethod').show();
            } else {
                $('#RemainDate').hide();
                $('#RemainAmount').hide();
                $('#RemainPaymentMethod').hide();
            }
        });



        if ($('#isFullAmountReceive').val() == 'True') {
            $('#RemainDate').show();
            $('#RemainAmount').show();
            $('#RemainPaymentMethod').show();
        }
        else{
            $('#RemainDate').hide();
            $('#RemainAmount').hide();
            $('#RemainPaymentMethod').hide();
        }

        $('#selectremainpaymentmethod').change(function () {
            if ($(this).val() === 'BANK') {
                $('#RemainbankSelection').show();
            } else {
                $('#RemainbankSelection').hide();
            }
        });

        if ($('#selectremainpaymentmethod').val() == 'BANK') {
            $('#RemainbankSelection').show();
        }

        function DateDefaultValue() {
            var today = new Date();
            var dd = String(today.getDate()).padStart(2, '0');
            var mm = String(today.getMonth() + 1).padStart(2, '0');
            var yyyy = today.getFullYear();
            today = yyyy + '-' + mm + '-' + dd;
            document.getElementById('datepicker').value = today;
        }

        window.onload = function () {
            DateDefaultValue();
            CalculateMethod();
        };

        function CalculateMethod() {
            var bags = document.getElementById('bags').value.trim();
            var bagsperkg = document.getElementById('bagsperkg').value.trim();
            var weightInput = document.getElementById('weight');
            var rate = document.getElementById('rate').value.trim();
            var totalprice = 0;
            var weight = (bags !== '' && bagsperkg !== '') ? bags * bagsperkg : parseFloat(weightInput.value) || 0;

            weightInput.value = weight === 0 ? '' : weight.toFixed(2);  // Ensuring weight is also shown with two decimals if needed

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
                case "3":  // Combined case since all these cases do the same thing
                    totalprice = weight * rate;
                    break;
            }

            document.getElementById('totalprice').value = totalprice === 0 ? '' : totalprice.toFixed(2);
        }

        document.getElementById("selectgrain").addEventListener("change", CalculateMethod);
        document.getElementById("bags").addEventListener("input", CalculateMethod);
        document.getElementById("bagsperkg").addEventListener("input", CalculateMethod);
        document.getElementById("rate").addEventListener("input", CalculateMethod);

    }); // Correctly closed $(document).ready function


    $(document).ready(function () {
        var isSuggestionSelected = false;

        function toggleNewCustomerFields(show) {
            $("#newCustomerMessage").toggle(show);
            $("#newcustomertype").toggle(show);
            $("#newcustomercity").toggle(show);
            $("#newcustomerphoneno").toggle(show);
        }

        toggleNewCustomerFields(false);

        $("#Customer").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Sale/GetSellerCustomerData',
                    type: "POST",
                    dataType: "json",
                    data: { CustomerName: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.customerName + ' (' + item.customerType + ') - ' + item.customerAddress,
                                value: item.customerName,
                                id: item.customerId,
                                type: item.customerType // Ensure 'customerType' is provided by your server
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
                event.preventDefault();
                $("#Customer").val(ui.item.value);
                $("#CustomerId").val(ui.item.id);
                $("#cttype").val(ui.item.type); // Update the customer type field
                toggleNewCustomerFields(false);
            }
        });
    });

    var toastrSettings = {
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
    };

    function CheckData() {
        var date = document.getElementById("datepicker").value;
        var grainName = document.getElementById("selectgrain").value;
        var customerName = document.getElementById("Customer").value;
        var paymentMethod = document.getElementById("selectpaymentmethod").value;
        var bankSelect = document.getElementById("bankSelect").value;
        var weight = document.getElementById("weight").value;
        var rate = document.getElementById("rate").value;
        var totalPrice = document.getElementById("totalprice").value;
        var fullAmountReceived = document.getElementById("isFullAmountReceive").value;
        var remainDate = document.getElementById("RemainDate").value;
        var remainAmount = document.getElementById("RemainAmount").value;
        var remainPaymentMethod = document.getElementById("selectremainpaymentmethod").value;
        var remainBankSelect = document.getElementById("remainbankSelect").value;

        if (!grainName || !customerName || !weight || !rate || !totalPrice || !date) {
            toastr.error('Please fill out all required fields.', toastrSettings);
            return false; // Prevent form submission
        }

        if (paymentMethod === "BANK" && !bankSelect) {
            toastr.error('Please select a bank.', toastrSettings);
            return false;
        }

        if (fullAmountReceived === "False" && (!remainDate || !remainAmount || !remainPaymentMethod || (remainPaymentMethod === "BANK" && !remainBankSelect))) {
            toastr.error('Please complete all remaining payment details.', toastrSettings);
            return false;
        }

        confirmAddition('/Sale/UpdateSaleDetails', customerName);
        return false; // Prevent form submission until confirmation and AJAX call are completed
    }

    function confirmAddition(addUrl, customerName) {
        Swal.fire({
            title: 'Are You Sure You Want To Update This Sale?',
            text: `${customerName}'s sale Updation?`,
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
                        sessionStorage.setItem('SaleStatus', 'Sale Update successfully!');
                        window.location.href = response.redirectUrl; // Redirect using the URL from the server response
                    },
                    error: function () {
                        sessionStorage.setItem('ErrorMsg', 'Somthing Went Wrong !');
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

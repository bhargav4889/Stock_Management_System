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
                url: '@Url.Action("Get_Buyer_Customer_Data", "Stock")', // Correct URL to your controller action.
                type: "POST",
                dataType: "json",
                data: { CustomerName: request.term },
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

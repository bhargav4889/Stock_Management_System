$(document).ready(function () {
    // Initialize Select2 instances
    $(".basic").select2({ tags: true });

    var formSmall = $(".form-small").select2({ tags: true });
    var nestedSelect = $(".nested").select2({ tags: true });
    var taggingSelect = $(".tagging").select2({ tags: true });
    var disabledResultsSelect = $(".disabled-results").select2();
    var placeholderSelect = $(".placeholder").select2({ placeholder: "Make a Selection", allowClear: true });
    var templatingSelect = $(".templating").select2({ templateSelection: formatState });
    var basicSingleSelect = $('.js-example-basic-single').select2();

    // Customize formSmall after it is fully initialized
    formSmall.on('select2:open', function (e) {
        formSmall.data('select2').$container.addClass('form-control-sm');
    });

    // Custom function for formatting Select2 state
    function formatState(state) {
        if (!state.id) {
            return state.text;
        }
        var baseClass = "flaticon-";
        var $state = $('<span><i class="' + baseClass + state.element.value.toLowerCase() + '" /> ' + state.text + '</i> </span>');
        return $state;
    }

    // Event handlers or additional custom logic can be added as needed

    // Example: Button click event
    $(".btn-submit").on("click", function () {
        // Your logic when the submit button is clicked
    });

    $(".btn-cancel").on("click", function () {
        // Your logic when the cancel button is clicked
    });
});

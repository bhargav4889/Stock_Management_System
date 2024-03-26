
    $(document).on("click", "#searchButton", function (event) {
        event.preventDefault(); // Prevent the default form submission behavior

    console.log("Button Clicked");

    // Get the selected grain name from the dropdown
    var Grain_Name = $("#graintype").val();

    console.log("Grain Name:", Grain_Name);

    $("tbody tr").each(function (index, element) {
                var row = $(element);
    var name = row.find("td:eq(3)").text();

    // Check if the name includes the selected Grain_Name
    if (!Grain_Name || Grain_Name === null || name.includes(Grain_Name)) {
        // Show the row if it meets the search criteria
        row.show();
                } else {
        // Hide the row if it doesn't meet the search criteria
        row.hide();
                }
            });

    // Optionally, you might want to keep the dropdown value
    // If you want to reset it, uncomment the line below
    $("#graintype").val('');
        });
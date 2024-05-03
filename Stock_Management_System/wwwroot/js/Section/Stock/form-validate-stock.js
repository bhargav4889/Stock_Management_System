function CheckData() {
    var date = document.getElementById("datepicker").value;
    var stockgrade = document.getElementById("stockgrade").value;
    var grainname = document.getElementById("selectgrain").value;
    var farmername = document.getElementById("Customer").value;
    var bags = document.getElementById("bags").value;
    var bagperkg = document.getElementById("bagsperkg").value;
    var weight = document.getElementById("weight").value;
    var productprice = document.getElementById("productprice").value;
    var totalprice = document.getElementById("totalprice").value;
    var vehicletype = document.getElementById("selectvehicletype").value;
    var vehicleno = document.getElementById("vehicleno").value;
    var drivername = document.getElementById("drivername").value;

    // Removed check for 'tolatname'
    if (!grainname || !farmername || !weight || !productprice || !totalprice || !vehicletype || !vehicleno || !drivername) {
        toastr.error('Please fill out all required fields.', {
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
        // If all fields are filled, use SweetAlert2 to confirm with the user before submission
        confirmStockAddition('@Url.Action("Add_Stock_Details", "Stock")', farmername);
        return false; // Prevent form submission until confirmation and AJAX call are completed
    }
}



function confirmStockAddition(addUrl, customerName) {
    Swal.fire({
        title: 'Are You Sure You Want To Add This Stock?',
        text: `${customerName}'s Stock Add?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, add it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: addUrl,  // Make sure addUrl is defined and not undefined
                type: 'POST',
                data: $("#AddStockForm").serialize(),  // Ensure this form ID matches your form
                success: function (response) {
                    // Assuming the server sends a JSON response with a redirectUrl field
                    sessionStorage.setItem('AddStatus', 'Stock added successfully!');
                    window.location.href = response.redirectUrl;  // Use the redirect URL from the response
                },
                error: function () {
                    sessionStorage.setItem('AddStatus', 'Error adding stock. Please try again.');
                    window.location.href = response.redirectUrl;  // Use the redirect URL from the response
                }
            });
        }
    });
}
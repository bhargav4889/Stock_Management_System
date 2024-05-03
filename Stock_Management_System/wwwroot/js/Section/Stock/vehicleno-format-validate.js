function formatVehicleNo(input) {
    let formattedInput = input.value.replace(/[^a-zA-Z0-9]/g, '').toUpperCase();
    formattedInput = formattedInput.substr(0, 14);
    formattedInput = formattedInput.replace(/(\w{2})(\d{2})(\w{2})(\d{4})/, '$1-$2-$3-$4');
    input.value = formattedInput;

    const regex = /^([A-Z]{2})-(\d{2})-([A-Z]{2})-(\d{4})$/;
    const errorMsg = document.getElementById('error-msg');
    errorMsg.textContent = regex.test(formattedInput) ? '' : 'Invalid format. Please follow the format: GJ-12-AB-1234';
}

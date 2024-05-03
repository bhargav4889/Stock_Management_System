function CalculateProductTotal() {
    var bags = document.getElementById('bags').value.trim();
    var bagsperkg = document.getElementById('bagsperkg').value.trim();
    var weightInput = document.getElementById('weight');
    var productprice = document.getElementById('productprice').value.trim();
    var totalprice = 0;

    var weight = (bags !== '' && bagsperkg !== '') ? bags * bagsperkg : parseFloat(weightInput.value) || 0;
    weightInput.value = weight === 0 ? '' : weight;

    var selectedValue = document.getElementById("selectgrain").value;
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
    }

    document.getElementById('totalprice').value = totalprice === 0 ? '' : totalprice;
}

function attachProductEventListeners() {
    document.getElementById("selectgrain").addEventListener("change", CalculateProductTotal);
    document.getElementById("bags").addEventListener("input", CalculateProductTotal);
    document.getElementById("bagsperkg").addEventListener("input", CalculateProductTotal);
    document.getElementById("productprice").addEventListener("input", CalculateProductTotal);
}

window.onload = attachProductEventListeners;

document.addEventListener('DOMContentLoaded', function () {
    var downloadButton = document.getElementById('downloadImage');
    var customerNameElement = document.getElementById('customerName');
    var productNameElement = document.getElementById('productName');
    var purchaseDateElement = document.getElementById('purchaseStockDate');

    if (downloadButton && customerNameElement && productNameElement && purchaseDateElement) {
        downloadButton.addEventListener('click', function () {
            html2canvas(document.querySelector('.table-responsive')).then(function (canvas) {
                // Creating a link to download
                var link = document.createElement('a');
                var customerName = customerNameElement.textContent || 'DefaultCustomer';
                var productName = productNameElement.textContent || 'DefaultProduct';
                var purchaseDate = purchaseDateElement.textContent || 'DefaultDate';

                link.download = `${purchaseDate}-${customerName}-${productName}-Stock-Information.png`;
                link.href = canvas.toDataURL();
                link.target = '_blank';
                link.click();
            }).catch(function (error) {
                console.log('Failed to capture canvas:', error);
            });
        });
    } else {
        console.log('Download button or required elements not found');
    }
});

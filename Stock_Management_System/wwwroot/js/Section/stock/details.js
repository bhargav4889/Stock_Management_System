document.addEventListener('DOMContentLoaded', function () {
    var downloadButton = document.getElementById('downloadsStockDetailsImage');
    var customerNameElement = document.getElementById('customerName');
    var productNameElement = document.getElementById('productName');
    var purchaseDateElement = document.getElementById('purchaseStockDate');

    if (downloadButton && customerNameElement && productNameElement && purchaseDateElement) {
        downloadButton.addEventListener('click', function () {
            html2canvas(document.querySelector('.table-responsive'), {
                scale: 3, // Ensuring high resolution
                useCORS: true // To handle external images if CORS policies allow
            }).then(function (canvas) {
                const ctx = canvas.getContext('2d');

                // Position where you want to draw additional images or text on the canvas
                const x = 730;
                const y = 275;

                // Example: Drawing an image on the canvas (you need an image element)
                // var image = new Image();
                // image.src = 'path/to/your/image.png';
                // image.onload = function() {
                //     ctx.drawImage(image, x, y);
                // }

                // Creating a link to download
                var link = document.createElement('a');
                var customerName = customerNameElement.textContent.trim().replace(/\s+/g, '-');
                var productName = productNameElement.textContent.trim().replace(/\s+/g, '-');
                var purchaseDate = purchaseDateElement.textContent.trim();

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

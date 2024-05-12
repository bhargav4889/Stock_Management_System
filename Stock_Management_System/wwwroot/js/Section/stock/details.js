document.addEventListener('DOMContentLoaded', function () {
    var downloadButton = document.getElementById('downloadImage');
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
                const watermark = new Image();
                watermark.crossOrigin = "Anonymous";
                watermark.src = 'https://localhost:7024/Images/Backimg.png';

                watermark.onload = () => {
                    const x = 730;
                    const y = 275;
                    const watermarkWidth = 342;
                    const watermarkHeight = 350;
                    ctx.globalAlpha = 0.4; // Setting opacity to 40%

                    ctx.drawImage(watermark, x, y, watermarkHeight, watermarkWidth);

                    // Creating a link to download
                    var link = document.createElement('a');
                    var customerName = customerNameElement.textContent.trim().replace(/\s+/g, '-');
                    var productName = productNameElement.textContent.trim().replace(/\s+/g, '-');
                    var purchaseDate = purchaseDateElement.textContent.trim();

                    link.download = `${purchaseDate}-${customerName}-${productName}-Stock-Information.png`;
                    link.href = canvas.toDataURL();
                    link.target = '_blank';
                    link.click();
                };

                watermark.onerror = () => {
                    console.error("Failed to load watermark image.");
                };
            }).catch(function (error) {
                console.log('Failed to capture canvas:', error);
            });
        });
    } else {
        console.log('Download button or required elements not found');
    }
});

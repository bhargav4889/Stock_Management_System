
document.addEventListener('DOMContentLoaded', function () {
    const downloadButton = document.getElementById('downloadImage');

    if (downloadButton) {
        downloadButton.addEventListener('click', function () {
            // Fetch values directly from the elements
            const dateElement = document.getElementById('purchaseDate');
            const customerNameElement = document.getElementById('customerName');
            const productNameElement = document.getElementById('productName');

            // Check if elements exist and fetch their text content, else use default values
            const date = dateElement ? dateElement.textContent.trim() : 'DefaultDate';
            const customerName = customerNameElement ? customerNameElement.textContent.trim() : 'DefaultCustomer';
            const productName = productNameElement ? productNameElement.textContent.trim() : 'DefaultProduct';

            // Format filename with the fetched values
            const filename = `${date}-${customerName}-${productName}-Purchase-Invoice-Information.png`;

            // Use html2canvas to capture the table area and download it as an image
            html2canvas(document.querySelector('.table-responsive')).then(canvas => {
                const link = document.createElement('a');
                link.download = filename;
                link.href = canvas.toDataURL('image/png');
                link.target = '_blank';
                link.click();
            }).catch(error => {
                console.error('Failed to capture canvas:', error);
            });
        });
    } else {
        console.error('Download button not found');
    }
});


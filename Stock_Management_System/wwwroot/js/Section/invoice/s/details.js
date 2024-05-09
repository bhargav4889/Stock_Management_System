document.addEventListener('DOMContentLoaded', function () {
    const downloadButton = document.getElementById('downloadImage'); // Make sure this ID matches your button's ID

    if (downloadButton) {
        downloadButton.addEventListener('click', function () {
            const dateElement = document.getElementById('salesInvoiceDate');
            const partyNameElement = document.getElementById('partyName');
            const productNameElement = document.getElementById('productName');
            const productBrandNameElement = document.getElementById('productBrandName');

            const date = dateElement ? dateElement.textContent.trim() : 'DefaultDate';
            const partyName = partyNameElement ? partyNameElement.textContent.trim() : 'DefaultParty';
            const productName = productNameElement ? productNameElement.textContent.trim() : 'DefaultProduct';
            const productBrandName = productBrandNameElement ? productBrandNameElement.textContent.trim() : 'DefaultBrand';

            const filename = `${date}-${partyName}-${productName}-${productBrandName}-Sale-Invoice-Information.png`;

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

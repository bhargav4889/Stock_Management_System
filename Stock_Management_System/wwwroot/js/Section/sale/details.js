document.addEventListener("DOMContentLoaded", function () {
    var downloadBtn = document.getElementById("downloadImage");
    if (downloadBtn) {
        downloadBtn.addEventListener("click", function () {
            html2canvas(document.querySelector(".table-responsive"), {
                scale: 3,
                useCORS: true
            }).then(canvas => {
                const ctx = canvas.getContext("2d");

                const x = 720;
                const y = 415;

                // Ensure you have an image element or data to draw on the canvas
                // For demonstration purposes, this part is commented out
                // var image = new Image();
                // image.src = 'path/to/your/image.png';
                // image.onload = function() {
                //     ctx.drawImage(image, x, y);
                // }

                // Creating a link to download
                const saleDate = document.getElementById('saleDate')?.innerText || '';
                const customerName = document.getElementById('customerName')?.innerText.replace(/\s+/g, '-') || '';
                const brandName = document.getElementById('brandName')?.innerText.replace(/\s+/g, '-') || '';
                const fileName = `${saleDate}-${customerName}-${brandName}-Sale-Information.png`;

                const image = canvas.toDataURL("image/png").replace("image/png", "image/octet-stream");
                const link = document.createElement('a');
                link.download = fileName;
                link.href = image;
                link.click();

                // Clean up the link element
                link.remove();

            }).catch(error => {
                console.error('Failed to capture canvas:', error);
            });
        });
    } else {
        console.error("Download button not found.");
    }
});

document.addEventListener("DOMContentLoaded", function () {
    var downloadBtn = document.getElementById("downloadImage");
    if (downloadBtn) {
        downloadBtn.addEventListener("click", function () {
            html2canvas(document.querySelector(".table-responsive"), {
                scale: 3,
                useCORS: true
            }).then(canvas => {
                const ctx = canvas.getContext("2d");
                const watermark = new Image();

              

                watermark.crossOrigin = "Anonymous";
                watermark.src = 'https://stock-manage-api-shree-ganesh-agro-ind.somee.com/Images/Backimg.png';

                watermark.onload = () => {
                    const x = 720;
                    const y = 415;

                
                    let watermarkWidth = 342;
                    let watermarkHeight = 350;
                    ctx.globalAlpha = 0.4; // 40% opacity

                    ctx.drawImage(watermark, x, y, watermarkHeight, watermarkWidth);

                    // Retrieve values using IDs
                    const saleDate = document.getElementById('saleDate')?.innerText || '';
                    const customerName = document.getElementById('customerName')?.innerText.replace(/\s+/g, '-') || '';
                    const brandName = document.getElementById('brandName')?.innerText.replace(/\s+/g, '-') || '';
                    const fileName = `${saleDate}-${customerName}-${brandName}-Sale-Information.png`;

                    const image = canvas.toDataURL("image/png").replace("image/png", "image/octet-stream");
                    let link = document.createElement('a');
                    link.download = fileName;
                    link.href = image;
                    link.click();
                };

                watermark.onerror = () => {
                    console.error("Failed to load watermark image.");
                };
            });
        });
    } else {
        console.error("Download button not found.");
    }
});

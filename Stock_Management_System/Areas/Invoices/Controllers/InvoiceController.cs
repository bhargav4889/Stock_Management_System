using Microsoft.AspNetCore.Mvc;
using System.Data;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Newtonsoft.Json;
using static Stock_Management_System.Areas.Invoices.Models.InvoiceModel;
using System.Text;
using System.Drawing;
using Stock_Management_System.Areas.Invoices.Models;
using Stock_Management_System.UrlEncryption;
using ClosedXML.Excel;
using Stock_Management_System.API_Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stock_Management_System.All_DropDowns;
using Font = iTextSharp.text.Font;
using System.Net.Http.Json;
using System.Net.Http;

namespace Stock_Management_System.Areas.Invoices.Controllers
{

    [Area("Invoices")]
    [Route("~/[controller]/[action]")]
    public class InvoiceController : Controller
    {

        #region Section: Constructor and Configuration

        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://localhost:7024/api");

        public readonly HttpClient _Client;

        private readonly Api_Service api_Service = new Api_Service();

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceController"/> class.
        /// </summary>
        /// <param name="configuration">Configuration settings to be used by the controller.</param>
        /// <remarks>
        /// Sets up the HttpClient with a base address and initializes the Api_Service.
        /// </remarks>
        public InvoiceController(IConfiguration configuration)
        {
            Configuration = configuration;

            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
        }

        #endregion

        #region Section: Dropdown Function

        /// <summary>
        /// Populates dropdown lists for the stock views.
        /// </summary>

        public async Task PopulateDropdownLists()
        {
            DropDown_Model dropDown_Model = await new DropDowns_Class().GetAllDropdownsAsync();
            if (dropDown_Model != null)
            {
                ViewBag.Products = new SelectList(dropDown_Model.Products_DropDowns_List, "ProductId", "ProductNameInGujarati");
                ViewBag.ProductsInEnglish = new SelectList(dropDown_Model.Products_DropDowns_List, "ProductId", "ProductNameInEnglish");
                ViewBag.ProductGrade = new SelectList(dropDown_Model.Products_Grade_DropDowns_List, "ProductGradeId", "ProductGrade");
                ViewBag.Vehicle = new SelectList(dropDown_Model.Vehicle_DropDowns_List, "VehicleId", "VehicleName");
            }
        }

        #endregion

        #region Area: Purchase Invoice

        #region Method: Create Purchase Invoice

        public async Task<IActionResult> CreatePurchaseInvoice()
        {
            await PopulateDropdownLists();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InsertPurchaseInvoice(Purchase_Invoice_Model purchase_InvoiceModel)
        {
            var jsonContent = JsonConvert.SerializeObject(purchase_InvoiceModel);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Invoices/AddPurchaseInvoice", stringContent);

            if (response.IsSuccessStatusCode)
            {
                var token = Guid.NewGuid().ToString();
                HttpContext.Session.Set($"InvoiceData_{token}", Encoding.UTF8.GetBytes(jsonContent));

                // Using "Url.Action" to generate the URL for redirecting to the preview page
                var redirectUrl = Url.Action("PurchaseInvoicePreview", "Invoice", new
                {
                    preview = token
                });

                // Return a JSON object with the redirect URL
                return Json(new
                {
                    success = true,
                    redirectUrl = redirectUrl
                });
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Log or display the responseContent for more detailed error information

                // Returning a status code and a message in JSON format
                return Json(new
                {
                    success = false,
                    message = "Failed to create invoice.",
                    responseContent
                });
            }
        }

        #endregion

        #region Method: Delete Purchase Invoice

        public IActionResult DeletePurchaseInvoice(string Invoice_ID)
        {
            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Invoices/DeletePurchaseInvoice?Purchase_Invoice_ID={UrlEncryptor.Decrypt(Invoice_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Delete Successfully!",
                    redirectUrl = Url.Action("PurchaseInvoices")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Error. Please try again."
                });
            }
        }

        #endregion

        #region Section: Update Purchase Invoice

        public async Task<IActionResult> EditPurchaseInvoice(string Invoice_ID)
        {

            await PopulateDropdownLists();

            Purchase_Invoice_Model purchase_Invoice_Model = new Purchase_Invoice_Model();

            purchase_Invoice_Model = await api_Service.Model_Of_Data_Display<Purchase_Invoice_Model>("Invoices/GetPurchaseInvoiceByID", Convert.ToInt32(UrlEncryptor.Decrypt(Invoice_ID)));

            return View(purchase_Invoice_Model);
        }

        public async Task<IActionResult> UpdatePurchaseInvoiceDetails(Purchase_Invoice_Model purchase_Invoice_Details)
        {

            var jsonContent = JsonConvert.SerializeObject(purchase_Invoice_Details);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _Client.PutAsync($"{_Client.BaseAddress}/Invoices/UpdatePurchaseInvoice", stringContent);

            if (response.IsSuccessStatusCode)
            {
                var token = Guid.NewGuid().ToString();
                HttpContext.Session.Set($"InvoiceData_{token}", Encoding.UTF8.GetBytes(jsonContent));

                // Using "Url.Action" to generate the URL for redirecting to the preview page
                var redirectUrl = Url.Action("PurchaseInvoicePreview", "Invoice", new
                {
                    preview = token
                });

                // Return a JSON object with the redirect URL
                return Json(new
                {
                    success = true,
                    redirectUrl = redirectUrl
                });
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Log or display the responseContent for more detailed error information

                // Returning a status code and a message in JSON format
                return Json(new
                {
                    success = false,
                    message = "Failed to create invoice.",
                    responseContent
                });
            }

        }

        #endregion

        #region Section: Show All Purchase Invoices

        public async Task<IActionResult> PurchaseInvoices()
        {

            await PopulateDropdownLists();

            List<Purchase_Invoice_Model> purchase_Invoices = await api_Service.List_Of_Data_Display<Purchase_Invoice_Model>("Invoices/GetAllPurchaseInvoices");

            return View(purchase_Invoices);

        }

        #endregion

        #region Section: Preview Purchase Invoice And Download

        public IActionResult PurchaseInvoicePreview(string preview)
        {
            byte[]? storedData = HttpContext.Session.Get($"InvoiceData_{preview}");

            if (string.IsNullOrEmpty(preview) || storedData == null)
            {
                return RedirectToAction("CreatePurchaseInvoice");
            }

            var invoiceModel = JsonConvert.DeserializeObject<Purchase_Invoice_Model>(Encoding.UTF8.GetString(storedData));

            // Check if invoiceModel is not null before using it

            if (invoiceModel == null)
            {
                // Handle the case where deserialization fails
                return RedirectToAction("CreatePurchaseInvoice");
            }

            // Assuming you have a method to generate PDF content
            FileContentResult? pdfFileResult = PurchaseInvoicePDF(invoiceModel);

            // Check if pdfFileResult is not null before using it
            if (pdfFileResult == null)
            {
                // Handle the case where PDF generation fails
                return RedirectToAction("CreatePurchaseInvoice");
            }

            byte[] pdf = pdfFileResult.FileContents;

            MutipleModel model = new MutipleModel();

            model.Pdf = pdf;

            model.purchase_Invoice = invoiceModel;

            // Pass the PDF content to the view
            return View(model);
        }

        public IActionResult PurchaseInvoiceDownload(int Invoice_ID)
        {

            Purchase_Invoice_Model purchase_Invoices = new Purchase_Invoice_Model();

            HttpResponseMessage response = _Client.GetAsync($"{_Client.BaseAddress}/Invoices/GetPurchaseInvoiceByID/{Invoice_ID}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic? jsonObject = JsonConvert.DeserializeObject(data);
                var DataObject = jsonObject?.data;
                var extractedDtaJson = JsonConvert.SerializeObject(DataObject, Formatting.Indented);
                purchase_Invoices = JsonConvert.DeserializeObject<Purchase_Invoice_Model>(extractedDtaJson);

            }

            var uniqueToken = Guid.NewGuid().ToString();

            HttpContext.Session.Set($"InvoiceData_{uniqueToken}", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(purchase_Invoices)));

            return RedirectToAction("PurchaseInvoicePreview", new
            {
                preview = uniqueToken
            });

        }

        #endregion

        #region Section: Purchase Invoice Details By Invoice ID

        public async Task<IActionResult> PurchaseInvoiceDetailsView(string Invoice_ID)
        {

            Purchase_Invoice_Model purchase_Invoices = new Purchase_Invoice_Model();

            purchase_Invoices = await api_Service.Model_Of_Data_Display<Purchase_Invoice_Model>("Invoices/GetPurchaseInvoiceByID", Convert.ToInt32(UrlEncryptor.Decrypt(Invoice_ID)));

            return View(purchase_Invoices);
        }

        #endregion

        #region Section: Create Purchase Invoice PDF

        public FileContentResult PurchaseInvoicePDF(Purchase_Invoice_Model invoiceModel)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Create the document
                using (Document document = new Document(iTextSharp.text.PageSize.A4, 0, 0, 0, 0))
                {
                    // Create PdfWriter instance
                    using (PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream))
                    {
                        // Open the document
                        document.Open();

                        // Get PdfContentByte
                        PdfContentByte contentByte = pdfWriter.DirectContent;

                        BaseFont defaultfont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                        iTextSharp.text.Font dfont = new iTextSharp.text.Font(defaultfont, 18);

                        BaseFont boldfont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.WINANSI, BaseFont.EMBEDDED);
                        iTextSharp.text.Font bfont = new iTextSharp.text.Font(boldfont, 18);

                        BaseFont inrfont = BaseFont.CreateFont("https://localhost:7024/Fonts/Rupee.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, true);
                        iTextSharp.text.Font ifont = new iTextSharp.text.Font(inrfont, 18);

                        BaseFont gujaratifont = BaseFont.CreateFont("https://localhost:7024/Fonts/NotoSansGujarati.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, true);
                        iTextSharp.text.Font font = new iTextSharp.text.Font(gujaratifont, 18);
                        font.Color = new BaseColor(Color.Red);

                        contentByte.BeginText();

                        contentByte.SetFontAndSize(gujaratifont, 14);
                        contentByte.SetTextMatrix(250, 820); // X, Y position
                        contentByte.ShowText("|| ગણેશાય નમઃ ||"); //header

                        contentByte.SetFontAndSize(boldfont, 13);
                        contentByte.SetTextMatrix(460, 810); // X, Y position
                        contentByte.ShowText("Mo: +91 98254 22091");

                        contentByte.SetFontAndSize(boldfont, 13);
                        contentByte.SetTextMatrix(460, 790); // X, Y position
                        contentByte.ShowText("Mo: +91 94277 23092");

                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance("https://localhost:7024/Images/Logo.png");

                        iTextSharp.text.Image backimage = iTextSharp.text.Image.GetInstance("https://localhost:7024/Images/Backimg.png");

                        image.ScaleToFit(60, 60); // Adjust width and height

                        image.SetAbsolutePosition(275, 765); // Position Of Image

                        document.Add(image); // Add the image to the PDF

                        backimage.ScaleToFit(300, 300); // Adjust width and height 

                        backimage.SetAbsolutePosition(145, 230); // Position Of Image

                        document.Add(backimage); // Add the image to the PDF

                        contentByte.SetFontAndSize(boldfont, 20);
                        contentByte.SetTextMatrix(155, 725); // X, Y position
                        contentByte.ShowText("Shree Ganesh Agro Industries"); // name

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(165, 690); // X, Y position
                        contentByte.ShowText("Address:- GIDC Plot No.36,Porbandar Road,"); // address

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(260, 670); // X, Y position
                        contentByte.ShowText("Upleta(Dist.Rajkot)360-490."); // address of city

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(275, 640); // X, Y position
                        contentByte.ShowText("Invoice"); // invoice

                        DateTime? invoiceDate = invoiceModel.PurchaseInvoiceDate;
                        DateTime? onlyDate = invoiceDate?.Date;

                        string? formattedDate = onlyDate?.ToString("dd-MM-yyyy");

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(450, 600); // X, Y position
                        contentByte.ShowText("Date: " + formattedDate); // date

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(25, 565); // X, Y position
                        contentByte.ShowText("Farmer Name: -"); // farmer

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(150, 568); // X, Y position
                        contentByte.ShowText(invoiceModel.CustomerName); // farmer name

                        // -- Table Content -- //

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(25, 515); // X, Y position
                        contentByte.ShowText("No."); // No.

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(65, 515); // X, Y position
                        contentByte.ShowText("Product Name"); // product name

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(190, 515); // X, Y position
                        contentByte.ShowText("Bags"); // bags 

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(245, 525); // X, Y position
                        contentByte.ShowText("Bags"); // pbags +

                        //+//

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(245, 505); // X, Y position
                        contentByte.ShowText("Per Kg"); // per kg

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(310, 515); // X, Y position
                        contentByte.ShowText("Weight"); // bags 

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(385, 525); // X, Y position
                        contentByte.ShowText("Product"); // product +

                        //+//

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(385, 508); // X, Y position
                        contentByte.ShowText("Price(  )"); // pr price

                        //+//
                        contentByte.SetFontAndSize(inrfont, 15);
                        contentByte.SetTextMatrix(427, 506); // X, Y position
                        contentByte.ShowText("K"); // ₹ symbol added

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(470, 515); // X, Y position
                        contentByte.ShowText("Total Price(  )"); // total price 

                        //+//
                        contentByte.SetFontAndSize(inrfont, 15);
                        contentByte.SetTextMatrix(552, 515); // X, Y position
                        contentByte.ShowText("K"); // ₹ symbol added

                        // ---- Details Invoice ---= //

                        contentByte.SetFontAndSize(boldfont, 13);
                        contentByte.SetTextMatrix(27, 475); // X, Y position
                        contentByte.ShowText("1."); // No.of items +

                        contentByte.SetFontAndSize(gujaratifont, 14);
                        contentByte.SetTextMatrix(90, 475); // X, Y position
                        contentByte.ShowText(Convert.ToString(invoiceModel.ProductName)); //  Product Type

                        contentByte.SetFontAndSize(boldfont, 13);
                        contentByte.SetTextMatrix(200, 475); // X, Y position

                        contentByte.ShowText(!string.IsNullOrWhiteSpace(Convert.ToString(invoiceModel.Bags)) ? Convert.ToString(invoiceModel.Bags) : "--");

                        contentByte.SetFontAndSize(boldfont, 13);
                        contentByte.SetTextMatrix(260, 475); // X, Y position
                        contentByte.ShowText(!string.IsNullOrWhiteSpace(Convert.ToString(invoiceModel.BagPerKg)) ? Convert.ToString(invoiceModel.BagPerKg) : "--");

                        contentByte.SetFontAndSize(boldfont, 13);
                        contentByte.SetTextMatrix(316, 475); // X, Y position
                        contentByte.ShowText(invoiceModel.TotalWeight.ToString()); // weight

                        contentByte.SetFontAndSize(boldfont, 13);
                        contentByte.SetTextMatrix(400, 475); // X, Y position
                        contentByte.ShowText(invoiceModel.ProductPrice.ToString()); // product price

                        contentByte.SetFontAndSize(boldfont, 13);
                        contentByte.SetTextMatrix(490, 475); // X, Y position
                        contentByte.ShowText(invoiceModel.TotalPrice.ToString()); // total price

                        // ------xxxxxxxxxxx------ //

                        //  -- footer table content -- //

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(490, 105); // X, Y position
                        contentByte.ShowText(invoiceModel.TotalPrice.ToString()); // final total

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(355, 105); // X, Y position
                        contentByte.ShowText("Final Total(  )"); // final total title

                        contentByte.SetFontAndSize(inrfont, 15);
                        contentByte.SetTextMatrix(435, 105); // X, Y position
                        contentByte.ShowText("K"); // ₹ symbol added

                        // --- main footwer -- //

                        contentByte.SetFontAndSize(boldfont, 14);
                        contentByte.SetTextMatrix(490, 60); // X, Y position
                        contentByte.ShowText("(Signature)."); // sign.

                        contentByte.SetFontAndSize(boldfont, 14);
                        contentByte.SetTextMatrix(445, 40); // X, Y position
                        contentByte.ShowText("Bhavesh S. Kachhela"); // sign name

                        contentByte.SetFontAndSize(boldfont, 14);
                        contentByte.SetTextMatrix(225, 15); // X, Y position
                        contentByte.ShowText("Thanks For Selling Us!."); // thanks you label 

                        contentByte.EndText(); // ---- Text End ---- //

                        // ---- Design Format Of Invoice ---- //

                        //-- Name Above line --//
                        contentByte.MoveTo(0, 750); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(600, 750); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        //-- Name Below line --//
                        contentByte.MoveTo(0, 710); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(600, 710); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        //-- Invoice Above line --//
                        contentByte.MoveTo(0, 660); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(600, 660); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        //-- Invoice Below line --//
                        contentByte.MoveTo(0, 630); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(600, 630); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        //-- Farmer Name Right line --//
                        contentByte.MoveTo(145, 562); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(450, 562); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        // -- Table Design -- //

                        // ------ //

                        contentByte.MoveTo(20, 540); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(575, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*    */
                        /*
                         * |
                         * | |||| 1
                         * |
                         * |
                         */
                        /**/

                        contentByte.MoveTo(575, 95); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(575, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();
                        /*
                         */
                        /*
                         * |
                         * ||||| 2
                         * |
                         * |
                         */
                        /**/

                        contentByte.MoveTo(20, 125); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(20, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        // ------ // - 2 

                        contentByte.MoveTo(20, 500); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(575, 500); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*         */
                        /*
                         * |
                         * ||||| 3
                         * |
                         * |
                         */
                        /**/

                        contentByte.MoveTo(55, 125); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(55, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*  */
                        /*
                         * |
                         * ||||| 4
                         * |
                         * |
                         */
                        /**/

                        contentByte.MoveTo(180, 125); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(180, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /**/
                        /*
                         * |
                         * ||||| 5
                         * |
                         * |
                         */
                        /**/

                        contentByte.MoveTo(240, 125); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(240, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*  */
                        /*
                         * |
                         * ||||| 6
                         * |
                         * |
                         */
                        /**/

                        contentByte.MoveTo(300, 125); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(300, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*  */
                        /*
                         * |
                         * ||||| 7
                         * |
                         * |
                         */
                        /**/

                        contentByte.MoveTo(375, 125); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(375, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /**/
                        /*
                         * |
                         * ||||| 8
                         * |
                         * |
                         */
                        /**/

                        contentByte.MoveTo(460, 95); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(460, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        // ------ // - 3

                        contentByte.MoveTo(20, 125); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(575, 125); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        // ------ // - 4

                        contentByte.MoveTo(460, 95); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(575, 95); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        // Close the document
                        document.Close();
                    }
                }

                return File(memoryStream.ToArray(), "application/pdf");

            }
        }

        #endregion

        #region Section: Download Statement PDF & EXCEL

        public async Task<IActionResult> GeneratePurchaseInvoicesPDFStatement()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/PurchaseInvoiceStatementPDF");

            if (response.IsSuccessStatusCode)
            {
                // Extract filename from Content-Disposition header
                var contentDisposition = response.Content.Headers.ContentDisposition;
                string filename = contentDisposition?.FileName;

                var pdfContent = await response.Content.ReadAsByteArrayAsync();
                return File(pdfContent, "application/pdf", filename);
            }
            else
            {
                // Handle error or return an error response
                return BadRequest("Could not generate PDF.");
            }

        }

        public async Task<IActionResult> GeneratePurchaseInvoicesEXCELStatement()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/PurchaseInvoiceStatementEXCEL");

            if (response.IsSuccessStatusCode)
            {
                // Extract filename from Content-Disposition header
                var contentDisposition = response.Content.Headers.ContentDisposition;
                string filename = contentDisposition?.FileName;

                var pdfContent = await response.Content.ReadAsByteArrayAsync();
                return File(pdfContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            else
            {
                // Handle error or return an error response
                return BadRequest("Could not generate Excel.");
            }
        }

        #endregion

        #endregion

        #region Area: Sale Invoice

        #region Section: Create Sale Invoice

        /// <summary>
        /// Initiates the creation of a new sales invoice by populating necessary dropdown lists.
        /// </summary>
        /// <returns>Returns a view for creating a new sales invoice.</returns>
        public async Task<IActionResult> CreateSaleInvoice()
        {
            await PopulateDropdownLists();
            return View();
        }

        /// <summary>
        /// Inserts a new sales invoice into the database after receiving a POST request with the invoice data.
        /// </summary>
        /// <param name="sales_Invoice">The sales invoice model containing all the invoice details.</param>
        /// <returns>JSON result indicating success or failure along with a redirect URL for preview on success, or error message on failure.</returns>

        [HttpPost]
        public async Task<IActionResult> InsertSaleInvoice(Sales_Invoice_Model sales_Invoice)
        {
            var jsonContent = JsonConvert.SerializeObject(sales_Invoice);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Invoices/AddSaleInvoice", stringContent);

            if (response.IsSuccessStatusCode)
            {
                var token = Guid.NewGuid().ToString();
                HttpContext.Session.Set($"InvoiceData_{token}", Encoding.UTF8.GetBytes(jsonContent));

                // Using "Url.Action" to generate the URL for redirecting to the preview page
                var redirectUrl = Url.Action("SaleInvoicePreview", "Invoice", new
                {
                    preview = token
                });

                // Return a JSON object with the redirect URL
                return Json(new
                {
                    success = true,
                    redirectUrl = redirectUrl
                });
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Log or display the responseContent for more detailed error information

                // Returning a status code and a message in JSON format
                return Json(new
                {
                    success = false,
                    message = "Failed to create invoice.",
                    responseContent
                });
            }
        }

        #endregion

        #region Section: Sale Invoices

        public async Task<IActionResult> SaleInvoices()
        {

            await PopulateDropdownLists();

            List<Sales_Invoice_Model> sales_Invoices = await api_Service.List_Of_Data_Display<Sales_Invoice_Model>("Invoices/GetAllSaleInvoices");

            return View(sales_Invoices);

        }

        #endregion

        #region Section: Delete Sale Invoice

        public IActionResult DeleteSaleInvoice(string Invoice_ID)
        {
            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Invoices/DeleteSaleInvoice?Sale_Invoice_ID={UrlEncryptor.Decrypt(Invoice_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Delete Successfully!",
                    redirectUrl = Url.Action("SaleInvoices")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Error. Please try again.",
                    redirectUrl = Url.Action("SaleInvoices")
                });
            }
        }

        #endregion

        #region Section: Update Sale Invoice

        public async Task<IActionResult> EditSaleInvoice(string Invoice_ID)
        {

            await PopulateDropdownLists();

            Sales_Invoice_Model sales_Invoice_Model = await api_Service.Model_Of_Data_Display<Sales_Invoice_Model>("Invoices/GetSaleInvoiceByID", Convert.ToInt32(UrlEncryptor.Decrypt(Invoice_ID)));

            return View(sales_Invoice_Model);
        }

        public async Task<IActionResult> UpdateSaleInvoiceDetails(Sales_Invoice_Model sales_Invoice_Model)
        {

            var jsonContent = JsonConvert.SerializeObject(sales_Invoice_Model);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _Client.PutAsync($"{_Client.BaseAddress}/Invoices/UpdateSaleInvoice", stringContent);

            if (response.IsSuccessStatusCode)
            {
                var token = Guid.NewGuid().ToString();
                HttpContext.Session.Set($"InvoiceData_{token}", Encoding.UTF8.GetBytes(jsonContent));

                // Using "Url.Action" to generate the URL for redirecting to the preview page
                var redirectUrl = Url.Action("SaleInvoicePreview", "Invoice", new
                {
                    preview = token
                });

                // Return a JSON object with the redirect URL
                return Json(new
                {
                    success = true,
                    redirectUrl = redirectUrl
                });
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Log or display the responseContent for more detailed error information

                // Returning a status code and a message in JSON format
                return Json(new
                {
                    success = false,
                    message = "Failed to create invoice.",
                    responseContent
                });
            }

        }

        #endregion

        #region Section: Sale Invoice Details Show By Invoice ID

        public async Task<IActionResult> SaleInvoiceDetailsView(string Invoice_ID)
        {
            if (HttpContext.Request.Headers["Referer"].ToString() == "")
            {
                return RedirectToAction("SaleInvoices");
            }

            Sales_Invoice_Model sales_Invoice_Model = new Sales_Invoice_Model();

            sales_Invoice_Model = await api_Service.Model_Of_Data_Display<Sales_Invoice_Model>("Invoices/GetSaleInvoiceByID", Convert.ToInt32(UrlEncryptor.Decrypt(Invoice_ID)));

            return View(sales_Invoice_Model);

        }

        #endregion

        #region Section: Preview Sale Invoice And Download

        public IActionResult SaleInvoicePreview(string preview)
        {
            byte[]? storedData = HttpContext.Session.Get($"InvoiceData_{preview}");

            if (string.IsNullOrEmpty(preview) || storedData == null)
            {
                return RedirectToAction("CreateSaleInvoice");
            }

            var invoiceModel = JsonConvert.DeserializeObject<Sales_Invoice_Model>(Encoding.UTF8.GetString(storedData));

            // Check if invoiceModel is not null before using it
            if (invoiceModel == null)
            {
                // Handle the case where deserialization fails
                return RedirectToAction("CreateSaleInvoice");
            }

            // Assuming you have a method to generate PDF content
            FileContentResult? pdfFileResult = SaleInvoicePDF(invoiceModel);

            // Check if pdfFileResult is not null before using it
            if (pdfFileResult == null)
            {
                // Handle the case where PDF generation fails
                return RedirectToAction("CreateSaleInvoice");
            }

            MutipleModel model = new MutipleModel();

            byte[] pdf = pdfFileResult.FileContents;

            model.Pdf = pdf;

            model.sales_Invoice = invoiceModel;

            // Pass the PDF content to the view
            return View(model);
        }

        public IActionResult SaleInvoiceDownload(int Invoice_ID)
        {

            if (HttpContext.Request.Headers["Referer"].ToString() == "")
            {
                return RedirectToAction("SaleInvoices");
            }

            Sales_Invoice_Model sales_Invoice_Model = new Sales_Invoice_Model();

            HttpResponseMessage response = _Client.GetAsync($"{_Client.BaseAddress}/Invoices/GetSaleInvoiceByID/{Invoice_ID}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic? jsonObject = JsonConvert.DeserializeObject(data);
                var DataObject = jsonObject?.data;
                var extractedDtaJson = JsonConvert.SerializeObject(DataObject, Formatting.Indented);
                sales_Invoice_Model = JsonConvert.DeserializeObject<Sales_Invoice_Model>(extractedDtaJson);

            }

            var uniqueToken = Guid.NewGuid().ToString();

            HttpContext.Session.Set($"InvoiceData_{uniqueToken}", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sales_Invoice_Model)));

            return RedirectToAction("SaleInvoicePreview", new
            {
                preview = uniqueToken
            });

        }

        #endregion

        #region Section: Create PDF Sale Invoice

        public FileContentResult SaleInvoicePDF(Sales_Invoice_Model invoiceModel)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Create the document
                using (Document document = new Document(iTextSharp.text.PageSize.A4, 0, 0, 0, 0))
                {
                    // Create PdfWriter instance
                    using (PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream))
                    {
                        // Open the document
                        document.Open();

                        // Get PdfContentByte
                        PdfContentByte contentByte = pdfWriter.DirectContent;

                        BaseFont defaultfont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                        iTextSharp.text.Font dfont = new iTextSharp.text.Font(defaultfont, 18);

                        BaseFont boldfont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.WINANSI, BaseFont.EMBEDDED);
                        iTextSharp.text.Font bfont = new iTextSharp.text.Font(boldfont, 18);

                        BaseFont inrfont = BaseFont.CreateFont("https://localhost:7024/Fonts/Rupee.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, true);
                        iTextSharp.text.Font ifont = new iTextSharp.text.Font(inrfont, 18);

                        BaseFont gujaratifont = BaseFont.CreateFont("https://localhost:7024/Fonts/NotoSansGujarati.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, true);
                        iTextSharp.text.Font font = new iTextSharp.text.Font(gujaratifont, 18);
                        font.Color = new BaseColor(Color.Red);

                        contentByte.BeginText();

                        contentByte.SetFontAndSize(gujaratifont, 14);
                        contentByte.SetTextMatrix(250, 820); // X, Y position
                        contentByte.ShowText("|| ગણેશાય નમઃ ||"); //header

                        contentByte.SetFontAndSize(boldfont, 13);
                        contentByte.SetTextMatrix(460, 810); // X, Y position
                        contentByte.ShowText("Mo: +91 98254 22091");

                        contentByte.SetFontAndSize(boldfont, 13);
                        contentByte.SetTextMatrix(460, 790); // X, Y position
                        contentByte.ShowText("Mo: +91 94277 23092");

                        contentByte.SetFontAndSize(boldfont, 13);
                        contentByte.SetTextMatrix(20, 810); // X, Y position
                        contentByte.ShowText("GSTIN:");

                        contentByte.SetFontAndSize(boldfont, 13);
                        contentByte.SetTextMatrix(70, 810); // X, Y position
                        contentByte.ShowText("24CSDPK5665E1ZS");

                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance("https://localhost:7024/Images/Logo.png");

                        iTextSharp.text.Image backimage = iTextSharp.text.Image.GetInstance("https://localhost:7024/Images/Backimg.png");

                        image.ScaleToFit(60, 60); // Adjust width and height

                        image.SetAbsolutePosition(275, 765); // Position Of Image

                        document.Add(image); // Add the image to the PDF

                        backimage.ScaleToFit(300, 300); // Adjust width and height 

                        backimage.SetAbsolutePosition(145, 230); // Position Of Image

                        document.Add(backimage); // Add the image to the PDF

                        contentByte.SetFontAndSize(boldfont, 20);
                        contentByte.SetTextMatrix(155, 725); // X, Y position
                        contentByte.ShowText("Shree Ganesh Agro Industries"); // name

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(165, 690); // X, Y position
                        contentByte.ShowText("Address:- GIDC Plot No.36,Porbandar Road,"); // address

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(260, 670); // X, Y position
                        contentByte.ShowText("Upleta(Dist.Rajkot)360-490."); // address of city

                        contentByte.SetFontAndSize(boldfont, 15);
                        contentByte.SetTextMatrix(275, 640); // X, Y position
                        contentByte.ShowText("Invoice"); // invoice

                        DateTime? invoiceDate = invoiceModel.SalesInvoiceDate;
                        DateTime? onlyDate = invoiceDate?.Date;

                        string? formattedDate = onlyDate?.ToString("dd-MM-yyyy");

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(480, 610); // X, Y position
                        contentByte.ShowText("Date: " + formattedDate); // date

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(25, 565); // X, Y position
                        contentByte.ShowText("Name: -"); // farmer

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(100, 568); // X, Y position
                        contentByte.ShowText(invoiceModel.PartyName); // party name

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(25, 535); // X, Y position
                        contentByte.ShowText("Address: -"); // farmer

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(100, 535); // X, Y position
                        contentByte.ShowText(invoiceModel.PartyAddress); // party address

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(25, 505); // X, Y position
                        contentByte.ShowText("GST NO: -"); // GSTNO

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(25, 30); // X, Y position
                        contentByte.ShowText("* This Invoice Generated by Computer"); // GSTNO

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(100, 505); // X, Y position
                        contentByte.ShowText(!string.IsNullOrWhiteSpace(invoiceModel.PartyGstNo) ? invoiceModel.PartyGstNo : "--"); //

                        // -- Table Content  -- //

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(45, 455); // X, Y position
                        contentByte.ShowText("Description"); // product name

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(160, 455); // X, Y position
                        contentByte.ShowText("Bag"); // bag

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(220, 463); // X, Y position
                        contentByte.ShowText("Bag"); // bag per kg

                        //------------+++++------------//

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(215, 450); // X, Y position
                        contentByte.ShowText("Per Kg"); // bag per kg 

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(290, 455); // X, Y position
                        contentByte.ShowText("Weight"); // weight

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(370, 463); // X, Y position
                        contentByte.ShowText("Product"); // product +*/

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(370, 450); // X, Y position
                        contentByte.ShowText("Price(  )"); // pr price

                        //+//
                        contentByte.SetFontAndSize(inrfont, 12);
                        contentByte.SetTextMatrix(404, 450); // X, Y position
                        contentByte.ShowText("K"); // ₹ symbol added

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(470, 455); // X, Y position
                        contentByte.ShowText("Total Price(  )"); // total price */

                        //+//
                        contentByte.SetFontAndSize(inrfont, 12);
                        contentByte.SetTextMatrix(536, 455); // X, Y position
                        contentByte.ShowText("K"); // ₹ symbol added

                        contentByte.SetFontAndSize(boldfont, 14);
                        contentByte.SetTextMatrix(368, 175); // X, Y position
                        contentByte.ShowText("Total(  ): ");

                        contentByte.SetFontAndSize(inrfont, 14);
                        contentByte.SetTextMatrix(406, 175); // X, Y position
                        contentByte.ShowText("K"); // ₹ symbol added

                        contentByte.SetFontAndSize(boldfont, 14);
                        contentByte.SetTextMatrix(368, 115); // X, Y position
                        contentByte.ShowText("For, Shree Ganesh Agro Ind.");

                        contentByte.SetFontAndSize(boldfont, 14);
                        contentByte.SetTextMatrix(395, 85); // X, Y position
                        contentByte.ShowText("Bhavesh S. Kachhela");

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(160, 227); // X, Y position
                        contentByte.ShowText("Bank Details:");

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(50, 205); // X, Y position
                        contentByte.ShowText("Bank Name: ");

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(120, 205); // X, Y position
                        contentByte.ShowText("State Bank Of India - RAJ MARG UPLETA");

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(50, 185); // X, Y position
                        contentByte.ShowText("Bank A/c No: ");

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(125, 185); // X, Y position
                        contentByte.ShowText("39007631907");

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(50, 165); // X, Y position
                        contentByte.ShowText("IFSC Code: ");

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(117, 165); // X, Y position
                        contentByte.ShowText("SBIN0060075");

                        // ---- Details Invoice ---- //

                        if (invoiceModel.ProductName == "WHEAT" || invoiceModel.ProductName == "BAJARA" || invoiceModel.ProductName == "DHANA" || invoiceModel.ProductName == "SESAME" || invoiceModel.ProductName == "JEERA" || invoiceModel.ProductName == "CASTOR")
                        {
                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(50, 425); // X, Y position
                            contentByte.ShowText(invoiceModel.ProductName); // product name

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(30, 405); // X, Y position
                            string? brand = invoiceModel.ProductBrandName;
                            contentByte.ShowText("(" + brand + ")"); // brand name
                        }
                        else if (invoiceModel.ProductName == "SOYABEAN")
                        {
                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(45, 425); // X, Y position
                            contentByte.ShowText(invoiceModel.ProductName); // product name

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(40, 405); // X, Y position
                            string? brand = invoiceModel.ProductBrandName;
                            contentByte.ShowText("(" + brand + ")"); // brand name
                        }
                        else
                        {
                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(30, 425); // X, Y position
                            contentByte.ShowText(invoiceModel.ProductName); // product name

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(30, 405); // X, Y position
                            string? brand = invoiceModel.ProductBrandName;
                            contentByte.ShowText("(" + brand + ")"); // brand name
                        }

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(163, 425); // X, Y position
                        contentByte.ShowText(!string.IsNullOrWhiteSpace(invoiceModel.Bags.ToString()) ? invoiceModel.Bags.ToString() : "--"); // bag

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(225, 425); // X, Y position
                        contentByte.ShowText(!string.IsNullOrWhiteSpace(invoiceModel.BagPerKg.ToString()) ? invoiceModel.BagPerKg.ToString() : "--"); // bag

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(290, 425); // X, Y position
                        string formattedWeight = invoiceModel.TotalWeight.ToString("N", new System.Globalization.CultureInfo("en-IN"));
                        contentByte.ShowText(formattedWeight); // weight

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(380, 425); // X, Y position
                        string formattedProductPrice = Convert.ToString(invoiceModel.ProductPrice.ToString("N", new System.Globalization.CultureInfo("en-IN")));
                        contentByte.ShowText(formattedProductPrice); // productprice

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(475, 425); // X, Y position

                        string? formattedTotalPrice = invoiceModel.WithoutGSTPrice?.ToString("N", new System.Globalization.CultureInfo("en-IN"));
                        contentByte.ShowText(formattedTotalPrice); // before tax price total    

                        string? CGST = invoiceModel.CGST.ToString();
                        string? SGST = invoiceModel.SGST.ToString();

                        if (!string.IsNullOrEmpty(CGST) && !string.IsNullOrEmpty(SGST))
                        {

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(365, 227); // X, Y position
                            contentByte.ShowText("CGST:");

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(365, 203); // X, Y position
                            contentByte.ShowText("SGST:");

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(405, 227); // X, Y position
                            contentByte.ShowText(CGST + " % ");

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(405, 203); // X, Y position
                            contentByte.ShowText(SGST + " % ");

                        }
                        else
                        {
                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(395, 227); // X, Y position

                            contentByte.ShowText("--"); // CGST

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(395, 203); // X, Y position

                            contentByte.ShowText("--"); // IGST

                        }

                        string? CGST_Total = invoiceModel.TotalCGSTPrice?.ToString("N", new System.Globalization.CultureInfo("en-IN"));
                        string? SGST_Total = invoiceModel.TotalSGSTPrice?.ToString("N", new System.Globalization.CultureInfo("en-IN"));

                        if (!string.IsNullOrEmpty(CGST_Total) && !string.IsNullOrEmpty(SGST_Total))
                        {
                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(475, 228); // X, Y position
                            contentByte.ShowText(CGST_Total); // CGST_Total

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(475, 205); // X, Y position
                            contentByte.ShowText(SGST_Total); // SGST_Total
                        }
                        else
                        {
                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(475, 228); // X, Y position
                            contentByte.ShowText("--"); // CGST_Total

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(475, 205); // X, Y position
                            contentByte.ShowText("--"); // SGST_Total
                        }

                        contentByte.SetFontAndSize(boldfont, 14);
                        contentByte.SetTextMatrix(470, 175); // X, Y position
                        string Final_Total = invoiceModel.TotalPrice.ToString("N", new System.Globalization.CultureInfo("en-IN"));
                        contentByte.ShowText(Final_Total); // IGST_Total

                        contentByte.SetFontAndSize(boldfont, 12);
                        contentByte.SetTextMatrix(150, 144); // X, Y position
                        contentByte.ShowText("Transport Details:");

                        if (invoiceModel.VehicleName == "CONTAINER")
                        {
                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(50, 115); // X, Y position
                            contentByte.ShowText("Vechicle Type: ");

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(50, 95); // X, Y position
                            contentByte.ShowText("Vechicle No: ");

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(137, 115); // X, Y position
                            contentByte.ShowText(invoiceModel.VehicleName);

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(123, 95); // X, Y position
                            contentByte.ShowText(invoiceModel.VehicleNo);

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(50, 75); // X, Y position
                            contentByte.ShowText("Container No: ");

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(130, 75); // X, Y position
                            contentByte.ShowText(invoiceModel.ContainerNo);
                        }
                        else
                        {
                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(50, 110); // X, Y position
                            contentByte.ShowText("Vechicle Type: ");

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(50, 85); // X, Y position
                            contentByte.ShowText("Vechicle No: ");

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(137, 110); // X, Y position
                            contentByte.ShowText(invoiceModel.VehicleName);

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(123, 85); // X, Y position
                            contentByte.ShowText(invoiceModel.VehicleNo);

                        }

                        // ------xxxxxxxxxxx------ //

                        contentByte.EndText(); // ---- Text End ---- //

                        // ---- Design Format Of Invoice ---- //

                        //--  Shree Ganesh Name Above line --//
                        #region HEDAER

                        contentByte.MoveTo(0, 750); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(600, 750); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        //-- Name Below line --//
                        contentByte.MoveTo(0, 710); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(600, 710); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        //-- Invoice Above line --//
                        contentByte.MoveTo(0, 660); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(600, 660); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        //-- Invoice Below line --//
                        contentByte.MoveTo(0, 630); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(600, 630); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        //-- Party Name Right line --//
                        contentByte.MoveTo(100, 562); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(450, 562); // Ending point (x, y) x--> straight line 

                        //-- Party Address Right line --//
                        contentByte.MoveTo(100, 532); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(450, 532); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        //-- GST NO  Right line --//
                        contentByte.MoveTo(100, 502); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(450, 502); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        #endregion

                        // -- Table Design -- //

                        // ------ //

                        contentByte.MoveTo(20, 475); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(575, 475); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*
                         * |
                         * | |||| 1  right side last 
                         * |
                         * |
                         */

                        contentByte.MoveTo(575, 160); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(575, 475); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*  
                         * |
                         * ||||| 2
                         * |
                         * |
                         */

                        contentByte.MoveTo(20, 65); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(20, 475); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        // ------ // - 2 

                        contentByte.MoveTo(20, 445); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(575, 445); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*
                         * |
                         * ||||| 3
                         * |
                         * |
                         */

                        contentByte.MoveTo(140, 245); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(140, 475); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*
                         * |
                         * ||||| 4
                         * |
                         * |
                         */

                        contentByte.MoveTo(205, 245); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(205, 475); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*
                         * |
                         * ||||| 5
                         * |
                         * |
                         */

                        contentByte.MoveTo(270, 245); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(270, 475); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*
                         * |
                         * ||||| 6
                         * |
                         * |
                         */

                        contentByte.MoveTo(360, 65); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(360, 475); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*
                         * |
                         * ||||| 7
                         * |
                         * |
                         */

                        contentByte.MoveTo(440, 160); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(440, 475); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        // ------ // - 3

                        contentByte.MoveTo(20, 245); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(575, 245); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        // ------ // -4

                        contentByte.MoveTo(20, 137); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(360, 137); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        // ------ // -4

                        contentByte.MoveTo(20, 65); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(360, 65); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*----  CGST & IGST TABLE LINE*/

                        /*
                         * |
                         * ||||| 1
                         * |
                         * |
                         */

                        contentByte.MoveTo(360, 160); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(360, 245); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        // ------ // - 1

                        contentByte.MoveTo(575, 220); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(20, 220); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        // ------ // - 2

                        contentByte.MoveTo(575, 195); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(360, 195); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        // ------ // - 3

                        contentByte.MoveTo(575, 160); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(20, 160); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        // Close the document
                        document.Close();
                    }
                }

                return File(memoryStream.ToArray(), "application/pdf");
                // Return the PDF as a file with a specified name
                /*  return File(memoryStream.ToArray(), "application/pdf", invoiceModel.Farmername+".pdf");*/

            }
        }

        #endregion

        #region Section: Download Statement PDF & EXCEL

        public async Task<IActionResult> GenerateSaleInvoicesPDFStatement()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/SalesInvoiceStatementPDF");

            if (response.IsSuccessStatusCode)
            {
                // Extract filename from Content-Disposition header
                var contentDisposition = response.Content.Headers.ContentDisposition;
                string filename = contentDisposition?.FileName;

                var pdfContent = await response.Content.ReadAsByteArrayAsync();
                return File(pdfContent, "application/pdf", filename);
            }
            else
            {
                // Handle error or return an error response
                return BadRequest("Could not generate PDF.");
            }

        }

        public async Task<IActionResult> GenerateSaleInvoicesEXCELStatement()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/SalesInvoiceStatementEXCEL");

            if (response.IsSuccessStatusCode)
            {
                // Extract filename from Content-Disposition header
                var contentDisposition = response.Content.Headers.ContentDisposition;
                string filename = contentDisposition?.FileName;

                var pdfContent = await response.Content.ReadAsByteArrayAsync();
                return File(pdfContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            else
            {
                // Handle error or return an error response
                return BadRequest("Could not generate Excel.");
            }

        }

        #endregion

        #endregion

    }
}
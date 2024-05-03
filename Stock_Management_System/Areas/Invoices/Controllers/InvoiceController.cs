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
using PdfSharp.Pdf.Advanced;

namespace Stock_Management_System.Areas.Invoices.Controllers
{

    [Area("Invoices")]
    [Route("~/[controller]/[action]")]
    public class InvoiceController : Controller
    {

        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://localhost:7024/api");

        public readonly HttpClient _Client;

        private Api_Service api_Service = new Api_Service();

        public InvoiceController(IConfiguration configuration)
        {
            Configuration = configuration;

            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;

        }

        #region  Method : Set And Get Data From Session For PDF And Excel

        #region Method : Set Data From Session For PDF and Excel

        public void SetData_From_Session_For_Pdf_And_Excel(object itmes, string SetSessionStringName)
        {
            var serializedSalesInvoices = JsonConvert.SerializeObject(itmes);
            HttpContext.Session.SetString(SetSessionStringName, serializedSalesInvoices);
        }

        #endregion

        #region Method : Get Data From Session For PDF and Excel

        public List<T> GetData_From_Session_For_Pdf_And_Excel<T>(string SessionStringName)
        {
            var serializedSalesInvoices = HttpContext.Session.GetString(SessionStringName);
            if (string.IsNullOrEmpty(serializedSalesInvoices))
            {
                throw new InvalidOperationException($"{SessionStringName} data not found in session.");
            }
            return JsonConvert.DeserializeObject<List<T>>(serializedSalesInvoices);
        }

        #endregion

        #endregion


        #region Method : Dropdown Function


        public async Task All_Dropdowns_Call()
        {
            All_DropDown_Model all_DropDown_Model = new All_DropDown_Model();

            All_DropDowns_Class all_DropDowns_Class = new All_DropDowns_Class();

            all_DropDown_Model = await all_DropDowns_Class.Get_All_DropdDowns_Data();





            if (all_DropDown_Model != null)
            {
                ViewBag.Products = new SelectList(all_DropDown_Model.Products_DropDowns_List, "ProductId", "ProductNameInGujarati");

                ViewBag.ProductsInEnglish = new SelectList(all_DropDown_Model.Products_DropDowns_List, "ProductId", "ProductNameInEnglish");

                ViewBag.ProductGrade = new SelectList(all_DropDown_Model.Products_Grade_DropDowns_List, "ProductGradeId", "ProductGrade");
                ViewBag.Vehicle = new SelectList(all_DropDown_Model.Vehicle_DropDowns_List, "VehicleId", "VehicleName");



            }




        }



        #endregion


        #region Method : Create Purchase Invoice 


        public async Task<IActionResult> Create_Purchase_Invoice()
        {
            await All_Dropdowns_Call();

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Insert_Purchase_Invoice_Details(Purchase_Invoice_Model purchase_InvoiceModel)
            {


            var jsonContent = JsonConvert.SerializeObject(purchase_InvoiceModel);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Invoices/Insert_Purchase_Invoice", stringContent);

            if (response.IsSuccessStatusCode)
            {
                var token = Guid.NewGuid().ToString();
                HttpContext.Session.Set($"InvoiceData_{token}", Encoding.UTF8.GetBytes(jsonContent));
                return RedirectToAction("Preview_Purchase_Invoice", new { preview = token });
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Log or display the responseContent for more detailed error information
                return StatusCode((int)response.StatusCode, responseContent);
            }

        }



        #endregion


        #region Method : Delete Purchase Invoice


        public IActionResult Delete_Purchase_Invoice(string Invoice_ID)
        {
            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Invoices/Delete_Purchase_Invoice?Purchase_Invoice_ID={UrlEncryptor.Decrypt(Invoice_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["DeleteMessage"] = "Invoice Delete Successfully !";
            }
            else
            {
                TempData["DeleteMessage"] = "Error. Please try again.";
            }

            return RedirectToAction("History_Purchase_Invoice");
        }


        #endregion


        #region Method : Update Purchase Invoice

        public async Task<IActionResult> Update_Purchase_Invoice(string Invoice_ID)
        {

            await All_Dropdowns_Call();

            Purchase_Invoice_Model purchase_Invoice_Model = new Purchase_Invoice_Model();

            purchase_Invoice_Model = await api_Service.Model_Of_Data_Display<Purchase_Invoice_Model>("Invoices/Get_Purchase_Invoice_By_Id", Convert.ToInt32(UrlEncryptor.Decrypt(Invoice_ID)));


            return View(purchase_Invoice_Model);
        }


        public async Task<IActionResult> Update_Purchase_Invoice_Details(Purchase_Invoice_Model purchase_Invoice_Details)
        {

           
            var jsonContent = JsonConvert.SerializeObject(purchase_Invoice_Details);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _Client.PutAsync($"{_Client.BaseAddress}/Invoices/Update_Purchase_Invoice", stringContent);

            if (response.IsSuccessStatusCode)
            {
                var token = Guid.NewGuid().ToString();
                HttpContext.Session.Set($"InvoiceData_{token}", Encoding.UTF8.GetBytes(jsonContent));
                return RedirectToAction("Preview_Purchase_Invoice", new { preview = token });
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Log or display the responseContent for more detailed error information
                return StatusCode((int)response.StatusCode, responseContent);
            }



           
        }


        #endregion


        #region Method : Show All Purchase Invoices

        public async Task<IActionResult> History_Purchase_Invoice()
        {

            await All_Dropdowns_Call();

            List<Purchase_Invoice_Model> purchase_Invoices = await api_Service.List_Of_Data_Display<Purchase_Invoice_Model>("Invoices/Purchase_Invoices");


            SetData_From_Session_For_Pdf_And_Excel(purchase_Invoices, "ListOfPurchaseInvoicesData");

            return View(purchase_Invoices);

        }


        #endregion


        #region Method : Preview Purchase Invoices And Download

        public IActionResult Preview_Purchase_Invoice(string preview)
        {
            byte[]? storedData = HttpContext.Session.Get($"InvoiceData_{preview}");

            if (string.IsNullOrEmpty(preview) || storedData == null)
            {
                return RedirectToAction("Create_Purchase_Invoice");
            }

            var invoiceModel = JsonConvert.DeserializeObject<Purchase_Invoice_Model>(Encoding.UTF8.GetString(storedData));

            // Check if invoiceModel is not null before using it

            if (invoiceModel == null)
            {
                // Handle the case where deserialization fails
                return RedirectToAction("Create_Purchase_Invoice");
            }

            // Assuming you have a method to generate PDF content
            FileContentResult? pdfFileResult = Purchase_InvoiceCreate_PDf(invoiceModel);

            // Check if pdfFileResult is not null before using it
            if (pdfFileResult == null)
            {
                // Handle the case where PDF generation fails
                return RedirectToAction("Create_Purchase_Invoice");
            }

            byte[] pdf = pdfFileResult.FileContents;

            MutipleModel model = new MutipleModel();

            model.Pdf = pdf;

            model.purchase_Invoice = invoiceModel;

            // Pass the PDF content to the view
            return View("Preview-Purchase-Invoice", model);
        }

        public IActionResult PDF_Preview_Download(int Invoice_ID)
        {

            Purchase_Invoice_Model purchase_Invoices = new Purchase_Invoice_Model();

            HttpResponseMessage response = _Client.GetAsync($"{_Client.BaseAddress}/Invoices/Get_Purchase_Invoice_By_Id/{Invoice_ID}").Result;

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

            return RedirectToAction("Preview_Purchase_Invoice", new { preview = uniqueToken });


        }


        #endregion


        #region Method : Purchase Invoice Details BY ID


        public async Task<IActionResult> Purchase_Invoice_Details(string Invoice_ID)
        {


            Purchase_Invoice_Model purchase_Invoices = new Purchase_Invoice_Model();

            purchase_Invoices = await api_Service.Model_Of_Data_Display<Purchase_Invoice_Model>("Invoices/Get_Purchase_Invoice_By_Id", Convert.ToInt32(UrlEncryptor.Decrypt(Invoice_ID)));

            return View(purchase_Invoices);
        }


        #endregion


        #region Method : Purchase Invoice PDF


        public FileContentResult Purchase_InvoiceCreate_PDf(Purchase_Invoice_Model invoiceModel)
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


                        BaseFont inrfont = BaseFont.CreateFont("D:\\Font\\ITF Rupee.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, true);
                        iTextSharp.text.Font ifont = new iTextSharp.text.Font(inrfont, 18);


                        BaseFont gujaratifont = BaseFont.CreateFont("D:\\Font\\NotoSansGujarati-Bold.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, true);
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

                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance("C:\\Users\\bharg\\Desktop\\Icons\\Logo-Size_M.png");

                        iTextSharp.text.Image backimage = iTextSharp.text.Image.GetInstance("C:\\Users\\bharg\\Desktop\\Icons\\Backimg.png");


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
                        contentByte.ShowText("No.");  // No.



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





                        contentByte.EndText();  // ---- Text End ---- //

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

                        /*    *//*
                             * |
                             * | |||| 1
                             * |
                             * |
                             *//**/

                        contentByte.MoveTo(575, 95); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(575, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();
                        /*
                                                *//*
                                                * |
                                                * ||||| 2
                                                * |
                                                * |
                                                *//**/

                        contentByte.MoveTo(20, 125); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(20, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();


                        // ------ // - 2 

                        contentByte.MoveTo(20, 500); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(575, 500); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();


                        /*         *//*
                                 * |
                                 * ||||| 3
                                 * |
                                 * |
                                 *//**/

                        contentByte.MoveTo(55, 125); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(55, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*  *//*
                           * |
                           * ||||| 4
                           * |
                           * |
                           *//**/

                        contentByte.MoveTo(180, 125); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(180, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /**//*
                         * |
                         * ||||| 5
                         * |
                         * |
                         *//**/

                        contentByte.MoveTo(240, 125); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(240, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();


                        /*  *//*
                           * |
                           * ||||| 6
                           * |
                           * |
                           *//**/

                        contentByte.MoveTo(300, 125); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(300, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /*  *//*
                          * |
                          * ||||| 7
                          * |
                          * |
                          *//**/

                        contentByte.MoveTo(375, 125); // Starting point (x, y) x--> starting line start  
                        contentByte.LineTo(375, 540); // Ending point (x, y) x--> straight line 
                        contentByte.Stroke();

                        /**//*
                      * |
                      * ||||| 8
                      * |
                      * |
                      *//**/

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

       /* public FileContentResult Purchase_Invoice_Statement_CreatePdf()
        {

            List<Purchase_Invoice_Model> purchase_Invoices = GetData_From_Session_For_Pdf_And_Excel<Purchase_Invoice_Model>("ListOfPurchaseInvoicesData");

            // Convert to DataTable
            DataTable dataTable = Convert_List_To_DataTable_For_Purchase_Invoice_Statement(purchase_Invoices);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Custom page size
                iTextSharp.text.Rectangle customPageSize = new iTextSharp.text.Rectangle(2300, 1200);
                using (Document document = new Document(customPageSize))
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Define fonts
                    BaseFont boldBaseFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.WINANSI, BaseFont.EMBEDDED);
                    BaseFont gujaratiBaseFont = BaseFont.CreateFont("D:\\Font\\NotoSansGujarati-Bold.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED, true);
                    Font boldFont = new Font(boldBaseFont, 12);
                    Font gujaratiFont = new Font(gujaratiBaseFont, 12);

                    // Title
                    Paragraph title = new Paragraph("Statement", new Font(boldBaseFont, 35));
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(title);
                    document.Add(new Chunk("\n"));

                    // Image
                    iTextSharp.text.Image backimage = iTextSharp.text.Image.GetInstance("C:\\Users\\bharg\\OneDrive\\Desktop\\Icons\\Backimg.png");
                    backimage.ScaleToFit(500, 500);
                    backimage.SetAbsolutePosition(900, 400);
                    document.Add(backimage);

                    // Table setup
                    PdfPTable pdfTable = new PdfPTable(dataTable.Columns.Count)
                    {
                        WidthPercentage = 100,
                        DefaultCell = { Padding = 10 }
                    };

                    // Headers
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        Font headerFont = column.ColumnName.Equals("Product", StringComparison.InvariantCultureIgnoreCase) ? gujaratiFont : boldFont;
                        PdfPCell headerCell = new PdfPCell(new Phrase(column.ColumnName, headerFont))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 10
                        };
                        pdfTable.AddCell(headerCell);
                    }

                    // Data rows
                    foreach (DataRow row in dataTable.Rows)
                    {
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            var item = row[column];
                            Font itemFont = column.ColumnName.Equals("Product", StringComparison.InvariantCultureIgnoreCase) ? gujaratiFont : boldFont;

                            PdfPCell dataCell = new PdfPCell(new Phrase(item?.ToString(), itemFont))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                Padding = 10
                            };
                            pdfTable.AddCell(dataCell);
                        }
                    }

                    document.Add(pdfTable);
                    document.Close();
                }

                // File result
                string fileName = "Purchase-Invoices-Statements.pdf";
                return File(memoryStream.ToArray(), "application/pdf", fileName);
            }
        }*/


        public async Task<IActionResult> Purchase_Invoice_Statement_CreatePDF()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/Purchase_Invoice_Statement_PDF");

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



        #endregion


        #region Method : Purchase Invoice Excel

        public async Task<IActionResult> Export_Purchase_Invoices_List_To_Excel()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/Purchase_Invoice_Statement_EXCEL");

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


        #region Method : Convert List To DataTable For Purchase Invoice  

        public DataTable Convert_List_To_DataTable_For_Purchase_Invoice_Statement(List<Purchase_Invoice_Model> purchase_Invoices)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Invoice-Date", typeof(string));
            dataTable.Columns.Add("Customer-Name", typeof(string));
            dataTable.Columns.Add("Product", typeof(string));
            dataTable.Columns.Add("Product-Grade", typeof(string));
            dataTable.Columns.Add("Bags", typeof(string));
            dataTable.Columns.Add("Bag-Per-Kg", typeof(string));
            dataTable.Columns.Add("Weight", typeof(string));
            dataTable.Columns.Add("Total-Price", typeof(string));
            dataTable.Columns.Add("Vehicle-Name", typeof(string));
            dataTable.Columns.Add("Vehicle-No", typeof(string));
            dataTable.Columns.Add("Tolat", typeof(string));
            dataTable.Columns.Add("Driver-Name", typeof(string));






            foreach (var invoice in purchase_Invoices)
            {
                DataRow row = dataTable.NewRow();

                DateTime date = invoice.PurchaseInvoiceDate; // Your original date
                string formattedDate = date.ToString("dd/MM/yyyy"); // Formatting the date

                row["Invoice-Date"] = formattedDate;
                row["Customer-Name"] = invoice.CustomerName;
                row["Product"] = invoice.ProductName;
                row["Product-Grade"] = invoice.ProductGrade;
                row["Bags"] = invoice.Bags.HasValue ? invoice.Bags.ToString() : "--";
                row["Bag-Per-Kg"] = invoice.BagPerKg.HasValue ? invoice.BagPerKg.ToString() : "--";
                row["Weight"] = invoice.TotalWeight;
                row["Total-Price"] = invoice.TotalPrice;
                row["Vehicle-Name"] = invoice.VehicleName;
                row["Vehicle-No"] = invoice.VehicleNo;
                row["Tolat"] = invoice.TolatName;
                row["Driver-Name"] = invoice.DriverName;





                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        #endregion





        /*========= SALES INVOICE =========*/



        #region Method : Create Sales Invoice

        public async Task<IActionResult> Create_Sales_Invoice()
        {
            await All_Dropdowns_Call();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Insert_Sales_Invoice_Details(Sales_Invoice_Model sales_Invoice)
        {
            var jsonContent = JsonConvert.SerializeObject(sales_Invoice);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Invoices/Insert_Sales_Invoice", stringContent);

            if (response.IsSuccessStatusCode)
            {
                var token = Guid.NewGuid().ToString();
                HttpContext.Session.Set($"InvoiceData_{token}", Encoding.UTF8.GetBytes(jsonContent));
                return RedirectToAction("Preview_Sales_Invoice", new { preview = token });
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Log or display the responseContent for more detailed error information
                return StatusCode((int)response.StatusCode, responseContent);
            }

        }


        #endregion


        #region Method : Show All Sales Invoice

        public async Task<IActionResult> History_Sales_Invoice()
        {

            await All_Dropdowns_Call();



            List<Sales_Invoice_Model> sales_Invoices = await api_Service.List_Of_Data_Display<Sales_Invoice_Model>("Invoices/Sales_Invoices");


            SetData_From_Session_For_Pdf_And_Excel(sales_Invoices, "ListOfSalesInvoicesData");


            return View(sales_Invoices);



        }


        #endregion


        #region Method : Delete Sales Invoice

        public IActionResult Delete_Sales_Invoice(string Invoice_ID)
        {
            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Invoices/Delete_Sales_Invoice?Sales_Invoice_ID={UrlEncryptor.Decrypt(Invoice_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["DeleteMessage"] = "Delete Successfully !";
            }
            else
            {
                TempData["DeleteMessage"] = "Error. Please try again.";
            }

            return RedirectToAction("History_Sales_Invoice");
        }


        #endregion


        #region Method : Update Sales Invoice 


        public async Task<IActionResult> Update_Sales_Invoice(string Invoice_ID)
        {

            await All_Dropdowns_Call();

            Sales_Invoice_Model sales_Invoice_Model = new Sales_Invoice_Model();

            sales_Invoice_Model = await api_Service.Model_Of_Data_Display<Sales_Invoice_Model>("Invoices/Get_Sales_Invoice_By_Id", Convert.ToInt32(UrlEncryptor.Decrypt(Invoice_ID)));


            return View(sales_Invoice_Model);
        }

        public async Task<IActionResult> Update_Sales_Invoice_Details(Sales_Invoice_Model sales_Invoice_Model)
        {


            var jsonContent = JsonConvert.SerializeObject(sales_Invoice_Model);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _Client.PutAsync($"{_Client.BaseAddress}/Invoices/Update_Sales_Invoice", stringContent);

            if (response.IsSuccessStatusCode)
            {
                var token = Guid.NewGuid().ToString();
                HttpContext.Session.Set($"InvoiceData_{token}", Encoding.UTF8.GetBytes(jsonContent));
                return RedirectToAction("Preview_Sales_Invoice", new { preview = token });
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
               
                return StatusCode((int)response.StatusCode, responseContent);
            }




        }


        #endregion


        #region Method : Sales Invoice Details BY ID 

        public async Task<IActionResult> Sales_Invoice_Details(string Invoice_ID)
        {
            if (HttpContext.Request.Headers["Referer"].ToString() == "")
            {
                return RedirectToAction("Manage_Stocks");
            }

            Sales_Invoice_Model sales_Invoice_Model = new Sales_Invoice_Model();


            sales_Invoice_Model = await api_Service.Model_Of_Data_Display<Sales_Invoice_Model>("Invoices/Get_Sales_Invoice_By_Id", Convert.ToInt32(UrlEncryptor.Decrypt(Invoice_ID)));



            return View(sales_Invoice_Model);




        }


        #endregion


        #region Method : Preview Sales Invoices And Download


        public IActionResult Preview_Sales_Invoice(string preview)
        {
            byte[]? storedData = HttpContext.Session.Get($"InvoiceData_{preview}");

            if (string.IsNullOrEmpty(preview) || storedData == null)
            {
                return RedirectToAction("Create_Sell_Invoice");
            }

            var invoiceModel = JsonConvert.DeserializeObject<Sales_Invoice_Model>(Encoding.UTF8.GetString(storedData));

            // Check if invoiceModel is not null before using it
            if (invoiceModel == null)
            {
                // Handle the case where deserialization fails
                return RedirectToAction("Create_Sales_Invoice");
            }

            // Assuming you have a method to generate PDF content
            FileContentResult? pdfFileResult = Sales_InvoiceCreate_PDF(invoiceModel);

            // Check if pdfFileResult is not null before using it
            if (pdfFileResult == null)
            {
                // Handle the case where PDF generation fails
                return RedirectToAction("Create_Sales_Invoice");
            }

            MutipleModel model = new MutipleModel();



            byte[] pdf = pdfFileResult.FileContents;

            model.Pdf = pdf;

            model.sales_Invoice = invoiceModel;

            ViewData["SellInvoiceModel"] = invoiceModel;

            // Pass the PDF content to the view
            return View("Preview-Sales-Invoice", model);
        }


        public IActionResult Sales_Invoice_PDF_Preview_Download(int Invoice_ID)
        {

            if (HttpContext.Request.Headers["Referer"].ToString() == "")
            {
                return RedirectToAction("History_Sale_Invoice");
            }

            Sales_Invoice_Model sales_Invoice_Model = new Sales_Invoice_Model();

            HttpResponseMessage response = _Client.GetAsync($"{_Client.BaseAddress}/Invoices/Get_Sales_Invoice_By_Id/{Invoice_ID}").Result;

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

            return RedirectToAction("Preview_Sales_Invoice", new { preview = uniqueToken });


        }



        #endregion


        #region Method : Sales Invoice PDF

        public FileContentResult Sales_InvoiceCreate_PDF(Sales_Invoice_Model invoiceModel)
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


                        BaseFont inrfont = BaseFont.CreateFont("D:\\Font\\ITF Rupee.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, true);
                        iTextSharp.text.Font ifont = new iTextSharp.text.Font(inrfont, 18);


                        BaseFont gujaratifont = BaseFont.CreateFont("D:\\Font\\NotoSansGujarati-Bold.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, true);
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



                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance("C:\\Users\\bharg\\Desktop\\Icons\\Logo-Size_M.png");

                        iTextSharp.text.Image backimage = iTextSharp.text.Image.GetInstance("C:\\Users\\bharg\\Desktop\\Icons\\Backimg.png");


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
                            contentByte.SetTextMatrix(50, 425);  // X, Y position
                            contentByte.ShowText(invoiceModel.ProductName); // product name

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(30, 405);  // X, Y position
                            string? brand = invoiceModel.ProductBrandName;
                            contentByte.ShowText("(" + brand + ")"); // brand name
                        }
                        else if (invoiceModel.ProductName == "SOYABEAN")
                        {
                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(45, 425); // X, Y position
                            contentByte.ShowText(invoiceModel.ProductName); // product name

                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(40, 405);  // X, Y position
                            string? brand = invoiceModel.ProductBrandName;
                            contentByte.ShowText("(" + brand + ")"); // brand name
                        }
                        else
                        {
                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(30, 425); // X, Y position
                            contentByte.ShowText(invoiceModel.ProductName); // product name


                            contentByte.SetFontAndSize(boldfont, 12);
                            contentByte.SetTextMatrix(30, 405);  // X, Y position
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






                        contentByte.EndText();  // ---- Text End ---- //

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

      /*  public FileContentResult Sales_Invoice_Statement_CreatePdf()
        {


            List<Sales_Invoice_Model> salesInvoices = GetData_From_Session_For_Pdf_And_Excel<Sales_Invoice_Model>("ListOfSalesInvoicesData");

            // Convert to DataTable
            DataTable dataTable = Convert_List_To_DataTable_For_Sale_Invoice_Statement(salesInvoices);





            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Custom page size
                iTextSharp.text.Rectangle customPageSize = new iTextSharp.text.Rectangle(2300, 1200);
                using (Document document = new Document(customPageSize))
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Define fonts
                    BaseFont boldBaseFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.WINANSI, BaseFont.EMBEDDED);
                    BaseFont gujaratiBaseFont = BaseFont.CreateFont("D:\\Font\\NotoSansGujarati-Bold.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED, true);
                    Font boldFont = new Font(boldBaseFont, 12);
                    Font gujaratiFont = new Font(gujaratiBaseFont, 12);

                    // Title
                    Paragraph title = new Paragraph("Statement", new Font(boldBaseFont, 35));
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(title);
                    document.Add(new Chunk("\n"));

                    // Image
                    iTextSharp.text.Image backimage = iTextSharp.text.Image.GetInstance("C:\\Users\\bharg\\OneDrive\\Desktop\\Icons\\Backimg.png");
                    backimage.ScaleToFit(500, 500);
                    backimage.SetAbsolutePosition(900, 400);
                    document.Add(backimage);

                    // Table setup
                    PdfPTable pdfTable = new PdfPTable(dataTable.Columns.Count)
                    {
                        WidthPercentage = 100,
                        DefaultCell = { Padding = 10 }
                    };

                    // Headers
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        Font headerFont = column.ColumnName.Equals("Product", StringComparison.InvariantCultureIgnoreCase) ? gujaratiFont : boldFont;
                        PdfPCell headerCell = new PdfPCell(new Phrase(column.ColumnName, headerFont))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 10
                        };
                        pdfTable.AddCell(headerCell);
                    }

                    // Data rows
                    foreach (DataRow row in dataTable.Rows)
                    {
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            var item = row[column];
                            Font itemFont = column.ColumnName.Equals("Product", StringComparison.InvariantCultureIgnoreCase) ? gujaratiFont : boldFont;

                            PdfPCell dataCell = new PdfPCell(new Phrase(item?.ToString(), itemFont))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                Padding = 10
                            };
                            pdfTable.AddCell(dataCell);
                        }
                    }

                    document.Add(pdfTable);
                    document.Close();
                }

                // File result
                string fileName = "Sales-Invoices-Statements.pdf";
                return File(memoryStream.ToArray(), "application/pdf", fileName);
            }
        }*/

        public async Task<IActionResult> Sales_Invoice_Statement_CreatePDF()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/Sales_Invoice_Statement_PDF");

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


        #endregion


        #region Method : Sales Invoice Excel


        public async Task<IActionResult> Export_Sales_Invoices_List_To_Excel()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/Sales_Invoice_Statement_EXCEL");

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


        #region Method :  Convert List To DataTable For Sales Invoice

        public DataTable Convert_List_To_DataTable_For_Sale_Invoice_Statement(List<Sales_Invoice_Model> salesInvoices)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Invoice-Date", typeof(string));
            dataTable.Columns.Add("Invoice-Type", typeof(string));
            dataTable.Columns.Add("Broker-Name", typeof(string));
            dataTable.Columns.Add("Party-Name", typeof(string));
            dataTable.Columns.Add("Party-GSTNO", typeof(string));
            dataTable.Columns.Add("Party-Address", typeof(string));
            dataTable.Columns.Add("Product", typeof(string));
            dataTable.Columns.Add("Brand", typeof(string));
            dataTable.Columns.Add("Bags", typeof(string));
            dataTable.Columns.Add("Bag-Per-Kg", typeof(string));
            dataTable.Columns.Add("Weight", typeof(string));
            dataTable.Columns.Add("SGST", typeof(string));
            dataTable.Columns.Add("CGST", typeof(string));
            dataTable.Columns.Add("Total-Price", typeof(string));
            dataTable.Columns.Add("Vehicle-Name", typeof(string));
            dataTable.Columns.Add("Vehicle-No", typeof(string));
            dataTable.Columns.Add("Driver-Name", typeof(string));
            dataTable.Columns.Add("Container-No", typeof(string));





            foreach (var invoice in salesInvoices)
            {
                DataRow row = dataTable.NewRow();

                DateTime date = invoice.SalesInvoiceDate; // Your original date
                string formattedDate = date.ToString("dd/MM/yyyy"); // Formatting the date

                row["Invoice-Date"] = formattedDate; // Assigning the formatted



                row["Invoice-Type"] = invoice.InvoiceType;
                row["Broker-Name"] = invoice.BrokerName;
                row["Party-Name"] = invoice.PartyName;
                row["Party-GSTNO"] = invoice.PartyGstNo;
                row["Party-Address"] = invoice.PartyAddress;
                row["Product"] = invoice.ProductName;
                row["Brand"] = invoice.ProductBrandName;
                row["Bags"] = invoice.Bags.HasValue ? invoice.Bags.ToString() : "--";
                row["Bag-Per-Kg"] = invoice.BagPerKg.HasValue ? invoice.BagPerKg.ToString() : "--";
                row["Weight"] = invoice.TotalWeight;
                row["SGST"] = invoice.SGST.HasValue ? invoice.SGST.ToString() : "--";
                row["CGST"] = invoice.CGST.HasValue ? invoice.CGST.ToString() : "--";
                row["Total-Price"] = invoice.TotalPrice;
                row["Vehicle-Name"] = invoice.VehicleName;
                row["Vehicle-No"] = invoice.VehicleNo;
                row["Driver-Name"] = invoice.DriverName;
                row["Container-No"] = !string.IsNullOrEmpty(invoice.ContainerNo) ? invoice.ContainerNo : "--";




                dataTable.Rows.Add(row);
            }

            return dataTable;
        }


        #endregion


        #region Comment Code 

        /* private async Task<DataTable> _Fetch_Sales_Invoice_Details()
         {

             DataTable _Fetch_Details_Table = new DataTable();

             HttpResponseMessage response = _Client.GetAsync($"{_Client.BaseAddress}/Invoices/Sales_Invoices").Result;

             if (response.IsSuccessStatusCode)
             {
                 string data = await response.Content.ReadAsStringAsync();
                 dynamic jsonObject = JsonConvert.DeserializeObject(data);
                 var dataObject = jsonObject.data;
                 var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
                 _Fetch_Details_Table = JsonConvert.DeserializeObject<DataTable>(extractedDataJson);

             }
             return _Fetch_Details_Table;

         }*/

        #endregion


    }
}

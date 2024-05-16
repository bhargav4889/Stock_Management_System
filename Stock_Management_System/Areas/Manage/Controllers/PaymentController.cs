using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Stock_Management_System.All_DropDowns;
using Stock_Management_System.API_Services;
using Stock_Management_System.Areas.Accounts.Models;
using Stock_Management_System.Areas.Manage.Models;
using Stock_Management_System.Areas.Stocks.Models;
using Stock_Management_System.BAL;
using Stock_Management_System.UrlEncryption;
using System.Net;
using System.Text;
using static Stock_Management_System.Areas.Manage.Models.Payment_All_Models;

namespace Stock_Management_System.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("~/[controller]/[action]")]
    [CheckAccess]
    public class PaymentController : Controller
    {
        public IConfiguration Configuration;
        Uri baseaddress = new Uri("https://localhost:7024/api");
        public readonly HttpClient _Client;
        public Api_Service api_Service = new Api_Service();
        public HttpContextAccessor _HttpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentController"/> class with specified configuration.
        /// </summary>
        /// <param name="configuration">Configuration interface provided by ASP.NET Core to handle settings.</param>

        public PaymentController(IConfiguration configuration)
        {
            Configuration = configuration;
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
            _HttpContextAccessor = new HttpContextAccessor();
        }

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

            // Bank 

            List<Bank_Model> bank_Models = await api_Service.List_Of_Data_Display<Bank_Model>("Bank/GetBanksList");

            if (bank_Models != null)
            {
                // Base URL where the images are hosted, ensure this matches the actual location
                string baseUrl = "https://localhost:7024/";

                // Append the base URL to each bank's icon path
                foreach (var bank in bank_Models)
                {
                    if (!string.IsNullOrEmpty(bank.BankIcon))
                    {
                        bank.BankIcon = baseUrl + bank.BankIcon;
                    }
                }

                ViewBag.Banks = bank_Models;
            }
        }

        #endregion

        #region Section: Create Payment(PENDING)

        /// <summary>
        /// Inserts a new payment entry into the system.
        /// </summary>
        /// <param name="payment_Model">The payment details to be inserted.</param>
        /// <returns>A redirection to the PendingPayments view if successful; otherwise, returns an error status code with content.</returns>

        public async Task<IActionResult> InsertPayment(Payment_All_Models.Payment_Model payment_Model)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(payment_Model);
                var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Payment/AddPayment", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("PendingPayments");
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, responseContent);
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Section: Create Payment(REMAIN)

        /// <summary>
        /// Inserts a remaining payment entry for a customer.
        /// </summary>
        /// <param name="remain_Payment_Model">The remaining payment details.</param>
        /// <returns>A redirection to the RemainPayments view if successful; otherwise, returns an error status code with content.</returns>

        public async Task<IActionResult> InsertRemainPayment(Remain_Payment_Model remain_Payment_Model)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(remain_Payment_Model);
                var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Payment/AddRemainPayment", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("RemainPayments");
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, responseContent);
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Section: List of Payment Pendings
        /// <summary>
        /// Retrieves and displays a list of pending payments.
        /// </summary>
        /// <returns>A view showing pending payments.</returns>
      
        public async Task<IActionResult> PendingPayments()
        {
            await PopulateDropdownLists();

            List<Payment_All_Models.Pending_Customers_Payments> List_of_Pending_Customers_Payments = await api_Service.List_Of_Data_Display<Payment_All_Models.Pending_Customers_Payments>("Payment/GetPendingCustomersPayments");
            return View(List_of_Pending_Customers_Payments);
        }

        #endregion

        #region Section: List of Payment Remain

        /// <summary>
        /// Retrieves and displays a list of remaining payments.
        /// </summary>
        /// <returns>A view showing remaining payments.</returns>

        public async Task<IActionResult> RemainPayments()
        {
            await PopulateDropdownLists();

            List<Payment_All_Models.Remain_Payment_Model> List_of_Remain_Customers_Payments = await api_Service.List_Of_Data_Display<Payment_All_Models.Remain_Payment_Model>("Payment/GetRemainingCustomersPayments");
            return View(List_of_Remain_Customers_Payments);
        }

        #endregion

        #region Section: List of Payment PAID

        /// <summary>
        /// Retrieves and displays a list of paid payments.
        /// </summary>
        /// <returns>A view showing paid payments.</returns>

        public async Task<IActionResult> PaidPayments()
        {
            await PopulateDropdownLists();

            List<Payment_All_Models.Show_Payment_Info> List_of_Paid_Customers_Payments = await api_Service.List_Of_Data_Display<Payment_All_Models.Show_Payment_Info>("Payment/GetPaidCustomersPayments");
            return View(List_of_Paid_Customers_Payments);
        }

        #endregion

        #region Section: Create Payment Need Data Retrieve For Modal

        /// <summary>
        /// Fetches payment information based on customer and stock ID.
        /// </summary>
        /// <param name="Customer_ID">Encrypted customer ID.</param>
        /// <param name="Stock_ID">Encrypted stock ID.</param>
        /// <returns>A partial view with payment information if successful; otherwise, returns null.</returns>

        public async Task<ActionResult> GetPaymentInfoByCustomerStockID(string Customer_ID, string Stock_ID)
        {
            Payment_All_Models.Payment_Model payment_Model = new Payment_All_Models.Payment_Model();

            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Payment/GetPaymentInfoByStockCustomerID/{UrlEncryptor.Decrypt(Customer_ID)}&{UrlEncryptor.Decrypt(Stock_ID)}");

            if (response.IsSuccessStatusCode)
            {
                await PopulateDropdownLists();
                string data = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                payment_Model = JsonConvert.DeserializeObject<Payment_All_Models.Payment_Model>(JsonConvert.SerializeObject(jsonObject.data, Formatting.Indented));
                return PartialView("_PaymentInfo_Box", payment_Model);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Section: Create Remain Payment Need Data Retrive For Modal

        /// <summary>
        /// Fetches remaining payment information for a specified customer and stock.
        /// </summary>
        /// <param name="Customer_ID">Encrypted customer ID.</param>
        /// <param name="Stock_ID">Encrypted stock ID.</param>
        /// <returns>A partial view displaying remaining payment details if successful; otherwise, returns default.</returns>

        public async Task<IActionResult> GetRemainPaymentInfo(string Customer_ID, string Stock_ID)
        {
            Remain_Payment_Model remain_Payment_Model = new Remain_Payment_Model();

            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Payment/RemainGetPaymentInfoByCustomerFKAndStockIdAndPaymentID/{UrlEncryptor.Decrypt(Customer_ID)}&{UrlEncryptor.Decrypt(Stock_ID)}");

            if (response.IsSuccessStatusCode)
            {
                await PopulateDropdownLists();
                string data = await response.Content.ReadAsStringAsync();
                remain_Payment_Model = JsonConvert.DeserializeObject<Remain_Payment_Model>(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(data).data, Formatting.Indented));
                return PartialView("_RemainPaymentInfo_Box", remain_Payment_Model);
            }
            else
            {
                return default;
            }
        }

        #endregion

        #region Section: Show Payment Information

        /// <summary>
        /// Fetches comprehensive payment information for a specified customer and stock.
        /// </summary>
        /// <param name="Customer_ID">Encrypted customer ID.</param>
        /// <param name="Stock_ID">Encrypted stock ID.</param>
        /// <returns>A partial view displaying comprehensive payment information if successful; otherwise, returns null.</returns>

        public async Task<ActionResult> GetPaymentInfo(string Customer_ID, string Stock_ID)
        {
            Payment_All_Models.Show_Payment_Info show_Payment_Info = new Payment_All_Models.Show_Payment_Info();

            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Payment/GetFullPaymentInfo/{UrlEncryptor.Decrypt(Customer_ID)}&{UrlEncryptor.Decrypt(Stock_ID)}");

            if (response.IsSuccessStatusCode)
            {

                string data = await response.Content.ReadAsStringAsync();
                show_Payment_Info = JsonConvert.DeserializeObject<Payment_All_Models.Show_Payment_Info>(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(data).data, Formatting.Indented));
                return PartialView("_ShowPaymentInfo_Box", show_Payment_Info);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Section: Pending Payments PDF & Excel
        /// <summary>
        /// Generates and returns a PDF document of pending payments.
        /// </summary>
        /// <returns>A PDF file if successful; otherwise, an error message.</returns>
        public async Task<IActionResult> PendingPaymentsPDF()
        {
            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/PendingPaymentsPDF");

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

        /// <summary>
        /// Generates and returns an Excel document of pending payments.
        /// </summary>
        /// <returns>An Excel file if successful; otherwise, an error message.</returns>
        public async Task<IActionResult> PendingPaymentsEXCEL()
        {
            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/PendingPaymentsEXCEL");

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

        #region Section: Remain Payments PDF & Excel

        /// <summary>
        /// Generates and returns a PDF document of remaining payments.
        /// </summary>
        /// <returns>A PDF file if successful; otherwise, an error message.</returns>

        public async Task<IActionResult> RemainPaymentsPDF()
        {
            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/RemainPaymentsPDF");

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

        /// <summary>
        /// Generates and returns an Excel document of remaining payments.
        /// </summary>
        /// <returns>An Excel file if successful; otherwise, an error message.</returns>
        public async Task<IActionResult> RemainPaymentsEXCEL()
        {
            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/RemainPaymentsEXCEL");

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

        #region Section: Paid Payments PDF & Excel

        /// <summary>
        /// Generates and returns a PDF document of paid payments.
        /// </summary>
        /// <returns>A PDF file if successful; otherwise, an error message.</returns>

        public async Task<IActionResult> PaidPaymentsPDF()
        {
            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/PaidPaymentsPDF");

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

        /// <summary>
        /// Generates and returns an Excel document of paid payments.
        /// </summary>
        /// <returns>An Excel file if successful; otherwise, an error message.</returns>
        public async Task<IActionResult> PaidPaymentsEXCEL()
        {
            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/PaidPaymentsEXCEL");

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


        [HttpPost]
        public IActionResult DeletePaidStatusPayment(string Payment_ID,string Stock_ID)
        {
            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Payment/DeletePaidStatusPayment?Payment_ID={UrlEncryptor.Decrypt(Payment_ID)}&Stock_ID{UrlEncryptor.Decrypt(Stock_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Delete Successfully!",
                    redirectUrl = Url.Action("PaidPayments")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Error. Please try again.",
                    redirectUrl = Url.Action("PaidPayments")
                });
            }
        }

        [HttpPost]
        public IActionResult DeleteRemainStatusPayment(string Payment_ID, string Stock_ID)
        {
            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Payment/DeleteRemainStatusPayment?Payment_ID={UrlEncryptor.Decrypt(Payment_ID)}&Stock_ID={UrlEncryptor.Decrypt(Stock_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Delete Successfully!",
                    redirectUrl = Url.Action("RemainPayments")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Error. Please try again.",
                    redirectUrl = Url.Action("RemainPayments")
                });
            }
        }

        [HttpPost]
        public IActionResult DeletePendingStatusPayment(string Stock_ID)
        {
            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Payment/DeletePendingStatusPayment?Stock_ID={UrlEncryptor.Decrypt(Stock_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Delete Successfully!",
                    redirectUrl = Url.Action("PendingPayments")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Error. Please try again.",
                    redirectUrl = Url.Action("PendingPayments")
                });
            }
        }


    }
}
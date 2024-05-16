using Microsoft.AspNetCore.Mvc;
using Stock_Management_System.Areas.Accounts.Models;
using System.Data;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Newtonsoft.Json;
using Stock_Management_System.Areas.Stocks.Models;
using Stock_Management_System.UrlEncryption;
using ClosedXML.Excel;
using Stock_Management_System.API_Services;
using Customer_Model = Stock_Management_System.Areas.Accounts.Models.Customer_Model;
using static Stock_Management_System.Areas.Invoices.Models.InvoiceModel;
using System.Net;
using Stock_Management_System.Areas.Manage.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using Stock_Management_System.BAL;
using System.Net.Http.Headers;

namespace Stock_Management_System.Areas.Accounts.Controllers
{

    [Area("Accounts")]
    [Route("~/[controller]/[action]")]
    [CheckAccess]
    public class AccountController : Controller
    {

        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://localhost:7024/api");

        public readonly HttpClient _Client;

        public IHttpContextAccessor _HttpContextAccessor;

        public Api_Service api_Service = new Api_Service();

        

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration where settings like API endpoints are stored.</param>
        /// <remarks>
        /// The constructor sets up the HttpClient that will be used for API calls. It sets the base address for all API requests
        /// to ensure all requests are prefixed correctly. It assumes that the 'baseAddress' provided is the correct and operational API URL.
        /// </remarks>

        public AccountController(IConfiguration configuration)
        {
            Configuration = configuration;
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
            _HttpContextAccessor = new HttpContextAccessor();
        }

        #region Section: Dropdown Function

        /// <summary>
        /// Fetches and prepares a list of banks for a dropdown menu, including updating icon paths with a full URL.
        /// </summary>
        /// <remarks>
        /// This method retrieves bank details from an external service, appending the base URL to the icon paths of each bank,
        /// and stores the list in a ViewBag for use in views. It's designed to be called in the context of preparing views that
        /// need a list of banks.
        /// </remarks>

        public async Task Dropdown_For_Bank_Names()
        {
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

        #region Section: Create Customer Account

        /// <summary>
        /// Displays the view for adding a new customer.
        /// </summary>
        /// <returns>Returns the view for adding a new customer.</returns>

        public IActionResult AddCustomer()
        {

            return View();
        }

        /// <summary>
        /// Inserts a new customer into the system based on the provided model.
        /// </summary>
        /// <param name="customers_Model">The customer model containing data to be inserted.</param>
        /// <returns>A JSON response indicating success or failure.</returns>
        public async Task<IActionResult> InsertCustomer(Customer_Model customers_Model)
        {

            var jsonContent = JsonConvert.SerializeObject(customers_Model);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Customers/AddCustomer", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Customers")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    redirectUrl = Url.Action("AddCustomer")
                });
            }

        }

        #endregion

        #region Section: Update Customer Account

        /// <summary>
        /// Edits a customer's information by retrieving it based on customer ID and type.
        /// </summary>
        /// <param name="Customer_ID">Encrypted customer ID.</param>
        /// <param name="Customer_Type">Customer type.</param>
        /// <returns>Returns the edit view populated with the customer's information.</returns>

        public async Task<IActionResult> EditCustomer(string Customer_ID, string Customer_Type)
        {

            Customer_Model customer_Model = new Customer_Model();

            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = _Client.GetAsync($"{_Client.BaseAddress}/Customers/GetCustomerByIDAndType/{UrlEncryptor.Decrypt(Customer_ID)}&{Customer_Type}").Result;

            string data = await response.Content.ReadAsStringAsync();
            dynamic jsonObject = JsonConvert.DeserializeObject(data);
            var dataObject = jsonObject.data;
            var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
            customer_Model = JsonConvert.DeserializeObject<Customer_Model>(extractedDataJson);

            return View(customer_Model);
        }

        /// <summary>
        /// Updates customer details in the database.
        /// </summary>
        /// <param name="customers_Model">Customer model data used for updating the customer details.</param>
        /// <returns>Returns JSON result indicating success or failure.</returns>
        public async Task<IActionResult> UpdateCustomerDetails(Customer_Model customers_Model)
        {

            var jsonContent = JsonConvert.SerializeObject(customers_Model);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.PutAsync($"{_Client.BaseAddress}/Customers/UpdateCustomer", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Customers", "Account")
                });
            }
            else
            {

                return Json(new
                {
                    success = false,
                    redirectUrl = Url.Action("Customers", "Account")
                });
            }

        }

        #endregion

        #region Section: Delete Customer And Account Statement

        /// <summary>
        /// Deletes a customer record.
        /// </summary>
        /// <param name="Customer_ID">Encrypted customer ID.</param>
        /// <param name="Customer_Type">Customer type.</param>
        /// <returns>Returns JSON result with a success or error message.</returns>

        [HttpPost]
        public IActionResult DeleteCustomer(string Customer_ID, string Customer_Type)
        {
            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Customers/DeleteCustomer?Customer_ID={UrlEncryptor.Decrypt(Customer_ID)}&Customer_Type={Customer_Type}").Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Delete Successfully!",
                    redirectUrl = Url.Action("Customers")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Error. Please try again.",
                    redirectUrl = Url.Action("Customers")
                });
            }
        }

        #endregion

        #region Section: Show All Customers Account

        /// <summary>
        /// Retrieves and displays a list of all customers.
        /// </summary>
        /// <returns>A view displaying a list of customers.</returns>
        public async Task<IActionResult> Customers()
        {

            List<Customer_Model> customers = new List<Customer_Model>();

            customers = await api_Service.List_Of_Data_Display<Customer_Model>("Customers/GetAllCustomers");

            return View(customers);
        }

        #endregion

        #region Section: Account Information

        /// <summary>
        /// Displays details for a specific customer account including purchased stock.
        /// </summary>
        /// <param name="Customer_ID">Encrypted customer ID.</param>
        /// <param name="Customer_Type">Customer type.</param>
        /// <returns>Returns a view displaying the account and stock details of a specific customer.</returns>
        public async Task<IActionResult> CustomerAccount(string Customer_ID, string Customer_Type)
        {
            if (HttpContext.Request.Headers["Referer"].ToString() == "")
            {
                return RedirectToAction("Customers");
            }

            await Dropdown_For_Bank_Names();

            CustomerDetails_With_Purchased_Stock_Model customerDetails_With_Purchased_Stock = new CustomerDetails_With_Purchased_Stock_Model();

            _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            /*  CustomerDetails_With_Purchased_Stock_Model customerDetails_With_Purchased_Stock = await api_Service.Model_Of_Data_Display<CustomerDetails_With_Purchased_Stock_Model>("Customers/Account_Details", Convert.ToInt32(UrlEncryptor.Decrypt(Customer_ID)),Customer_Type);*/
            HttpResponseMessage response = _Client.GetAsync($"{_Client.BaseAddress}/Customers/GetAccountDetails/{UrlEncryptor.Decrypt(Customer_ID)}&{Customer_Type}").Result;

            string data = await response.Content.ReadAsStringAsync();
            dynamic jsonObject = JsonConvert.DeserializeObject(data);
            var dataObject = jsonObject.data;
            var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
            customerDetails_With_Purchased_Stock = JsonConvert.DeserializeObject<CustomerDetails_With_Purchased_Stock_Model>(extractedDataJson);

            return View(customerDetails_With_Purchased_Stock);

        }

        #endregion

        #region Section: Customer Accounts PDF And Excel

        /// <summary>
        /// Generates and provides a downloadable PDF statement of customer accounts.
        /// </summary>
        /// <returns>A file result containing the PDF document.</returns>
        public async Task<IActionResult> GenerateCustomersPDFStatement()
        {

            _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/CustomersStatementPDF");

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
        /// Generates and provides a downloadable Excel statement of customer accounts.
        /// </summary>
        /// <returns>A file result containing the Excel document.</returns>
        public async Task<IActionResult> GenerateCustomersEXCELStatement()
        {
            _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/CustomersStatementEXCEL");

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

        #region Section: Accounts Statement PDF And Excel

        /// <summary>
        /// Generates a PDF statement for a specific customer account.
        /// </summary>
        /// <param name="Customer_ID">Encrypted customer ID.</param>
        /// <param name="Customer_Type">Customer type.</param>
        /// <returns>A file result containing the PDF statement for the specified customer account.</returns>
        public async Task<IActionResult> GenerateAccountPDFStatement(string Customer_ID, string Customer_Type)
        {
            _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/CustomerAccountStatementPDF/{Convert.ToInt32(UrlEncryptor.Decrypt(Customer_ID))}&{Customer_Type}");

            if (response.IsSuccessStatusCode)
            {
                // Extract filename from Content-Disposition header
                var contentDisposition = response.Content.Headers.ContentDisposition;

                // Set a default filename or use the one from the header if available

                string filename = contentDisposition?.FileNameStar;

                // Decode the filename to handle URL encoded characters
                filename = Uri.UnescapeDataString(filename);

                // Ensure the filename has the correct ".pdf" extension and no unwanted characters
                filename = filename.Replace("\"", "").Trim(); // Remove any quotes and trim spaces
                if (!filename.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    filename += ".pdf"; // Safely append .pdf if it's not there
                }

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
        /// Generates an Excel statement for a specific customer account.
        /// </summary>
        /// <param name="Customer_ID">Encrypted customer ID.</param>
        /// <param name="Customer_Type">Customer type.</param>
        /// <returns>A file result containing the Excel statement for the specified customer account.</returns>
        public async Task<IActionResult> GenerateAccountEXCELStatement(string Customer_ID, string Customer_Type)
        {
            _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/CustomerAccountStatementEXCEL/{Convert.ToInt32(UrlEncryptor.Decrypt(Customer_ID))}&{Customer_Type}");

            if (response.IsSuccessStatusCode)
            {

                // Extract filename from Content-Disposition header
                var contentDisposition = response.Content.Headers.ContentDisposition;

                // Set a default filename or use the one from the header if available

                string filename = contentDisposition?.FileNameStar;

                // Decode the filename to handle URL encoded characters
                filename = Uri.UnescapeDataString(filename);

                // Ensure the filename has the correct ".pdf" extension and no unwanted characters
                filename = filename.Replace("\"", "").Trim(); // Remove any quotes and trim spaces
                if (!filename.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    filename += ".xlsx"; // Safely append .pdf if it's not there
                }

                var ExcelContent = await response.Content.ReadAsByteArrayAsync();
                return File(ExcelContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            else
            {
                // Handle error or return an error response
                return BadRequest("Could not generate Excel.");
            }

        }

        #endregion

        /*       #region Section : Get Data For Payment

                public async Task<ActionResult> GetPaymentInfo(string Customer_ID, string Stock_ID)
                {
                    Payment_All_Models.Payment_Model payment_Model = new Payment_All_Models.Payment_Model();
                    // Assuming _Client is properly initialized HttpClient
                    HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Payment/GetFullPaymentInfo/{UrlEncryptor.Decrypt(Customer_ID)}&{UrlEncryptor.Decrypt(Stock_ID)}");

                    if (response.IsSuccessStatusCode)
                    {
                        await Dropdown_For_Bank_Names();

                        string data = await response.Content.ReadAsStringAsync();
                        dynamic jsonObject = JsonConvert.DeserializeObject(data);
                        var dataObject = jsonObject.data;
                        var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
                        payment_Model = JsonConvert.DeserializeObject<Payment_All_Models.Payment_Model>(extractedDataJson);



                        return PartialView("_PaymentInfo_Box", payment_Model);
                    }
                    else
                    {
                        return null;
                    }






                    #endregion
        */

    }
}
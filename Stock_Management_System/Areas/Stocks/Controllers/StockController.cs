using Microsoft.AspNetCore.Mvc;

using System.Data;
using Stock_Management_System.Areas.Stocks.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Text;
using Newtonsoft.Json;
using Stock_Management_System.UrlEncryption;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using ClosedXML.Excel;
using Stock_Management_System.API_Services;
using Stock_Management_System.All_DropDowns;
using Microsoft.AspNetCore.Mvc.Rendering;
using Customers_Model = Stock_Management_System.Areas.Accounts.Models.Customer_Model;
using Stock_Management_System.Areas.Accounts.Models;
using Stock_Management_System.Services;

namespace Stock_Management_System.Areas.Stocks.Controllers
{

    [Area("Stocks")]
    [Route("~/[controller]/[action]")]
    public class StockController : Controller
    {

        #region Section: Constructor and Configuration

        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://localhost:7024/api");

        public readonly HttpClient _Client;

        private readonly Api_Service api_Service = new Api_Service();

        /// <summary>
        /// Initializes a new instance of the <see cref="StockController"/> class.
        /// </summary>
        /// <param name="configuration">Configuration settings to be used by the controller.</param>
        /// <remarks>
        /// Sets up the HttpClient with a base address and initializes the Api_Service.
        /// </remarks>
        public StockController(IConfiguration configuration)
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

        #region Section: Add Purchase Stock

        /// <summary>
        /// Initiates the process to add a new stock entry.
        /// </summary>

        public async Task<IActionResult> AddStock()
        {

            await PopulateDropdownLists();

            return View();
        }

        /// <summary>
        /// Adds details for a new stock entry.
        /// </summary>

        public async Task<IActionResult> InsertStockAndCustomerDetails(Customers_Stock_Combined_Model model)
        {
            // Check if the customer is new; if so, add them
            if (model.Customers.CustomerId == 0)
            {
                // Add the new customer and directly use the returned model
                Customer_Model customerInfo = await CreateCustomer(model.Customers);
                if (customerInfo == null || customerInfo.CustomerId == 0)
                {
                    return BadRequest("Failed to create a new customer.");
                }
                model.Customers = customerInfo; // Update the model with the new customer info
            }

            // Prepare the data object for updating stock details
            var dataObject = new
            {
                purchase_Stock = model.Insert_Purchase_Stock,
                customers_Model = model.Customers // Use the updated/existing customer details
            };

            // Serialize the data object to JSON
            var jsonContent = JsonConvert.SerializeObject(dataObject);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the PUT request to the "Stock/Update_Purchase_Stock" endpoint
            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Stock/AddPurchaseStockWithCustomerDetails", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("Stocks")
                });
            }
            else
            {
                // Handle failures, possibly returning an error status or message
                return StatusCode((int)response.StatusCode, "Error message here");
            }

        }

        #endregion

        #region Section: Update Purchase Stock

        public async Task<IActionResult> EditStock(string TN_ID, string Customer_ID)
        {

            await PopulateDropdownLists();

            Customers_Stock_Combined_Model customers_Stock_Combined_Model = new Customers_Stock_Combined_Model();

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Stock/GetPurchaseStockAndCustomerDetails/{UrlEncryptor.Decrypt(TN_ID)}&{UrlEncryptor.Decrypt(Customer_ID)}");

            if (response.IsSuccessStatusCode)
            {

                string data = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                var dataObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
                customers_Stock_Combined_Model = JsonConvert.DeserializeObject<Customers_Stock_Combined_Model>(extractedDataJson);

            }

            return View(customers_Stock_Combined_Model);

        }

        public async Task<IActionResult> UpdateStockAndCustomerDetails(Customers_Stock_Combined_Model model)
        {

            // Check if the customer is new; if so, add them
            if (model.Customers.CustomerId == 0)
            {
                // Add the new customer and directly use the returned model
                Customer_Model customerInfo = await CreateCustomer(model.Customers);
                if (customerInfo == null || customerInfo.CustomerId == 0)
                {
                    return BadRequest("Failed to create a new customer.");
                }
                model.Customers = customerInfo; // Update the model with the new customer info
            }

            // Prepare the data object for updating stock details
            var dataObject = new
            {
                purchase_Stock = model.Insert_Purchase_Stock,
                customers_Model = model.Customers // Use the updated/existing customer details
            };

            // Serialize the data object to JSON
            var jsonContent = JsonConvert.SerializeObject(dataObject);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the PUT request to the "Stock/Update_Purchase_Stock" endpoint
            HttpResponseMessage response = await _Client.PutAsync($"{_Client.BaseAddress}/Stock/UpdatePurchaseStock", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("Stocks")
                });
            }
            else
            {
                // Handle failures, possibly returning an error status or message
                return StatusCode((int)response.StatusCode, "Error message here");
            }
        }

        #endregion

        #region Section: Get Customer Details From AutoComplete

        /// <summary>
        /// Retrieves customer data by name for autocomplete feature.
        /// </summary>
        private async Task<Customer_Model> GetCustomerProfile(int Customer_ID, string Customer_Type)
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Customers/GetCustomerByIDAndType/{Customer_ID}&{Customer_Type}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                Customer_Model customerInfo = JsonConvert.DeserializeObject<Customer_Model>(JsonConvert.SerializeObject(jsonObject.data, Formatting.Indented));
                return customerInfo;
            }
            return null;
        }

        /// <summary>
        /// Asynchronously gets a list of customer models matching a specified name, intended for use with an autocomplete feature.
        /// </summary>
        /// <param name="CustomerName">The name of the customer to search for.</param>
        /// <returns>A JsonResult containing a list of matching customer models.</returns>
        public async Task<JsonResult> GetBuyerCustomerData(string CustomerName)
        {
            List<Customer_Model> customerModels = await FetchBuyerCustomerByName(CustomerName);
            return Json(customerModels);
        }

        /// <summary>
        /// Asynchronously fetches customer models by name from a remote API, intended to check if the buyer customer exists in the system.
        /// </summary>
        /// <param name="CustomerName">The name of the customer to be fetched.</param>
        /// <returns>A list of customer models that match the provided name.</returns>
        private async Task<List<Customer_Model>> FetchBuyerCustomerByName(string CustomerName)
        {
            List<Customer_Model> customer_Models = new List<Customer_Model>();

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Customers/IsBuyerCustomerExist/{CustomerName}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                var dataObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
                customer_Models = JsonConvert.DeserializeObject<List<Customer_Model>>(extractedDataJson);
            }

            return customer_Models;
        }

        #endregion

        #region Section: Stocks

        public async Task<IActionResult> Stocks()
        {

            await PopulateDropdownLists();

            List<Purchase_Stock> stockModels = await api_Service.List_Of_Data_Display<Purchase_Stock>("Stock/GetAllPurchaseStocks");

            return View(stockModels);
        }

        #endregion

        #region Section: Check Customer Exist Or Not

        /// <summary>
        /// Synchronously checks if a customer exists in the system by their ID.
        /// </summary>
        /// <param name="Customer_ID">The ID of the customer to check.</param>
        /// <returns>A Customer_Model if the customer exists; otherwise, null.</returns>
        private Customer_Model FindCustomer(int Customer_ID)
        {
            HttpResponseMessage response = _Client.GetAsync($"{_Client.BaseAddress}/Customers/Get_Customer/{Customer_ID}").Result;
            Customer_Model Customer_Info = new Customer_Model();

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                var DataObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject(DataObject, Formatting.Indented);
                Customer_Info = JsonConvert.DeserializeObject<Customer_Model>(extractedDataJson);
                return Customer_Info;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Section: Create New Customer

        /// <summary>
        /// Asynchronously adds a new customer to the system.
        /// </summary>
        /// <param name="customerModel">The customer model to add.</param>
        /// <returns>The added Customer_Model with updated details (e.g., new ID), or null if the operation fails.</returns>
        [HttpPost]
        public async Task<Customer_Model> CreateCustomer(Customer_Model customerModel)
        {
            var jsonContent = JsonConvert.SerializeObject(customerModel);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Customers/AddCustomer", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<Dictionary<string,
                  dynamic>>(responseString);

                if (responseObject != null && responseObject["status"] == true)
                {
                    var updatedCustomer = JsonConvert.DeserializeObject<Customer_Model>(Convert.ToString(responseObject["data"]));
                    return updatedCustomer; // This will now include all details, including the new ID
                }
            }
            return null;
        }

        #endregion

        #region Section: Delete Stock

        /// <summary>
        /// Synchronously deletes a stock item based on its transaction ID (TN_ID).
        /// </summary>
        /// <param name="TN_ID">The encrypted transaction ID of the stock item to delete.</param>
        /// <returns>A JsonResult indicating success or failure of the operation.</returns>
        [HttpPost]
        public IActionResult DeleteStock(string TN_ID)
        {
            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Stock/RemovePurchaseStock?TN_ID={UrlEncryptor.Decrypt(TN_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Deleted Successfully!",
                    redirectUrl = Url.Action("Stocks")
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

        #region Section: Stock Details Show By Stock ID

        /// <summary>
        /// Displays the details of a stock item identified by a transaction ID.
        /// </summary>
        /// <param name="TN_ID">The encrypted transaction ID of the stock item.</param>
        /// <returns>An IActionResult that renders a view to show detailed stock information.</returns>
        public async Task<IActionResult> StockDetailsView(string TN_ID)
        {
            if (HttpContext.Request.Headers["Referer"].ToString() == "")
            {
                return RedirectToAction("Stocks");
            }

            Purchase_Stock purchase_Stock = new Purchase_Stock();
            purchase_Stock = await api_Service.Model_Of_Data_Display<Purchase_Stock>("Stock/GetPurchaseStockById", Convert.ToInt32(UrlEncryptor.Decrypt(TN_ID)));
            return View(purchase_Stock);
        }

        #endregion

        #region Section: Download Statement PDF & EXCEL

        /// <summary>
        /// Generates a PDF statement for purchased stocks and sends it to the client.
        /// </summary>
        /// <returns>An IActionResult containing the PDF file or an error message.</returns>
        public async Task<IActionResult> GenerateStocksPDFStatement()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/PurchaseStocksStatementPDF");

            if (response.IsSuccessStatusCode)
            {
                var contentDisposition = response.Content.Headers.ContentDisposition;
                string filename = contentDisposition?.FileName;
                var pdfContent = await response.Content.ReadAsByteArrayAsync();
                return File(pdfContent, "application/pdf", filename);
            }
            else
            {
                return BadRequest("Could not generate PDF.");
            }
        }

        /// <summary>
        /// Exports a list of all purchased stocks to an Excel file and sends it to the client.
        /// </summary>
        /// <returns>An IActionResult containing the Excel file or an error message.</returns>
        public async Task<IActionResult> GenerateStocksExcelStatement()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/PurchaseStocksStatementEXCEL");

            if (response.IsSuccessStatusCode)
            {
                var contentDisposition = response.Content.Headers.ContentDisposition;
                string filename = contentDisposition?.FileName;
                var ExcelContent = await response.Content.ReadAsByteArrayAsync();
                return File(ExcelContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            else
            {
                return BadRequest("Could not generate Excel.");
            }
        }

        #endregion

    }

}
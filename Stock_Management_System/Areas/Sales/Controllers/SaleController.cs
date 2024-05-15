using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Stock_Management_System.All_DropDowns;
using Stock_Management_System.API_Services;
using Stock_Management_System.Areas.Accounts.Models;
using Stock_Management_System.Areas.Manage.Models;
using Stock_Management_System.Areas.Sales.Models;
using Stock_Management_System.Areas.Stocks.Models;
using Stock_Management_System.UrlEncryption;

using System.Text;
using static Stock_Management_System.Areas.Manage.Models.Payment_All_Models;

namespace Stock_Management_System.Areas.Sales.Controllers
{
    [Area("Sales")]
    [Route("~/[controller]/[action]")]
    public class SaleController : Controller
    {
        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://localhost:7024/api");

        public readonly HttpClient _Client;

        public Api_Service api_Service = new Api_Service();

        public SaleController(IConfiguration configuration)
        {
            Configuration = configuration;
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
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

            List<Our_Banks_Dropdown> bank_Models = await api_Service.List_Of_Data_Display<Our_Banks_Dropdown>("Bank/GetOurBanksSelectList");

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

                ViewBag.OurBanks = bank_Models;
            }
        }

        #endregion

        #region Section: Add Sale

        /// <summary>
        /// Displays the view to add a new sale.
        /// </summary>
        public async Task<IActionResult> AddSale()
        {

            await PopulateDropdownLists();

            return View();
        }

        /// <summary>
        /// Inserts a new sale into the system along with customer details.
        /// </summary>
        /// <param name="model">Combined model of sale and customer data.</param>
        public async Task<IActionResult> InsertSale(Sale_Customer_Combied_Model model)
        {
            // Check if the customer is new; if so, add them
            if (model.customer.CustomerId == 0)
            {
                // Add the new customer and directly use the returned model
                Customer_Model customerInfo = await CreateCustomer(model.customer);
                if (customerInfo == null || customerInfo.CustomerId == 0)
                {
                    return BadRequest("Failed to create a new customer.");
                }
                model.customer = customerInfo; // Update the model with the new customer info
            }
            else
            {
                // Fetch existing customer details if not adding new customer
                model.customer = await GetCustomerProfile(model.customer.CustomerId, model.customer.CustomerType);
                if (model.customer == null)
                {
                    return NotFound("Customer not found.");
                }
            }

            // Prepare the data object for posting sale details
            var dataObject = new
            {
                sale = model.sale,
                customer = model.customer // Use the updated/existing customer details
            };

            // Serialize the data object to JSON
            var jsonContent = JsonConvert.SerializeObject(dataObject);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Post the model to the "Sales/AddSale" endpoint
            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Sale/AddSale", stringContent);
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Sales")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    redirectUrl = Url.Action("AddSale")
                });
            }
        }


        #endregion

        #region Section: Update Sale

        /// <summary>
        /// Displays the view for editing a sale and its associated customer.
        /// </summary>
        /// <param name="Sale_ID">Encrypted ID of the sale.</param>
        /// <param name="Customer_ID">Encrypted ID of the customer associated with the sale.</param>

        public async Task<IActionResult> EditSale(string Sale_ID, string Customer_ID)
        {

            await PopulateDropdownLists();

            Sale_Customer_Combied_Model sale_Customer_Combied_Model = new Sale_Customer_Combied_Model();

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Sale/GetSaleAndCustomerDetails/{UrlEncryptor.Decrypt(Sale_ID)}&{UrlEncryptor.Decrypt(Customer_ID)}");

            if (response.IsSuccessStatusCode)
            {

                string data = await response.Content.ReadAsStringAsync();

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                

                dynamic jsonObject = JsonConvert.DeserializeObject(data,settings);
                var dataObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
                sale_Customer_Combied_Model = JsonConvert.DeserializeObject<Sale_Customer_Combied_Model>(extractedDataJson);

            }

            return View(sale_Customer_Combied_Model);

        }

        /// <summary>
        /// Updates the details of an existing sale and customer in the system.
        /// </summary>
        /// <param name="model">Combined model of sale and customer data.</param>
        public async Task<IActionResult> UpdateSaleDetails(Sale_Customer_Combied_Model model)
        {
            // Check if the customer is new; if so, add them
            if (model.customer.CustomerId == 0)
            {
                // Add the new customer and directly use the returned model
                Customer_Model customerInfo = await CreateCustomer(model.customer);
                if (customerInfo == null || customerInfo.CustomerId == 0)
                {
                    return BadRequest("Failed to create a new customer.");
                }
                model.customer = customerInfo; // Update the model with the new customer info
            }
            else
            {
                // Fetch existing customer details if not adding new customer
                model.customer = await GetCustomerProfile(model.customer.CustomerId, model.customer.CustomerType);
                if (model.customer == null)
                {
                    return NotFound("Customer not found.");
                }
            }

            // Prepare the data object for posting sale details
            var dataObject = new
            {
                sale = model.sale,
                customer = model.customer // Use the updated/existing customer details
            };

            // Serialize the data object to JSON
            var jsonContent = JsonConvert.SerializeObject(dataObject);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Post the model to the "Sales/AddSale" endpoint
            HttpResponseMessage response = await _Client.PutAsync($"{_Client.BaseAddress}/Sale/UpdateSale", stringContent);
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Sales")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    redirectUrl = Url.Action("AddSale")
                });
            }

        }

        #endregion

        #region Section: Sales

        /// <summary>
        /// Displays the list of all sales.
        /// </summary>

        public async Task<IActionResult> Sales()
        {
            await PopulateDropdownLists();

            List<Show_Sale> sales = await api_Service.List_Of_Data_Display<Show_Sale>("Sale/GetAllSales");
            return View(sales);
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
        public async Task<JsonResult> GetSellerCustomerData(string CustomerName)
        {
            List<Customer_Model> customerModels = await FetchSellerCustomerByName(CustomerName);
            return Json(customerModels);
        }

        /// <summary>
        /// Asynchronously fetches customer models by name from a remote API, intended to check if the buyer customer exists in the system.
        /// </summary>
        /// <param name="CustomerName">The name of the customer to be fetched.</param>
        /// <returns>A list of customer models that match the provided name.</returns>
        private async Task<List<Customer_Model>> FetchSellerCustomerByName(string CustomerName)
        {
            List<Customer_Model> customer_Models = new List<Customer_Model>();

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Customers//{CustomerName}");

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

        #region Section: Delete Sale

        /// <summary>
        /// Deletes a sale based on its ID.
        /// </summary>
        /// <param name="Sale_ID">Encrypted ID of the sale to be deleted.</param>

        [HttpPost]
        public IActionResult DeleteSale(string Sale_ID)
        {
            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Sale/DeleteSale?Sale_ID={UrlEncryptor.Decrypt(Sale_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Delete Successfully!",
                    redirectUrl = Url.Action("Sales")
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

        #region Section: Sale Details View By Sale ID

        /// <summary>
        /// Displays the details of a specific sale.
        /// </summary>
        /// <param name="Sale_ID">Encrypted ID of the sale.</param>

        public async Task<IActionResult> SaleDetailsView(string Sale_ID)
        {
            if (HttpContext.Request.Headers["Referer"].ToString() == "")
            {
                return RedirectToAction("Sales");
            }

            Show_Sale show_Sale = new Show_Sale();

            show_Sale = await api_Service.Model_Of_Data_Display<Show_Sale>("Sale/GetSaleByID", Convert.ToInt32(UrlEncryptor.Decrypt(Sale_ID)));

            return View(show_Sale);
        }

        #endregion

        #region Section: Download Statement PDF & EXCEL

        /// <summary>
        /// Downloads the sales statement as a PDF.
        /// </summary>

        public async Task<IActionResult> SalesStatementPDF()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/SalesStatementPDF");

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
        /// Downloads the sales statement as an Excel file.
        /// </summary>
        public async Task<IActionResult> SalesStatementEXCEL()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/SalesStatementEXCEL");

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

        #region Section: Sale Information By ID

        /// <summary>
        /// Fetches and displays specific sale information based on the sale ID.
        /// </summary>
        /// <param name="Sale_ID">Encrypted ID of the sale.</param>

        public async Task<IActionResult> GetSaleInfo(string Sale_ID)
        {
            Show_Sale show_Sale = new Show_Sale();
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Sale/GetSaleByID/{UrlEncryptor.Decrypt(Sale_ID)}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                show_Sale = JsonConvert.DeserializeObject<Show_Sale>(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(data).data, Formatting.Indented));
                return PartialView("_ShowSaleInfo_Box", show_Sale);
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
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

        public async Task<IActionResult> AddSale()
        {
          

            await PopulateDropdownLists();

            return View();
        }



        public async Task<JsonResult> GetSellerCustomerData(string CustomerName)
        {
            List<Customer_Model> customerModels = await FetchSellerCustomerByName(CustomerName);
            return Json(customerModels);
        }

        private async Task<List<Customer_Model>> FetchSellerCustomerByName(string CustomerName)
        {
            List<Customer_Model> customer_Models = new List<Customer_Model>();

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Customers/IsSellerCustomerExist/{CustomerName}");

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




        #region Insert Sale If Customer Check not Exitst and then Insert Also Customer 


        public async Task<IActionResult> InsertSale(Sale_Customer_Combied_Model model)
        {
            // Check if the customer exists; if not, add them
            Customer_Model customerInfo = await GetCustomerProfile(model.customer.CustomerId, model.customer.CustomerType);
            if (customerInfo == null)
            {
                // Add the new customer and directly use the returned model
                customerInfo = await CreateCustomer(model.customer);
                if (customerInfo == null || customerInfo.CustomerId == 0)
                {
                    return BadRequest("Failed to create a new customer.");
                }
            }

            // Prepare the data object for posting sale details
            var dataObject = new
            {
                sale = model.sale,
                customer = customerInfo // Use the updated or existing customer details
            };

            // Serialize the anonymous object to JSON
            var jsonContent = JsonConvert.SerializeObject(dataObject);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Post the model to the "Sales/Insert_Sale" endpoint
            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Sale/AddSale", content);
            if (response.IsSuccessStatusCode)
            {
                // Return a JSON response to redirect the client to another action
                return Json(new { success = true, redirectUrl = Url.Action("Sales") });
            }
            else
            {
                // Read and handle failures, possibly returning an error status or message
                var responseContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, responseContent);
            }
        }




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

        public async Task<IActionResult> Sales()
        {
            await PopulateDropdownLists();

            List<Show_Sale> sales = await api_Service.List_Of_Data_Display<Show_Sale>("Sale/GetAllSales");
            return View(sales);
        }



        [HttpPost]
        public IActionResult DeleteSale(string Sale_ID)
        {
            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Sale/DeleteSale?Sale_ID={UrlEncryptor.Decrypt(Sale_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Delete Successfully!", redirectUrl = Url.Action("Sales") });
            }
            else
            {
                return Json(new { success = false, message = "Error. Please try again." });
            }
        }


        public async Task<IActionResult> UpdateSaleDetails(Sale_Customer_Combied_Model model)
        {
            // Check if the customer exists; if not, add them
            Customer_Model customerInfo = await GetCustomerProfile(model.customer.CustomerId, model.customer.CustomerType);
            if (customerInfo == null)
            {
                // Add the new customer and directly use the returned model
                customerInfo = await CreateCustomer(model.customer);
                if (customerInfo == null || customerInfo.CustomerId == 0)
                {
                    return BadRequest("Failed to create a new customer.");
                }
            }

            // Prepare the data object for posting sale details
            var dataObject = new
            {
                sale = model.sale,
                customer = customerInfo // Use the updated or existing customer details
            };

            // Serialize the anonymous object to JSON
            var jsonContent = JsonConvert.SerializeObject(dataObject);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Post the model to the "Sales/Insert_Sale" endpoint
            HttpResponseMessage response = await _Client.PutAsync($"{_Client.BaseAddress}/Sale/UpdateSale", content);
            if (response.IsSuccessStatusCode)
            {
                // Return a JSON response to redirect the client to another action
                return Json(new { success = true, redirectUrl = Url.Action("Sales") });
            }
            else
            {
                // Read and handle failures, possibly returning an error status or message
                var responseContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, responseContent);
            }

        }

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



        public async Task<IActionResult> EditSale(string Sale_ID, string Customer_ID)
        {

            await PopulateDropdownLists();

           

            Sale_Customer_Combied_Model sale_Customer_Combied_Model = new Sale_Customer_Combied_Model();

            
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Sale/GetSaleAndCustomerDetails/{UrlEncryptor.Decrypt(Sale_ID)}&{UrlEncryptor.Decrypt(Customer_ID)}");


            if (response.IsSuccessStatusCode)
            {

                string data = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                var dataObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
                sale_Customer_Combied_Model = JsonConvert.DeserializeObject<Sale_Customer_Combied_Model>(extractedDataJson);



            }

            return View(sale_Customer_Combied_Model);






        }

        #region Method : Download Statement PDF & EXCEL 
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

    }
}

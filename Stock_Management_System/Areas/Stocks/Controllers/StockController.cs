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

namespace Stock_Management_System.Areas.Stocks.Controllers
{

    [Area("Stocks")]
    [Route("~/[controller]/[action]")]
    public class StockController : Controller
    {
        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://localhost:7024/api");

        public readonly HttpClient _Client;


        private readonly Api_Service api_Service = new Api_Service();


        public StockController(IConfiguration configuration)
        {
            Configuration = configuration;

            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;



        }


        #region Method : Dropdown Function

        public async Task All_Dropdowns_Call()
        {
            All_DropDown_Model all_DropDown_Model = new All_DropDown_Model();

            All_DropDowns_Class all_DropDowns_Class = new All_DropDowns_Class();

            all_DropDown_Model = await all_DropDowns_Class.Get_All_DropdDowns_Data();





            if (all_DropDown_Model != null)
            {
                ViewBag.Products = new SelectList(all_DropDown_Model.Products_DropDowns_List, "ProductId", "ProductNameInGujarati");

                ViewBag.ProductGrade = new SelectList(all_DropDown_Model.Products_Grade_DropDowns_List, "ProductGradeId", "ProductGrade");
                ViewBag.Vehicle = new SelectList(all_DropDown_Model.Vehicle_DropDowns_List, "VehicleId", "VehicleName");
            }




        }

        #endregion



        #region Method : Add Stock Function 

        public async Task<IActionResult> Add_Stock()
        {

            await All_Dropdowns_Call();

            return View();
        }


        public async Task<IActionResult> Add_Stock_Details(Customers_Stock_Combined_Model model)
        {
            // Check if the customer exists; if not, add them
            Customer_Model customerInfo = await Existing_Customer_Details(model.Customers.CustomerId, model.Customers.CustomerType);
            if (customerInfo == null)
            {
                // Add the new customer and directly use the returned model
                customerInfo = await Add_New_Customer(model.Customers);
                if (customerInfo == null || customerInfo.CustomerId == 0)
                {
                    return BadRequest("Failed to create a new customer.");
                }
            }

            // Prepare the data object for posting stock details
            var dataObject = new
            {
                purchase_Stock = model.Insert_Purchase_Stock,
                customers_Model = customerInfo // Use the updated/existing customer details
            };

            // Serialize the anonymous object to JSON
            var jsonContent = JsonConvert.SerializeObject(dataObject);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Post the model to the "Stock/Insert_Purchase_Stock" endpoint
            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Stock/Insert_Purchase_Stock", content);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { redirectUrl = Url.Action("Manage_Stocks", "Stock") });
            }
            else
            {
                // Handle failures, possibly returning an error status or message
                return StatusCode((int)response.StatusCode, "Error message here");
            }

        }


        private async Task<Customer_Model> Existing_Customer_Details(int Customer_ID, string Customer_Type)
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Customers/Get_Customer/{Customer_ID}&{Customer_Type}");
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


        #region Method : Update Stock 

        public async Task<IActionResult> Update_Stock(string TN_ID, string Customer_ID)
        {

            await All_Dropdowns_Call();


            Customers_Stock_Combined_Model customers_Stock_Combined_Model = new Customers_Stock_Combined_Model();


            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Stock/Fetch_Stock_And_Customer_Details/{UrlEncryptor.Decrypt(TN_ID)}&{UrlEncryptor.Decrypt(Customer_ID)}");


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

        public async Task<IActionResult> Update_Stock_Details(Customers_Stock_Combined_Model model)
        {



            // Check if the customer is new; if so, add them
            if (model.Customers.CustomerId == 0)
            {
                // Add the new customer and directly use the returned model
                Customer_Model customerInfo = await Add_New_Customer(model.Customers);
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
            HttpResponseMessage response = await _Client.PutAsync($"{_Client.BaseAddress}/Stock/Update_Purchase_Stock", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { redirectUrl = Url.Action("Manage_Stocks", "Stock") });
            }
            else
            {
                // Handle failures, possibly returning an error status or message
                return StatusCode((int)response.StatusCode, "Error message here");
            }
        }



        #endregion

        #region Method : Get Customer Details From AutoComplete
        public async Task<JsonResult> Get_Buyer_Customer_Data(string CustomerName)
        {
            List<Customer_Model> customerModels = await fetch_buyer_customer_name(CustomerName);
            return Json(customerModels);
        }


        private async Task<List<Customer_Model>> fetch_buyer_customer_name(string CustomerName)
        {
            List<Customer_Model> customer_Models = new List<Customer_Model>();

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Customers/BUYER_CUSTOMER_EXIST_IN_SYSTEM/{CustomerName}");

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


        #region Method : Manage Stock



        public async Task<IActionResult> Manage_Stocks()
        {




            await All_Dropdowns_Call();



            List<Purchase_Stock> stockModels = await api_Service.List_Of_Data_Display<Purchase_Stock>("Stock/Purchase_Stocks");


            return View(stockModels);
        }


        #endregion


        #region Method : Check Customer Exist Or Not 





        private Customer_Model CHECK_CUSTOMER_INFO_IN_SYSTEM(int Customer_ID)
        {



            HttpResponseMessage response = _Client.GetAsync($"{_Client.BaseAddress}/Customers/Get_Customer/{Customer_ID}").Result;

            Customer_Model Customer_Info = new Customer_Model();

            if (response.IsSuccessStatusCode)
            {


                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                var DataObject = jsonObject.data;
                var extractedDtaJson = JsonConvert.SerializeObject(DataObject, Formatting.Indented);
                Customer_Info = JsonConvert.DeserializeObject<Customer_Model>(extractedDtaJson);
                return Customer_Info;
            }
            else
            {
                return null;
            }


        }




        [HttpPost]
        public async Task<Customer_Model> Add_New_Customer(Customer_Model customerModel)
        {
            var jsonContent = JsonConvert.SerializeObject(customerModel);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Customers/Insert_Customer", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseString);

                if (responseObject != null && responseObject["status"] == true)
                {
                    var updatedCustomer = JsonConvert.DeserializeObject<Customer_Model>(Convert.ToString(responseObject["data"]));
                    return updatedCustomer; // This will now include all details, including the new ID
                }
            }
            return null;
        }





        #endregion


        #region Method : Delete Stock

        [HttpPost]
        public IActionResult Delete_Stock(string TN_ID)
        {
            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Stock/Delete_Purchase_Stock?TN_ID={UrlEncryptor.Decrypt(TN_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Delete Successfully!", redirectUrl = Url.Action("Manage_Stocks") });
            }
            else
            {
                return Json(new { success = false, message = "Error. Please try again." });
            }
        }





        #endregion


        #region Method : Stock Details Show By Stock ID 




        public async Task<IActionResult> Added_Stock_Details(string TN_ID)
        {
            if (HttpContext.Request.Headers["Referer"].ToString() == "")
            {
                return RedirectToAction("Manage_Stocks");
            }



            Purchase_Stock purchase_Stock = new Purchase_Stock();


            purchase_Stock = await api_Service.Model_Of_Data_Display<Purchase_Stock>("Stock/Get_Purchase_Stock_By_Id", Convert.ToInt32(UrlEncryptor.Decrypt(TN_ID)));




            return View(purchase_Stock);




        }

        #endregion


        #region Method : Download Statement PDF & EXCEL 
        public async Task<IActionResult> Purchase_Stocks_Statement_CreatePDF()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/Purchase_Stocks_Statement_PDF");

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


        public async Task<IActionResult> Export_Stock_List_To_Excel()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/Purchase_Stocks_Statement_EXCEL");

            if (response.IsSuccessStatusCode)
            {
                // Extract filename from Content-Disposition header
                var contentDisposition = response.Content.Headers.ContentDisposition;
                string filename = contentDisposition?.FileName;

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

    }






}
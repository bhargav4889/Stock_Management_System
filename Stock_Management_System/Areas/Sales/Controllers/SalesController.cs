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
    public class SalesController : Controller
    {
        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://localhost:7024/api");

        public readonly HttpClient _Client;

        public Api_Service api_Service = new Api_Service();


        public SalesController(IConfiguration configuration)
        {
            Configuration = configuration;
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
        }
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

        public async Task Dropdown_For_Our_Bank_Names()
        {
            List<Our_Banks_Dropdown> bank_Models = await api_Service.List_Of_Data_Display<Our_Banks_Dropdown>("Bank/Get_Our_Banks");
            if (bank_Models != null)
            {
                string baseUrl = "https://localhost:7024/";
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

        public async Task<IActionResult> Create_Sales()
        {
            await All_Dropdowns_Call();

            await Dropdown_For_Our_Bank_Names();

            return View();
        }



        public async Task<JsonResult> Get_Seller_Customer_Data(string CustomerName)
        {
            List<Customers_Model> customerModels = await fetch_seller_customer_name(CustomerName);
            return Json(customerModels);
        }

        private async Task<List<Customers_Model>> fetch_seller_customer_name(string CustomerName)
        {
            List<Customers_Model> customer_Models = new List<Customers_Model>();

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Customers/SELLER_CUSTOMER_EXIST_IN_SYSTEM/{CustomerName}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                var dataObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
                customer_Models = JsonConvert.DeserializeObject<List<Customers_Model>>(extractedDataJson);
            }

            return customer_Models;
        }




        #region Insert Sale If Customer Check not Exitst and then Insert Also Customer 


        public async Task<IActionResult> Add_Sale_Details(Sale_Customer_Combied_Model model)
        {
            // Check if the customer exists; if not, add them
            Customer_Model customerInfo = await Existing_Customer_Details(model.customer.CustomerId);
            if (customerInfo == null)
            {
                // Attempt to add a new customer
                customerInfo = await Add_New_Customer(model.customer);
                if (customerInfo == null || customerInfo.CustomerId == 0)
                {
                    // Log the error or handle it as needed
                    return BadRequest("Failed to create a new customer.");
                }
            }

            // Prepare the data object for posting sale details
            var dataObject = new
            {
                sale = model.sale,
                customer = customerInfo // Using the updated or existing customer details
            };

            // Serialize the data object to JSON
            var jsonContent = JsonConvert.SerializeObject(dataObject);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Post the data to the "Sales/Insert_Sale" endpoint
            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Sales/Insert_Sale", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Manage_Sales");
            }
            else
            {
                // Read the error response to help with debugging
                var responseContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, responseContent);
            }
        }



        private async Task<Customer_Model> Existing_Customer_Details(int customerId)
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Customers/Get_Customer/{customerId}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                Customer_Model customerInfo = JsonConvert.DeserializeObject<Customer_Model>(JsonConvert.SerializeObject(jsonObject.data, Formatting.Indented));
                return customerInfo;
            }
            return null;
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



        public  async Task<IActionResult> Manage_Sales()
        {
            await All_Dropdowns_Call();

            List<Show_Sale> sales = await api_Service.List_Of_Data_Display<Show_Sale>("Sales/Sales");
            return View(sales);
        }

        public async Task<IActionResult> Added_Sales_Details(string Sale_ID)
        {
            if (HttpContext.Request.Headers["Referer"].ToString() == "")
            {
                return RedirectToAction("Manage_Sales");
            }


            Show_Sale show_Sale = new Show_Sale();


            show_Sale = await api_Service.Model_Of_Data_Display<Show_Sale>("Sales/Get_Sale_By_ID", Convert.ToInt32(UrlEncryptor.Decrypt(Sale_ID)));

            return View(show_Sale);
        }

        #region Method : Download Statement PDF & EXCEL 
        public async Task<IActionResult> Sales_Statement_CreatePDF()
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


        public async Task<IActionResult> Export_Sales_To_Excel()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/Purchase_Stocks_Statement_EXCEL");

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


        public async Task<IActionResult> Get_Sale_Info(string Sale_ID)
        {
            Show_Sale show_Sale = new Show_Sale();
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Sales/Get_Sale_By_ID/{UrlEncryptor.Decrypt(Sale_ID)}");

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

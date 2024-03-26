using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Stock_Management_System.All_DropDowns;
using Stock_Management_System.API_Services;
using Stock_Management_System.Areas.Accounts.Models;
using Stock_Management_System.Areas.Manage.Models;
using Stock_Management_System.Areas.Stocks.Models;
using Stock_Management_System.UrlEncryption;
using System.Net;
using System.Text;

namespace Stock_Management_System.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("~/[controller]/[action]")]
    public class PaymentController : Controller
    {


        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://localhost:7024/api");

        public readonly HttpClient _Client;

        public Api_Service api_Service = new Api_Service();


        public PaymentController(IConfiguration configuration)
        {
            Configuration = configuration;
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
        }

        #region Method : All Dropdown 

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

        public async Task Dropdown_For_Bank_Names()
        {
            List<Bank_Model> bank_Models = await api_Service.List_Of_Data_Display<Bank_Model>("Bank/Get_Bank_Names");

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

        public async  Task<IActionResult> Create_Payment(Payment_All_Models.Payment_Model payment_Model)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(payment_Model);
                var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Payment/Create_Payment", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Pending_Payments");
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

            return RedirectToAction("Pending_Payments");
        }







        public async Task<IActionResult> Pending_Payments()
        {
            await All_Dropdowns_Call();
            await Dropdown_For_Bank_Names();
            List<Payment_All_Models.Pending_Customers_Payments> List_of_Pending_Customers_Payments = await api_Service.List_Of_Data_Display<Payment_All_Models.Pending_Customers_Payments>("Payment/Pending_Customers_Payments");

            return View(List_of_Pending_Customers_Payments);
        }

       


        #region Method : Get Data For Payment For Modal

        public async Task<ActionResult> Get_Payment_Info(string Customer_ID, string Stock_ID)
        {
            Payment_All_Models.Payment_Model payment_Model = new Payment_All_Models.Payment_Model();

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Payment/Get_Payment_Info/{UrlEncryptor.Decrypt(Customer_ID)}&{UrlEncryptor.Decrypt(Stock_ID)}");

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

           



        }
    }
}

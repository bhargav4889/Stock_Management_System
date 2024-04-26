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
using static Stock_Management_System.Areas.Manage.Models.Payment_All_Models;

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
            All_DropDown_Model all_DropDown_Model = await new All_DropDowns_Class().Get_All_DropdDowns_Data();
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
                string baseUrl = "https://localhost:7024/";
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

        #region Method : Create Payment (PENDING) 

        public async Task<IActionResult> Create_Payment(Payment_All_Models.Payment_Model payment_Model)
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
        }

        #endregion

        #region Method : Create Payment (REMAIN) 

        public async Task<IActionResult> Create_Remain_Payment(Remain_Payment_Model remain_Payment_Model)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(remain_Payment_Model);
                var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Payment/Create_Remain_Payment", stringContent);

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
        }

        #endregion

        #region List of Payment Pendings 

        public async Task<IActionResult> Pending_Payments()
        {
            await All_Dropdowns_Call();
            await Dropdown_For_Bank_Names();
            List<Payment_All_Models.Pending_Customers_Payments> List_of_Pending_Customers_Payments = await api_Service.List_Of_Data_Display<Payment_All_Models.Pending_Customers_Payments>("Payment/Pending_Customers_Payments");
            return View(List_of_Pending_Customers_Payments);
        }

        #endregion

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
                payment_Model = JsonConvert.DeserializeObject<Payment_All_Models.Payment_Model>(JsonConvert.SerializeObject(jsonObject.data, Formatting.Indented));
                return PartialView("_PaymentInfo_Box", payment_Model);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Method : Remain Payment Get Info

        public async Task<IActionResult> Remain_Get_Payment_Info(string Customer_ID, string Stock_ID)
        {
            Remain_Payment_Model remain_Payment_Model = new Remain_Payment_Model();
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Payment/Get_Remain_Payment_Info/{UrlEncryptor.Decrypt(Customer_ID)}&{UrlEncryptor.Decrypt(Stock_ID)}");

            if (response.IsSuccessStatusCode)
            {
                await Dropdown_For_Bank_Names();
                string data = await response.Content.ReadAsStringAsync();
                remain_Payment_Model = JsonConvert.DeserializeObject<Remain_Payment_Model>(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(data).data, Formatting.Indented));
                return PartialView("_RemainPaymentInfo_Box", remain_Payment_Model);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Method : Get Payment Info By Customer ID and Stock ID 

        public async Task<ActionResult> Payment_Info_By_Customer_And_Stock(string Customer_ID, string Stock_ID)
        {
            Payment_All_Models.Show_Payment_Info show_Payment_Info = new Payment_All_Models.Show_Payment_Info();
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Payment/Payment_Info_By_Customer_ID_AND_Stock_ID/{UrlEncryptor.Decrypt(Customer_ID)}&{UrlEncryptor.Decrypt(Stock_ID)}");

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
    }
}

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

        #region Method : Create Payment (PENDING) 

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

        #region Method : Create Payment (REMAIN) 

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

        #region List of Payment Pendings 

        public async Task<IActionResult> PendingPayments()
        {
            await PopulateDropdownLists();
          
            List<Payment_All_Models.Pending_Customers_Payments> List_of_Pending_Customers_Payments = await api_Service.List_Of_Data_Display<Payment_All_Models.Pending_Customers_Payments>("Payment/GetPendingCustomersPayments");
            return View(List_of_Pending_Customers_Payments);
        }

        #endregion

        #region List of Payment Remain 

        public async Task<IActionResult> RemainPayments()
        {
            await PopulateDropdownLists();
         
            List<Payment_All_Models.Remain_Payment_Model> List_of_Remain_Customers_Payments = await api_Service.List_Of_Data_Display<Payment_All_Models.Remain_Payment_Model>("Payment/GetRemainingCustomersPayments");
            return View(List_of_Remain_Customers_Payments);
        }

        #endregion

        #region List of Payment PAID 

        public async Task<IActionResult> PaidPayments()
        {
            await PopulateDropdownLists();
        
            List<Payment_All_Models.Show_Payment_Info> List_of_Paid_Customers_Payments = await api_Service.List_Of_Data_Display<Payment_All_Models.Show_Payment_Info>("Payment/GetPaidCustomersPayments");
            return View(List_of_Paid_Customers_Payments);
        }

        #endregion

        #region Method : Get Data For Payment For Modal

        public async Task<ActionResult> GetPaymentInfoByCustomerStockID(string Customer_ID, string Stock_ID)
        {
            Payment_All_Models.Payment_Model payment_Model = new Payment_All_Models.Payment_Model();
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

        #region Method : Remain Payment Get Info

        public async Task<IActionResult> GetRemainPaymentInfo(string Customer_ID, string Stock_ID)
        {
            Remain_Payment_Model remain_Payment_Model = new Remain_Payment_Model();
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

        #region Method : Get Payment Info By Customer ID and Stock ID 

        public async Task<ActionResult> GetPaymentInfo(string Customer_ID, string Stock_ID)
        {
            Payment_All_Models.Show_Payment_Info show_Payment_Info = new Payment_All_Models.Show_Payment_Info();
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


        #region Method : Pending Payments PDF & Excel

        public async Task<IActionResult> PendingPaymentsPDF()
        {


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

        public async Task<IActionResult> PendingPaymentsEXCEL()
        {
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

        #region Method : Remain Payments PDF & Excel

        public async Task<IActionResult> RemainPaymentsPDF()
        {


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

        public async Task<IActionResult> RemainPaymentsEXCEL()
        {
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

        #region Method : Paid Payments PDF & Excel

        public async Task<IActionResult> PaidPaymentsPDF()
        {


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

        public async Task<IActionResult> PaidPaymentsEXCEL()
        {
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


    }
}
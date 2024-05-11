using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stock_Management_System.API_Services;
using Stock_Management_System.Areas.Accounts.Models;
using Stock_Management_System.Areas.Information.Models;
using Stock_Management_System.Areas.Manage.Models;
using Stock_Management_System.UrlEncryption;
using System.Text;

namespace Stock_Management_System.Areas.Information.Controllers
{
    [Area("Information")]
    [Route("~/[controller]/[action]")]
    public class InformationController : Controller
    {
        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://localhost:7024/api");

        public readonly HttpClient _Client;

        public Api_Service api_Service = new Api_Service();


        public InformationController(IConfiguration configuration)
        {
            Configuration = configuration;
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
        }

        public async Task Dropdown_For_Bank_Names()
        {
            List<Bank_Model> bank_Models = await api_Service.List_Of_Data_Display<Bank_Model>("Bank/GetBanksList");
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

        public async Task<IActionResult> AddBankInformation()
        {
            await Dropdown_For_Bank_Names();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> InsertBankInformation(Information_Model information_Model)
        {
            
            
                var jsonContent = JsonConvert.SerializeObject(information_Model);
                var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Information/AddBankInformation", stringContent);


                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("ShowSaveInformations", "Information") });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Server error: {errorResponse}" });
                }

            }


        [HttpPost]
        public async Task<IActionResult> UpdateBankInformationDetails(Information_Model information_Model)
        {


            var jsonContent = JsonConvert.SerializeObject(information_Model);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _Client.PutAsync($"{_Client.BaseAddress}/Information/UpdateBankInformation", stringContent);


            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, redirectUrl = Url.Action("ShowSaveInformations") });
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = $"Server error: {errorResponse}" });
            }

        }

        [HttpPost]
        public IActionResult DeleteBankInformation(string Information_ID)
        {
            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Information/DeleteInformation?Information_ID={UrlEncryptor.Decrypt(Information_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Delete Successfully!", redirectUrl = Url.Action("ShowSaveInformations") });
            }
            else
            {
                return Json(new { success = false, message = "Error. Please try again." });
            }
        }

        public async Task<IActionResult> ShowSaveInformations()
        {
            List<Information_Model> information_Models = await api_Service.List_Of_Data_Display<Information_Model>("Information/GetAllSaveInformation");
            return View(information_Models);
        }

        public async Task<IActionResult> GetInformationByID(string Information_ID)
        {
            Information_Model information_Model = new Information_Model();
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Information/InformationByID/{UrlEncryptor.Decrypt(Information_ID)}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                information_Model = JsonConvert.DeserializeObject<Information_Model>(JsonConvert.SerializeObject(jsonObject.data, Formatting.Indented));
                return PartialView("_ShowBankInfo_Box", information_Model);
            }
            else
            {
                return null;
            }
        }

        public async Task<IActionResult> EditBankInformation(string Information_ID)
        {
            await Dropdown_For_Bank_Names();

            Information_Model information_Model = new Information_Model();
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Information/InformationByID/{UrlEncryptor.Decrypt(Information_ID)}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                information_Model = JsonConvert.DeserializeObject<Information_Model>(JsonConvert.SerializeObject(jsonObject.data, Formatting.Indented));
                
            }

            return View(information_Model);
        }
    }
}

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

        public async Task<IActionResult> Add_Bank_Information()
        {
            await Dropdown_For_Bank_Names();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Insert_Bank_Information(Information_Model information_Model)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(information_Model);
                var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Information/Insert_Bank_Information", stringContent);

                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction("Dashboard", "Manage");
                }

            }
            catch
            {
                return null;
            }

            return RedirectToAction("Dashboard", "Manage");

        }

        public async Task<IActionResult> Show_Save_Informations()
        {
            List<Information_Model> information_Models = await api_Service.List_Of_Data_Display<Information_Model>("Information/Show_All_Save_Informations");
            return View(information_Models);
        }

        public async Task<IActionResult> Get_Information_By_ID(string Information_ID)
        {
            Information_Model information_Model = new Information_Model();
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Information/Information_By_ID/{UrlEncryptor.Decrypt(Information_ID)}");

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
    }
}

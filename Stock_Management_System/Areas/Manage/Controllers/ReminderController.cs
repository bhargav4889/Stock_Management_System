using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stock_Management_System.API_Services;
using Stock_Management_System.Areas.Information.Models;
using Stock_Management_System.Areas.Manage.Models;
using Stock_Management_System.UrlEncryption;
using System.Text;
using static Stock_Management_System.Areas.Manage.Models.Payment_All_Models;

namespace Stock_Management_System.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("~/[controller]/[action]")]
    public class ReminderController : Controller
    {
        Uri baseaddress = new Uri("https://localhost:7024/api");

        public readonly HttpClient _Client;

        private readonly Api_Service api_Service = new Api_Service();

        public ReminderController()
        {
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
        }


        public IActionResult Create_Reminder()
        {
            return View();
        }

        public async Task<IActionResult> Insert_Reminder(Reminder_Model reminder)
        {
            var jsonContent = JsonConvert.SerializeObject(reminder);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Reminder/Create_Reminder", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Manage_Reminders");
            }
            else
            {
                return RedirectToAction("Manage_Reminders");
            }
        }


        public async Task<IActionResult> Manage_Reminders()
        {
            List<Reminder_Model> reminders = await api_Service.List_Of_Data_Display<Reminder_Model>("Reminder/Reminders");
            return View(reminders);
        }

        public async Task<IActionResult> Update_Reminder(string Reminder_ID)
        {
            Reminder_Model reminder = new Reminder_Model();
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Reminder/Get_Reminder_By_ID/{UrlEncryptor.Decrypt(Reminder_ID)}");

            if (response.IsSuccessStatusCode)
            {

                string data = await response.Content.ReadAsStringAsync();
                reminder = JsonConvert.DeserializeObject<Reminder_Model>(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(data).data, Formatting.Indented));
                return View(reminder);
            }
            else
            {
                return default;
            }
        }


        public async Task<IActionResult> Get_Reminder_By_ID(string Reminder_ID)
        {
            Reminder_Model reminder = new Reminder_Model();
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Reminder/Get_Reminder_By_ID/{UrlEncryptor.Decrypt(Reminder_ID)}");

            if (response.IsSuccessStatusCode)
            {

                string data = await response.Content.ReadAsStringAsync();
                reminder = JsonConvert.DeserializeObject<Reminder_Model>(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(data).data, Formatting.Indented));
                return PartialView("_ShowReminderInfo_Box", reminder);
            }
            else
            {
                return default;
            }
        }
    }
}

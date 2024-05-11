using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stock_Management_System.API_Services;
using Stock_Management_System.Areas.Accounts.Models;
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


        public IActionResult CreateReminder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InsertReminder(Reminder_Model reminder)
        {
            var jsonContent = JsonConvert.SerializeObject(reminder);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Reminder/AddReminder", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, redirectUrl = Url.Action("Reminders") });
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = $"Server error: {errorResponse}" });
            }
        }



        [HttpPost]
        public async Task<IActionResult> UpdateReminderDetails(Reminder_Model reminder)
        {
            var jsonContent = JsonConvert.SerializeObject(reminder);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _Client.PutAsync($"{_Client.BaseAddress}/Reminder/UpdateReminder", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, redirectUrl = Url.Action("Reminders") });
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = $"Server error: {errorResponse}" });
            }
        }




        public async Task<IActionResult> Reminders()
        {
            List<Reminder_Model> reminders = await api_Service.List_Of_Data_Display<Reminder_Model>("Reminder/GetAllReminders");
            return View(reminders);
        }

        public async Task<IActionResult> EditReminder(string Reminder_ID)
        {
            Reminder_Model reminder = new Reminder_Model();
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Reminder/GetReminderByID/{UrlEncryptor.Decrypt(Reminder_ID)}");

            if (response.IsSuccessStatusCode)
            {

                string data = await response.Content.ReadAsStringAsync();
                reminder = JsonConvert.DeserializeObject<Reminder_Model>(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(data).data, Formatting.Indented));
               
            }
            return View(reminder);
        }


        public async Task<IActionResult> GetReminderByID(string Reminder_ID)
        {
            Reminder_Model reminder = new Reminder_Model();
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Reminder/GetReminderByID/{UrlEncryptor.Decrypt(Reminder_ID)}");

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


        [HttpPost]
        public IActionResult DeleteReminder(string Reminder_ID)
        {
            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Reminder/DeleteReminder?Reminder_ID={UrlEncryptor.Decrypt(Reminder_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Delete Successfully!", redirectUrl = Url.Action("Reminders") });
            }
            else
            {
                return Json(new { success = false, message = "Error. Please try again." });
            }
        }
    }
}

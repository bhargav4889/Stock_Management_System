using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stock_Management_System.API_Services;
using Stock_Management_System.Areas.Accounts.Models;
using Stock_Management_System.Areas.Information.Models;
using Stock_Management_System.Areas.Manage.Models;
using Stock_Management_System.BAL;
using Stock_Management_System.UrlEncryption;
using System.Text;
using static Stock_Management_System.Areas.Manage.Models.Payment_All_Models;

namespace Stock_Management_System.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("~/[controller]/[action]")]
    [CheckAccess]
    public class ReminderController : Controller
    {
        Uri baseaddress = new Uri("https://stock-manage-api-shree-ganesh-agro-ind.somee.com/api");

        public readonly HttpClient _Client;

        private readonly Api_Service api_Service = new Api_Service();

        public HttpContextAccessor _HttpContextAccessor;

        public ReminderController()
        {
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
            _HttpContextAccessor = new HttpContextAccessor();
        }

        #region Section: Create Reminder

        /// <summary>
        /// Returns the Create Reminder view.
        /// </summary>

        public IActionResult CreateReminder()
        {
            return View();
        }

        /// <summary>
        /// Inserts a new reminder into the database via an API call.
        /// </summary>
        /// <param name="reminder">The reminder model to insert.</param>

        [HttpPost]
        public async Task<IActionResult> InsertReminder(Reminder_Model reminder)
        {
            var jsonContent = JsonConvert.SerializeObject(reminder);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Reminder/AddReminder", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Reminders")
                });
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return Json(new
                {
                    success = false,
                    message = $"Server error: {errorResponse}"
                });
            }
        }

        #endregion

        #region Section: Update Remidner

        /// <summary>
        /// Fetches and displays a specific reminder by ID for editing.
        /// </summary>
        /// <param name="Reminder_ID">The encrypted ID of the reminder.</param>

        public async Task<IActionResult> EditReminder(string Reminder_ID)
        {
            Reminder_Model reminder = new Reminder_Model();

            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Reminder/GetReminderByID/{UrlEncryptor.Decrypt(Reminder_ID)}");

            if (response.IsSuccessStatusCode)
            {

                string data = await response.Content.ReadAsStringAsync();
                reminder = JsonConvert.DeserializeObject<Reminder_Model>(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(data).data, Formatting.Indented));

            }
            return View(reminder);
        }

        /// <summary>
        /// Updates a reminder's details via an API call.
        /// </summary>
        /// <param name="reminder">The reminder model to update.</param>

        [HttpPost]
        public async Task<IActionResult> UpdateReminderDetails(Reminder_Model reminder)
        {
            var jsonContent = JsonConvert.SerializeObject(reminder);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.PutAsync($"{_Client.BaseAddress}/Reminder/UpdateReminder", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Reminders")
                });
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return Json(new
                {
                    success = false,
                    message = $"Server error: {errorResponse}"
                });
            }
        }

        #endregion

        #region Section: Reminders

        /// <summary>
        /// Displays all reminders via an API call.
        /// </summary>

        public async Task<IActionResult> Reminders()
        {
            List<Reminder_Model> reminders = await api_Service.List_Of_Data_Display<Reminder_Model>("Reminder/GetAllReminders");
            return View(reminders);
        }

        #endregion

        #region Section: Get Reminder By ID

        /// <summary>
        /// Fetches a specific reminder by ID via an API call.
        /// </summary>
        /// <param name="Reminder_ID">The encrypted ID of the reminder.</param>
        public async Task<IActionResult> GetReminderByID(string Reminder_ID)
        {
            Reminder_Model reminder = new Reminder_Model();

            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

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

        #endregion

        #region Section: Delete Reminder

        /// <summary>
        /// Deletes a specific reminder by ID via an API call.
        /// </summary>
        /// <param name="Reminder_ID">The encrypted ID of the reminder to delete.</param>

        [HttpPost]
        public IActionResult DeleteReminder(string Reminder_ID)
        {
            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/Reminder/DeleteReminder?Reminder_ID={UrlEncryptor.Decrypt(Reminder_ID)}").Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Delete Successfully!",
                    redirectUrl = Url.Action("Reminders")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Error. Please try again."
                });
            }
        }

        #endregion
    }
}
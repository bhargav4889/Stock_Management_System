using Microsoft.AspNetCore.Mvc;
using Stock_Management_System.BAL;
using System.Data.SqlClient;
using System.Data;
using Stock_Management_System.Areas.Manage.Models;
using Newtonsoft.Json;
using Stock_Management_System.API_Services;
using Stock_Management_System.Areas.Accounts.Models;
using Stock_Management_System.UrlEncryption;

namespace Stock_Management_System.Areas.Manage.Controllers
{

    [Area("Manage")]
    [Route("~/[controller]/[action]")]
    [CheckAccess]
    public class ManageController : Controller
    {
        #region Section :  Configuration

        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://localhost:7024/api");

        public readonly HttpClient _Client;

        public HttpContextAccessor _HttpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageController"/> class with specified configuration.
        /// </summary>
        /// <param name="configuration">Configuration interface provided by ASP.NET Core to handle settings.</param>

        public ManageController(IConfiguration configuration)
        {

            Configuration = configuration;
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
            _HttpContextAccessor = new HttpContextAccessor();
        }

        #endregion

        /// <summary>
        /// Displays the profile view.
        /// </summary>
        /// <returns>The Profile view.</returns>

        public IActionResult Profile()
        {
            return View();
        }

        /// <summary>
        /// Asynchronously displays the dashboard view with various models containing system data.
        /// </summary>
        /// <returns>The Dashboard view populated with data from various APIs.</returns>
        public async Task<IActionResult> Dashboard()
        {

            Api_Service api_Service = new Api_Service();

            Dashboard_All_Models _All_Models = new Dashboard_All_Models();

            _All_Models.Pending_Customers_Payment_Sort_List = await api_Service.List_Of_Data_Display<Pending_Customers_Payment_Sort_List>("Features/PendingCustomersPaymentSortList");

            _All_Models.recent_Actions_With_Info = await api_Service.List_Of_Data_Display<Recent_Action_Model>("Recent_Actions/GetRecentActions");

            _All_Models.upcoming_Reminders = await api_Service.List_Of_Data_Display<Upcoming_Reminders_Model>("Features/UpcomingRemindersList");


            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Counts");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                var dataObject = jsonObject.data;
                _All_Models.All_Counts_Model = JsonConvert.DeserializeObject<AllCountsModel>(JsonConvert.SerializeObject(dataObject));
            }
            else
            {
                return default;
            }


            return View(_All_Models);
        }

        /// <summary>
        /// Displays the error page.
        /// </summary>
        /// <returns>The Error Page view.</returns>
        public IActionResult Error_Page()
        {
            return View();
        }

        /// <summary>
        /// Displays the features view.
        /// </summary>
        /// <returns>The Features view.</returns>
        public IActionResult Features()
        {
            return View();
        }

    }
}
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
    /*    [CheckAccess]*/

    public class ManageController : Controller
    {
        #region Database Configuration


        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://localhost:7024/api");

        public readonly HttpClient _Client;


        public ManageController(IConfiguration configuration)
        {

            Configuration = configuration;
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;

        }

        #endregion Database Configuration

       
        public IActionResult Profile()
        {
            return View();
        }

   
        public async Task <IActionResult> Dashboard()
        {

            Api_Service api_Service = new Api_Service();

            Dashboard_All_Models _All_Models = new Dashboard_All_Models();

           

          

            _All_Models.Pending_Customers_Payment_Sort_List = await api_Service.List_Of_Data_Display<Pending_Customers_Payment_Sort_List>("Dashbaord_Features/Pending_Customers_Payment_Sort_List");

            _All_Models.All_Counts_Model = await api_Service.Model_Of_Data_Display<All_Counts_Model>("All_Counts");

            _All_Models.recent_Actions_With_Info = await api_Service.List_Of_Data_Display<Recent_Action_Model>("Recent_Actions/Recent_Actions_List");

            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/All_Counts");


            if (response.IsSuccessStatusCode)
            {

                string data = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                var dataObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
                _All_Models.All_Counts_Model = JsonConvert.DeserializeObject<All_Counts_Model>(extractedDataJson);
               


            }
            else
            {

                return default;
            }


            return View(_All_Models);
        }

       
        public IActionResult Error_Page()
        {
            return View();
        }


        public IActionResult Features()
        {
            return View();
        }

    }
}

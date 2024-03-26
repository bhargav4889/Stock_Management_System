using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stock_Management_System.Areas.Manage.Models;
using Stock_Management_System.Email_Services;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;

namespace Stock_Management_System.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("~/[controller]/[action]")]
    public class LoginController : Controller
    {


        // IConfiguration instance to access configuration settings
        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://localhost:7024/api");

        private readonly HttpClient _Client;

        private readonly IEmailSender _emailSender;


        // Constructor for LoginController, accepting IConfiguration for dependency injection
        public LoginController(IConfiguration configuration, IEmailSender emailSender)
        {
            // Initialize the Configuration property with the injected IConfiguration
            Configuration = configuration;
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
            _emailSender = emailSender;
        }



        public IActionResult Login_Page()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(AdminDetailsModel adminDetails)
        {
            string connection = this.Configuration.GetConnectionString("ConnectionDB");

            if (ModelState.IsValid)
            {
                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand command = sqlConnection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "PR_ADMIN_LOGIN";

                        command.Parameters.AddWithValue("@Admin_Username", SqlDbType.VarChar).Value = adminDetails.UserName;
                        command.Parameters.AddWithValue("@Password", SqlDbType.VarChar).Value = adminDetails.Password;

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            DataTable admininfo = new DataTable();
                            admininfo.Load(dataReader);

                            if (admininfo.Rows.Count > 0)
                            {
                                DataRow row = admininfo.Rows[0];
                                HttpContext.Session.SetString("AdminID", row["Admin_ID"].ToString());
                                HttpContext.Session.SetString("AdminName", row["Admin_Name"].ToString());
                                HttpContext.Session.SetString("AdminPassword", row["Admin_Password"].ToString());
                                HttpContext.Session.SetString("AdminPhoneNo", row["Admin_PhoneNo"].ToString());
                                HttpContext.Session.SetString("AdminEmail", row["Admin_Email"].ToString());
                                HttpContext.Session.SetString("LastLogin", row["Last_Login_Time"].ToString());


                                


                            }
                            else
                            {
                                TempData["ErrorMsg"] = "Invalid Username or Password.";
                                return RedirectToAction("Login_Page");
                            }
                        }
                    }
                }

                if (HttpContext.Session.GetString("AdminName") != null && HttpContext.Session.GetString("AdminPassword") != null)
                {
                    string Email = HttpContext.Session.GetString("AdminEmail");
                    string Username = HttpContext.Session.GetString("AdminName");

                    HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Email/Login_Email/{Email}&{Username}");

                    

                    return RedirectToAction("Dashboard", "Manage");
                }
            }

            return View("Login_Page");
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login_Page");
        }

    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stock_Management_System.API_Services;
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

       
        private Api_Service api_Service = new Api_Service();

        // Constructor for LoginController, accepting IConfiguration for dependency injection
        public LoginController(IConfiguration configuration, IEmailSender emailSender)
        {
            
            Configuration = configuration;
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
            _emailSender = emailSender;
           
        }



        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(Auth_Model model)
            {
            if (ModelState.IsValid)
            {
                string url = $"https://localhost:7024/Auth/Login";
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _Client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonResponse);

                    if (result["success"])
                    {
                        var userData = JsonConvert.SerializeObject(result["user"]);
                        User_Model user = JsonConvert.DeserializeObject<User_Model>(userData);

                        // Set individual session items
                       
                        HttpContext.Session.SetString("Auth_Name", user.Username);
                        HttpContext.Session.SetString("Auth_Email", user.Emailaddress); // Handle possible null values
                        HttpContext.Session.SetString("Auth_Phone", user.Phoneno); // Handle possible null values
                        HttpContext.Session.SetString("Last_Login", DateTime.Now.ToString());
                        HttpContext.Session.SetString("JWT_Token", user.Token);

                        return RedirectToAction("Dashboard", "Manage"); // Redirect to home page or dashboard
                    }
                    else
                    {
                        TempData["ErrorMsg"] = "User Not Found. Please check username and password.";
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return View(model);
        }








        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Forget_Password()
        {
            return View();
        }

    }
}

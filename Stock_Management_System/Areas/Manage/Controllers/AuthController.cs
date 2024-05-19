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
    public class AuthController : Controller
    {


        // IConfiguration instance to access configuration settings
        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://stock-manage-api-shree-ganesh-agro-ind.somee.com/");

        private readonly HttpClient _Client;

        private readonly IEmailSender _emailSender;


        private Api_Service api_Service = new Api_Service();

        // Constructor for LoginController, accepting IConfiguration for dependency injection
        public AuthController(IConfiguration configuration, IEmailSender emailSender)
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
                string url = $"https://stock-manage-api-shree-ganesh-agro-ind.somee.com/Auth/Login";
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
                        HttpContext.Session.SetString("Auth_ID", user.UserId.ToString());
                        HttpContext.Session.SetString("Auth_Name", user.Username);
                        HttpContext.Session.SetString("Auth_Email", user.Emailaddress); // Handle possible null values
                        HttpContext.Session.SetString("Auth_Phone", user.Phoneno); // Handle possible null values
                        HttpContext.Session.SetString("Last_Login", DateTime.Now.ToString());
                        HttpContext.Session.SetString("JWT_Token", user.Token);

                        return Json(new { success = true, redirectUrl = Url.Action("Dashboard", "Manage") });
                    }
                    else
                    {
                        return Json(new { success = false, redirectUrl = Url.Action("Login", "Auth"), errorMessage = "Invalid Username and Password !" });
                    }
                }
                else
                {
                    return Json(new { success = false, redirectUrl = Url.Action("Login", "Auth"), errorMessage = " Invalid Username or Password !" });
                }
            }

            return BadRequest(ModelState);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


        public IActionResult ResetPassword()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ResetRequestPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new
                {
                    success = false,
                    message = "Email address is required.",
                    redirectUrl = Url.Action("ResetPassword", "Auth")
                });
            }

            // Construct the full URL including the email as a query parameter
            var url = $"https://stock-manage-api-shree-ganesh-agro-ind.somee.com/Auth/request-reset?email={Uri.EscapeDataString(email)}";

            HttpResponseMessage response = await _Client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return Json(new
                {
                    success = true,
                    message = "A password reset link will be sent if the account with that email exists.",
                    redirectUrl = Url.Action("Login", "Auth")
                });
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return Json(new
                {
                    success = false,
                    message = "Email Address or Account Not Registered.",
                    redirectUrl = Url.Action("ResetPassword", "Auth")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "There was an error processing your request. Please try again.",
                    redirectUrl = Url.Action("ResetPassword", "Auth")
                });
            }
        }



        [HttpGet]
        public async Task<IActionResult> ChangePassword(string token, string email)
        {
            // Call your API or service to validate the token
            string url = $"https://stock-manage-api-shree-ganesh-agro-ind.somee.com/Auth/validate-token?email={email}&token={token}";

            HttpResponseMessage response = await _Client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonResponse);

                if (result["success"])
                {
                    // Token is valid; show the change password form
                    return View(new ResetPasswordViewModel { Email = email, Token = token });
                }
            }


            return RedirectToAction("Login", "Auth");
        }



        [HttpPost]
        public async Task<IActionResult> ChangeRequestPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid Data." });
            }

            string url = "https://stock-manage-api-shree-ganesh-agro-ind.somee.com/Auth/reset-password";
            StringContent content = new StringContent(JsonConvert.SerializeObject(new
            {
                email = model.Email,
                token = model.Token,
                newPassword = model.Password
            }), Encoding.UTF8, "application/json");
                
            HttpResponseMessage response = await _Client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonResponse);

                if (result["success"])
                {
                    return Json(new { success = true, message = "Your password has been updated successfully.", redirectUrl = Url.Action("Login", "Auth") });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to reset password: " + result["message"] });
                }
            }
            else
            {
                return Json(new { success = false, message = "Error contacting the server." });
            }
        }


    }

}

using Microsoft.AspNetCore.Http;
namespace Stock_Management_System.BAL
{
    public static class CV
    {
        private static IHttpContextAccessor _httpContextAccessor;


        static CV()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }

        public static string? Username()
        {
            string? Username = null;

            if(_httpContextAccessor.HttpContext.Session.GetString("AdminName") != null)
            {
                Username = _httpContextAccessor.HttpContext.Session.GetString("AdminName").ToString();
            }
            return Username;

        }

        public static int? UserID()
        {
            int? UserID = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("AdminID") != null)
            {
                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.Session.GetString("AdminID"));
            }
            return UserID;

        }


        public static string? Password()
        {
            string? Password = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("AdminPassword") != null)
            {
                Password = _httpContextAccessor.HttpContext.Session.GetString("AdminPassword").ToString();
            }
            return Password;

        }

        public static string? Email()
        {
            string? Email = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("AdminEmail") != null)
            {
                Email = _httpContextAccessor.HttpContext.Session.GetString("AdminEmail").ToString();
            }
            return Email;

        }

        public static string? ContactNo()
        {
            string? ContactNo = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("AdminPhoneNo") != null)
            {
                ContactNo = _httpContextAccessor.HttpContext.Session.GetString("AdminPhoneNo").ToString();
            }
            return ContactNo;

        }


        public static DateTime? LastLoginTime()
        {
            DateTime? LastLoginTime = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("LastLogin") != null)
            {
                LastLoginTime = Convert.ToDateTime(_httpContextAccessor.HttpContext.Session.GetString("LastLogin").ToString());
            }
            return LastLoginTime;

        }
    }
}

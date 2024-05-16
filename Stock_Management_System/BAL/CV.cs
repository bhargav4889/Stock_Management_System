using Microsoft.AspNetCore.Http;

namespace Stock_Management_System.BAL
{
    public class CV
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CV(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CV()
        {
        }

        public string? Username()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("Auth_Name");
        }

        public string? Email()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("Auth_Email");
        }

        public string? ContactNo()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("Auth_Phone");
        }

        public DateTime? LastLoginTime()
        {
            var lastLogin = _httpContextAccessor.HttpContext?.Session.GetString("Last_Login");
            return lastLogin != null ? Convert.ToDateTime(lastLogin) : null;
        }

        public string? JWT_Token()
        {

            return _httpContextAccessor.HttpContext?.Session.GetString("JWT_Token");
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Stock_Management_System.Areas.Manage.Models
{
    public class AdminDetailsModel
    {
        public int UserID { get; set; }

        [Required(ErrorMessage ="Please Enter Username !")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Please Enter Password !")]
        public string? Password { get; set; }


        public string? Email { get; set; }


        public int PhoneNo { get; set; }

        public DateTime LastLoginTime { get; set; }


    }
}

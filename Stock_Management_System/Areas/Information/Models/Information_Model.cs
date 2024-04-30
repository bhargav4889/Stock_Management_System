using System.ComponentModel.DataAnnotations;

namespace Stock_Management_System.Areas.Information.Models
{
    public class Information_Model
    {
        public int InformationID { get; set; }  

        [Required(ErrorMessage = "Please Select Bank")]
        public int BankId { get; set; }

        [Required(ErrorMessage = "Please enter the account number")]
        [MinLength(10, ErrorMessage = "The account number must be at least 10 digits long")]
        [MaxLength(18, ErrorMessage = "The account number must not exceed 18 digits")]
        public string? AccountNo { get; set; }


        [Required(ErrorMessage = "Please Enter Bank IFSC Code")]
        [MaxLength(11, ErrorMessage = "IFSC Code Not Grather Than 11 Charcaters")]
        public string? ifsc_code { get; set; }

        [Required(ErrorMessage = "Please Enter Account Holder Name")]
        [MinLength(10, ErrorMessage = "The Account Holder Name Not Below 10 Charcaters")]
        public string? AccountHolderName { get; set; }

        public string? BankName { get; set; }

        public string? BankIcon { get; set; }



    }

  
}

﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Stock_Management_System.Areas.Manage.Models
{
    public class Auth_Model
    {
        
          

            [Required(ErrorMessage = "Enter Username")]
            [MaxLength(10)]
            public string? Username { get; set; }

            [Required(ErrorMessage = "Please Enter Password")]
         
            [MaxLength(20,ErrorMessage ="Password Must be 10 or Above charcters")]
            public string? Password { get; set; }




        
    }

    public class User_Model
    {
        
        public int UserId { get; set; }

        public string Username { get; set; }

       
        public string Password { get; set; }

      
        public string? Emailaddress { get; set; }

     
        public string? Phoneno { get; set; }

        public string? Token { get; set; }
    }


    public class ResetPasswordViewModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

}

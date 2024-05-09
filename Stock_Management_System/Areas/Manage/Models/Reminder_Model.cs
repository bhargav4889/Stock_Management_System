using System.ComponentModel.DataAnnotations;

namespace Stock_Management_System.Areas.Manage.Models
{

    public class Reminder_Model
    {
        public string? ReminderId { get; set; }

        [Required(ErrorMessage ="Please Select Reminder Date and Time")]
        public DateTime ReminderDateTime { get; set; }

        [Required(ErrorMessage ="Please Select Type Reminder")]
        public string? ReminderType { get; set; }

        [Required(ErrorMessage ="Please Enter Custom Type Reminder")]
        public string? ReminderCustomType { get; set; }

        [Required(ErrorMessage ="Please Enter Description")]
        [MinLength(10)]
        public string? ReminderDescription { get; set; }

        public string? SentEmailAddress { get; set; }

        public string? SentPhoneNo { get; set; }



    }
}

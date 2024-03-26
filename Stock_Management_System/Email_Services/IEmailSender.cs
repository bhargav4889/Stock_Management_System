namespace Stock_Management_System.Email_Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

}

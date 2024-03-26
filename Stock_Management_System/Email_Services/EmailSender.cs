namespace Stock_Management_System.Email_Services
{
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;

    public class EmailSender : IEmailSender
    {
        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSSL;
        private readonly string _userName;
        private readonly string _password;

        public EmailSender(string host, int port, bool enableSSL, string userName, string password)
        {
            _host = host;
            _port = port;
            _enableSSL = enableSSL;
            _userName = userName;
            _password = password;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpClient = new SmtpClient(_host)
            {
                Port = _port,
                Credentials = new NetworkCredential(_userName, _password),
                EnableSsl = _enableSSL,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_userName,"Stock Manage System"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            return smtpClient.SendMailAsync(mailMessage);
        }
    }

}

using server.Bll.Interfaces;
using System.Net.Mail;

namespace server.Bll
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendWinnerEmailAsync(string toEmail, string giftName)
        {
            _logger.LogInformation("Sending winner email started. To={ToEmail}, Gift={GiftName}", toEmail, giftName);

            try
            {
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var appPassword = _configuration["EmailSettings:AppPassword"];
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var port = int.Parse(_configuration["EmailSettings:Port"]);

                var mail = new MailMessage();
                mail.To.Add(toEmail);
                mail.Subject = "זכית בהגרלה!";
                mail.Body = $"מזל טוב! זכית במתנה: {giftName}";
                mail.From = new MailAddress(fromEmail);

                using var smtp = new SmtpClient(smtpServer, port);
                smtp.Credentials = new System.Net.NetworkCredential(fromEmail, appPassword);
                smtp.EnableSsl = true;

                await smtp.SendMailAsync(mail);

                _logger.LogInformation("Winner email sent successfully. To={ToEmail}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send winner email. To={ToEmail}, Gift={GiftName}", toEmail, giftName);
            }
        }
    }
}

using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;

namespace E_commerce_Back_end.OPT
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(EmailDto request,string email_user)
        {
            var email = new MimeMessage();
            // email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailSettings:SmtpUsername").Value));
            email.From.Add(new MailboxAddress(_config.GetSection("EmailSettings:SenderName").Value, _config.GetSection("EmailSettings:SenderEmail").Value));
            email.To.Add(MailboxAddress.Parse(email_user));
            email.Subject = request.Subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = request.SetBody("Hi, " +
                "ngày mai sẽ là ngày tổ chức sự kiện, bật app check thời gian và tham " +
                "gia đúng giờ nhé!") };

            using var stmp = new MailKit.Net.Smtp.SmtpClient();
            stmp.Connect(_config.GetSection("EmailSettings:SmtpServer").Value,
                         int.Parse(_config.GetSection("EmailSettings:SmtpPort").Value),
                         MailKit.Security.SecureSocketOptions.StartTls);
            stmp.Authenticate(_config.GetSection("EmailSettings:SmtpUsername").Value,
                _config.GetSection("EmailSettings:SmtpPassword").Value);
            stmp.Send(email);
            stmp.Disconnect(true);
        }
    }
}

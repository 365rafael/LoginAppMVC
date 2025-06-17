using MailKit.Net.Smtp;
using MimeKit;

namespace LoginAppMVC.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void Enviar(string destinatario, string assunto, string mensagem)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["EmailSettings:Remetente"]));
            email.To.Add(MailboxAddress.Parse(destinatario));
            email.Subject = assunto;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = mensagem
            };

            using var smtp = new SmtpClient();
            smtp.Connect(_config["EmailSettings:Smtp"], 587, false);
            smtp.Authenticate(_config["EmailSettings:Usuario"], _config["EmailSettings:Senha"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}

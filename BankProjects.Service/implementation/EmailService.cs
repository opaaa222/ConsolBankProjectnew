using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using BankProjects.Service.interfaces;

namespace BankProjects.Service.implementation
{
    public class EmailService : IEmailService
    {
        private readonly string _senderEmail;
        private readonly string _appPassword;

        public EmailService()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<EmailService>()
                .Build();

            _senderEmail = config["EmailSettings:SenderEmail"];
            _appPassword = config["EmailSettings:AppPassword"];
        }

        public void SendLoanApprovalEmail(string toEmail, string userName, decimal loanAmount)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress("Bank Simulator", _senderEmail));
                message.To.Add(new MailboxAddress(userName, toEmail));
                message.Subject = "🎉 თქვენი სესხი დამტკიცებულია!";

                string htmlBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Segoe UI', Arial, sans-serif; background-color: #f4f7f6; margin: 0; padding: 20px; }}
                        .container {{ max-width: 550px; background-color: #ffffff; margin: 0 auto; padding: 30px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.08); }}
                        .header {{ text-align: center; padding-bottom: 20px; border-bottom: 2px solid #eef2f5; }}
                        .header h2 {{ color: #2c3e50; margin: 0; font-size: 24px; }}
                        .content {{ padding: 25px 0; color: #4a5568; line-height: 1.6; }}
                        .amount-card {{ background: linear-gradient(135deg, #10b981, #059669); color: white; text-align: center; padding: 20px; border-radius: 8px; margin: 20px 0; }}
                        .amount-card h3 {{ margin: 0; font-size: 14px; text-transform: uppercase; opacity: 0.9; }}
                        .amount-card p {{ margin: 5px 0 0 0; font-size: 32px; font-weight: bold; }}
                        .footer {{ text-align: center; padding-top: 20px; border-top: 1px solid #eef2f5; color: #a0aec0; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>🏦 Bank Simulator</h2>
                        </div>
                        <div class='content'>
                            <p>გამარჯობა, <strong>{userName}</strong>!</p>
                            <p>
                                გაცნობებთ, რომ თქვენი მოთხოვნა სესხის მიღებაზე
                                წარმატებით განხილულია და <strong>დამტკიცებულია</strong>.
                            </p>
                            <div class='amount-card'>
                                <h3>დარიცხული თანხა</h3>
                                <p>{loanAmount:N2} GEL</p>
                            </div>
                            <p>
                                თანხა უკვე ასახულია თქვენს საბანკო ანგარიშზე
                                და შეგიძლიათ გამოიყენოთ ნებისმიერ დროს.
                            </p>
                        </div>
                        <div class='footer'>
                            <p>© {DateTime.Now.Year} Bank Simulator. ყველა უფლება დაცულია.</p>
                        </div>
                    </div>
                </body>
                </html>";

                message.Body = new TextPart("html") { Text = htmlBody };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    client.Authenticate(_senderEmail, _appPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }

                Console.WriteLine($"\n[Email Sent] Email was successfully sent to: {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Email Error] {ex.GetType().Name}: {ex.Message}");
            }
        }

        public void SendLoanRejectionEmail(string toEmail, string userName, decimal loanAmount)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Bank Simulator", _senderEmail));
                message.To.Add(new MailboxAddress(userName, toEmail));
                message.Subject = "❌ სესხის მოთხოვნის სტატუსი";

                string htmlBody = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                body {{ font-family: 'Segoe UI', Arial, sans-serif; background-color: #f4f7f6; margin: 0; padding: 20px; }}
                .container {{ max-width: 550px; background-color: #ffffff; margin: 0 auto; padding: 30px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.08); }}
                .header {{ text-align: center; padding-bottom: 20px; border-bottom: 2px solid #eef2f5; }}
                .header h2 {{ color: #2c3e50; margin: 0; font-size: 24px; }}
                .content {{ padding: 25px 0; color: #4a5568; line-height: 1.6; }}
                .status-card {{ background: linear-gradient(135deg, #ef4444, #dc2626); color: white; text-align: center; padding: 20px; border-radius: 8px; margin: 20px 0; }}
                .status-card h3 {{ margin: 0; font-size: 14px; text-transform: uppercase; opacity: 0.9; }}
                .status-card p {{ margin: 5px 0 0 0; font-size: 24px; font-weight: bold; }}
                .footer {{ text-align: center; padding-top: 20px; border-top: 1px solid #eef2f5; color: #a0aec0; font-size: 12px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h2>🏦 Bank Simulator</h2>
                </div>
                <div class='content'>
                    <p>გამარჯობა, <strong>{userName}</strong>!</p>
                    <p>სამწუხაროდ, გაცნობებთ, რომ თქვენი მოთხოვნა სესხის მიღებაზე <strong>უარყოფილია</strong>.</p>
                    <div class='status-card'>
                        <h3>მოთხოვნილი თანხა</h3>
                        <p>{loanAmount:N2} GEL</p>
                    </div>
                    <p>დამატებითი კითხვების შემთხვევაში შეგიძლიათ დაუკავშირდეთ ბანკის მხარდაჭერის გუნდს.</p>
                </div>
                <div class='footer'>
                    <p>© {DateTime.Now.Year} Bank Simulator. ყველა უფლება დაცულია.</p>
                </div>
            </div>
        </body>
        </html>";

                message.Body = new TextPart("html") { Text = htmlBody };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    client.Authenticate(_senderEmail, _appPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }

                Console.WriteLine($"\n[Email Sent] email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Email Error] {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
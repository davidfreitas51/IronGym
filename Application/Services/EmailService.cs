using System.Net.Mail;
using System.Net;

namespace IronGym.Application.Services
{
    public class EmailService
    {
        public string SendVerificationEmail(string userEmail)
        {
            try
            {
                string verificationCode = GenerateRandomDigits(6);
                MailMessage mail = WriteEmail(userEmail, verificationCode);
                SmtpClient smtp = PrepareSending();
                smtp.Send(mail);
                return verificationCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public static MailMessage WriteEmail(string userEmail, string verificationCode)
        {
            MailMessage message = new MailMessage();
            message.To.Add(userEmail);
            message.From = new MailAddress("verifylockbox@gmail.com");


            message.Body = $@"
            <html>
            <head>
                <style>
                    .email-container {{
                        font-family: Arial, sans-serif;
                        line-height: 1.6;
                        color: #333333;
                        padding: 20px;
                        max-width: 600px;
                        margin: auto;
                    }}
                    .email-header {{
                        background-color: #f7f7f7;
                        padding: 10px 20px;
                        border-bottom: 1px solid #dddddd;
                    }}
                    .email-header h2 {{
                        margin: 0;
                    }}
                    .email-content {{
                        padding: 20px;
                        background-color: #ffffff;
                        border: 1px solid #dddddd;
                    }}
                    .verification-code {{
                        font-size: 1.5em;
                        color: #4CAF50;
                        font-weight: bold;
                        margin: 20px 0;
                    }}
                    .email-footer {{
                        padding: 10px 20px;
                        background-color: #f7f7f7;
                        border-top: 1px solid #dddddd;
                        text-align: center;
                        font-size: 0.9em;
                        color: #777777;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <div class='email-header'>
                        <h2>Lockbox Verification</h2>
                    </div>
                    <div class='email-content'>
                        <p>Dear User,</p>
                        <p>Thank you for using Lockbox! To complete your verification process, please use the verification code provided below:</p>
                        <p class='verification-code'>{verificationCode}</p>
                        <p>If you did not request this verification code, please ignore this email or contact our support team.</p>
                        <p>Best regards,<br>Lockbox Team</p>
                    </div>
                    <div class='email-footer'>
                        &copy; 2024 Lockbox. All rights reserved.
                    </div>
                </div>
            </body>
            </html>";
            message.IsBodyHtml = true;


            message.Subject = "LockBox Verification Code";
            return message;
        }
        private static SmtpClient PrepareSending()
        {
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential("verifylockbox@gmail.com", "qenl flea ugsp lsnt");
            return smtp;
        }
        public static string GenerateRandomDigits(int numberOfDigits)
        {
            string verificationCode = "";
            Random random = new Random();
            for (int i = 0; i < numberOfDigits; i++)
            {
                int randomNumber = random.Next(10);
                verificationCode += randomNumber.ToString();
            }
            return verificationCode;
        }
    }
}
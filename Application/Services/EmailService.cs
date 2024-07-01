using System.Net.Mail;
using System.Net;
using System.Text;
using System;

namespace IronGym.Application.Services
{
    public class EmailService
    {
        private static readonly Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public string SendVerificationEmail(string userEmail)
        {
            try
            {
                string verificationCode = GenerateRandomDigits(6);
                MailMessage mail = WriteVerificationEmail(userEmail, verificationCode);
                SmtpClient smtp = PrepareSending();
                smtp.Send(mail);
                return verificationCode;
            }
            catch
            {
                return "";
            }
        }
        public string SendPasswordEmail(string userEmail)
        {
            try
            {
                string passwordCode = GenerateRandomString(8);
                MailMessage mail = WritePasswordEmail(userEmail, passwordCode);
                SmtpClient smtp = PrepareSending();
                smtp.Send(mail);
                return passwordCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public static MailMessage WriteVerificationEmail(string userEmail, string verificationCode)
        {
            MailMessage message = new MailMessage();
            message.To.Add(userEmail);
            message.From = new MailAddress("verifyirongym@gmail.com");

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
                background-color: #000000;
                padding: 10px 20px;
                border-bottom: 1px solid #dddddd;
            }}
            .email-header h2 {{
                margin: 0;
                color: #f36100;
            }}
            .email-content {{
                padding: 20px;
                background-color: #000000;
                border: 1px solid #dddddd;
                color: #ffffff;
            }}
            .verification-code {{
                font-size: 1.5em;
                color: #f36100;
                font-weight: bold;
                margin: 20px 0;
            }}
            .email-footer {{
                padding: 10px 20px;
                background-color: #000000;
                border-top: 1px solid #dddddd;
                text-align: center;
                font-size: 0.9em;
                color: #ffffff;
            }}
        </style>
    </head>
    <body>
        <div class='email-container'>
            <div class='email-header'>
                <h2>IronGym Verification</h2>
            </div>
            <div class='email-content'>
                <p>Dear User,</p>
                <p>Thank you for using IronGym! To complete your verification process, please use the verification code provided below:</p>
                <p class='verification-code'>{verificationCode}</p>
                <p>If you did not request this verification code, please ignore this email or contact our support team.</p>
                <p>Best regards,<br>IronGym Team</p>
            </div>
            <div class='email-footer'>
                &copy; 2024 IronGym. All rights reserved.
            </div>
        </div>
    </body>
    </html>";
            message.IsBodyHtml = true;

            message.Subject = "IronGym Verification Code";
            return message;
        }


        public static MailMessage WritePasswordEmail(string userEmail, string password)
        {
            MailMessage message = new MailMessage();
            message.To.Add(userEmail);
            message.From = new MailAddress("verifyirongym@gmail.com");

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
                background-color: #000000;
                padding: 10px 20px;
                border-bottom: 1px solid #dddddd;
            }}
            .email-header h2 {{
                margin: 0;
                color: #f36100;
            }}
            .email-content {{
                padding: 20px;
                background-color: #000000;
                border: 1px solid #dddddd;
                color: #ffffff;
            }}
            .password {{
                font-size: 1.5em;
                color: #f36100;
                font-weight: bold;
                margin: 20px 0;
            }}
            .email-footer {{
                padding: 10px 20px;
                background-color: #000000;
                border-top: 1px solid #dddddd;
                text-align: center;
                font-size: 0.9em;
                color: #ffffff;
            }}
        </style>
    </head>
    <body>
        <div class='email-container'>
            <div class='email-header'>
                <h2>IronGym Employee Password</h2>
            </div>
            <div class='email-content'>
                <p>Dear Employee,</p>
                <p>Welcome to the IronGym team! We are excited to have you on board. Your login password is provided below:</p>
                <p class='password'>{password}</p>
                <p>Please use this password to log in along with the email address you provided.</p>
                <p>If you did not expect this email, please contact our support team immediately.</p>
                <p>Best regards,<br>IronGym Team</p>
            </div>
            <div class='email-footer'>
                &copy; 2024 IronGym. All rights reserved.
            </div>
        </div>
    </body>
    </html>";
            message.IsBodyHtml = true;

            message.Subject = "Your IronGym Employee Password";
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

        public string GenerateRandomString(int length)
        {
            if (length < 1)
                throw new ArgumentException("Length must be a positive number", nameof(length));

            var stringBuilder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }
            return stringBuilder.ToString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Antlr.StringTemplate;
using SendGrid;
using BCS.Framework.Utilities;

namespace BCS.Framework.EmailServices
{
    public static class EmailService
    {
        private const string MAIL_USER = "mail-user";
        private const string MAIL_SENDER = "mail-sender";
        private const string MAIL_HOST = "mail-host";
        private const string MAIL_PASSWORD = "mail-password";

        public static void Send(string sender, string to, string subject, string body)
        {
            Send(sender, new[] { to }, null, null, subject, body);
        }

        public static void Send(string sender, string[] to, string subject, string content)
        {
            Send(sender, to, null, null, subject, content);
        }

        public static void Send(string sender, string[] to, string[] cc,string subject, string content)
        {
            Send(sender, to, cc, null, subject, content);
        }

        public static void Send(string sender, string[] tolist, string[] ccList, string[] bccList, string subject, string body,bool iSJob = false)
        {
            try
            {
                var message = new SendGridMessage
                {
                    From = new MailAddress(Utils.GetSetting(MAIL_SENDER, ""), Utils.GetSetting(MAIL_USER, ""), Encoding.UTF8)
                };

                message.AddTo(tolist);
                message.AddCc(ccList);
                message.AddBcc(bccList);
                message.Subject = subject;
                message.Html = body;

                //var credentials = new NetworkCredential("ssp_system_admin", "Asdcvnmmvb");
                var credentials = new NetworkCredential(Utils.GetSetting(MAIL_USER, "ssp_system_admin"), Utils.GetSetting(MAIL_PASSWORD, "Asdcvnmmvb"));
                // Create a Web transport for sending email.
                var transportWeb = new SendGrid.Web(credentials);
                if (iSJob)
                {
                    transportWeb.DeliverAsync(message).Wait();
                }
                else
                {
                    transportWeb.DeliverAsync(message);
                }
                
            }
            catch (Exception ex)
            {
            }
        }
        
        private static async void SendAsync(SendGridMessage message)
        {
            // Create credentials, specifying your user name and password.
            //var credentials = new NetworkCredential("ssp_system_admin", "Asdcvnmmvb");
            var credentials = new NetworkCredential(Utils.GetSetting(MAIL_USER, "ssp_system_admin"), Utils.GetSetting(MAIL_PASSWORD, "Asdcvnmmvb"));

            // Create a Web transport for sending email.
            var transportWeb = new SendGrid.Web(credentials);

            // Send the email.
            try
            {
                await transportWeb.DeliverAsync(message);
            }
            catch (Exception ex)
            {
            }
        }
    }
}

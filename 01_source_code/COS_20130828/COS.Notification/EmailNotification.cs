using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Application.Shared;

namespace COS.Notification
{
    public class EmailNotification
    {
        public static void SendEmail(string subject, string emailMessage, SysNotification notification)
        {

            var users = COSContext.Current.NotificationUsers.Where(a => a.ID_notification == notification.ID);

            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            // message.To.Add("dashboard@lindab.cz");

            foreach (var itm in users.Where(a => a.ID_user != null))
            {
                if (!string.IsNullOrEmpty(itm.User.Email))
                    message.To.Add(itm.User.Email);
            }

            foreach (var itm in users.Where(a => a.ID_group != null))
            {
                var uss = COSContext.Current.NotifyGroupUsers.Where(a => a.ID_notifyGroup == itm.ID_group);
                foreach (var usr in uss)
                {
                    if (!string.IsNullOrEmpty(usr.User.Email))
                        message.To.Add(usr.User.Email);
                }
            }


            message.Subject = subject;
            message.From = new System.Net.Mail.MailAddress("cos@lindab.cz");
            message.Body = subject + ": ";
            message.Body += Environment.NewLine;
            message.Body += Environment.NewLine;
            message.Body += DateTime.Now.ToString();
            message.Body += Environment.NewLine;
            message.Body += Environment.NewLine;

            message.Body += Environment.NewLine;
            message.Body += Environment.NewLine;

            message.Body += emailMessage;
            message.Body += Environment.NewLine;

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("se861057");


            //System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential("helpdesk@calch.cz", "Calch12349");
            smtp.UseDefaultCredentials = true;
            //smtp.Credentials = SMTPUserInfo;


            message.BodyEncoding = Encoding.UTF8;

            smtp.Send(message);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace OnlineBanking.MyClass
{
    public class EmailUser
    {
        private string EmailForm { get; set; }
        private string EmailTo { get; set; }
        private string STMP { get; set; }
        private string PassSTMP { get; set; }
        private int Port { get; set; }
        public string Subject { get; set; }

        public string Messege { get; set; }

        public EmailUser( string to,  string sub, string mess)
        {
            this.EmailForm = "tpthanhphong111@gmail.com";
            this.EmailTo = to;
            this.STMP = "smtp.gmail.com";
            this.PassSTMP = "quvhlbhvtubzhirk";
            this.Port = 465;
            this.Subject = sub;
            this.Messege = mess;
        }



        public bool SendEmail(EmailUser email)
        {

            //Soạn người nhận thư
            MimeMessage message = new MimeMessage();

            MailboxAddress fromUser = new MailboxAddress("Online Banking",email.EmailTo);
            message.From.Add(fromUser);

            MailboxAddress toUser = new MailboxAddress("User", email.EmailTo);
            message.To.Add(toUser);

            message.Subject = email.Subject;

            //Soạn nội dung thư
            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = email.Messege;
            message.Body = bodyBuilder.ToMessageBody();


            //Kết nối cổng STMP
            SmtpClient client = new SmtpClient();
            client.Connect(this.STMP, this.Port, true);
            client.Authenticate(this.EmailForm, this.PassSTMP);

            //Gửi Email và đóng kết nối
            client.Send(message);
            client.Disconnect(true);
            client.Dispose();

            return true;
        }
    }

}

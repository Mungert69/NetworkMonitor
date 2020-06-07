
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using NetworkMonitor.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Services
{
    public class MailMessageService : IMessageService
    {
        private IWebHostEnvironment _env;
        public void init()
        {
            throw new NotImplementedException();
        }

        public void setWebEnv(IWebHostEnvironment env) {
            _env = env;
        }

        public ResultObj send(string messsageBody)
        {
            ResultObj result = new ResultObj();
            try {
                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress("Mahadeva",
                "mungert@gmail.com");
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress("Mahadeva",
                "mungert@gmail.com");
                message.To.Add(to);

                message.Subject = "Alert message";
                BodyBuilder bodyBuilder = new BodyBuilder();

                bodyBuilder.TextBody = messsageBody;
                //bodyBuilder.Attachments.Add(_env.WebRootPath + "\\file.png");
                message.Body = bodyBuilder.ToMessageBody();


                SmtpClient client = new SmtpClient();
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("mungert@gmail.com", "jrnoggrptqmtrkwf");

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
                result.Message = "Email sent ok";
                result.Success = true;
            }
            catch (Exception e) {
                result.Message = "Email failed to send . Error was :"+e.Message;
                result.Success = false;
            }
            return result;

        }
    }
}

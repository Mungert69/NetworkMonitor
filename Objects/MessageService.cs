using Microsoft.AspNetCore.Hosting;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Objects
{
    public class MessageService : IMessageService
    {
        public MessageService()
        {
        }

        public void setWebEnv(IWebHostEnvironment env)
        {
            
        }
        public void init() { }

        public ResultObj send(string message)
        {
            ResultObj result = new ResultObj();
       

            return result;
        }
    }
}

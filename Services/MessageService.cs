using Microsoft.AspNetCore.Hosting;
using NetworkMonitor.Objects;

namespace NetworkMonitor.Services
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

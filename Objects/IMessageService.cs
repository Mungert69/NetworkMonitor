using Microsoft.AspNetCore.Hosting;

namespace NetworkMonitor.Objects
{
    public interface IMessageService
    {
        void init();

        void setWebEnv(IWebHostEnvironment webEnv);
        ResultObj send(string message);
    }
}
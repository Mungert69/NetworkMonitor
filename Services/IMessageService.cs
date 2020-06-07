using Microsoft.AspNetCore.Hosting;
using NetworkMonitor.Objects;

namespace NetworkMonitor.Services
{
    public interface IMessageService
    {
        void init();

        void setWebEnv(IWebHostEnvironment webEnv);
        ResultObj send(string message);
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace MyDiscordBot
{
    public interface IBot
    {
        Task StartAsync(ServiceProvider services);

        Task StopAsync();
    }
}
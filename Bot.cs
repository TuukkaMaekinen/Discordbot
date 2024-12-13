using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyDiscordBot.Commands.Bugi;
using System.Threading.Tasks;

namespace MyDiscordBot
{
    public class Bot : IBot
    {
        private readonly IConfiguration _configuration;
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private ServiceProvider? _serviceProvider;
        private BugiReport _bugiReport;

        public Bot(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new DiscordSocketClient();
            _commands = new InteractionService(_client.Rest);
            _bugiReport = new BugiReport("bugs.txt");
            _client.Ready += async () => await OnReadyAsync();
        }

        public async Task StartAsync(ServiceProvider services)
        {
            string discordToken = _configuration["DiscordToken"] ?? throw new Exception("Missing Discord token");

            _serviceProvider = services;

            await _client.LoginAsync(TokenType.Bot, discordToken);
            await _client.StartAsync();

            Console.WriteLine("Bot on käynnissä!");
        }

        public async Task StopAsync()
        {
            if (_client != null)
            {
                await _client.LogoutAsync();
                await _client.StopAsync();
            }
        }

        private async Task OnReadyAsync()
        {
            Console.WriteLine($"Logged in as {_client.CurrentUser}");
            var guilds = _client.Guilds;
            if (guilds.Count > 0)
            {
                var firstGuild = guilds.First();
                await _bugiReport.RegisterAsync(firstGuild);
                Console.WriteLine("Komento rekisteröity guildille!");
            }
            else
            {
                Console.WriteLine("Ei guildia löytynyt.");
            }
        }
    }
}
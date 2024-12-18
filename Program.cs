using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MyDiscordBot
{
    internal class Program
    {
        private readonly DiscordSocketClient _client;
        private string _bugReportFilePath = "bugs.txt";

        public static async Task Main(string[] args)
        {
            var program = new Program();
            await program.RunAsync();
        }

        public Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
            });

            // Botin tapahtumien kuuntelu
            _client.Log += LogAsync;
            _client.Ready += OnReadyAsync;
            _client.SlashCommandExecuted += OnSlashCommandExecutedAsync;
        }

        public async Task RunAsync()
        {
            // Lataa User Secrets token
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            var token = config["DiscordToken"];

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Token puuttuu. Tarkista User Secrets.");
                return;
            }

            // Kirjaudu Discordiin
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            Console.WriteLine("Botti on käynnissä. Paina Ctrl+C lopettaaksesi.");
            await Task.Delay(-1); // Pidä sovellus käynnissä
        }

        private async Task OnReadyAsync()
        {
            Console.WriteLine("Botti on valmis!");

            // Hae ensimmäinen guild ja rekisteröi komento sinne
            var guild = _client.Guilds.FirstOrDefault();
            if (guild != null)
            {
                Console.WriteLine($"Rekisteröidään komento guildille: {guild.Name}");
                await RegisterBugCommandAsync(guild);
            }
            else
            {
                Console.WriteLine("Guildia ei löytynyt.");
            }
        }

        private async Task RegisterBugCommandAsync(SocketGuild guild)
        {
            var bugCommand = new SlashCommandBuilder()
                .WithName("bugi")
                .WithDescription("Raportoi bugi")
                .AddOption("ongelma", ApplicationCommandOptionType.String, "Kuvaile ongelma", isRequired: true);

            try
            {
                await guild.CreateApplicationCommandAsync(bugCommand.Build());
                Console.WriteLine("Komento /bugi rekisteröity!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Virhe rekisteröinnissä: {ex.Message}");
            }
        }

        private async Task OnSlashCommandExecutedAsync(SocketSlashCommand command)
        {
            if (command.Data.Name == "bugi")
            {
                var bugDescription = (string)command.Data.Options.First().Value;

                // Vahvista käyttäjälle
                await command.RespondAsync($"Kiitos bugiraportista! Ongelmakuvaus: {bugDescription}");

                // Tallenna raportti tiedostoon
                await SaveBugReportAsync(command.User.Username, bugDescription);
            }
        }

        private async Task SaveBugReportAsync(string username, string bugDescription)
        {
            try
            {
                string report = $"Käyttäjä: {username}\nBugi: {bugDescription}\nAika: {DateTime.Now}\n---\n";
                await File.AppendAllTextAsync(_bugReportFilePath, report);
                Console.WriteLine($"Bugi tallennettu: {bugDescription}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Virhe bugin tallentamisessa: {ex.Message}");
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }
    }
}
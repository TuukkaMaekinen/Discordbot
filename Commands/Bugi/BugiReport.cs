using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyDiscordBot.Commands.Bugi
{
    internal class BugiReport
    {
        private readonly string _bugReportFilePath;

        public BugiReport(string filePath)
        {
            _bugReportFilePath = filePath;
        }

        public async Task RegisterAsync(SocketGuild guild)
        {
            try
            {
                var bugCommand = new SlashCommandBuilder()
                    .WithName("bugi")
                    .WithDescription("Raportoi bugi")
                    .AddOption("ongelma", ApplicationCommandOptionType.String, "Kuvaile ongelma", isRequired: true);

                // Rekisteröi komento globaalisti tai yksittäiselle guildille
                await guild.CreateApplicationCommandAsync(bugCommand.Build());
                Console.WriteLine("Komento /bugi rekisteröity!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Virhe komennon rekisteröinnissä: {ex.Message}");
            }
        }

        public async Task HandleAsync(SocketSlashCommand command)
        {
            if (command.Data.Name == "bugi")
            {
                var bugDescription = (string)command.Data.Options.First().Value;

                // Vahvista käyttäjälle
                await command.RespondAsync($"Kiitos! Raportoitu ongelma: {bugDescription}");

                // Tallenna raportti
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
                Console.WriteLine($"Virhe raportin tallentamisessa: {ex.Message}");
            }
        }
    }
}


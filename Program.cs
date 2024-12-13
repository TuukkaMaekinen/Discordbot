using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MyDiscordBot
{
    internal class Program
    {
        private static void Main(string[] args) =>
            MainAsync(args).GetAwaiter().GetResult();

        private static async Task MainAsync(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddScoped<IBot, Bot>()
                .BuildServiceProvider();

            try
            {
                // Haetaan botin instanssi
                IBot bot = serviceProvider.GetRequiredService<IBot>();
                await bot.StartAsync(serviceProvider);

                Console.WriteLine("Bot on yhteydessä Discordiin");

                // Pysy pääsilmukassa, kunnes käyttäjä haluaa sulkea botin
                do
                {
                    var keyInfo = Console.ReadKey();

                    if (keyInfo.Key == ConsoleKey.Q)
                    {
                        Console.WriteLine("\nSuljetaan botti!");
                        await bot.StopAsync();
                        return;
                    }
                } while (true);
            }
            catch (Exception exception)
            {
                // Lokitetaan mahdolliset virheet
                Console.WriteLine($"Virhe: {exception.Message}");
                Environment.Exit(-1);
            }
        }
    }
}
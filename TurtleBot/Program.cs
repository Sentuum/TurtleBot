using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TurtleBot
{
    internal static class Program
    {
        private static IConfiguration Configuration { get; set; }

        private static readonly Dictionary<DiscordMessage, List<string>> ReactionQueue = new();

        private static readonly Dictionary<string, ulong> UserId = new()
        {
            {"Sentum", 116625769777725449},
            {"Ross", 238743848761688064},
            {"Turtle", 793964320571523092},
            {"Nem", 336916797750116352}
        };

        private static readonly List<ulong> SeriousChannels = new()
        {
            441080049756536844, //Webhosting - General
        };

        private static void Main (string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static IHostBuilder CreateHostBuilder (string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, config) =>
                {
                    config.AddJsonFile("appsettings.json");
                    config.AddEnvironmentVariables("TurtleBot_");
                    Configuration = config.Build();
                });

        private static async Task MainAsync (string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Configuration.GetSection("Discord").GetValue<string>("Token"),
                TokenType = TokenType.Bot
            });

            var random = new Random();

            Task MessageCreatedHandler (DiscordClient client, MessageCreateEventArgs e)
            {
                ReactionQueue[e.Message] = new List<string>();

                if (!e.HasAuthor("Sentum") && e.NotSerious())
                {
                    var lottery = random.Next(1, 100);
                    switch (lottery)
                    {
                        case 10:
                            ReactWithHey(e);
                            break;
                        case 20:
                            ReactWithPoop(e);
                            break;
                        case 30:
                            ReactWithWave(e);
                            break;
                    }
                }

                if (e.HasAuthor("Sentum") && e.NotSerious())
                {
                    var lottery = random.Next(1, 50);
                    switch (lottery)
                    {
                        case 1:
                            ReactWithHeart(e);
                            break;
                    }
                }

                if (e.Contains("rocket league", "rocketleague"))
                {
                    ReactToRocketLeague(e);
                }

                if (e.Contains("turtle", "🐢"))
                {
                    ReactWithTurtle(e);
                }

                if (e.MentionedUsers.Contains(client.CurrentUser) ||
                    (e.Contains("hey!") || e.StartsWith("hey")) && e.NotSerious())
                {
                    ReactWithHey(e);
                }

                if (e.Contains("covid", "corona", "pandemic", "vaccine", "virus"))
                {
                    ReactToCorona(e);
                }

                if (e.Contains("cloudey") && e.NotSerious())
                {
                    ReactWithBlueHeart(e);
                }

                if (e.Contains("rainbow"))
                {
                    ReactWithRainbow(e);
                }

                if (e.Contains(" eu ", "european union") && e.NotSerious())
                {
                    ReactToEurope(e);
                }

                if (e.Contains("united states", "america", " usa ", " usa!", "murica") && e.NotSerious())
                {
                    ReactToAmerica(e);
                }

                if (e.Contains("open source", "foss") && e.NotSerious())
                {
                    ReactWithMoney(e);
                }

                if (e.Contains("good night", "sleep"))
                {
                    ReactWithNight(e);
                }

                Task.Run(async () =>
                {
                    while (ReactionQueue[e.Message].Count > 0)
                    {
                        await Task.Run(async () =>
                        {
                            var reaction = ReactionQueue[e.Message].First();
                            await e.Message.CreateReactionAsync(DiscordEmoji.FromName(client, reaction));
                            ReactionQueue[e.Message].Remove(reaction);
                        });
                        await Task.Delay(1000);
                    }

                    ReactionQueue.Remove(e.Message);
                });

                return Task.CompletedTask;
            }

            discord.MessageCreated += MessageCreatedHandler;

            await discord.ConnectAsync();

            await host.RunAsync();
        }

        private static void ReactWithHey (MessageCreateEventArgs e)
        {
            e.React(
                ":regional_indicator_h:",
                ":regional_indicator_e:",
                ":regional_indicator_y:",
                ":wave:"
            );
        }

        private static void ReactWithWave (MessageCreateEventArgs e)
        {
            e.React(":wave:");
        }

        private static void ReactWithPoop (MessageCreateEventArgs e)
        {
            e.React(":poop:");
        }

        private static void ReactWithHeart (MessageCreateEventArgs e)
        {
            e.React(":heart:");
        }

        private static void ReactWithBlueHeart (MessageCreateEventArgs e)
        {
            e.React(":blue_heart:");
        }

        private static void ReactWithRainbow (MessageCreateEventArgs e)
        {
            e.React(":rainbow:");
        }

        private static void ReactWithTurtle (MessageCreateEventArgs e)
        {
            e.React(":turtle:", ":green_heart:");
        }

        private static void ReactToRocketLeague (MessageCreateEventArgs e)
        {
            e.React(
                ":interrobang:",
                ":no_entry_sign:",
                ":rage:"
            );
        }

        private static void ReactToCorona (MessageCreateEventArgs e)
        {
            var random = new Random();
            var lottery = random.Next(1, 4);

            e.React(":rotating_light:");

            switch (lottery)
            {
                case 1:
                    e.React(":microbe:");
                    break;
                case 2:
                    e.React(":syringe:");
                    break;
                case 3:
                    e.React(":hospital:");
                    break;
                case 4:
                    e.React(":ambulance:");
                    break;
            }
        }

        private static void ReactWithMoney (MessageCreateEventArgs e)
        {
            e.React(
                ":money_mouth:",
                ":money_with_wings:"
            );
        }

        private static void ReactToEurope (MessageCreateEventArgs e)
        {
            e.React(
                ":flag_eu:",
                ":blue_heart:"
            );
        }

        private static void ReactToAmerica (MessageCreateEventArgs e)
        {
            e.React(
                ":gun:",
                ":eagle:",
                ":scream:"
            );
        }

        private static void ReactWithNight (MessageCreateEventArgs e)
        {
            e.React(
                ":kissing_heart:",
                ":zzz:",
                ":crescent_moon:"
            );
        }

        private static bool Contains (this MessageCreateEventArgs e, params string[] keywords)
        {
            var content = e.Message.Content.ToLower();
            return keywords.Any(keyword => content.Contains(keyword.ToLower()));
        }

        private static bool StartsWith (this MessageCreateEventArgs e, params string[] keywords)
        {
            var content = e.Message.Content.ToLower();
            return keywords.Any(keyword => content.StartsWith(keyword.ToLower()));
        }

        private static bool HasAuthor (this MessageCreateEventArgs e, params string[] authors)
        {
            return authors.Any(author => e.Author.Username == author || e.Author.Id == UserId[author]);
        }

        private static bool NotSerious (this MessageCreateEventArgs e)
        {
            return !SeriousChannels.Contains(e.Channel.Id);
        }

        private static void React (this MessageCreateEventArgs e, params string[] reactions)
        {
            ReactionQueue[e.Message].AddRange(reactions);
        }
    }
}
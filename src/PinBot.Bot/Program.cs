using System;
using System.Threading.Tasks;
using DSharpPlus;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PinBot.Core;
using PinBot.Core.Services;
using PinBot.Data;
using Serilog;
using Serilog.Events;

namespace PinBot.Application
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console()
                .Enrich.FromLogContext();
            Log.Logger = loggerConfig.CreateLogger();
            await host.RunAsync();

            // we shouldn't get here
            throw new Exception("Wtf happened?");
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    // Add other configuration files...
                    builder.AddEnvironmentVariables("PINBOT_");
                    builder.AddJsonFile("pinBotConfig.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                    ConfigureServices(context.Configuration, services)
                );

        private static void ConfigureServices(IConfiguration configuration,
            IServiceCollection services)
        {
            services.Configure<PinBotConfig>(configuration);
            services.AddSingleton(x => x.GetRequiredService<IOptions<PinBotConfig>>().Value);
            services.AddHostedService<PinBotBackgroundService>();
            services.AddMediatR(config =>
                    config.AsScoped(),
                typeof(Program),
                typeof(DiscordMonitor) // TODO: figure out why/how/if it matters, but notifications all have their own scope
            );
            // services.AddScoped<ReactionMonitor>();
            services.AddSingleton(p =>
                new DiscordClient(
                    new DiscordConfiguration
                    {
                        LoggerFactory = new LoggerFactory().AddSerilog(),
                        Token = p.GetRequiredService<PinBotConfig>().Token,
                        TokenType = TokenType.Bot
                    }
                )
            );
            services.AddDbContextPool<PinBotContext>((p, options) =>
            {
                var config = p.GetService<PinBotConfig>() ?? throw new Exception("AddDbContextPool: Config is null");

                options.UseMySql(
                        config.ConnectionString,
                        new MySqlServerVersion(new Version(config.MySqlMajor, config.MySqlMinor, config.MySqlBuild))
                    )
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            });
            services.AddScoped<AuthorizationService>();
            services.AddScoped<PinService>();
            services.AddScoped<PinBoardService>(); 
        }
    }
}
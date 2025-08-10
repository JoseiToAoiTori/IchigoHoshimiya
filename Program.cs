using IchigoHoshimiya.Interfaces;
using IchigoHoshimiya.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IPingService, PingService>();

builder.Services
       .AddDiscordGateway(options =>
        {
            options.Intents = GatewayIntents.GuildMessages |
                              GatewayIntents.DirectMessages |
                              GatewayIntents.MessageContent |
                              GatewayIntents.DirectMessageReactions |
                              GatewayIntents.GuildMessageReactions;
        })
       .AddCommands()
       .AddApplicationCommands()
       .AddGatewayHandlers(typeof(Program).Assembly);

IHost host = builder.Build()
                    .UseGatewayHandlers();

// NetCord: Add commands from modules
host.AddModules(typeof(Program).Assembly);

await host.RunAsync();
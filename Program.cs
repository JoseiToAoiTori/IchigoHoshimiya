using IchigoHoshimiya.Adapters;
using IchigoHoshimiya.BackgroundServices;
using IchigoHoshimiya.Context;
using IchigoHoshimiya.Interfaces;
using IchigoHoshimiya.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<AnimeThemesUpdaterSettings>(
    builder.Configuration.GetSection("AnimeThemesUpdater"));

builder.Services.AddHttpClient<AnimeThemesDbUpdateService>();

builder.Services.AddHostedService<AnimeThemesDbUpdateService>();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// AnimeThemeEntry is the key
builder.Services.AddDbContext<AnimethemesDbContext>(options =>
    options.UseMySQL(connectionString!));

builder.Services.AddTransient<IClient, RestClientAdapter>();
builder.Services.AddSingleton<IPingService, PingService>();
builder.Services.AddSingleton<ITwitterReplacementService, TwitterReplacementService>();
builder.Services.AddScoped<IAnimethemeService, AnimethemeService>();

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
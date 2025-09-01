using IchigoHoshimiya.Adapters;
using IchigoHoshimiya.BackgroundServices;
using IchigoHoshimiya.Context;
using IchigoHoshimiya.Interfaces;
using IchigoHoshimiya.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;
using NetCord.Services;
using NetCord.Services.Commands;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<AnimeThemesUpdaterSettings>(
    builder.Configuration.GetSection("AnimeThemesUpdater"));

builder.Services.AddHttpClient<AnimeThemesDbUpdateService>();
builder.Services.AddHttpClient<SeasonalCalendarDbUpdateService>();
builder.Services.AddHostedService<AnimeThemesDbUpdateService>();
builder.Services.AddHostedService<SeasonalCalendarDbUpdateService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<AnimethemesDbContext>(options =>
    options.UseMySQL(connectionString!));

builder.Services.AddDbContext<IchigoContext>(options =>
    options.UseMySQL(connectionString!));


builder.Services.AddTransient<IClient, RestClientAdapter>();
builder.Services.AddSingleton<IPingService, PingService>();
builder.Services.AddSingleton<ITwitterReplacementService, TwitterReplacementService>();
builder.Services.AddScoped<IAnimethemeService, AnimethemeService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();

builder.Services
       .AddDiscordGateway(options =>
        {
            options.Intents = GatewayIntents.GuildMessages |
                              GatewayIntents.DirectMessages |
                              GatewayIntents.MessageContent |
                              GatewayIntents.DirectMessageReactions |
                              GatewayIntents.GuildMessageReactions;
        })
       .AddCommands<CommandContext>(options =>
        {
            // Suppress "Command not found"
            options.ResultHandler = new InlineResultHandler();
        })
       .AddApplicationCommands()
       .AddGatewayHandlers(typeof(Program).Assembly);

var host = builder.Build();

// NetCord: Add commands from modules
host.AddModules(typeof(Program).Assembly);

await host.RunAsync();

file sealed class InlineResultHandler : ICommandResultHandler<CommandContext>
{
    private readonly CommandResultHandler<CommandContext> _default = new();

    public ValueTask HandleResultAsync(
        IExecutionResult result,
        CommandContext context,
        GatewayClient client,
        ILogger logger,
        IServiceProvider services)
    {
        return result is NotFoundResult
            ? ValueTask.CompletedTask
            : _default.HandleResultAsync(result, context, client, logger, services);
    }
}
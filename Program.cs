using IchigoHoshimiya.Interfaces;
using IchigoHoshimiya.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IPingService, PingService>();

builder.Services
       .AddDiscordGateway()
       .AddApplicationCommands();

IHost host = builder.Build();

// NetCord: Add commands from modules
host.AddModules(typeof(Program).Assembly);


// NetCord: Add handlers to handle the commands
host.UseGatewayHandlers();

await host.RunAsync();
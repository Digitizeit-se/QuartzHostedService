using Digitizeit.Quartz.HostedService.Extensions;
using HostedServiceImplementation.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace HostedServiceImplementation
{
    internal class Program
    {
        private static int Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Building host.");
                var host = BuildHost()
                    .Build();

                Log.Information("Starting host");
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder BuildHost()
        {
            var hostBuilder = new HostBuilder();

            hostBuilder.UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Debug")
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                })
                .ConfigureAppConfiguration((_, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", true);
                })
                .ConfigureAppConfiguration((_, configApp) =>
                {
                    configApp.AddJsonFile($"appsettings.{ProviderSwitch.SqLiteNoXml}.json",
                        true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddQuartzHostedService(hostContext.Configuration);
                    services.AddSingleton<FirstJob, FirstJob>();
                    services.AddSingleton<SecondJob, SecondJob>();
                })
                .UseConsoleLifetime()
                .UseSerilog();
            return hostBuilder;
        }
    }

    /// <summary>
    /// This is used to switch what application.**.Json file to use.
    /// You need to run the right docker container for this to work if choice is a database provider
    /// change this row   configApp.AddJsonFile($"appsettings.{ProviderSwitch.Memory.ToString()}.json", above to desired database provider
    /// </summary>
    public enum ProviderSwitch
    {
        Memory,
        SqLite,
        SqlServer,
        MySql,
        Postgres,
        SqLiteNoXml
    }
}
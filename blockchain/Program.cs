namespace Blockchain
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Configuration;
    using Infrastructure.Options;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Modules;
    using Serilog;

    public class Program
    {
        private static readonly CancellationTokenSource _cancellationTokenSource = new();

        public static async Task Main(string[]? args)
        {
            var ct = _cancellationTokenSource.Token;

            Console.CancelKeyPress += (_, eventArgs) =>
            {
                _cancellationTokenSource.Cancel();
                eventArgs.Cancel = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
                 Log.Fatal(
                     (Exception)eventArgs.ExceptionObject,
                     "Encountered a fatal exception, exiting program.");

            var configuration = GetConfiguration(args);

            var container = ConfigureServices(configuration);
            var logger = container.GetRequiredService<ILogger<Program>>();

            logger.LogInformation($"Starting {Assembly.GetEntryAssembly()?.GetName().Name}");

            try
            {
                #if DEBUG
                Console.WriteLine($@"Press ENTER to start {Assembly.GetEntryAssembly()?.GetName().Name}...");
                Console.ReadLine();
                #endif

                var runner = container.GetRequiredService<Runner>();

                var runnerTask = runner.StartAsync(ct);

                #if DEBUG
                Console.WriteLine("Running... Press CTRL + C to exit.");
                #endif

                await runnerTask.WaitAsync(ct);
            }
            catch (Exception e)
            {
                if (e is not TaskCanceledException)
                    logger.LogCritical(e, "Encountered a fatal exception, exiting program.");

                Log.CloseAndFlush();

                // Allow some time for flushing before shutdown.
                Thread.Sleep(1000);
            }

            logger.LogInformation("Stopping...");

            ct.WaitHandle.Close();
        }

        private static IServiceProvider ConfigureServices(
            IConfigurationRoot configuration)
        {
            var services = new ServiceCollection();

            var builder = new ContainerBuilder();

            services
                .ConfigureAndValidate<BlockchainConfiguration>(configuration.GetSection(BlockchainConfiguration.Section));

            builder
                .RegisterModule(new LoggingModule(configuration, services));

            builder
                .RegisterInstance(configuration);

            builder
                .RegisterType<Runner>()
                .SingleInstance();

            builder
                .Populate(services);

            return new AutofacServiceProvider(builder.Build());
        }

        private static IConfigurationRoot GetConfiguration(string[]? args)
            => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .AddCommandLine(args ?? Array.Empty<string>())
                .Build();
    }
}

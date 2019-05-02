using System;
using System.Net;
using System.Text;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IPChecker
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static IServiceCollection Services { get; set; }
        public static ILogger Logger { get; set; }
        private static string Email { get; set; }

        public class Options
        {
            [Option('e', "email", Required = true, HelpText =
                "(Required) - Set email address to send report to: (e.g abc@hotmail.com) [No Default]")]
            public string OEmail { get; set; }
        }

        static void Main(string[] args)
        {
            // Example call: dotnet IPChecker.dll -e wadem@pm.me
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(o =>
                   {
                       Email = o.OEmail;
                   });
            SetUpApp();
            GetAndSendIP();
            Logger.LogInformation($"Finishing application at: {DateTime.Now}");
        }

        private static void SetUpApp()
        {
            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) || devEnvironmentVariable.ToLower() == "development";

            var builder = new ConfigurationBuilder();

            if (isDevelopment)
            {
                builder.AddUserSecrets<Settings>();
            }

            Configuration = builder.Build();

            // Services
            var services = new ServiceCollection()
                .AddLogging(b => b
                    .AddConsole()
                    .AddFilter(level => level >= LogLevel.Information))
                .AddSingleton(Configuration)
                .BuildServiceProvider();

            Logger = services.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            Logger.LogInformation($"Starting application at: {DateTime.Now}");
        }

        private static void GetAndSendIP()
        {
            StringBuilder result = new StringBuilder();
            string externalip = new WebClient().DownloadString("https://icanhazip.com/");

            result.Append($"Your IP today is: {externalip}");

            Mailer mailer = new Mailer(Configuration, Email, "IPChecker", result.ToString());
            mailer.Send();
        }
    }
}

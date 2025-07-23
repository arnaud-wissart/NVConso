using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NVConso
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Cr�ation d'un conteneur DI minimal
            var services = new ServiceCollection()
                .AddLogging(config => config.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "[HH:mm:ss] ";
                }))
                .AddSingleton<INvmlManager, NvmlManager>()
                .BuildServiceProvider();

            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("NVConso");
            logger.LogInformation("Application d�marr�e");

            var nvml = services.GetRequiredService<INvmlManager>();
            Application.Run(new TrayAppContext(nvml));
        }
    }
}
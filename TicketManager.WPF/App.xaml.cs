using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Windows;
using TicketManager.WPF.Data;
using TicketManager.WPF.ViewModels;

namespace TicketManager.WPF
{
    public partial class App : Application
    {
        private IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .UseSerilog((context, loggerConfig) => 
                    loggerConfig
                        .MinimumLevel.Debug()
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Application", "TicketManager")
                        .WriteTo.Console()
                        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
                )
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<TicketContext>();
                    services.AddScoped<ITicketRepository, TicketRepository>();
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<MainWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }

            base.OnExit(e);
        }
    }
}

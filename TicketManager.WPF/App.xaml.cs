using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Linq;
using System.Windows;
using TicketManager.WPF.Data;
using TicketManager.WPF.Models;
using TicketManager.WPF.ViewModels;

namespace TicketManager.WPF
{
    public partial class App : Application
    {
        private IHost _host;

        public App()
        {
            Console.WriteLine("App constructor started");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("pt-BR");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                        System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)));

            _host = Host.CreateDefaultBuilder()
                .UseSerilog((context, loggerConfig) =>
                    loggerConfig
                        .MinimumLevel.Debug()
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Application", "TicketManager")
                        .WriteTo.Console()
                )
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<TicketContext>();
                    services.AddScoped<ITicketRepository, TicketRepository>();
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<MainWindow>();
                })
                .Build();
            Console.WriteLine("App constructor finished");
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            Console.WriteLine("OnStartup started");
            await _host.StartAsync();

            // The logic has been moved to InitialScreen.xaml.cs to ensure a window handle exists for message boxes.
            var initialScreen = new InitialScreen();
            initialScreen.Show();

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
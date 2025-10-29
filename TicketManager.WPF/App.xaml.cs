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
                        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
                )
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<TicketContext>();
                    services.AddSingleton<ITicketRepository, TicketRepository>();
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<MainWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            using (var scope = _host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<TicketContext>();
                    
                    if (!context.AuthorizedUsers.Any())
                    {
                        var firstUser = new AuthorizedUser { WindowsUserName = Environment.UserName, IsAdminBool = true, IsActive = 1 };
                        context.AuthorizedUsers.Add(firstUser);
                        context.SaveChanges();
                    }

                    var currentWindowsUser = Environment.UserName;
                    var authorizedUser = context.AuthorizedUsers.FirstOrDefault(u => u.WindowsUserName.ToUpper() == currentWindowsUser.ToUpper());

                    if (authorizedUser != null)
                    {
                        if (authorizedUser.IsActive == 1)
                        {
                            var mainWindow = new MainWindow(authorizedUser);
                            mainWindow.Show();
                        }
                        else
                        {
                            MessageBox.Show("Acesso Negado. Seu usuário está inativo. Contate um administrador.", "Usuário Inativo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            Shutdown();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Acesso Negado. Você não tem permissão para usar esta aplicação.", "Erro de Autorização", MessageBoxButton.OK, MessageBoxImage.Error);
                        Shutdown();
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = ex.InnerException?.Message ?? ex.Message;
                    MessageBox.Show("Ocorreu um erro crítico na inicialização ao tentar conectar ao banco de dados.\n\nDetalhes: " + errorMessage,
                                    "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                }
            }

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
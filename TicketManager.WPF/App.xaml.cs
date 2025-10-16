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
                    // Se a tabela de usuários estiver vazia, adiciona o usuário atual como admin.
                    // Isso só acontece na primeira execução para popular o banco de dados.
                    if (context.AuthorizedUsers.Count() == 0)
                    {
                        var firstUser = new Models.AuthorizedUser { WindowsUserName = Environment.UserName, IsAdminBool = true };
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
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show("Ocorreu um erro crítico na inicialização ao tentar conectar ao banco de dados.\n\nDetalhes: " + errorMessage, 
                                "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
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

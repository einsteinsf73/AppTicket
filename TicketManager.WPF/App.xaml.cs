using System.Windows;
using TicketManager.WPF.Data;
using System.Linq;
using System.Windows.Threading;

namespace TicketManager.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += Application_DispatcherUnhandledException;

            try
            {
                using (var context = new TicketContext())
                {
                    // Se a tabela de usuários estiver vazia, adiciona o usuário atual como admin.
                    // Isso só acontece na primeira execução para popular o banco de dados.
                    if (context.AuthorizedUsers.Count() == 0)
                    {
                        var firstUser = new Models.AuthorizedUser { WindowsUserName = Environment.UserName, IsAdmin = 1 };
                        context.AuthorizedUsers.Add(firstUser);
                        context.SaveChanges();
                    }

                    var currentWindowsUser = Environment.UserName;
                    var authorizedUser = context.AuthorizedUsers.FirstOrDefault(u => u.WindowsUserName.ToUpper() == currentWindowsUser.ToUpper());

                    if (authorizedUser != null)
                    {
                        var mainWindow = new MainWindow(authorizedUser);
                        mainWindow.Show();
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

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var errorMessage = e.Exception.InnerException?.Message ?? e.Exception.Message;
            MessageBox.Show("Ocorreu um erro crítico e a aplicação será encerrada.\n\nDetalhes: " + errorMessage, 
                            "Erro Inesperado", MessageBoxButton.OK, MessageBoxImage.Error);
            
            e.Handled = true;
            Shutdown();
        }
    }
}


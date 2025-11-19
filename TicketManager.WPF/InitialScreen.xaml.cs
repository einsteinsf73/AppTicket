using System;
using System.Linq;
using System.Windows;
using TicketManager.WPF.Data;
using TicketManager.WPF.Models;
using TicketManager.WPF.Services; // Import the new service

namespace TicketManager.WPF
{
    public partial class InitialScreen : MahApps.Metro.Controls.MetroWindow
    {
        private AuthorizedUser? _user;
        private SettingsWindow? _settingsWindow;

        public InitialScreen()
        {
            InitializeComponent();
        }

        private void InitialScreen_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new TicketContext())
                {
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
                            _user = authorizedUser;

                            // Load theme settings using the service
                            var userSettings = ThemeManagerService.LoadUserColumnSettings();
                            ThemeManagerService.ChangeTheme(userSettings.Theme);

                            // Check for single-module access and navigate away if necessary
                            bool hasOnlyTicketAccess = _user.HasTicketAccessBool && !_user.HasAssetAccessBool;
                            bool hasOnlyAssetAccess = !_user.HasTicketAccessBool && _user.HasAssetAccessBool;

                            if (hasOnlyTicketAccess)
                            {
                                var mainWindow = new MainWindow(_user);
                                mainWindow.Show();
                                Application.Current.Dispatcher.InvokeAsync(() => this.Close());
                                return;
                            }
                            else if (hasOnlyAssetAccess)
                            {
                                var assetControlWindow = new AssetControlWindow(_user);
                                assetControlWindow.Show();
                                Application.Current.Dispatcher.InvokeAsync(() => this.Close());
                                return;
                            }

                            // This logic runs if the screen is shown (user has access to multiple modules)
                            TicketButton.IsEnabled = _user.HasTicketAccessBool;
                            AssetControlButton.IsEnabled = _user.HasAssetAccessBool;

                            if (_user.IsAdminBool)
                            {
                                SettingsButton.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Acesso Negado. Seu usuário está inativo. Contate um administrador.", "Usuário Inativo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Acesso Negado. Você não tem permissão para usar esta aplicação.", "Erro de Autorização", MessageBoxButton.OK, MessageBoxImage.Error);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show("Ocorreu um erro crítico na inicialização ao tentar conectar ao banco de dados.\n\nDetalhes: " + errorMessage,
                                "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void TicketButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user == null) return;
            var mainWindow = new MainWindow(_user);
            mainWindow.Show();
            this.Close();
        }

        private void AssetControlButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user == null) return;
            var assetControlWindow = new AssetControlWindow(_user);
            assetControlWindow.Show();
            this.Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = new SettingsWindow(new TicketContext());
                _settingsWindow.Owner = this;
                _settingsWindow.Closed += (s, args) => _settingsWindow = null;
                _settingsWindow.Show();
            }
            else
            {
                _settingsWindow.Activate();
            }
        }

        private void LightThemeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ThemeManagerService.ChangeTheme("Light.Blue");
        }

        private void DarkThemeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ThemeManagerService.ChangeTheme("Dark.Blue");
        }
    }
}
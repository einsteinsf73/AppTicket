using ControlzEx.Theming;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using TicketManager.WPF.Models;

namespace TicketManager.WPF
{
    public partial class InitialScreen : MahApps.Metro.Controls.MetroWindow
    {
        private readonly AuthorizedUser _user;
        private UserColumnSettings _userColumnSettings;
        private string _columnSettingsFilePath;
        private SettingsWindow? _settingsWindow;

        public InitialScreen(AuthorizedUser user)
        {
            InitializeComponent();
            _user = user;

            // Check for single-module access
            bool hasOnlyTicketAccess = _user.HasTicketAccessBool && !_user.HasAssetAccessBool;
            bool hasOnlyAssetAccess = !_user.HasTicketAccessBool && _user.HasAssetAccessBool;

            if (hasOnlyTicketAccess)
            {
                var mainWindow = new MainWindow(_user);
                mainWindow.Show();
                this.Close();
                return; // Stop further execution in this window
            }

            if (hasOnlyAssetAccess)
            {
                var assetControlWindow = new AssetControlWindow(_user);
                assetControlWindow.Show();
                this.Close();
                return; // Stop further execution in this window
            }

            // Original logic for when the screen is shown
            LoadUserColumnSettings();
            ChangeTheme(_userColumnSettings.Theme);

            TicketButton.IsEnabled = _user.HasTicketAccessBool;
            AssetControlButton.IsEnabled = _user.HasAssetAccessBool;

            if (_user.IsAdminBool)
            {
                SettingsButton.Visibility = Visibility.Visible;
            }
        }

        private void TicketButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow(_user);
            mainWindow.Show();
            this.Close();
        }

        private void AssetControlButton_Click(object sender, RoutedEventArgs e)
        {
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
            ChangeTheme("Light.Blue");
        }

        private void DarkThemeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ChangeTheme("Dark.Blue");
        }

        private void LoadUserColumnSettings()
        {
            _columnSettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TicketManager", $"{Environment.UserName}_column_settings.json");

            var defaultUserColumnSettings = UserColumnSettings.GetDefaultSettings();
            UserColumnSettings? loadedUserColumnSettings = null;

            if (File.Exists(_columnSettingsFilePath))
            {
                try
                {
                    var jsonString = File.ReadAllText(_columnSettingsFilePath);
                    loadedUserColumnSettings = JsonSerializer.Deserialize<UserColumnSettings>(jsonString);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar configurações de coluna: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            _userColumnSettings = defaultUserColumnSettings;

            if (loadedUserColumnSettings != null)
            {
                _userColumnSettings.Theme = loadedUserColumnSettings.Theme;
            }
        }

        private void SaveUserColumnSettings()
        {
            try
            {
                _userColumnSettings.UserName = Environment.UserName;
                _userColumnSettings.Theme = ThemeManager.Current.DetectTheme(Application.Current)?.Name ?? "Light.Blue";

                var jsonString = JsonSerializer.Serialize(_userColumnSettings, new JsonSerializerOptions { WriteIndented = true });
                Directory.CreateDirectory(Path.GetDirectoryName(_columnSettingsFilePath)!);
                File.WriteAllText(_columnSettingsFilePath, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar configurações de coluna: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangeTheme(string theme)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, theme);

            var colorDictionary = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && (d.Source.OriginalString.EndsWith("LightColors.xaml") || d.Source.OriginalString.EndsWith("DarkColors.xaml")));

            var index = -1;
            if (colorDictionary != null)
            {
                index = Application.Current.Resources.MergedDictionaries.IndexOf(colorDictionary);
                Application.Current.Resources.MergedDictionaries.Remove(colorDictionary);
            }

            var newColorDictionary = new ResourceDictionary();
            if (theme.StartsWith("Dark"))
            {
                newColorDictionary.Source = new Uri("DarkColors.xaml", UriKind.Relative);
            }
            else
            {
                newColorDictionary.Source = new Uri("LightColors.xaml", UriKind.Relative);
            }

            if (index == -1)
            {
                Application.Current.Resources.MergedDictionaries.Add(newColorDictionary);
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Insert(index, newColorDictionary);
            }

            SaveUserColumnSettings();
        }
    }
}
using ControlzEx.Theming;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using TicketManager.WPF.Models;

namespace TicketManager.WPF.Services
{
    public static class ThemeManagerService
    {
        private static string GetSettingsFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TicketManager", $"{Environment.UserName}_column_settings.json");
        }

        public static UserColumnSettings LoadUserColumnSettings()
        {
            var settingsFilePath = GetSettingsFilePath();
            var defaultSettings = UserColumnSettings.GetDefaultSettings();

            if (File.Exists(settingsFilePath))
            {
                try
                {
                    var jsonString = File.ReadAllText(settingsFilePath);
                    var loadedSettings = JsonSerializer.Deserialize<UserColumnSettings>(jsonString);
                    if (loadedSettings != null)
                    {
                        // Combine loaded settings with defaults to ensure all properties are set
                        defaultSettings.Theme = loadedSettings.Theme;
                        // Future settings can be merged here
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar configurações: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return defaultSettings;
        }

        public static void SaveUserColumnSettings(UserColumnSettings settings)
        {
            try
            {
                var settingsFilePath = GetSettingsFilePath();
                settings.UserName = Environment.UserName;
                settings.Theme = ThemeManager.Current.DetectTheme(Application.Current)?.Name ?? "Light.Blue";

                var jsonString = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath)!);
                File.WriteAllText(settingsFilePath, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar configurações: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void ChangeTheme(string theme)
        {
            if (Application.Current == null) return;

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

            // Save the new theme setting
            var settings = LoadUserColumnSettings();
            settings.Theme = theme;
            SaveUserColumnSettings(settings);
        }
    }
}

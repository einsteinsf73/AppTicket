using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using Microsoft.Win32;
using Microsoft.EntityFrameworkCore;
using TicketManager.WPF.Data;
using TicketManager.WPF.Models;
using System.Windows.Input;
using System.Net;
using ControlzEx.Theming;
using System.Windows.Controls;
using System.IO;
using System.Text.Json;

namespace TicketManager.WPF;

public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
{
    private readonly TicketContext _context = new TicketContext();
    private readonly bool _isAdmin;
    private UserColumnSettings _userColumnSettings;
    private string _columnSettingsFilePath;

    private TicketWindow? _ticketWindow;
    private ViewTicketWindow? _viewTicketWindow;
    private ReopenReasonWindow? _reopenReasonWindow;
    private SettingsWindow? _settingsWindow;

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

            if (loadedUserColumnSettings.ColumnSettings != null)
            {
                foreach (var loadedSetting in loadedUserColumnSettings.ColumnSettings)
                {
                    var existingSetting = _userColumnSettings.ColumnSettings.FirstOrDefault(s => s.Name == loadedSetting.Name);
                    if (existingSetting != null)
                    {
                        existingSetting.IsVisible = loadedSetting.IsVisible;
                        existingSetting.DisplayIndex = loadedSetting.DisplayIndex;
                    }
                    else
                    {
                        _userColumnSettings.ColumnSettings.Add(loadedSetting);
                    }
                }

                _userColumnSettings.ColumnSettings.RemoveAll(s => !defaultUserColumnSettings.ColumnSettings.Any(ds => ds.Name == s.Name));
            }
        }

        var orderedSettings = _userColumnSettings.ColumnSettings.OrderBy(s => s.DisplayIndex).ToList();
        for (int i = 0; i < orderedSettings.Count; i++)
        {
            orderedSettings[i].DisplayIndex = i;
        }
        _userColumnSettings.ColumnSettings = orderedSettings;
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

    public MainWindow(AuthorizedUser user)
    {
        InitializeComponent();
        _isAdmin = user.IsAdminBool;

        LoadUserColumnSettings();
        ChangeTheme(_userColumnSettings.Theme);
        ApplyColumnSettings();

        CurrentUserTextBlock.Text = $"Usuário: {user.WindowsUserName}";
        InitializeFilters();
        LoadTickets();
        SetButtonVisibility();
    }

    private void ApplyColumnSettings()
    {
        TicketsGrid.Columns.Clear();

        foreach (var setting in _userColumnSettings.ColumnSettings.OrderBy(s => s.DisplayIndex))
        {
            if (setting.IsVisible)
            {
                var column = new DataGridTextColumn
                {
                    Header = setting.Header,
                    Binding = new System.Windows.Data.Binding(setting.Name),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Auto)
                };
                TicketsGrid.Columns.Add(column);
            }
        }
    }

    private void InitializeFilters()
    {
        var statusOptions = new List<string> { "Todos", "Todos Abertos", "Aberto", "EmAndamento", "Resolvido", "Fechado" };
        StatusFilterComboBox.ItemsSource = statusOptions;
        StatusFilterComboBox.SelectedIndex = 1;

        var priorityOptions = new List<string> { "Todas" };
        priorityOptions.AddRange(Enum.GetNames(typeof(TicketPriority)));
        PriorityFilterComboBox.ItemsSource = priorityOptions;
        PriorityFilterComboBox.SelectedIndex = 0;
    }

    private void LoadTickets()
    {
        try
        {
            var query = _context.Tickets.AsQueryable();

            if (StatusFilterComboBox.SelectedItem is string statusFilter && statusFilter != "Todos")
            {
                if (statusFilter == "Todos Abertos")
                {
                    var openStatuses = new List<TicketStatus> { TicketStatus.Aberto, TicketStatus.EmAndamento };
                    query = query.Where(t => openStatuses.Contains(t.Status));
                }
                else if (Enum.TryParse<TicketStatus>(statusFilter, out var status))
                {
                    query = query.Where(t => t.Status == status);
                }
            }

            if (PriorityFilterComboBox.SelectedItem is string priorityFilter && priorityFilter != "Todas")
            {
                if (Enum.TryParse<TicketPriority>(priorityFilter, out var priority))
                {
                    query = query.Where(t => t.Priority == priority);
                }
            }

            if (StartDatePicker.SelectedDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt.Date >= StartDatePicker.SelectedDate.Value.Date);
            }

            if (EndDatePicker.SelectedDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt.Date <= EndDatePicker.SelectedDate.Value.Date);
            }

            TicketsGrid.ItemsSource = query
                .AsEnumerable() // Bring to memory for custom ordering
                .OrderBy(t => t.Priority == TicketPriority.Alta ? 0 :
                              t.Priority == TicketPriority.Media ? 1 : 2) // Custom order for priorities
                .ThenBy(t => t.CreatedAt) // Order by oldest first within each priority
                .ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao carregar os tickets: " + ex.Message, "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DatePicker_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.H)
        {
            if (sender is DatePicker datePicker)
            {
                datePicker.SelectedDate = DateTime.Now;
                e.Handled = true;
            }
        }
    }

    private void FilterButton_Click(object sender, RoutedEventArgs e)
    {
        LoadTickets();
    }

    private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
    {
        StatusFilterComboBox.SelectedIndex = 0;
        PriorityFilterComboBox.SelectedIndex = 0;
        StartDatePicker.SelectedDate = null;
        EndDatePicker.SelectedDate = null;
        LoadTickets();
    }

    protected override void OnClosed(EventArgs e)
    {
        _context.Dispose();
        base.OnClosed(e);
    }

    private void NewTicketButton_Click(object sender, RoutedEventArgs e)
    {
        if (_ticketWindow == null)
        {
            _ticketWindow = new TicketWindow(_context, null, LoadTickets);
            _ticketWindow.Owner = this;
            _ticketWindow.Closed += (s, args) => _ticketWindow = null;
            _ticketWindow.Show();
        }
        else
        {
            _ticketWindow.Activate();
        }
    }

    private void EditTicketButton_Click(object sender, RoutedEventArgs e)
    {
        if (TicketsGrid.SelectedItem is Ticket selectedTicket)
        {
            if (_ticketWindow == null)
            {
                _ticketWindow = new TicketWindow(_context, selectedTicket, LoadTickets);
                _ticketWindow.Owner = this;
                _ticketWindow.Closed += (s, args) => _ticketWindow = null;
                _ticketWindow.Show();
            }
            else
            {
                _ticketWindow.Activate();
            }
        }
        else
        {
            MessageBox.Show("Por favor, selecione um ticket para editar.", "Nenhum Ticket Selecionado", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ViewTicketButton_Click(object sender, RoutedEventArgs e)
    {
        if (TicketsGrid.SelectedItem is Ticket selectedTicket)
        {
            if (_viewTicketWindow == null)
            {
                try
                {
                    var ticketWithHistory = _context.Tickets
                        .Include(t => t.ReopeningLogs)
                        .FirstOrDefault(t => t.Id == selectedTicket.Id);

                    if (ticketWithHistory != null)
                    {
                        _viewTicketWindow = new ViewTicketWindow(ticketWithHistory);
                        _viewTicketWindow.Owner = this;
                        _viewTicketWindow.Closed += (s, args) => _viewTicketWindow = null;
                        _viewTicketWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("O ticket selecionado não foi encontrado no banco de dados.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar os detalhes do ticket: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                _viewTicketWindow.Activate();
            }
        }
        else
        {
            MessageBox.Show("Por favor, selecione um ticket para visualizar.", "Nenhum Ticket Selecionado", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void DeleteTicketButton_Click(object sender, RoutedEventArgs e)
    {
        if (TicketsGrid.SelectedItem is Ticket selectedTicket)
        {
            var message = $"Tem certeza que deseja excluir o ticket \"{selectedTicket.Title}\"? Esta ação é irreversível e será registrada.";
            if (MessageBox.Show(message, "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    var logEntry = new TicketLog
                    {
                        OriginalTicketId = selectedTicket.Id,
                        Title = selectedTicket.Title,
                        Description = selectedTicket.Description,
                        Status = selectedTicket.Status,
                        Priority = selectedTicket.Priority,
                        SlaMinutes = selectedTicket.SlaMinutes,
                        CreatedAt = selectedTicket.CreatedAt,
                        UpdatedAt = selectedTicket.UpdatedAt,
                        DeletedByHostname = Dns.GetHostName(),
                        DeletedByWindowsUser = Environment.UserName,
                        DeletionTimestamp = DateTime.Now
                    };

                    _context.TicketLogs.Add(logEntry);
                    _context.Tickets.Remove(selectedTicket);
                    
                    _context.SaveChanges();
                    
                    LoadTickets();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao excluir o ticket: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        else
        {
            MessageBox.Show("Por favor, selecione um ticket para excluir.", "Nenhum Ticket Selecionado", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Arquivo Excel|*.xlsx",
            Title = "Salvar Planilha de Tickets",
            FileName = $"Tickets_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Tickets");
                    var currentRow = 1;
                    var currentColumn = 1;

                    foreach (var setting in _userColumnSettings.ColumnSettings.Where(s => s.IsVisible).OrderBy(s => s.DisplayIndex))
                    {
                        worksheet.Cell(currentRow, currentColumn).Value = setting.Header;
                        currentColumn++;
                    }

                    worksheet.Cell(currentRow, currentColumn++).Value = "Histórico de Reaberturas";
                    worksheet.Cell(currentRow, currentColumn++).Value = "Histórico de Logs";

                    if (TicketsGrid.ItemsSource is IEnumerable<Ticket> tickets)
                    {
                        foreach (var ticket in tickets)
                        {
                            currentRow++;
                            currentColumn = 1;

                            foreach (var setting in _userColumnSettings.ColumnSettings.Where(s => s.IsVisible).OrderBy(s => s.DisplayIndex))
                            {
                                var property = typeof(Ticket).GetProperty(setting.Name);
                                if (property != null)
                                {
                                    worksheet.Cell(currentRow, currentColumn).Value = property.GetValue(ticket)?.ToString();
                                }
                                currentColumn++;
                            }

                            var reopeningLogs = _context.ReopeningLogs.Where(rl => rl.TicketId == ticket.Id).ToList();
                            var reopeningHistory = string.Join("; ", reopeningLogs.Select(rl => $"{rl.ReopenedAt:dd/MM/yyyy HH:mm} por {rl.ReopenedBy}: {rl.Reason}"));
                            worksheet.Cell(currentRow, currentColumn++).Value = reopeningHistory;

                            var ticketLogs = _context.TicketLogs.Where(tl => tl.OriginalTicketId == ticket.Id).ToList();
                            var logsHistory = string.Join("; ", ticketLogs.Select(tl => $"{tl.DeletionTimestamp:dd/MM/yyyy HH:mm} por {tl.DeletedByWindowsUser} (Exclusão)"));
                            worksheet.Cell(currentRow, currentColumn++).Value = logsHistory;
                        }
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs(saveFileDialog.FileName);
                }

                MessageBox.Show("Planilha exportada com sucesso!", "Exportação Concluída", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao exportar a planilha: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ReopenTicketButton_Click(object sender, RoutedEventArgs e)
    {
        if (TicketsGrid.SelectedItem is Ticket selectedTicket)
        {
            AttemptToReopenTicket(selectedTicket);
        }
    }

    private void TicketsGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        ViewTicketButton.IsEnabled = false;
        EditTicketButton.IsEnabled = false;
        ReopenTicketButton.IsEnabled = false;

        if (TicketsGrid.SelectedItem is Ticket selectedTicket)
        {
            ViewTicketButton.IsEnabled = true;

            if (selectedTicket.Status == TicketStatus.Fechado || selectedTicket.Status == TicketStatus.Resolvido)
            {
                ReopenTicketButton.IsEnabled = true;
            }
            else
            {
                EditTicketButton.IsEnabled = true;
            }
        }
    }

    private void TicketsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (TicketsGrid.SelectedItem is Ticket selectedTicket)
        {
            if (selectedTicket.Status == TicketStatus.Fechado || selectedTicket.Status == TicketStatus.Resolvido)
            {
                AttemptToReopenTicket(selectedTicket);
            }
            else
            {
                if (_ticketWindow == null)
                {
                    _ticketWindow = new TicketWindow(_context, selectedTicket, LoadTickets);
                    _ticketWindow.Owner = this;
                    _ticketWindow.Closed += (s, args) => _ticketWindow = null;
                    _ticketWindow.Show();
                }
                else
                {
                    _ticketWindow.Activate();
                }
            }
        }
    }

    private void AttemptToReopenTicket(Ticket ticket)
    {
        if (ticket.Status != TicketStatus.Fechado && ticket.Status != TicketStatus.Resolvido) return;

        if (ticket.Status == TicketStatus.Resolvido && !_isAdmin)
        {
            MessageBox.Show("Apenas administradores podem reabrir um ticket resolvido.", "Acesso Negado", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_reopenReasonWindow == null)
        {
            _reopenReasonWindow = new ReopenReasonWindow();
            _reopenReasonWindow.Owner = this;
            _reopenReasonWindow.Closed += (s, args) =>
            {
                if (_reopenReasonWindow.IsConfirmed)
                {
                    try
                    {
                        var reopeningLog = new ReopeningLog
                        {
                            TicketId = ticket.Id,
                            ReopenedAt = DateTime.Now,
                            ReopenedBy = Environment.UserName,
                            Reason = _reopenReasonWindow.Reason
                        };
                        _context.ReopeningLogs.Add(reopeningLog);

                        ticket.Status = TicketStatus.Aberto;
                        ticket.UpdatedAt = DateTime.Now;
                        
                        _context.SaveChanges();
                        LoadTickets();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao reabrir o ticket: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                _reopenReasonWindow = null;
            };
            _reopenReasonWindow.Show();
        }
        else
        {
            _reopenReasonWindow.Activate();
        }
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        if (_settingsWindow == null)
        {
            _settingsWindow = new SettingsWindow(_context);
            _settingsWindow.Owner = this;
            _settingsWindow.Closed += (s, args) => _settingsWindow = null;
            _settingsWindow.Show();
        }
        else
        {
            _settingsWindow.Activate();
        }
    }

    private void OpenColumnConfigurationWindow()
    {
        var columnConfigurationWindow = new ColumnConfigurationWindow(new System.Collections.ObjectModel.ObservableCollection<ColumnSetting>(_userColumnSettings.ColumnSettings));
        columnConfigurationWindow.Owner = this;
        if (columnConfigurationWindow.ShowDialog() == true)
        {
            _userColumnSettings.ColumnSettings = new System.Collections.Generic.List<ColumnSetting>(columnConfigurationWindow.ColumnSettings);
            ApplyColumnSettings();
            SaveUserColumnSettings();
        }
    }

    private void ColumnSettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var contextMenu = new ContextMenu();

        var configureItem = new MenuItem { Header = "Configurar Colunas" };
        configureItem.Click += (s, args) => OpenColumnConfigurationWindow();
        contextMenu.Items.Add(configureItem);

        contextMenu.Items.Add(new Separator());

        foreach (var setting in _userColumnSettings.ColumnSettings.OrderBy(s => s.DisplayIndex))
        {
            var menuItem = new MenuItem
            {
                Header = setting.Header,
                IsCheckable = true,
                IsChecked = setting.IsVisible,
                Tag = setting
            };
            menuItem.Click += ColumnMenuItem_Click;
            contextMenu.Items.Add(menuItem);
        }

        contextMenu.PlacementTarget = sender as UIElement;
        contextMenu.IsOpen = true;
    }

    private void ColumnMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && menuItem.Tag is ColumnSetting setting)
        {
            setting.IsVisible = menuItem.IsChecked;
            ApplyColumnSettings();
            SaveUserColumnSettings();
        }
    }

    private void SetButtonVisibility()
    {
        SettingsButton.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
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

    private void LightThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ChangeTheme("Light.Blue");
    }

    private void DarkThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ChangeTheme("Dark.Blue");
    }
}

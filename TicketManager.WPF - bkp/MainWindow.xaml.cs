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

namespace TicketManager.WPF;

public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
{
    private readonly TicketContext _context = new TicketContext();
    private readonly bool _isAdmin;

    public MainWindow(AuthorizedUser user)
    {
        InitializeComponent();
        _isAdmin = user.IsAdminBool;

        CurrentUserTextBlock.Text = $"Usuário: {user.WindowsUserName}";
        InitializeFilters();
        LoadTickets();
        SetButtonVisibility();
    }

    private void InitializeFilters()
    {
        // Popula Status
        var statusOptions = new List<string> { "Todos", "Todos Abertos", "Aberto", "EmAndamento", "Resolvido", "Fechado" };
        StatusFilterComboBox.ItemsSource = statusOptions;
        StatusFilterComboBox.SelectedIndex = 1; // Padrão: "Todos Abertos"

        // Popula Prioridade
        var priorityOptions = new List<string> { "Todas" };
        priorityOptions.AddRange(Enum.GetNames(typeof(TicketPriority)));
        PriorityFilterComboBox.ItemsSource = priorityOptions;
        PriorityFilterComboBox.SelectedIndex = 0; // Padrão: "Todas"
    }

    private void LoadTickets()
    {
        try
        {
            var query = _context.Tickets.AsQueryable();

            // Aplica filtro de Status
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

            // Aplica filtro de Prioridade
            if (PriorityFilterComboBox.SelectedItem is string priorityFilter && priorityFilter != "Todas")
            {
                if (Enum.TryParse<TicketPriority>(priorityFilter, out var priority))
                {
                    query = query.Where(t => t.Priority == priority);
                }
            }

            // Aplica filtro de Data de Criação
            if (StartDatePicker.SelectedDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt.Date >= StartDatePicker.SelectedDate.Value.Date);
            }

            if (EndDatePicker.SelectedDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt.Date <= EndDatePicker.SelectedDate.Value.Date);
            }

            TicketsGrid.ItemsSource = query.ToList();
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
        LoadTickets();
    }

    protected override void OnClosed(EventArgs e)
    {
        _context.Dispose();
        base.OnClosed(e);
    }

    private void NewTicketButton_Click(object sender, RoutedEventArgs e)
    {
        var ticketWindow = new TicketWindow(_context);
        if (ticketWindow.ShowDialog() == true)
        {
            LoadTickets();
        }
    }

    private void EditTicketButton_Click(object sender, RoutedEventArgs e)
    {
        if (TicketsGrid.SelectedItem is Ticket selectedTicket)
        {
            var ticketWindow = new TicketWindow(_context, selectedTicket);
            if (ticketWindow.ShowDialog() == true)
            {
                LoadTickets();
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
            try
            {
                // Carrega o ticket com seu histórico de reaberturas
                var ticketWithHistory = _context.Tickets
                    .Include(t => t.ReopeningLogs)
                    .FirstOrDefault(t => t.Id == selectedTicket.Id);

                if (ticketWithHistory != null)
                {
                    var viewWindow = new ViewTicketWindow(ticketWithHistory);
                    viewWindow.ShowDialog();
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
                    // Cria a entrada de log
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

                    // Cabeçalho
                    worksheet.Cell(currentRow, 1).Value = "ID";
                    worksheet.Cell(currentRow, 2).Value = "Título";
                    worksheet.Cell(currentRow, 3).Value = "Descrição";
                    worksheet.Cell(currentRow, 4).Value = "Status";
                    worksheet.Cell(currentRow, 5).Value = "Prioridade";
                    worksheet.Cell(currentRow, 6).Value = "SLA (minutos)";
                    worksheet.Cell(currentRow, 7).Value = "Data de Criação";
                    worksheet.Cell(currentRow, 8).Value = "Última Atualização";

                    // Dados (usa a lista que já está na grade, que já está filtrada)
                    if (TicketsGrid.ItemsSource is IEnumerable<Ticket> tickets)
                    {
                        foreach (var ticket in tickets)
                        {
                            currentRow++;
                            worksheet.Cell(currentRow, 1).Value = ticket.Id;
                            worksheet.Cell(currentRow, 2).Value = ticket.Title;
                            worksheet.Cell(currentRow, 3).Value = ticket.Description;
                            worksheet.Cell(currentRow, 4).Value = ticket.Status.ToString();
                            worksheet.Cell(currentRow, 5).Value = ticket.Priority.ToString();
                            worksheet.Cell(currentRow, 6).Value = ticket.SlaMinutes;
                            worksheet.Cell(currentRow, 7).Value = ticket.CreatedAt;
                            worksheet.Cell(currentRow, 8).Value = ticket.UpdatedAt;
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
            ViewTicketButton.IsEnabled = true; // Botão Visualizar sempre ativo se algo estiver selecionado

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
                // Abrir modo de edição para tickets não fechados
                var ticketWindow = new TicketWindow(_context, selectedTicket);
                if (ticketWindow.ShowDialog() == true)
                {
                    LoadTickets();
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

        var reasonWindow = new ReopenReasonWindow();
        if (reasonWindow.ShowDialog() == true)
        {
            try
            {
                // Criar o log de reabertura
                var reopeningLog = new ReopeningLog
                {
                    TicketId = ticket.Id,
                    ReopenedAt = DateTime.Now,
                    ReopenedBy = Environment.UserName,
                    Reason = reasonWindow.Reason
                };
                _context.ReopeningLogs.Add(reopeningLog);

                // Atualizar o ticket
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
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow(_context);
        settingsWindow.ShowDialog();
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
using System;
using System.Windows;
using TicketManager.WPF.Data;
using TicketManager.WPF.Models;
using System.Net;
using System.Windows.Input;
using System.Windows.Controls;

namespace TicketManager.WPF
{
    public partial class TicketWindow : MahApps.Metro.Controls.MetroWindow
    {
        private readonly TicketContext _context;
        private Ticket _ticket;
        private readonly TicketStatus _originalStatus;
        private readonly Action? _onSave;

        public TicketWindow(TicketContext context, Ticket? ticket = null, Action? onSave = null)
        {
            InitializeComponent();
            _context = context;
            _onSave = onSave;

            StatusComboBox.ItemsSource = Enum.GetValues(typeof(TicketStatus));
            PriorityComboBox.ItemsSource = Enum.GetValues(typeof(TicketPriority));

            if (ticket == null)
            {
                _ticket = new Ticket();
                Title = "Novo Ticket";
                StatusComboBox.SelectedItem = TicketStatus.Aberto;
                PriorityComboBox.SelectedItem = TicketPriority.Media;
                _originalStatus = _ticket.Status;
                SlaFinalTextBox.IsEnabled = false;
            }
            else
            {
                _ticket = ticket;
                _originalStatus = _ticket.Status;

                Title = "Editar Ticket";
                TitleTextBox.Text = _ticket.Title;
                DescriptionTextBox.Text = _ticket.Description;
                StatusComboBox.SelectedItem = _ticket.Status;
                PriorityComboBox.SelectedItem = _ticket.Priority;
                SlaTextBox.Text = _ticket.SlaMinutes.ToString();
                SlaFinalTextBox.Text = _ticket.SLAFinal?.ToString() ?? string.Empty;

                SlaLabel.Content = "SLA (min est.)";
                SlaTextBox.IsEnabled = false;

                SlaFinalTextBox.IsEnabled = _ticket.SLAFinal.HasValue;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_ticket.Id == 0)
            {
                if (!int.TryParse(SlaTextBox.Text, out int slaMinutes) || slaMinutes <= 0)
                {
                    MessageBox.Show("O valor para SLA (minutos) deve ser um número inteiro positivo.", "Entrada Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _ticket.SlaMinutes = slaMinutes;
            }

            _ticket.Title = TitleTextBox.Text;
            _ticket.Description = DescriptionTextBox.Text;
            _ticket.Status = (TicketStatus)StatusComboBox.SelectedItem;
            _ticket.Priority = (TicketPriority)PriorityComboBox.SelectedItem;
            _ticket.UpdatedAt = DateTime.Now;

            if (SlaFinalTextBox.IsEnabled)
            {
                if (int.TryParse(SlaFinalTextBox.Text, out int slaFinalValue))
                {
                    _ticket.SLAFinal = slaFinalValue;
                }
                else
                {
                    MessageBox.Show("O valor para SLA Final (min) é obrigatório e deve ser um número inteiro válido.", "Entrada Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (_ticket.Id == 0)
            {
                _ticket.CreatedAt = DateTime.Now;
                _ticket.CreatedByWindowsUser = Environment.UserName;
                _ticket.CreatedByHostname = Dns.GetHostName();
                _context.Tickets.Add(_ticket);
            }

            try
            {
                _context.SaveChanges();
                this.Close();
                _onSave?.Invoke();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is Ticket)
                    {
                        var databaseEntry = entry.GetDatabaseValues();
                        if (databaseEntry == null)
                        {
                            MessageBox.Show("O ticket foi excluído por outro usuário.", "Erro de Concorrência", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            var result = MessageBox.Show(
                                "O ticket foi modificado por outro usuário. Deseja sobrescrever as alterações deles com as suas?",
                                "Conflito de Concorrência",
                                MessageBoxButton.YesNoCancel,
                                MessageBoxImage.Warning);

                            if (result == MessageBoxResult.Yes)
                            {
                                entry.OriginalValues.SetValues(databaseEntry);
                            }
                            else if (result == MessageBoxResult.No)
                            {
                                entry.Reload();
                                MessageBox.Show("As alterações de outro usuário foram recarregadas. Por favor, revise e tente salvar novamente.", "Conflito Resolvido", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
                try
                {
                    _context.SaveChanges();
                    this.Close();
                    _onSave?.Invoke();
                }
                catch (Exception innerEx)
                {
                    MessageBox.Show("Erro ao salvar o ticket após resolução de concorrência: " + innerEx.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar o ticket: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 0) return;

            var newStatus = (TicketStatus)StatusComboBox.SelectedItem;
            var previousStatus = (TicketStatus)e.RemovedItems[0]!;

            bool isClosingOrResolving = (newStatus == TicketStatus.Fechado || newStatus == TicketStatus.Resolvido);
            bool wasAlreadyClosedOrResolved = (previousStatus == TicketStatus.Fechado || previousStatus == TicketStatus.Resolvido);

            if (isClosingOrResolving && !wasAlreadyClosedOrResolved)
            {
                var slaConfirmationDialog = new SlaConfirmationWindow(_ticket.SlaMinutes, _ticket.Id == 0 ? DateTime.Now : _ticket.CreatedAt);
                slaConfirmationDialog.Owner = this;
                if (slaConfirmationDialog.ShowDialog() == true)
                {
                    SlaFinalTextBox.Text = slaConfirmationDialog.FinalSla?.ToString() ?? string.Empty;
                    SlaFinalTextBox.IsEnabled = true;
                }
                else
                {
                    StatusComboBox.SelectedItem = previousStatus;
                }
            }
        }

        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (sender is ComboBox cmb)
                {
                    cmb.IsDropDownOpen = true;
                    e.Handled = true;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

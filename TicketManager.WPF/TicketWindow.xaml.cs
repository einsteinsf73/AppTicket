using System;
using System.Windows;
using TicketManager.WPF.Data;
using TicketManager.WPF.Models;
using System.Net;
using System.Windows.Input;
using System.Windows.Controls;

namespace TicketManager.WPF
{
    public partial class TicketWindow : Window
    {
        private readonly TicketContext _context;
        private Ticket _ticket;
        private readonly TicketStatus _originalStatus;

        public TicketWindow(TicketContext context, Ticket? ticket = null)
        {
            InitializeComponent();
            _context = context;

            StatusComboBox.ItemsSource = Enum.GetValues(typeof(TicketStatus));
            PriorityComboBox.ItemsSource = Enum.GetValues(typeof(TicketPriority));

            if (ticket == null)
            {
                _ticket = new Ticket();
                Title = "Novo Ticket";
                StatusComboBox.SelectedItem = TicketStatus.Aberto;
                PriorityComboBox.SelectedItem = TicketPriority.Media;
                _originalStatus = _ticket.Status; // Status inicial para um novo ticket
                SlaFinalTextBox.IsEnabled = false; // Desabilita na criação
            }
            else
            {
                _ticket = ticket;
                _originalStatus = _ticket.Status; // Armazena o status original

                Title = "Editar Ticket";
                TitleTextBox.Text = _ticket.Title;
                DescriptionTextBox.Text = _ticket.Description;
                StatusComboBox.SelectedItem = _ticket.Status;
                PriorityComboBox.SelectedItem = _ticket.Priority;
                SlaTextBox.Text = _ticket.SlaMinutes.ToString();
                SlaFinalTextBox.Text = _ticket.SLAFinal?.ToString() ?? string.Empty;

                // Protege o campo SLA (min) e ajusta o rótulo
                SlaLabel.Content = "SLA (min est.)";
                SlaTextBox.IsEnabled = false;

                // Habilita a edição do SLA Final apenas se um valor já foi definido anteriormente (em caso de reabertura)
                SlaFinalTextBox.IsEnabled = _ticket.SLAFinal.HasValue;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validação do SLA inicial apenas para novos tickets
            if (_ticket.Id == 0)
            {
                if (!int.TryParse(SlaTextBox.Text, out int slaMinutes) || slaMinutes <= 0)
                {
                    MessageBox.Show("O valor para SLA (minutos) deve ser um número inteiro positivo.", "Entrada Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _ticket.SlaMinutes = slaMinutes;
            }

            // Atualiza o objeto _ticket com os dados do formulário
            _ticket.Title = TitleTextBox.Text;
            _ticket.Description = DescriptionTextBox.Text;
            _ticket.Status = (TicketStatus)StatusComboBox.SelectedItem;
            _ticket.Priority = (TicketPriority)PriorityComboBox.SelectedItem;
            _ticket.UpdatedAt = DateTime.Now;

            // Atualiza o SLA Final a partir do campo de texto, se estiver habilitado
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
                DialogResult = true; // Fecha a janela e indica sucesso
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar o ticket: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Garante que a lógica não rode na inicialização da janela
            if (e.RemovedItems.Count == 0)
                return;

            var newStatus = (TicketStatus)StatusComboBox.SelectedItem;
            
            // Acessa o status anterior através do primeiro item removido da seleção
            var previousStatus = (TicketStatus)e.RemovedItems[0]!;

            bool isClosingOrResolving = (newStatus == TicketStatus.Fechado || newStatus == TicketStatus.Resolvido);
            bool wasAlreadyClosedOrResolved = (previousStatus == TicketStatus.Fechado || previousStatus == TicketStatus.Resolvido);

            if (isClosingOrResolving && !wasAlreadyClosedOrResolved)
            {
                var slaConfirmationDialog = new SlaConfirmationWindow(_ticket.SlaMinutes);
                if (slaConfirmationDialog.ShowDialog() == true)
                {
                    // Atualiza o campo de texto e habilita para edição se necessário
                    SlaFinalTextBox.Text = slaConfirmationDialog.FinalSla?.ToString() ?? string.Empty;
                    SlaFinalTextBox.IsEnabled = true;
                }
                else
                {
                    // Se o usuário cancelar o popup, reverte a mudança de status
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
    }
}

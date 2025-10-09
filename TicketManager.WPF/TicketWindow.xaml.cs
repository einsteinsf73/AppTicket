using System;
using System.Windows;
using TicketManager.WPF.Data;
using TicketManager.WPF.Models; // <-- LINHA ADICIONADA
using System.Net;
using System.Windows.Input;
using System.Windows.Controls;

namespace TicketManager.WPF
{
    public partial class TicketWindow : Window
    {
        private readonly TicketContext _context;
        private Ticket? _ticket;

        public TicketWindow(TicketContext context, Ticket? ticket = null)
        {
            InitializeComponent();
            _context = context;
            _ticket = ticket;

            // Preenche os ComboBoxes
            StatusComboBox.ItemsSource = Enum.GetValues(typeof(TicketStatus));
            PriorityComboBox.ItemsSource = Enum.GetValues(typeof(TicketPriority));

            if (_ticket == null)
            {
                // Modo de criação: Novo ticket
                _ticket = new Ticket();
                Title = "Novo Ticket";
                StatusComboBox.SelectedItem = TicketStatus.Aberto;
                PriorityComboBox.SelectedItem = TicketPriority.Media;
            }
            else
            {
                // Modo de edição: Carrega dados existentes
                Title = "Editar Ticket";
                TitleTextBox.Text = _ticket.Title;
                DescriptionTextBox.Text = _ticket.Description;
                StatusComboBox.SelectedItem = _ticket.Status;
                PriorityComboBox.SelectedItem = _ticket.Priority;
                SlaTextBox.Text = _ticket.SlaMinutes.ToString();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_ticket is null)
            {
                MessageBox.Show("Erro inesperado: O objeto do ticket não foi inicializado.", "Erro Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validação do SLA
            if (!int.TryParse(SlaTextBox.Text, out int slaMinutes))
            {
                MessageBox.Show("O valor para SLA (minutos) deve ser um número inteiro válido.", "Entrada Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Atualiza o objeto _ticket com os dados do formulário
            _ticket.Title = TitleTextBox.Text;
            _ticket.Description = DescriptionTextBox.Text;
            _ticket.Status = (TicketStatus)StatusComboBox.SelectedItem;
            _ticket.Priority = (TicketPriority)PriorityComboBox.SelectedItem;
            _ticket.SlaMinutes = slaMinutes;
            _ticket.UpdatedAt = DateTime.Now;

            if (_ticket.Id == 0)
            {
                // Novo ticket
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

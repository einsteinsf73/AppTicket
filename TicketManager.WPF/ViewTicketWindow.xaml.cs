using System.Windows;
using TicketManager.WPF.Models;

namespace TicketManager.WPF
{
    public partial class ViewTicketWindow : Window
    {
        public ViewTicketWindow(Ticket ticket)
        {
            InitializeComponent();

            // Preenche os campos do ticket
            TitleTextBlock.Text = ticket.Title;
            DescriptionTextBlock.Text = ticket.Description;
            PriorityTextBlock.Text = ticket.Priority.ToString();
            StatusTextBlock.Text = ticket.Status.ToString();
            SlaTextBlock.Text = ticket.SlaMinutes.ToString();
            CreatedAtTextBlock.Text = ticket.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            CreatedByTextBlock.Text = $"{ticket.CreatedByWindowsUser} ({ticket.CreatedByHostname})";

            // Preenche o histórico de reaberturas
            ReopeningLogsListView.ItemsSource = ticket.ReopeningLogs;
        }
    }
}

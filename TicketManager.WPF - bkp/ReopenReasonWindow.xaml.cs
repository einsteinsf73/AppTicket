using System.Windows;

namespace TicketManager.WPF
{
    public partial class ReopenReasonWindow : MahApps.Metro.Controls.MetroWindow
    {
        public string Reason { get; private set; } = string.Empty;

        public ReopenReasonWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReasonTextBox.Text))
            {
                MessageBox.Show("O motivo da reabertura não pode estar em branco.", "Campo Obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Reason = ReasonTextBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

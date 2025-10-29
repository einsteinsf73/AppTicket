using System.Windows;

namespace TicketManager.WPF
{
    public partial class ReopenReasonWindow : MahApps.Metro.Controls.MetroWindow
    {
        public string Reason { get; private set; } = string.Empty;
        public bool IsConfirmed { get; private set; } = false;

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
            IsConfirmed = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }
    }
}
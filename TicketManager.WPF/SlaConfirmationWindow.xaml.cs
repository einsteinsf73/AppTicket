using System.Windows;

namespace TicketManager.WPF
{
    public partial class SlaConfirmationWindow : Window
    {
        public int? FinalSla { get; private set; }
        private readonly int _estimatedSla;

        public SlaConfirmationWindow(int estimatedSla)
        {
            InitializeComponent();
            _estimatedSla = estimatedSla;
        }

        private void SlaMaintainedCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (FinalSlaTextBox != null) // Previne erro durante a inicialização
            {
                FinalSlaTextBox.IsEnabled = SlaMaintainedCheckBox.IsChecked != true;
                if (SlaMaintainedCheckBox.IsChecked == true)
                {
                    FinalSlaTextBox.Text = string.Empty; // Limpa o campo se a caixa for marcada
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SlaMaintainedCheckBox.IsChecked == true)
            {
                FinalSla = _estimatedSla;
            }
            else
            {
                if (!int.TryParse(FinalSlaTextBox.Text, out int finalSlaValue) || string.IsNullOrWhiteSpace(FinalSlaTextBox.Text))
                {
                    MessageBox.Show("Por favor, insira um valor numérico válido para o SLA Final.", "Entrada Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                FinalSla = finalSlaValue;
            }

            DialogResult = true;
        }
    }
}

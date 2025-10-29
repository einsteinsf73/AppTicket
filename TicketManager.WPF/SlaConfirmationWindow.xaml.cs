using System;
using System.Windows;

namespace TicketManager.WPF
{
    public partial class SlaConfirmationWindow : MahApps.Metro.Controls.MetroWindow
    {
        public int? FinalSla { get; private set; }
        private readonly int _estimatedSla;
        private readonly DateTime _createdAt;

        public SlaConfirmationWindow(int estimatedSla, DateTime createdAt)
        {
            InitializeComponent();
            _estimatedSla = estimatedSla;
            _createdAt = createdAt;
        }

        private void SlaOption_Changed(object sender, RoutedEventArgs e)
        {
            if (FinalSlaTextBox != null)
            {
                FinalSlaTextBox.IsEnabled = ManualSlaRadioButton.IsChecked == true;
                if (SlaMaintainedRadioButton.IsChecked == true || CalculateSlaRadioButton.IsChecked == true)
                {
                    FinalSlaTextBox.Text = string.Empty;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SlaMaintainedRadioButton.IsChecked == true)
            {
                FinalSla = _estimatedSla;
            }
            else if (CalculateSlaRadioButton.IsChecked == true)
            {
                FinalSla = CalculateBusinessMinutes(_createdAt, DateTime.Now);
            }
            else if (ManualSlaRadioButton.IsChecked == true)
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

        private int CalculateBusinessMinutes(DateTime start, DateTime end)
        {
            double totalMinutes = 0;
            var businessStartTime = new TimeSpan(7, 30, 0);
            var businessEndTime = new TimeSpan(17, 30, 0);

            var currentDate = start.Date;
            while (currentDate <= end.Date)
            {
                if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    var startTime = (currentDate == start.Date) ? start.TimeOfDay : businessStartTime;
                    var endTime = (currentDate == end.Date) ? end.TimeOfDay : businessEndTime;

                    if (startTime < businessStartTime) startTime = businessStartTime;
                    if (endTime > businessEndTime) endTime = businessEndTime;

                    if (endTime > startTime)
                    {
                        totalMinutes += (endTime - startTime).TotalMinutes;
                    }
                }
                currentDate = currentDate.AddDays(1);
            }

            return (int)Math.Round(totalMinutes);
        }
    }
}

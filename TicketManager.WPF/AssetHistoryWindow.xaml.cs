using System.Collections.Generic;
using System.Windows;
using TicketManager.WPF.Models;

namespace TicketManager.WPF
{
    public partial class AssetHistoryWindow : MahApps.Metro.Controls.MetroWindow
    {
        public AssetHistoryWindow(List<AssetHistory> history)
        {
            InitializeComponent();
            HistoryGrid.ItemsSource = history;
        }

        private void MetroWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }
    }
}

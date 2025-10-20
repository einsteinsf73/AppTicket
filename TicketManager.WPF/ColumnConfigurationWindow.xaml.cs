using MahApps.Metro.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TicketManager.WPF.Models;

namespace TicketManager.WPF
{
    public partial class ColumnConfigurationWindow : MetroWindow
    {
        public ObservableCollection<ColumnSetting> ColumnSettings { get; set; }

        public ColumnConfigurationWindow(ObservableCollection<ColumnSetting> currentColumnSettings)
        {
            InitializeComponent();
            ColumnSettings = new ObservableCollection<ColumnSetting>(currentColumnSettings.Select(c => (ColumnSetting)c.Clone()));
            this.DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                DragDrop.DoDragDrop(item, item.DataContext, DragDropEffects.Move);
            }
        }

        private void ListBoxItem_Drop(object sender, DragEventArgs e)
        {
            if (sender is ListBoxItem targetItem && e.Data.GetData(typeof(ColumnSetting)) is ColumnSetting draggedItem)
            {
                int removedIdx = ColumnSettings.IndexOf(draggedItem);
                int targetIdx = ColumnSettings.IndexOf(targetItem.DataContext as ColumnSetting);

                if (removedIdx < targetIdx)
                {
                    ColumnSettings.Insert(targetIdx + 1, draggedItem);
                    ColumnSettings.RemoveAt(removedIdx);
                }
                else
                {
                    int remIdx = removedIdx + 1;
                    if (ColumnSettings.Count > remIdx)
                    {
                        ColumnSettings.Insert(targetIdx, draggedItem);
                        ColumnSettings.RemoveAt(remIdx);
                    }
                }
            }
        }
    }


}
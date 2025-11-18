using System.Windows;
using TicketManager.WPF.Data;
using System.Linq;
using TicketManager.WPF.Models;
using Microsoft.Win32;
using System.IO;

namespace TicketManager.WPF
{
    public partial class AssetControlWindow : MahApps.Metro.Controls.MetroWindow
    {
        private readonly TicketContext _context;
        private readonly AuthorizedUser _user;

        public AssetControlWindow(AuthorizedUser user)
        {
            InitializeComponent();
            _context = new TicketContext();
            _user = user;
            LoadAssets();
        }

        private void LoadAssets(string filter = null)
        {
            var query = _context.Patrimonio.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var lowerFilter = filter.ToLower();
                query = query.Where(a =>
                    (a.Item != null && a.Item.ToLower().Contains(lowerFilter)) ||
                    (a.Brand != null && a.Brand.ToLower().Contains(lowerFilter)) ||
                    (a.Model != null && a.Model.ToLower().Contains(lowerFilter)) ||
                    (a.SerialNumber != null && a.SerialNumber.ToLower().Contains(lowerFilter)) ||
                    (a.AssetNumber != null && a.AssetNumber.ToLower().Contains(lowerFilter)) ||
                    (a.Supplier != null && a.Supplier.ToLower().Contains(lowerFilter)) ||
                    (a.InvoiceNumber != null && a.InvoiceNumber.ToLower().Contains(lowerFilter)) ||
                    (a.Location != null && a.Location.ToLower().Contains(lowerFilter)) ||
                    (a.Sector != null && a.Sector.ToLower().Contains(lowerFilter)) ||
                    (a.Department != null && a.Department.ToLower().Contains(lowerFilter)) ||
                    (a.Employee != null && a.Employee.ToLower().Contains(lowerFilter)) ||
                    (a.Status != null && a.Status.ToLower().Contains(lowerFilter)));
            }

            AssetsGrid.ItemsSource = query.ToList();
        }
        
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var initialScreen = new InitialScreen(_user);
            initialScreen.Show();
            this.Close();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            LoadAssets(SearchTextBox.Text);
        }

        private void AddAssetButton_Click(object sender, RoutedEventArgs e)
        {
            var newAsset = new Asset();
            var editWindow = new AssetEditWindow(_context, newAsset, () => LoadAssets());
            editWindow.ShowDialog();
        }

        private void EditAssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (AssetsGrid.SelectedItem is Asset selectedAsset)
            {
                var editWindow = new AssetEditWindow(_context, selectedAsset, () => LoadAssets());
                editWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Por favor, selecione um patrimônio para editar.", "Nenhum Patrimônio Selecionado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteAssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (AssetsGrid.SelectedItem is Asset selectedAsset)
            {
                if (MessageBox.Show($"Tem certeza que deseja excluir o patrimônio \"{selectedAsset.Item}\"?", "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _context.Patrimonio.Remove(selectedAsset);
                    _context.SaveChanges();
                    LoadAssets();
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione um patrimônio para excluir.", "Nenhum Patrimônio Selecionado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (AssetsGrid.SelectedItem is Asset selectedAsset)
            {
                var history = _context.AssetHistories.Where(h => h.AssetId == selectedAsset.Id).OrderByDescending(h => h.ModifiedAt).ToList();

                var currentAssetAsHistory = new AssetHistory
                {
                    AssetId = selectedAsset.Id,
                    Item = selectedAsset.Item,
                    Brand = selectedAsset.Brand,
                    Model = selectedAsset.Model,
                    SerialNumber = selectedAsset.SerialNumber,
                    AssetNumber = selectedAsset.AssetNumber,
                    Value = selectedAsset.Value,
                    PurchaseDate = selectedAsset.PurchaseDate,
                    Supplier = selectedAsset.Supplier,
                    InvoiceNumber = selectedAsset.InvoiceNumber,
                    Warranty = selectedAsset.Warranty,
                    Location = selectedAsset.Location,
                    Sector = selectedAsset.Sector,
                    Department = selectedAsset.Department,
                    Employee = selectedAsset.Employee,
                    Status = selectedAsset.Status,
                    Manutencao = selectedAsset.Manutencao,
                    PrevisaoManutencao = selectedAsset.PrevisaoManutencao,
                    Observations = selectedAsset.Observations,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = "Atual"
                };

                history.Insert(0, currentAssetAsHistory);

                var historyWindow = new AssetHistoryWindow(history);
                historyWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Por favor, selecione um patrimônio para ver o histórico.", "Nenhum Patrimônio Selecionado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
            if (openFileDialog.ShowDialog() == true)
            {
                ProcessExcelFile(openFileDialog.FileName);
            }
        }

        private void ProcessExcelFile(string filePath)
        {
            // I will implement this method after the user provides the CSV data.
        }

        private void AssetsGrid_Sorting(object sender, System.Windows.Controls.DataGridSortingEventArgs e)
        {
            if (e.Column.SortMemberPath == "AssetNumber")
            {
                var dataGrid = (System.Windows.Controls.DataGrid)sender;
                var items = dataGrid.ItemsSource.Cast<Asset>().ToList();
                var direction = e.Column.SortDirection == System.ComponentModel.ListSortDirection.Ascending ?
                                System.ComponentModel.ListSortDirection.Descending :
                                System.ComponentModel.ListSortDirection.Ascending;

                e.Column.SortDirection = direction;

                Func<string, int> parseAssetNumber = (assetNumber) =>
                {
                    if (int.TryParse(assetNumber, out int result))
                    {
                        return result;
                    }
                    return int.MaxValue; // ou algum outro valor padrão para strings não numéricas
                };

                if (direction == System.ComponentModel.ListSortDirection.Ascending)
                {
                    items.Sort((a, b) => parseAssetNumber(a.AssetNumber).CompareTo(parseAssetNumber(b.AssetNumber)));
                }
                else
                {
                    items.Sort((a, b) => parseAssetNumber(b.AssetNumber).CompareTo(parseAssetNumber(a.AssetNumber)));
                }

                dataGrid.ItemsSource = items;
                e.Handled = true;
            }
        }
    }
}
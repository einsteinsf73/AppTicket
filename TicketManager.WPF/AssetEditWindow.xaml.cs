using System.Windows;
using TicketManager.WPF.Data;
using TicketManager.WPF.Models;
using System;

namespace TicketManager.WPF
{
    public partial class AssetEditWindow : MahApps.Metro.Controls.MetroWindow
    {
        private readonly TicketContext _context;
        private readonly Asset _asset;
        private readonly Action _onSave;

        public AssetEditWindow(TicketContext context, Asset asset, Action onSave)
        {
            InitializeComponent();
            _context = context;
            _asset = asset;
            _onSave = onSave;

            if (_asset.Id != 0)
            {
                ItemTextBox.Text = _asset.Item;
                BrandTextBox.Text = _asset.Brand;
                ModelTextBox.Text = _asset.Model;
                SerialNumberTextBox.Text = _asset.SerialNumber;
                AssetNumberTextBox.Text = _asset.AssetNumber;
                ValueTextBox.Text = _asset.Value.ToString();
                PurchaseDatePicker.SelectedDate = _asset.PurchaseDate;
                SupplierTextBox.Text = _asset.Supplier;
                InvoiceNumberTextBox.Text = _asset.InvoiceNumber;
                WarrantyTextBox.Text = _asset.Warranty;
                LocationTextBox.Text = _asset.Location;
                SectorTextBox.Text = _asset.Sector;
                DepartmentTextBox.Text = _asset.Department;
                EmployeeTextBox.Text = _asset.Employee;
                StatusTextBox.Text = _asset.Status;
                ManutencaoTextBox.Text = _asset.Manutencao;
                PrevisaoManutencaoPicker.SelectedDate = _asset.PrevisaoManutencao;
                ObservationsTextBox.Text = _asset.Observations;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_asset.Id != 0)
            {
                var history = new AssetHistory
                {
                    AssetId = _asset.Id,
                    Item = _asset.Item,
                    Brand = _asset.Brand,
                    Model = _asset.Model,
                    SerialNumber = _asset.SerialNumber,
                    AssetNumber = _asset.AssetNumber,
                    Value = _asset.Value,
                    PurchaseDate = _asset.PurchaseDate,
                    Supplier = _asset.Supplier,
                    InvoiceNumber = _asset.InvoiceNumber,
                    Warranty = _asset.Warranty,
                    Location = _asset.Location,
                    Sector = _asset.Sector,
                    Department = _asset.Department,
                    Employee = _asset.Employee,
                    Status = _asset.Status,
                    Manutencao = _asset.Manutencao,
                    PrevisaoManutencao = _asset.PrevisaoManutencao,
                    Observations = _asset.Observations,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = Environment.UserName
                };
                _context.AssetHistories.Add(history);
            }

            _asset.Item = ItemTextBox.Text;
            _asset.Brand = BrandTextBox.Text;
            _asset.Model = ModelTextBox.Text;
            _asset.SerialNumber = SerialNumberTextBox.Text;
            _asset.AssetNumber = AssetNumberTextBox.Text;
            _asset.Value = decimal.Parse(ValueTextBox.Text);
            _asset.PurchaseDate = PurchaseDatePicker.SelectedDate ?? DateTime.Now;
            _asset.Supplier = SupplierTextBox.Text;
            _asset.InvoiceNumber = InvoiceNumberTextBox.Text;
            _asset.Warranty = WarrantyTextBox.Text;
            _asset.Location = LocationTextBox.Text;
            _asset.Sector = SectorTextBox.Text;
            _asset.Department = DepartmentTextBox.Text;
            _asset.Employee = EmployeeTextBox.Text;
            _asset.Status = StatusTextBox.Text;
            _asset.Manutencao = ManutencaoTextBox.Text;
            _asset.PrevisaoManutencao = PrevisaoManutencaoPicker.SelectedDate;
            _asset.Observations = ObservationsTextBox.Text;

            if (_asset.Id == 0)
            {
                _context.Patrimonio.Add(_asset);
            }

            _context.SaveChanges();
            _onSave?.Invoke();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
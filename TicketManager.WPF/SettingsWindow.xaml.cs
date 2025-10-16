using System.Linq;
using System.Windows;
using TicketManager.WPF.Data;
using TicketManager.WPF.Models;

namespace TicketManager.WPF
{
    public partial class SettingsWindow : Window
    {
        private readonly TicketContext _context;

        public SettingsWindow(TicketContext context)
        {
            InitializeComponent();
            _context = context;
            LoadAuthorizedUsers();
        }

        private void LoadAuthorizedUsers()
        {
            UsersGrid.ItemsSource = _context.AuthorizedUsers.ToList();
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var newUserName = NewUserTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(newUserName))
            {
                MessageBox.Show("Por favor, insira um nome de usuário válido.", "Entrada Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var userExists = _context.AuthorizedUsers.FirstOrDefault(u => u.WindowsUserName.ToUpper() == newUserName.ToUpper()) != null;
            if (userExists)
            {
                MessageBox.Show("Este usuário já está na lista de autorizados.", "Usuário Duplicado", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var newUser = new AuthorizedUser { WindowsUserName = newUserName, IsAdminBool = IsAdminCheckBox.IsChecked == true };
            _context.AuthorizedUsers.Add(newUser);
            _context.SaveChanges();

            NewUserTextBox.Clear();
            IsAdminCheckBox.IsChecked = false; // Reseta a checkbox
            LoadAuthorizedUsers(); // Recarrega a grade
        }

        private void RemoveUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is AuthorizedUser selectedUser)
            {
                // Impede a exclusão do próprio usuário se for o único na lista
                if (_context.AuthorizedUsers.Count() <= 1 && selectedUser.WindowsUserName.Equals(Environment.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Você não pode remover a si mesmo se for o único usuário autorizado.", "Operação Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Verifica se o usuário possui tickets associados
                var hasTickets = _context.Tickets.Any(t => t.CreatedByWindowsUser == selectedUser.WindowsUserName);
                if (hasTickets)
                {
                    MessageBox.Show($"O usuário '{selectedUser.WindowsUserName}' não pode ser excluído pois possui tickets registrados em seu nome. Para impedir o acesso, você pode marcá-lo como 'Inativo'.", "Exclusão Não Permitida", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var message = $"Tem certeza que deseja remover o acesso para o usuário '{selectedUser.WindowsUserName}'? Esta ação não poderá ser desfeita.";
                if (MessageBox.Show(message, "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _context.AuthorizedUsers.Remove(selectedUser);
                    _context.SaveChanges();
                    LoadAuthorizedUsers(); // Recarrega a grade
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione um usuário para remover.", "Nenhum Usuário Selecionado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void UsersGrid_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            // Salva as alterações no banco de dados sempre que uma célula é editada
            _context.SaveChanges();
        }
    }
}

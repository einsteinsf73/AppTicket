using System.Linq;
using System.Windows;
using TicketManager.WPF.Data;
using TicketManager.WPF.Models;
using MahApps.Metro.Controls;

namespace TicketManager.WPF
{
    public partial class SettingsWindow : MetroWindow
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

            var newUser = new AuthorizedUser 
            { 
                WindowsUserName = newUserName, 
                IsAdminBool = IsAdminCheckBox.IsChecked == true,
                HasTicketAccessBool = HasTicketAccessCheckBox.IsChecked == true,
                HasAssetAccessBool = HasAssetAccessCheckBox.IsChecked == true
            };
            _context.AuthorizedUsers.Add(newUser);
            _context.SaveChanges();

            NewUserTextBox.Clear();
            IsAdminCheckBox.IsChecked = false;
            HasTicketAccessCheckBox.IsChecked = false;
            HasAssetAccessCheckBox.IsChecked = false;
            LoadAuthorizedUsers(); // Recarrega a grade
        }

        private void RemoveUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is AuthorizedUser selectedUser)
            {
                // Impede a exclusão do próprio usuário
                if (selectedUser.WindowsUserName.Equals(System.Environment.UserName, System.StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Você não pode remover seu próprio usuário.", "Operação Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var currentWindowsUser = System.Environment.UserName;
            var users = UsersGrid.ItemsSource as System.Collections.Generic.List<AuthorizedUser>;

            if (users == null) return;

            // Validação para impedir que o usuário logado remova seu próprio acesso
            foreach (var user in users)
            {
                if (user.WindowsUserName.Equals(currentWindowsUser, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (!user.IsAdminBool)
                    {
                        MessageBox.Show("Você não pode remover suas próprias permissões de Administrador.", "Operação Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                        LoadAuthorizedUsers(); // Recarrega para reverter a alteração visual
                        return;
                    }
                    if (user.IsInactiveBool)
                    {
                        MessageBox.Show("Você não pode desativar seu próprio acesso.", "Operação Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                        LoadAuthorizedUsers(); // Recarrega para reverter a alteração visual
                        return;
                    }
                }
            }

            try
            {
                _context.SaveChanges();
                MessageBox.Show("Alterações salvas com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao salvar as alterações: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadAuthorizedUsers(); // Recarrega em caso de erro para garantir consistência
            }
        }
    }
}
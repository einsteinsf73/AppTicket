using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TicketManager.WPF.Models
{
    public class UserColumnSettings
    {
        public string UserName { get; set; }
        public List<ColumnSetting> ColumnSettings { get; set; }

        public UserColumnSettings()
        {
            ColumnSettings = new List<ColumnSetting>();
        }

        public static UserColumnSettings GetDefaultSettings()
        {
            return new UserColumnSettings
            {
                ColumnSettings = new List<ColumnSetting>
                {
                    new ColumnSetting { Name = "Id", Header = "ID", IsVisible = true, DisplayIndex = 0 },
                    new ColumnSetting { Name = "Title", Header = "Título", IsVisible = true, DisplayIndex = 1 },
                    new ColumnSetting { Name = "Status", Header = "Status", IsVisible = true, DisplayIndex = 2 },
                    new ColumnSetting { Name = "Priority", Header = "Prioridade", IsVisible = true, DisplayIndex = 3 },
                    new ColumnSetting { Name = "SlaMinutes", Header = "SLA (min est.)", IsVisible = true, DisplayIndex = 4 },
                    new ColumnSetting { Name = "SLAFinal", Header = "SLA Final (min)", IsVisible = true, DisplayIndex = 5 },
                    new ColumnSetting { Name = "CreatedAt", Header = "Data de Criação", IsVisible = true, DisplayIndex = 6 },
                    new ColumnSetting { Name = "UpdatedAt", Header = "Última Atualização", IsVisible = true, DisplayIndex = 7 },
                    new ColumnSetting { Name = "CreatedByWindowsUser", Header = "Usuário", IsVisible = true, DisplayIndex = 8 },
                    new ColumnSetting { Name = "CreatedByHostname", Header = "Hostname", IsVisible = true, DisplayIndex = 9 }
                }
            };
        }
    }


}
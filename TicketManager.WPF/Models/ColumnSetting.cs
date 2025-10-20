using System;

namespace TicketManager.WPF.Models
{
    public class ColumnSetting : ICloneable
    {
        public string Name { get; set; }
        public string Header { get; set; }
        public bool IsVisible { get; set; }
        public int DisplayIndex { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
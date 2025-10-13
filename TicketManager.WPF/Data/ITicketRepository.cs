using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManager.WPF.Models;

namespace TicketManager.WPF.Data
{
    public interface ITicketRepository
    {
        Task<Ticket> GetByIdAsync(int id);
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task AddAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task DeleteAsync(int id);
        Task<int> SaveChangesAsync();
    }
}

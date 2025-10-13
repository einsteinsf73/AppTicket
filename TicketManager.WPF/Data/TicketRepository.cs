using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using TicketManager.WPF.Models;

namespace TicketManager.WPF.Data
{
    public class TicketRepository : ITicketRepository
    {
        private readonly TicketContext _context;

        public TicketRepository(TicketContext context)
        {
            _context = context;
        }

        public async Task<Ticket> GetByIdAsync(int id)
        {
            return await _context.Tickets.FindAsync(id);
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await _context.Tickets.ToListAsync();
        }

        public async Task AddAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
        }

        public Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var ticket = await GetByIdAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Trata a exceção de concorrência
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is Ticket)
                    {
                        var databaseValues = await entry.GetDatabaseValuesAsync();

                        if (databaseValues == null)
                        {
                            MessageBox.Show("O ticket que você está tentando salvar foi excluído por outro usuário.", "Conflito de Concorrência", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show("O ticket que você está tentando salvar foi modificado por outro usuário. Suas alterações não foram salvas. Por favor, recarregue os dados.", "Conflito de Concorrência", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                // Retorna 0 para indicar que nenhuma alteração foi salva devido ao conflito
                return 0;
            }
        }
    }
}


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TicketManager.WPF.Models;

namespace TicketManager.WPF.Data
{
    public class TicketContext : DbContext
    {
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketLog> TicketLogs { get; set; }
        public DbSet<AuthorizedUser> AuthorizedUsers { get; set; }
        public DbSet<ReopeningLog> ReopeningLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseOracle(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeamento explícito para Oracle, se necessário (ex: nomes de tabelas)
            modelBuilder.Entity<Ticket>().ToTable("TICKETS");
            modelBuilder.Entity<TicketLog>().ToTable("TICKET_LOGS");
            modelBuilder.Entity<AuthorizedUser>().ToTable("AUTHORIZED_USERS");
            modelBuilder.Entity<ReopeningLog>().ToTable("REOPENING_LOGS");
        }
    }
}


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
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

            // Mapeamento explícito de tabelas para Oracle
            modelBuilder.Entity<Ticket>().ToTable("TICKETS");
            modelBuilder.Entity<TicketLog>().ToTable("TICKET_LOGS");
            modelBuilder.Entity<AuthorizedUser>().ToTable("AUTHORIZED_USERS");
            modelBuilder.Entity<ReopeningLog>().ToTable("HIST_REABERTURA");

            // Adiciona comentários nas colunas de enum para documentação no BD
            modelBuilder.Entity<Ticket>()
                .Property(t => t.Status)
                .HasComment("0=Aberto, 1=EmAndamento, 2=Resolvido, 3=Fechado");

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Priority)
                .HasComment("0=Baixa, 1=Media, 2=Alta");

            // Converte todos os nomes de colunas para o padrão UPPER_SNAKE_CASE
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    var columnName = Regex.Replace(property.Name, @"([a-z0-9])([A-Z])", "$1_$2").ToUpper();
                    property.SetColumnName(columnName);
                }
            }
        }
    }
}

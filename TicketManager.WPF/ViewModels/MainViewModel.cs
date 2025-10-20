using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TicketManager.WPF.Data;
using TicketManager.WPF.Models;

namespace TicketManager.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ITicketRepository _ticketRepository;

        public ObservableCollection<Ticket> Tickets { get; } = new ObservableCollection<Ticket>();

        private Ticket _selectedTicket;
        public Ticket SelectedTicket
        {
            get => _selectedTicket;
            set => SetProperty(ref _selectedTicket, value);
        }

        public MainViewModel(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task LoadTicketsAsync()
        {
            var tickets = await _ticketRepository.GetAllTicketsAsync();
            Tickets.Clear();
            foreach (var ticket in tickets)
            {
                Tickets.Add(ticket);
            }
        }
    }
}

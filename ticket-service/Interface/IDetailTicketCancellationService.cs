using ticket_service.DTO;

namespace ticket_service.Interface
{
    public interface IDetailTicketCancellationService
    {
        Task CreateDetailTicketCancellation(detail_ticket_cancellation_period_DTO detail);
        Task<string> getStatusTicketCancellation(int eventId, string uid);
        Task<List<Deatail_cancel_reponse>> GetDetailCancelAsync(int eventId, string status);
    }
}

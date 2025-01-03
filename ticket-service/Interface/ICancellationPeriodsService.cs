using ticket_service.Model;

namespace ticket_service.Interface
{
    public interface ICancellationPeriodsService
    {
        Task CreateCancellationPeriods(ticket_cancellation_period period);
        Task<ticket_cancellation_period> GetPeriod(int EventId);
        Task update(ticket_cancellation_period period);
    }
}

using event_service.Model;

namespace event_service.Interface
{
    public interface ISpendingService
    {
        Task<List<Spending>> GetSpendingsAsyncByEventId(int eventId);
        Task<Spending> AddSpendingAsync(int eventId, string category, double amount, string type);
        Task<Spending> UpdateSpendingAsync(int id, double amount, string category);
        Task<bool> RemoveSpendingAsync(int eventId, int id);
    }
}

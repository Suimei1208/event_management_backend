using event_service.DTO;
using event_service.Interface;

namespace event_service.Service
{
    public class ParticipantsService : IParticipantsService
    {
        private readonly EventDbContext _context;

        public ParticipantsService(EventDbContext context)
        {
            _context = context;
        }

        public async Task AddParticipants(List<ParticipantsDto> participantsDtos)
        {
            var participants = participantsDtos.Select(p => p.ToEntity()).ToList();  
            await _context.Participants.AddRangeAsync(participants); 
            await _context.SaveChangesAsync(); 
        }
    }
}

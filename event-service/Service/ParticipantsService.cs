using event_service.DTO;
using event_service.Interface;
using Microsoft.EntityFrameworkCore;

namespace event_service.Service
{
    public class ParticipantsService : IParticipantsService
    {
        private readonly EventDbContext _context;
        private readonly IKafkaProducerService _kafkaProducerService;

        public ParticipantsService(EventDbContext context, IKafkaProducerService kafkaProducerService)
        {
            _context = context;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task AddParticipants(List<ParticipantsDto> participantsDtos)
        {
            var participants = participantsDtos.Select(p => p.ToEntity()).ToList();
            await _context.Participants.AddRangeAsync(participants);
            await _context.SaveChangesAsync();
        }

        public async Task GetParticipants(int eventId)
        {
            var participants = await _context.Participants
       .Where(e => e.eventId == eventId)
       .ToListAsync();
            var participantsDtos = participants.Select(p => p.ToDto()).ToList();
            await _kafkaProducerService.SendMessageAsync(participantsDtos);
        }
    }
}

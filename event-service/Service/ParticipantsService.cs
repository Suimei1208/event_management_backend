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

        public async Task<List<object>> getEventRegisterPending(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                return null;
            }

            List<object> result = new List<object>();

            try
            {
                var list = await _context.Participants.Where(u => u.userId == uid.ToString()).ToArrayAsync();

                if (list.Length == 0)
                {
                    return result;
                }

                foreach (var i in list)
                {
                    result.Add(new
                    {
                        id = i.eventId.ToString(),
                        status = i.status.ToString()
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task UnregisterEvent(string eventid, string uid) {
            var result = _context.Participants.FirstOrDefault(u => u.userId == uid && u.eventId == int.Parse(eventid));
            _context.Participants.Remove(result);
            await _context.SaveChangesAsync();
        }

    }
}

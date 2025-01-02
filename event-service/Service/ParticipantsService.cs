using event_service.DTO;
using event_service.Interface;
using event_service.Model;
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
                throw new ArgumentException("UID cannot be null or empty.", nameof(uid));
            }

            try
            {
                var participants = await _context.Participants
                                                 .Where(p => p.userId == uid)
                                                 .ToArrayAsync();

                if (participants.Length == 0)
                {
                    return new List<object>(); // Return an empty list if no participants are found
                }

                return participants.Select(p => new
                {
                    id = p.eventId.ToString(),
                    status = p.status.ToString()
                }).ToList<object>();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving participant data.", ex);
            }
        }


        public async Task UnregisterEvent(string eventid, string uid) {
            var result = _context.Participants.FirstOrDefault(u => u.userId == uid && u.eventId == int.Parse(eventid));
            _context.Participants.Remove(result);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Participants>> AddParticipantsFromExcelAsync(int eventId, List<string> userIds)
        {
            var existingUserIds = await _context.Participants
                .Where(p => p.eventId == eventId)
                .Select(p => p.userId)
                .ToListAsync();

            var newUserIds = userIds.Where(userId => !existingUserIds.Contains(userId)).ToList();

            var participants = newUserIds.Select(userId => new Participants
            {
                userId = userId,
                eventId = eventId,
                registration_Date = DateTime.Now,
                status = "Added",
                role = "Participant",
                EmailSent = false
            }).ToList();

            if (participants.Any())
            {
                await _context.Participants.AddRangeAsync(participants);
                await _context.SaveChangesAsync();
            }

            return participants;
        }

    }
}

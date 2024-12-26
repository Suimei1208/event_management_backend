using Confluent.Kafka;
using event_service.DTO;
using event_service.Interface;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;

namespace event_service.Service
{
    public class Notification_service: INotification
    {
        private readonly FirebaseApp _firebaseApp;

        public Notification_service(FirebaseApp firebaseApp)
        {
            _firebaseApp = firebaseApp;
        }
        public async Task<string> SendNotification(string title, string body, string topic)
        {
            if (FirebaseMessaging.DefaultInstance == null)
            {
                throw new InvalidOperationException("FirebaseMessaging.DefaultInstance chưa được khởi tạo.");
            }

            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Topic = topic,
                //Token = "chfqDg5sS1C_BFfJKJYC8m:APA91bGemMk2eaVHM87TvC7Qcfkg00mS0UTOK7695HMlKksuS_RdyDajPtMohan1I4h6HJGs6SYSNsJI6naGH8U28-XUzt5jfmOKQS12d2czZXmoQ7uPOmw"

            };

            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            Console.WriteLine("Successfully sent message: " + response);
            return response;
        }    

    }
}

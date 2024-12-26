using FirebaseAdmin.Auth;

namespace event_service.Service
{
    public class FirebaseService
    {
        private readonly FirebaseAuth _firebaseAuth;

        public FirebaseService(FirebaseAuth firebaseAuth)
        {
            _firebaseAuth = firebaseAuth;
        }

        // Phương thức lấy email của người dùng từ Firebase
        public async Task<string> GetUserEmailAsync(string userId)
        {
            try
            {
                var user = await _firebaseAuth.GetUserAsync(userId);
                return user.Email;
            }
            catch (FirebaseAuthException ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin người dùng {userId}: {ex.Message}");
                return null;
            }
        }
    }
}

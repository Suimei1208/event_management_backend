using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder.Extensions;
using user_services.Interface;

namespace user_services.Services
{
    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly FirebaseApp _firebaseApp;
        private readonly FirebaseAuth _firebaseAuth;

        public FirebaseAuthService(FirebaseApp firebaseApp)
        {
            _firebaseApp = firebaseApp;
            _firebaseAuth = FirebaseAuth.GetAuth(firebaseApp); 
        }

        public async Task<FirebaseToken> VerifyTokenAsync(string idToken)
        {
            try
            {
                var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(idToken);
                return decodedToken;
            }
            catch (Exception e)
            {
                throw new Exception($"Token verification failed: {e.Message}");
            }
        }
        public async Task<string> GetUserPhotoUrl(string uid)
        {
            try
            {
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);

                string photoUrl = userRecord.PhotoUrl;
                return photoUrl;
            }
            catch (FirebaseAuthException ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin người dùng: {ex.Message}");
                return null;
            }
        }
    }
}

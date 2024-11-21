using FirebaseAdmin.Auth;

namespace user_services.Interface
{
    public interface IFirebaseAuthService
    {
        Task<FirebaseToken> VerifyTokenAsync(string idToken);
    }

}

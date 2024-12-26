

namespace E_commerce_Back_end.OPT
{
    public interface IEmailService
    {    
        void SendEmail(EmailDto request, string email);
    }
}

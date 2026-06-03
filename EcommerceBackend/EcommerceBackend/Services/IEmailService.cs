namespace EcommerceBackend.Services;
public interface IEmailService
{
    void SendOtpEmail(string toEmail, string otp);
}

using System.Net.Mail;
using EcommerceBackend.Exceptions;

namespace EcommerceBackend.Services;
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    public void SendOtpEmail(string toEmail, string otp)
    {
        try
        {
            var smtpServer = _configuration["Smtp:Server"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
            var fromEmail = _configuration["Smtp:FromEmail"];
            var password = _configuration["Smtp:Password"];

            // Email content with HTML layout
            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail!, "Ecommerce Admin"),
                Subject = "Your OTP for Password Reset",
                Body = $@"
                    <div style='font-family: Arial, sans-serif; border: 1px solid #ddd; padding: 20px; max-width: 500px;'>
                        <h2 style='color: #333;'>Password Reset Request</h2>
                        <p>Bhai, aapne password reset ke liye request kiya tha. Aapka security OTP niche diya gaya hai:</p>
                        <div style='background-color: #f4f4f4; padding: 10px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; color: #007bff; border-radius: 5px;'>
                            {otp}
                        </div>
                        <p style='color: #666; font-size: 12px; margin-top: 20px;'>Yeh OTP sirf 5 minutes tak valid hai. Agar aapne ye request nahi ki thi, toh tension mat lo, ise ignore karo.</p>
                    </div>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            // SMTP client setup
            using var smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new System.Net.NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            // send email
            smtpClient.Send(mailMessage);
            _logger.LogInformation("Email sent successfully via SMTP");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email via SMTP");
            throw new BadRequestException("Failed to send email.");
        }
    }
}

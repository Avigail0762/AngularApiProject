namespace server.Bll.Interfaces
{
    public interface IEmailService
    {
        Task SendWinnerEmailAsync(string toEmail, string giftName);
    }
}

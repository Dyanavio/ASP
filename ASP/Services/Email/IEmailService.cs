namespace ASP.Services.Email
{
    public interface IEmailService
    {
        public abstract void Send(string to, string subject, string content);
    }
}

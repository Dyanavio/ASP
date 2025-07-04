namespace ASP.Services.Random
{
    public interface IRandomService
    {
        public abstract string Otp(int length = 4); // One time password
    }
}

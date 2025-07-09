namespace ASP.Services.Kdf
{
    public interface IKdfService
    {
        public abstract string Dk(string password, string salt);
    }
}

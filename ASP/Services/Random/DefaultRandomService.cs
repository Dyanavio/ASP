using ASP.Services.Time;

namespace ASP.Services.Random
{
    public class DefaultRandomService : IRandomService
    {
        private readonly System.Random random;
        public DefaultRandomService(ITimeService timeService)
        {
            random = new System.Random((int)timeService.Timestamp());
        }
        public string Otp(int length)
        {
            return (string.Join("", from i in Enumerable.Range(0, length) select random.Next(0, random.Next(10))));
            //return string.Join("", Enumerable.Range(0, length).Select(_ => random.Next(10)));
        }
    }
}

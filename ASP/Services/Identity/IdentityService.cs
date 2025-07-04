using ASP.Services.Time;

namespace ASP.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly ITimeService timeService;
        private long lastTimestamp;
        private int count = 0;
        public IdentityService(ITimeService timeService)
        {
            this.timeService = timeService;
            lastTimestamp = this.timeService.Timestamp();
        }
        public long GenerateId()
        {
            long timestamp = timeService.Timestamp();
            if(timestamp != lastTimestamp)
            {
                lastTimestamp = timestamp;
                count = 0;
            }
            char[] reversed = timestamp.ToString().ToCharArray();
            Console.WriteLine("Original timestamp: " + new string(reversed));
            Array.Reverse(reversed);
            return (long)Convert.ToDouble(new string(reversed) + count++);
        }
    }
}

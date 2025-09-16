namespace RepoGateway.Configuration
{
    public class RateLimitConfig
    {
        public int RateWindowsMinutes { get; set; } = 1;

        public int MaxRequestsPerMinute { get; set;} = 10;
    }
}

namespace DotNetBack.Models.DTOs
{
    public class UpdateSubscriptionRequest
    {
        public int UserId { get; set; }
        public string Subscription { get; set; }
        public DateTime SubscriptionPeriod { get; set; }
    }
}

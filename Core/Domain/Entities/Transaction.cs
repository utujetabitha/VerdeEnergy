namespace Core.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int? TripId { get; set; }
        public string? OwnerUserId { get; set; }
        public string UserId { get; set; }
        public string Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentReference { get; set; }
        public string TransactionStatus { get; set; }
        public string ServiceType { get; set; }

    }
}
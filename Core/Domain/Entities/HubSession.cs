namespace Core.Domain.Entities
{
    public class HubSession
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string StationId { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public int HubBayId { get; set; }
        public string InitialChargeLevel { get; set; }
        public string CurrentChargeLevel { get; set; } = string.Empty;
        public string FinalChargeLevel { get; set; }
        public string KwDelivered { get; set; }
        public string EnergyCost { get; set; }
        public DateTime ChargingDuration { get; set; }
    }
}
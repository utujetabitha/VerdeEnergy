namespace Core.Domain.Entities
{
    public class InstallationQuote
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhoneNumber { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string SupplyType { get; set; }
        public string DistanceToPanelMeter { get; set; }
        public string PanelPhotoUrl { get; set; }
        public string AssignedEngineerId { get; set; }
        public string EstimatedCost { get; set; }
        public string Status { get; set; }
    }
}
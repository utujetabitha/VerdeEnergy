namespace Core.Domain.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerPhoneNumber { get; set; } = string.Empty;
        public string MakeModel { get; set; } = string.Empty;
        public string LicensePlate { get; set; }
        public string BatteryCapacity { get; set; }
        public string PreferredChargeTarget { get; set; }
    }
}
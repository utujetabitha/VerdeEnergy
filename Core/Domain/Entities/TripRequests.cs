using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class TripRequest
    {
        public int Id { get; set; }
        public int? VehicleId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhoneNumber { get; set; } = string.Empty;
        public string? CustomerUserId { get; set; }
        public string AssignedDriverName { get; set; } = string.Empty;
        public string DriverPhoneNumber { get; set; } = string.Empty;
        public string PickupLocation { get; set; }
        public string ReturnLocation { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime ReturnTime { get; set; }
        public string Status { get; set; }
        public string HandoverConditionNotes { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string StationId { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string VehicleDescription { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public int InitialChargePercent { get; set; }
        public int RequestedEnergyPercent { get; set; }
        public decimal RequestedEnergyKwh { get; set; }
        public string EstimatedCost { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentReference { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentProofFileName { get; set; } = string.Empty;
        public string PaymentProofStorageName { get; set; } = string.Empty;
        [NotMapped] public string? PaymentProofDataUrl { get; set; }
    }
}
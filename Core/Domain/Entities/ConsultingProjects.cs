namespace Core.Domain.Entities
{
    public class ConsultingProject
    {
        public int Id { get; set; }
        public string CorporateClientName { get; set; } = string.Empty;
        public string CorporateClientPhoneNumber { get; set; } = string.Empty;
        public string ServiceCategory { get; set; } = string.Empty;
        public string ContractValue { get; set; }
        public DateTime CompletionDate { get; set; }
        public string AuditReport { get; set; }
    }
}
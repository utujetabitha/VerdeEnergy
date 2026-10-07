using System.Globalization;
using Core.Domain.Entities;

namespace WebUI.Services;

public sealed class OperationsService
{
    public OperationsService(IConfiguration configuration)
    {
        Stations = configuration.GetSection("ChargingStations").Get<List<ChargingStation>>() ?? [];
    }

    public List<ChargingStation> Stations { get; }
    public static IReadOnlyList<string> VehicleModels { get; } =
    [
        "BYD Atto 3", "BYD Dolphin", "BYD e2", "Hyundai Ioniq 5", "Hyundai Kona Electric",
        "Kia Niro EV", "Nissan Leaf", "Renault Zoe", "Tesla Model 3", "Tesla Model Y",
        "Volkswagen ID.4"
    ];
    public List<Vehicle> Vehicles { get; } = [];
    public List<TripRequest> Trips { get; } = [];
    public List<InstallationQuote> Quotes { get; } = [];
    public List<Transaction> Transactions { get; } = [];
    public List<ConsultingProject> ConsultingProjects { get; } = [];
    public List<HubSession> HubSessions { get; } = [];
    public List<QueuedSms> PendingSms { get; } = [];

    public string GetTransactionTotal(string status)
    {
        var total = Transactions
            .Where(transaction => string.Equals(transaction.TransactionStatus, status, StringComparison.OrdinalIgnoreCase))
            .Sum(transaction => ParseAmount(transaction.Amount));
        return $"FRW {total.ToString("N0", CultureInfo.InvariantCulture)}";
    }

    public int GetTransactionCount(string status) => Transactions.Count(
        transaction => string.Equals(transaction.TransactionStatus, status, StringComparison.OrdinalIgnoreCase));

    public string FormatCurrency(string? amount)
    {
        var valueText = amount?.Trim();
        if (valueText?.StartsWith("FRW", StringComparison.OrdinalIgnoreCase) == true)
        {
            valueText = valueText[3..].Trim();
        }

        return decimal.TryParse(valueText, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
            ? $"FRW {value.ToString("N0", CultureInfo.InvariantCulture)}"
            : string.IsNullOrWhiteSpace(amount) ? string.Empty : $"FRW {amount.Trim()}";
    }

    private static decimal ParseAmount(string? amount)
    {
        var value = amount?.Trim();
        if (value?.StartsWith("FRW", StringComparison.OrdinalIgnoreCase) == true)
        {
            value = value[3..].Trim();
        }

        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result) ? result : 0;
    }

    public void AddVehicle(Vehicle vehicle) => Vehicles.Add(vehicle);
    public void AddTrip(TripRequest trip) => Trips.Add(trip);
    public void AddTransaction(Transaction transaction)
    {
        if (transaction.Id == 0)
        {
            transaction.Id = Transactions.Count == 0 ? 1 : Transactions.Max(item => item.Id) + 1;
        }
        Transactions.Add(transaction);
    }
    public void QueueSms(QueuedSms message)
    {
        message.Id = PendingSms.Count == 0 ? 1 : PendingSms.Max(item => item.Id) + 1;
        PendingSms.Add(message);
    }

    public void RemoveQueuedSms(QueuedSms message) => PendingSms.Remove(message);

    public void AddQuote(InstallationQuote quote)
    {
        quote.EstimatedCost = FormatCurrency(quote.EstimatedCost);
        Quotes.Add(quote);
    }

    public void AddConsultingProject(ConsultingProject project)
    {
        project.ContractValue = FormatCurrency(project.ContractValue);
        ConsultingProjects.Add(project);
    }
}

public sealed class ChargingStation
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int AvailableBays { get; set; }
    public string Connector { get; set; } = string.Empty;
    public string Availability { get; set; } = "Availability not live";
}

public sealed class QueuedSms
{
    public int Id { get; set; }
    public int TripId { get; init; }
    public string RecipientName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string LastError { get; set; } = string.Empty;
}

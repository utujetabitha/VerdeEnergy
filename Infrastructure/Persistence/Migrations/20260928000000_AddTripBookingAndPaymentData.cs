using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
	[DbContext(typeof(VerdeEnergyDbContext))]
	[Migration("20260928000000_AddTripBookingAndPaymentData")]
	public partial class AddTripBookingAndPaymentData : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(name: "ServiceType", table: "TripRequests", type: "nvarchar(max)", nullable: false, defaultValue: "");
			migrationBuilder.AddColumn<string>(name: "StationId", table: "TripRequests", type: "nvarchar(max)", nullable: false, defaultValue: "");
			migrationBuilder.AddColumn<string>(name: "StationName", table: "TripRequests", type: "nvarchar(max)", nullable: false, defaultValue: "");
			migrationBuilder.AddColumn<string>(name: "VehicleDescription", table: "TripRequests", type: "nvarchar(max)", nullable: false, defaultValue: "");
			migrationBuilder.AddColumn<string>(name: "LicensePlate", table: "TripRequests", type: "nvarchar(max)", nullable: false, defaultValue: "");
			migrationBuilder.AddColumn<int>(name: "InitialChargePercent", table: "TripRequests", type: "int", nullable: false, defaultValue: 0);
			migrationBuilder.AddColumn<decimal>(name: "RequestedEnergyKwh", table: "TripRequests", type: "decimal(18,2)", nullable: false, defaultValue: 0m);
			migrationBuilder.AddColumn<string>(name: "EstimatedCost", table: "TripRequests", type: "nvarchar(max)", nullable: false, defaultValue: "");
			migrationBuilder.AddColumn<string>(name: "PaymentMethod", table: "TripRequests", type: "nvarchar(max)", nullable: false, defaultValue: "");
			migrationBuilder.AddColumn<string>(name: "PaymentReference", table: "TripRequests", type: "nvarchar(max)", nullable: false, defaultValue: "");
			migrationBuilder.AddColumn<string>(name: "PaymentStatus", table: "TripRequests", type: "nvarchar(max)", nullable: false, defaultValue: "");
			migrationBuilder.AddColumn<string>(name: "PaymentProofFileName", table: "TripRequests", type: "nvarchar(max)", nullable: false, defaultValue: "");
			migrationBuilder.AddColumn<string>(name: "PaymentProofStorageName", table: "TripRequests", type: "nvarchar(max)", nullable: false, defaultValue: "");
			migrationBuilder.AddColumn<int>(name: "TripId", table: "Transactions", type: "int", nullable: true);

			migrationBuilder.AddColumn<string>(name: "StationId", table: "HubSessions", type: "nvarchar(max)", nullable: false, defaultValue: "");
			migrationBuilder.AddColumn<string>(name: "StationName", table: "HubSessions", type: "nvarchar(max)", nullable: false, defaultValue: "");
			migrationBuilder.AddColumn<string>(name: "CurrentChargeLevel", table: "HubSessions", type: "nvarchar(max)", nullable: false, defaultValue: "");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(name: "ServiceType", table: "TripRequests");
			migrationBuilder.DropColumn(name: "StationId", table: "TripRequests");
			migrationBuilder.DropColumn(name: "StationName", table: "TripRequests");
			migrationBuilder.DropColumn(name: "VehicleDescription", table: "TripRequests");
			migrationBuilder.DropColumn(name: "LicensePlate", table: "TripRequests");
			migrationBuilder.DropColumn(name: "InitialChargePercent", table: "TripRequests");
			migrationBuilder.DropColumn(name: "RequestedEnergyKwh", table: "TripRequests");
			migrationBuilder.DropColumn(name: "EstimatedCost", table: "TripRequests");
			migrationBuilder.DropColumn(name: "PaymentMethod", table: "TripRequests");
			migrationBuilder.DropColumn(name: "PaymentReference", table: "TripRequests");
			migrationBuilder.DropColumn(name: "PaymentStatus", table: "TripRequests");
			migrationBuilder.DropColumn(name: "PaymentProofFileName", table: "TripRequests");
			migrationBuilder.DropColumn(name: "PaymentProofStorageName", table: "TripRequests");
			migrationBuilder.DropColumn(name: "TripId", table: "Transactions");
			migrationBuilder.DropColumn(name: "StationId", table: "HubSessions");
			migrationBuilder.DropColumn(name: "StationName", table: "HubSessions");
			migrationBuilder.DropColumn(name: "CurrentChargeLevel", table: "HubSessions");
		}
	}
}
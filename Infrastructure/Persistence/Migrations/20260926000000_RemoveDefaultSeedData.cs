using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(VerdeEnergyDbContext))]
    [Migration("20260926000000_RemoveDefaultSeedData")]
    public partial class RemoveDefaultSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("ConsultingProjects", "Id", 1);
            migrationBuilder.DeleteData("ConsultingProjects", "Id", 2);
            migrationBuilder.DeleteData("HubSessions", "Id", 1);
            migrationBuilder.DeleteData("HubSessions", "Id", 2);
            migrationBuilder.DeleteData("InstallationQuotes", "Id", 1);
            migrationBuilder.DeleteData("InstallationQuotes", "Id", 2);
            migrationBuilder.DeleteData("InstallationQuotes", "Id", 3);
            migrationBuilder.DeleteData("Transactions", "Id", 1);
            migrationBuilder.DeleteData("Transactions", "Id", 2);
            migrationBuilder.DeleteData("Transactions", "Id", 3);
            migrationBuilder.DeleteData("TripRequests", "Id", 1);
            migrationBuilder.DeleteData("TripRequests", "Id", 2);
            migrationBuilder.DeleteData("TripRequests", "Id", 3);
            migrationBuilder.DeleteData("Users", "Id", 1);
            migrationBuilder.DeleteData("Users", "Id", 2);
            migrationBuilder.DeleteData("Users", "Id", 3);
            migrationBuilder.DeleteData("Vehicles", "Id", 1);
            migrationBuilder.DeleteData("Vehicles", "Id", 2);
            migrationBuilder.DeleteData("Vehicles", "Id", 3);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
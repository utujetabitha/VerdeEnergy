using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
	[DbContext(typeof(VerdeEnergyDbContext))]
	[Migration("20261006000000_AddRequestedEnergyPercent")]
	public partial class AddRequestedEnergyPercent : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "RequestedEnergyPercent",
				table: "TripRequests",
				type: "int",
				nullable: false,
				defaultValue: 0);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "RequestedEnergyPercent",
				table: "TripRequests");
		}
	}
}
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
	[DbContext(typeof(VerdeEnergyDbContext))]
	[Migration("20260928000002_AddLegacyTripAccountFields")]
	public partial class AddLegacyTripAccountFields : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("IF COL_LENGTH(N'dbo.TripRequests', N'CustomerName') IS NULL ALTER TABLE [dbo].[TripRequests] ADD [CustomerName] nvarchar(max) NOT NULL CONSTRAINT [DF_TripRequests_CustomerName_Auth] DEFAULT (N'');");
			migrationBuilder.Sql("IF COL_LENGTH(N'dbo.TripRequests', N'CustomerPhoneNumber') IS NULL ALTER TABLE [dbo].[TripRequests] ADD [CustomerPhoneNumber] nvarchar(max) NOT NULL CONSTRAINT [DF_TripRequests_CustomerPhoneNumber_Auth] DEFAULT (N'');");
			migrationBuilder.Sql("IF COL_LENGTH(N'dbo.TripRequests', N'AssignedDriverName') IS NULL ALTER TABLE [dbo].[TripRequests] ADD [AssignedDriverName] nvarchar(max) NOT NULL CONSTRAINT [DF_TripRequests_AssignedDriverName_Auth] DEFAULT (N'');");
			migrationBuilder.Sql("IF COL_LENGTH(N'dbo.TripRequests', N'DriverPhoneNumber') IS NULL ALTER TABLE [dbo].[TripRequests] ADD [DriverPhoneNumber] nvarchar(max) NOT NULL CONSTRAINT [DF_TripRequests_DriverPhoneNumber_Auth] DEFAULT (N'');");
			migrationBuilder.AddColumn<string>(name: "OwnerUserId", table: "Transactions", type: "nvarchar(450)", maxLength: 450, nullable: true);
			migrationBuilder.CreateIndex(name: "IX_Transactions_OwnerUserId", table: "Transactions", column: "OwnerUserId");
			migrationBuilder.AddForeignKey(name: "FK_Transactions_AspNetUsers_OwnerUserId", table: "Transactions", column: "OwnerUserId", principalTable: "AspNetUsers", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey("FK_Transactions_AspNetUsers_OwnerUserId", "Transactions");
			migrationBuilder.DropIndex("IX_Transactions_OwnerUserId", "Transactions");
			migrationBuilder.DropColumn("OwnerUserId", "Transactions");
		}
	}
}
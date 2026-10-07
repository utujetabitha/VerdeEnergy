using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
	[DbContext(typeof(VerdeEnergyDbContext))]
	[Migration("20260928000001_AddIdentityAndTripOwnership")]
	public partial class AddIdentityAndTripOwnership : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("IF COL_LENGTH(N'dbo.TripRequests', N'CustomerId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID(N'dbo.TripRequests') AND c.name = N'CustomerId') ALTER TABLE [dbo].[TripRequests] ADD CONSTRAINT [DF_TripRequests_CustomerId_Identity] DEFAULT (0) FOR [CustomerId];");
			migrationBuilder.Sql("IF COL_LENGTH(N'dbo.TripRequests', N'AssignedDriverId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID(N'dbo.TripRequests') AND c.name = N'AssignedDriverId') ALTER TABLE [dbo].[TripRequests] ADD CONSTRAINT [DF_TripRequests_AssignedDriverId_Identity] DEFAULT (0) FOR [AssignedDriverId];");

			migrationBuilder.CreateTable(
				name: "AspNetRoles",
				columns: table => new
				{
					Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table => table.PrimaryKey("PK_AspNetRoles", x => x.Id));

			migrationBuilder.CreateTable(
				name: "AspNetUsers",
				columns: table => new
				{
					Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
					AccessFailedCount = table.Column<int>(type: "int", nullable: false),
					ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
					FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
					LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
					SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
					TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
					UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
				},
				constraints: table => table.PrimaryKey("PK_AspNetUsers", x => x.Id));

			migrationBuilder.CreateTable(
				name: "AspNetRoleClaims",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
					RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
					table.ForeignKey("FK_AspNetRoleClaims_AspNetRoles_RoleId", x => x.RoleId, "AspNetRoles", "Id", onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserClaims",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
					table.ForeignKey("FK_AspNetUserClaims_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserLogins",
				columns: table => new
				{
					LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
					ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
					ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
					table.ForeignKey("FK_AspNetUserLogins_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserRoles",
				columns: table => new
				{
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
					table.ForeignKey("FK_AspNetUserRoles_AspNetRoles_RoleId", x => x.RoleId, "AspNetRoles", "Id", onDelete: ReferentialAction.Cascade);
					table.ForeignKey("FK_AspNetUserRoles_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserTokens",
				columns: table => new
				{
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
					Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
					Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
					table.ForeignKey("FK_AspNetUserTokens_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex("IX_AspNetRoleClaims_RoleId", "AspNetRoleClaims", "RoleId");
			migrationBuilder.CreateIndex("RoleNameIndex", "AspNetRoles", "NormalizedName", unique: true, filter: "[NormalizedName] IS NOT NULL");
			migrationBuilder.CreateIndex("IX_AspNetUserClaims_UserId", "AspNetUserClaims", "UserId");
			migrationBuilder.CreateIndex("IX_AspNetUserLogins_UserId", "AspNetUserLogins", "UserId");
			migrationBuilder.CreateIndex("IX_AspNetUserRoles_RoleId", "AspNetUserRoles", "RoleId");
			migrationBuilder.CreateIndex("EmailIndex", "AspNetUsers", "NormalizedEmail");
			migrationBuilder.CreateIndex("UserNameIndex", "AspNetUsers", "NormalizedUserName", unique: true, filter: "[NormalizedUserName] IS NOT NULL");

			migrationBuilder.AddColumn<string>(name: "CustomerUserId", table: "TripRequests", type: "nvarchar(450)", maxLength: 450, nullable: true);
			migrationBuilder.CreateIndex("IX_TripRequests_CustomerUserId", "TripRequests", "CustomerUserId");
			migrationBuilder.AddForeignKey("FK_TripRequests_AspNetUsers_CustomerUserId", "TripRequests", "CustomerUserId", "AspNetUsers", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("IF OBJECT_ID(N'dbo.DF_TripRequests_CustomerId_Identity', N'D') IS NOT NULL ALTER TABLE [dbo].[TripRequests] DROP CONSTRAINT [DF_TripRequests_CustomerId_Identity];");
			migrationBuilder.Sql("IF OBJECT_ID(N'dbo.DF_TripRequests_AssignedDriverId_Identity', N'D') IS NOT NULL ALTER TABLE [dbo].[TripRequests] DROP CONSTRAINT [DF_TripRequests_AssignedDriverId_Identity];");
			migrationBuilder.DropForeignKey("FK_TripRequests_AspNetUsers_CustomerUserId", "TripRequests");
			migrationBuilder.DropIndex("IX_TripRequests_CustomerUserId", "TripRequests");
			migrationBuilder.DropColumn("CustomerUserId", "TripRequests");
			migrationBuilder.DropTable("AspNetRoleClaims");
			migrationBuilder.DropTable("AspNetUserClaims");
			migrationBuilder.DropTable("AspNetUserLogins");
			migrationBuilder.DropTable("AspNetUserRoles");
			migrationBuilder.DropTable("AspNetUserTokens");
			migrationBuilder.DropTable("AspNetRoles");
			migrationBuilder.DropTable("AspNetUsers");
		}
	}
}
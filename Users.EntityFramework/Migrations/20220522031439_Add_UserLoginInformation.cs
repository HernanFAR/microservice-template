using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Authentications.Infrastructure.Migrations
{
    public partial class Add_UserLoginInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLoginInformation",
                schema: "Authentications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ip = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Continent = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Region = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    City = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Latitude = table.Column<long>(type: "bigint", nullable: true),
                    Longitude = table.Column<long>(type: "bigint", nullable: true),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLoginInformation_ApplicationUser_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "Authentications",
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginInformation_ApplicationUserId",
                schema: "Authentications",
                table: "UserLoginInformation",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLoginInformation",
                schema: "Authentications");
        }
    }
}

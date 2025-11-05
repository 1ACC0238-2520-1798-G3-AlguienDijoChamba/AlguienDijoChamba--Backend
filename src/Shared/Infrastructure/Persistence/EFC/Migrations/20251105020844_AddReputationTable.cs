using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlguienDijoChamba.Api.src.Shared.Infrastructure.Persistence.EFC.Migrations
{
    public partial class AddReputationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                    name: "Reputations",
                    columns: table => new
                    {
                        Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                        ProfessionalId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                        Rating = table.Column<double>(type: "double", nullable: false),
                        ReviewsCount = table.Column<int>(type: "int", nullable: false),
                        Level = table.Column<string>(type: "longtext", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        HourlyRate = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Reputations", x => x.Id);
                    })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reputations");
        }
    }
}
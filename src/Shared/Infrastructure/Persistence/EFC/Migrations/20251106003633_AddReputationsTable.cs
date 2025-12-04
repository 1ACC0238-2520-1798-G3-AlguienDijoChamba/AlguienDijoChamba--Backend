using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlguienDijoChamba.Api.src.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddReputationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserReputationTechnician",
                table: "UserReputationTechnician");

            migrationBuilder.RenameTable(
                name: "UserReputationTechnician",
                newName: "Reputations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reputations",
                table: "Reputations",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Reputations",
                table: "Reputations");

            migrationBuilder.RenameTable(
                name: "Reputations",
                newName: "UserReputationTechnician");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserReputationTechnician",
                table: "UserReputationTechnician",
                column: "Id");
        }
    }
}

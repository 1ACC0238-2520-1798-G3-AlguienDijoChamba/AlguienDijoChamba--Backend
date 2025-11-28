using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlguienDijoChamba.Api.src.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerPreferencesColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_JobRequests_ProfessionalId",
                table: "JobRequests",
                column: "ProfessionalId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequests_Professionals_ProfessionalId",
                table: "JobRequests",
                column: "ProfessionalId",
                principalTable: "Professionals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobRequests_Professionals_ProfessionalId",
                table: "JobRequests");

            migrationBuilder.DropIndex(
                name: "IX_JobRequests_ProfessionalId",
                table: "JobRequests");
        }
    }
}

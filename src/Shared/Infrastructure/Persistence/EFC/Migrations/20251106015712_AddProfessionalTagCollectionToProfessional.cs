using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlguienDijoChamba.Api.src.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalTagCollectionToProfessional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalTags_Tags_TagId1",
                table: "ProfessionalTags");

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalTags_TagId1",
                table: "ProfessionalTags");

            migrationBuilder.DropColumn(
                name: "TagId1",
                table: "ProfessionalTags");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalTags_Professionals_ProfessionalId",
                table: "ProfessionalTags",
                column: "ProfessionalId",
                principalTable: "Professionals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalTags_Professionals_ProfessionalId",
                table: "ProfessionalTags");

            migrationBuilder.AddColumn<Guid>(
                name: "TagId1",
                table: "ProfessionalTags",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalTags_TagId1",
                table: "ProfessionalTags",
                column: "TagId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalTags_Tags_TagId1",
                table: "ProfessionalTags",
                column: "TagId1",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

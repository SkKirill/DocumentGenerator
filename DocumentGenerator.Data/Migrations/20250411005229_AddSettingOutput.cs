using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentGenerator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingOutput : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConfigurationId",
                table: "Layouts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConfigurationModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConfigurationType = table.Column<int>(type: "INTEGER", nullable: false),
                    PageOrientations = table.Column<int>(type: "INTEGER", nullable: true),
                    ExportFormats = table.Column<int>(type: "INTEGER", nullable: true),
                    SaveToFolder = table.Column<string>(type: "TEXT", nullable: true),
                    WatermarkFolder = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Layouts_ConfigurationId",
                table: "Layouts",
                column: "ConfigurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Layouts_ConfigurationModels_ConfigurationId",
                table: "Layouts",
                column: "ConfigurationId",
                principalTable: "ConfigurationModels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Layouts_ConfigurationModels_ConfigurationId",
                table: "Layouts");

            migrationBuilder.DropTable(
                name: "ConfigurationModels");

            migrationBuilder.DropIndex(
                name: "IX_Layouts_ConfigurationId",
                table: "Layouts");

            migrationBuilder.DropColumn(
                name: "ConfigurationId",
                table: "Layouts");
        }
    }
}

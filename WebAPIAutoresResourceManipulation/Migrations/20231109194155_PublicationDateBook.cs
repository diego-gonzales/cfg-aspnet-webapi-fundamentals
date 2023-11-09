using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIAutoresResourceManipulation.Migrations
{
    /// <inheritdoc />
    public partial class PublicationDateBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PublicationDate",
                table: "Libros",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicationDate",
                table: "Libros");
        }
    }
}

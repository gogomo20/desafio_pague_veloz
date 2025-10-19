using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DesafioPagueVeloz.Persistense.Migrations
{
    /// <inheritdoc />
    public partial class ExecutionDateOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExecutionDate",
                table: "Operation",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionDate",
                table: "Operation");
        }
    }
}

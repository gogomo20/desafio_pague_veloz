using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DesafioPagueVeloz.Persistense.Migrations
{
    /// <inheritdoc />
    public partial class ApplyIrreversibleOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReversable",
                table: "Operation",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReversable",
                table: "Operation");
        }
    }
}

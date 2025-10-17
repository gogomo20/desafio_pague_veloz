using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DesafioPagueVeloz.Persistense.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    Entity = table.Column<string>(type: "text", nullable: false),
                    OldValue = table.Column<string>(type: "jsonb", nullable: false),
                    NewValue = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "true - Active, false - Deleted"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Command = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<string>(type: "jsonb", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    ReservedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditLimit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AvaliableCredit = table.Column<decimal>(type: "numeric", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "true - Active, false - Deleted"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Operation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OperationType = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PreviousBalance = table.Column<decimal>(type: "numeric", nullable: true),
                    ResultBalance = table.Column<decimal>(type: "numeric", nullable: true),
                    ReservedAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    AvaliableCredit = table.Column<decimal>(type: "numeric", nullable: true),
                    UndoId = table.Column<Guid>(type: "uuid", nullable: true),
                    RetryCounter = table.Column<int>(type: "integer", nullable: false),
                    ErrorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "true - Active, false - Deleted"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operation_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Operation_Account_TransferId",
                        column: x => x.TransferId,
                        principalTable: "Account",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Operation_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Operation_ErrorLogs_ErrorId",
                        column: x => x.ErrorId,
                        principalTable: "ErrorLogs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Operation_Operation_UndoId",
                        column: x => x.UndoId,
                        principalTable: "Operation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_CurrencyId",
                table: "Account",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Operation_AccountId",
                table: "Operation",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Operation_CurrencyId",
                table: "Operation",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Operation_ErrorId",
                table: "Operation",
                column: "ErrorId");

            migrationBuilder.CreateIndex(
                name: "IX_Operation_TransferId",
                table: "Operation",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_Operation_UndoId",
                table: "Operation",
                column: "UndoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Operation");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "Currency");
        }
    }
}

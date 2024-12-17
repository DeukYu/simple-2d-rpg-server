using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations
{
    /// <inheritdoc />
    public partial class migrationDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "account_info",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountName = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_info", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_info",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerName = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_info", x => x.Id);
                    table.ForeignKey(
                        name: "FK_player_info_account_info_AccountId",
                        column: x => x.AccountId,
                        principalTable: "account_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_item_info",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    Slot = table.Column<int>(type: "int", nullable: false),
                    Equipped = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_item_info", x => x.Id);
                    table.ForeignKey(
                        name: "FK_player_item_info_player_info_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "player_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_stat_info",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Hp = table.Column<int>(type: "int", nullable: false),
                    MaxHp = table.Column<int>(type: "int", nullable: false),
                    Mp = table.Column<int>(type: "int", nullable: false),
                    MaxMp = table.Column<int>(type: "int", nullable: false),
                    Attack = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<float>(type: "float", nullable: false),
                    TotalExp = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_stat_info", x => x.Id);
                    table.ForeignKey(
                        name: "FK_player_stat_info_player_info_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "player_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_account_info_AccountName",
                table: "account_info",
                column: "AccountName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_info_AccountId",
                table: "player_info",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_player_info_PlayerName",
                table: "player_info",
                column: "PlayerName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_item_info_PlayerId",
                table: "player_item_info",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_player_stat_info_PlayerId",
                table: "player_stat_info",
                column: "PlayerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_item_info");

            migrationBuilder.DropTable(
                name: "player_stat_info");

            migrationBuilder.DropTable(
                name: "player_info");

            migrationBuilder.DropTable(
                name: "account_info");
        }
    }
}

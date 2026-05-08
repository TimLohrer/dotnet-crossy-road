using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrossyRoadApi.Migrations
{
    /// <inheritdoc />
    public partial class player : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_crossy_players_crossy_user_user_id",
                table: "crossy_players");

            migrationBuilder.AddColumn<float>(
                name: "x",
                table: "crossy_players",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "y",
                table: "crossy_players",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "z",
                table: "crossy_players",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddForeignKey(
                name: "fk_crossy_players_users_user_id",
                table: "crossy_players",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_crossy_players_users_user_id",
                table: "crossy_players");

            migrationBuilder.DropColumn(
                name: "x",
                table: "crossy_players");

            migrationBuilder.DropColumn(
                name: "y",
                table: "crossy_players");

            migrationBuilder.DropColumn(
                name: "z",
                table: "crossy_players");

            migrationBuilder.AddForeignKey(
                name: "fk_crossy_players_crossy_user_user_id",
                table: "crossy_players",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrossyRoadApi.Migrations
{
    /// <inheritdoc />
    public partial class player_fk_and_composite_key : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_crossy_players_crossy_games_crossy_game_id",
                table: "crossy_players");

            migrationBuilder.DropPrimaryKey(
                name: "pk_crossy_players",
                table: "crossy_players");

            migrationBuilder.DropIndex(
                name: "ix_crossy_players_crossy_game_id",
                table: "crossy_players");

            migrationBuilder.AlterColumn<Guid>(
                name: "crossy_game_id",
                table: "crossy_players",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "crossy_games",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "pk_crossy_players",
                table: "crossy_players",
                columns: new[] { "crossy_game_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_crossy_players_user_id",
                table: "crossy_players",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_crossy_players_crossy_games_crossy_game_id",
                table: "crossy_players",
                column: "crossy_game_id",
                principalTable: "crossy_games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_crossy_players_crossy_games_crossy_game_id",
                table: "crossy_players");

            migrationBuilder.DropPrimaryKey(
                name: "pk_crossy_players",
                table: "crossy_players");

            migrationBuilder.DropIndex(
                name: "ix_crossy_players_user_id",
                table: "crossy_players");

            migrationBuilder.AlterColumn<Guid>(
                name: "crossy_game_id",
                table: "crossy_players",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "crossy_games",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_crossy_players",
                table: "crossy_players",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_crossy_players_crossy_game_id",
                table: "crossy_players",
                column: "crossy_game_id");

            migrationBuilder.AddForeignKey(
                name: "fk_crossy_players_crossy_games_crossy_game_id",
                table: "crossy_players",
                column: "crossy_game_id",
                principalTable: "crossy_games",
                principalColumn: "id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrossyRoadApi.Migrations
{
    /// <inheritdoc />
    public partial class game_host_fk_set_null : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "host_id",
                table: "crossy_games",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "ix_crossy_games_host_id",
                table: "crossy_games",
                column: "host_id");

            migrationBuilder.AddForeignKey(
                name: "fk_crossy_games_users_host_id",
                table: "crossy_games",
                column: "host_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_crossy_games_users_host_id",
                table: "crossy_games");

            migrationBuilder.DropIndex(
                name: "ix_crossy_games_host_id",
                table: "crossy_games");

            migrationBuilder.AlterColumn<Guid>(
                name: "host_id",
                table: "crossy_games",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}

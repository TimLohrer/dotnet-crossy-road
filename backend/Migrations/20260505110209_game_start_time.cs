using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrossyRoadApi.Migrations
{
    /// <inheritdoc />
    public partial class game_start_time : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "start_time",
                table: "crossy_games",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "start_time",
                table: "crossy_games");
        }
    }
}

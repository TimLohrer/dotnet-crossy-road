using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrossyRoadApi.Migrations
{
    /// <inheritdoc />
    public partial class longer_username : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(35)",
                oldMaxLength: 35);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "AspNetUsers",
                type: "character varying(35)",
                maxLength: 35,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHorrorMovieApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Admin",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);


        }


    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHorrorMovieApp.Migrations
{
    public partial class AddYearAndPlotToMovies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Plot",
                table: "Movies",
                type: "TEXT",
                nullable: true);
        }
    }
}



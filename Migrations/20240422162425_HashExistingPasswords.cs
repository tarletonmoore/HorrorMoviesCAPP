using Microsoft.EntityFrameworkCore.Migrations;
using MyHorrorMovieApp.Models;
using MyHorrorMovieApp.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace MyHorrorMovieApp.Migrations
{
    public partial class HashExistingPasswords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Retrieve an instance of MyDbContext
            using (var context = new MyDbContext(new DbContextOptionsBuilder<MyDbContext>()
                .UseSqlite("Data Source=MyHorrorMovieApp.db") // Adjust the connection string accordingly
                .Options))
            {
                var existingUsers = context.Users.ToList();
                var passwordHashingService = new PasswordHashingService();

                foreach (var user in existingUsers)
                {
                    user.Password = passwordHashingService.HashPassword(user.Password);
                    context.Update(user);
                }

                context.SaveChanges();
            }
        }


    }
}








using Microsoft.EntityFrameworkCore;

namespace MyHorrorMovieApp.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        // DbSet properties for database tables
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        // public DbSet<FriendRequest> FriendRequests { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MovieId);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Movie)
                .WithMany(m => m.Favorites)
                .HasForeignKey(f => f.MovieId);

            //       modelBuilder.Entity<FriendRequest>()
            // .HasOne(fr => fr.Receiver)
            // .WithMany(u => u.ReceivedFriendRequests)
            // .HasForeignKey(fr => fr.ReceiverId)
            // .OnDelete(DeleteBehavior.Cascade);

            //       modelBuilder.Entity<FriendRequest>()
            //           .HasOne(fr => fr.Sender)
            //           .WithMany(u => u.SentFriendRequests)
            //           .HasForeignKey(fr => fr.SenderId)
            //           .OnDelete(DeleteBehavior.Cascade);



            // Seed data
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "tarleton", Password = "password" },
                new User { Id = 2, Username = "test", Password = "password" }
            );

            modelBuilder.Entity<Movie>().HasData(
                     new Movie { Id = 1, Title = "Texas Chainsaw Massacre", Image = "https://m.media-amazon.com/images/M/MV5BMTU1MzY2NDc2MV5BMl5BanBnXkFtZTgwMTc3MTUzMzI@._V1_.jpg", Plot = "Default plot for Texas Chainsaw Massacre" },
                     new Movie { Id = 2, Title = "The Descent", Image = "https://m.media-amazon.com/images/M/MV5BMjA5NzQ1NTgwNV5BMl5BanBnXkFtZTcwNjUxMzUzMw@@._V1_FMjpg_UX1000_.jpg", Plot = "Default plot for The Descent" }
                 );


            modelBuilder.Entity<Review>().HasData(
                new Review { Id = 1, UserId = 1, MovieId = 1, Comment = "Classic slasher movie!!!" },
                new Review { Id = 2, UserId = 2, MovieId = 2, Comment = "It is ok" },
                new Review { Id = 3, UserId = 1, MovieId = 2, Comment = "Great creature feature movie!" }
            );
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            base.OnModelCreating(modelBuilder);

        }
    }
}

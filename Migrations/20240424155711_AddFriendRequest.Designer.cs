﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHorrorMovieApp.Models;

#nullable disable

namespace MyHorrorMovieApp.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20240424155711_AddFriendRequest")]
    partial class AddFriendRequest
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("MyHorrorMovieApp.Models.Favorite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MovieId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.HasIndex("UserId");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("MyHorrorMovieApp.Models.FriendRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("RecipientId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SenderId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RecipientId");

                    b.HasIndex("SenderId");

                    b.ToTable("FriendRequests");
                });

            modelBuilder.Entity("MyHorrorMovieApp.Models.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Plot")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Movies");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Image = "https://m.media-amazon.com/images/M/MV5BMTU1MzY2NDc2MV5BMl5BanBnXkFtZTgwMTc3MTUzMzI@._V1_.jpg",
                            Plot = "Default plot for Texas Chainsaw Massacre",
                            Title = "Texas Chainsaw Massacre",
                            Year = 0
                        },
                        new
                        {
                            Id = 2,
                            Image = "https://m.media-amazon.com/images/M/MV5BMjA5NzQ1NTgwNV5BMl5BanBnXkFtZTcwNjUxMzUzMw@@._V1_FMjpg_UX1000_.jpg",
                            Plot = "Default plot for The Descent",
                            Title = "The Descent",
                            Year = 0
                        });
                });

            modelBuilder.Entity("MyHorrorMovieApp.Models.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("MovieId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Comment = "Classic slasher movie!!!",
                            MovieId = 1,
                            UserId = 1
                        },
                        new
                        {
                            Id = 2,
                            Comment = "It is ok",
                            MovieId = 2,
                            UserId = 2
                        },
                        new
                        {
                            Id = 3,
                            Comment = "Great creature feature movie!",
                            MovieId = 2,
                            UserId = 1
                        });
                });

            modelBuilder.Entity("MyHorrorMovieApp.Models.User", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<bool>("Admin")
                    .HasColumnType("INTEGER");

                b.Property<string>("Password")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("TEXT");

                b.Property<string>("Username")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.HasIndex("Username")
                    .IsUnique();

                b.ToTable("Users");

                b.HasData(
                    new
                    {
                        Id = 1,
                        Admin = false,
                        Password = "password",
                        Username = "tarleton"
                    },
                    new
                    {
                        Id = 2,
                        Admin = false,
                        Password = "password",
                        Username = "test"
                    });
            });



            modelBuilder.Entity("MyHorrorMovieApp.Models.Favorite", b =>
                {
                    b.HasOne("MyHorrorMovieApp.Models.Movie", "Movie")
                        .WithMany("Favorites")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyHorrorMovieApp.Models.User", "User")
                        .WithMany("Favorites")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MyHorrorMovieApp.Models.FriendRequest", b =>
                {
                    b.HasOne("MyHorrorMovieApp.Models.User", "Recipient")
                        .WithMany("ReceivedFriendRequests")
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyHorrorMovieApp.Models.User", "Sender")
                        .WithMany("SentFriendRequests")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipient");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("MyHorrorMovieApp.Models.Review", b =>
                {
                    b.HasOne("MyHorrorMovieApp.Models.Movie", "Movie")
                        .WithMany("Reviews")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyHorrorMovieApp.Models.User", "User")
                        .WithMany("Reviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MyHorrorMovieApp.Models.Movie", b =>
                {
                    b.Navigation("Favorites");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("MyHorrorMovieApp.Models.User", b =>
                {
                    b.Navigation("Favorites");

                    b.Navigation("ReceivedFriendRequests");

                    b.Navigation("Reviews");

                    b.Navigation("SentFriendRequests");
                });
#pragma warning restore 612, 618
        }
    }
}
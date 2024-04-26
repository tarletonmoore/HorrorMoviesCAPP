using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MyHorrorMovieApp.Models;
using MyHorrorMovieApp.Services;
using System;
using System.IO;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MyDbContext>(options =>
{
  options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
  options.EnableSensitiveDataLogging(); // Enable logging of parameter values
  options.LogTo(Console.WriteLine); // Log to console
});

var secretKey = builder.Configuration["Jwt:SecretKey"];
if (secretKey == null)
{
  throw new InvalidOperationException("Jwt:SecretKey is not configured in appsettings.json");
}

builder.Services.AddScoped<AuthService>(); // Register AuthService
builder.Services.AddScoped<PasswordHashingService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "http://localhost:5062",
        ValidAudience = "http://localhost:5062",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
      };

      options.Events = new JwtBearerEvents
      {
        OnAuthenticationFailed = context =>
        {
          // Log authentication failure with additional information
          Console.WriteLine("Authentication failed: " + context.Exception.Message);

          // Log token and headers for debugging
          Console.WriteLine("Token: " + context.Request.Headers["Authorization"]);

          return Task.CompletedTask;
        },
        // Add other event handlers as needed
      };
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
var env = app.Services.GetRequiredService<IWebHostEnvironment>();
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
  FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Views")),
  RequestPath = "/static"
});

app.UseRouting();

// Enable CORS (Cross-Origin Resource Sharing)
app.UseCors(options => options.WithOrigins("http://localhost:5062").AllowAnyMethod().AllowAnyHeader());

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "",
    defaults: new { controller = "Auth", action = "Login" }
);

app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Auth", action = "Login" }
);

app.MapControllerRoute(
    name: "users",
    pattern: "Users/Create",
    defaults: new { controller = "Users", action = "Create" }
);

app.MapControllerRoute(
    name: "me",
    pattern: "me",
    defaults: new { controller = "Users", action = "Details" }
);


app.MapControllerRoute(
    name: "ViewProfile",
    pattern: "Users/Profile/{username}",
    defaults: new { controller = "Users", action = "ViewProfile" }
);


app.MapControllerRoute(
    name: "movies",
    pattern: "Movies",
    defaults: new { controller = "Movies", action = "Index" }
);

app.MapControllerRoute(
    name: "movies",
    pattern: "Movies/Details/{id}",
    defaults: new { controller = "Movies", action = "Details" }
);

app.MapControllerRoute(
    name: "movies",
    pattern: "Movies/Create",
    defaults: new { controller = "Movies", action = "Create" }
);

app.MapControllerRoute(
    name: "movies",
    pattern: "Movies/Edit/{id}",
    defaults: new { controller = "Movies", action = "Edit" }
);

app.MapControllerRoute(
    name: "movies",
    pattern: "Movies/Delete/{id}",
    defaults: new { controller = "Movies", action = "Delete" }
);



app.MapControllerRoute(
    name: "reviews",
    pattern: "Reviews/Create",
    defaults: new { controller = "Reviews", action = "Create" }
);

app.MapControllerRoute(
    name: "reviews",
    pattern: "Reviews/Delete/{id}",
    defaults: new { controller = "Reviews", action = "Delete" }
);

app.MapControllerRoute(
    name: "favorites",
    pattern: "Favorites/Create",
    defaults: new { controller = "Favorites", action = "Create" }
);

app.MapControllerRoute(
    name: "favorites",
    pattern: "Favorites/Delete/{id}",
    defaults: new { controller = "Favorites", action = "Delete" }
);

app.MapControllerRoute(
    name: "checkIfInFavorites",
    pattern: "Favorites/CheckIfInFavorites/{movieId}",
    defaults: new { controller = "Favorites", action = "CheckIfInFavorites" }
);

app.MapControllerRoute(
    name: "friendrequests",
    pattern: "FriendRequests",
    defaults: new { controller = "FriendRequests", action = "Index" }
);

app.MapControllerRoute(
    name: "friendrequests",
    pattern: "FriendRequests/Create",
    defaults: new { controller = "FriendRequests", action = "Create" }
);

app.MapControllerRoute(
    name: "friendrequests",
    pattern: "FriendRequests/Delete/{id}",
    defaults: new { controller = "FriendRequests", action = "Delete" }
);

app.MapControllerRoute(
        name: "friendRequests",
        pattern: "FriendRequests/HasPendingFriendRequest",
        defaults: new { controller = "FriendRequests", action = "HasPendingFriendRequest" });



app.MapFallbackToController("Index", "Movies");


app.Run();

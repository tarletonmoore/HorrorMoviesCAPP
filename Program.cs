// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.IdentityModel.Tokens;
// using MyHorrorMovieApp.Models;
// using MyHorrorMovieApp.Services;
// using System;
// using System.Text;
// using Microsoft.Extensions.FileProviders.Physical;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddDbContext<MyDbContext>(options =>
// {
//   options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
// });

// // Generate a secret key
// var secretKeyGenerator = new SecretKeyGenerator();
// var secretKey = secretKeyGenerator.GenerateSecretKey(32);
// Console.WriteLine("Generated Secret Key: " + secretKey);

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//       options.TokenValidationParameters = new TokenValidationParameters
//       {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = "http://localhost:5062",
//         ValidAudience = "http://localhost:5062",
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
//       };
//     });

// builder.Services.AddControllersWithViews();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//   app.UseExceptionHandler("/Error");
//   app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles(new StaticFileOptions
// {
//   FileProvider = new PhysicalFileProvider(
//         Path.Combine(env.ContentRootPath, "Views")),
//   RequestPath = "/Views"
// });


// app.UseRouting();

// app.MapControllerRoute(
//     name: "login",
//     pattern: "login",
//     defaults: new { controller = "Auth", action = "Login" }
// );


// app.UseAuthentication();
// app.UseAuthorization();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

// app.Run();

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
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Builder;



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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Auth", action = "Login" }
);

app.MapControllerRoute(
    name: "movies",
    pattern: "Movies",
    defaults: new { controller = "Movies", action = "Index" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}"
);

app.MapControllerRoute(
    name: "reviews",
    pattern: "Review/Create",
    defaults: new { controller = "Reviews", action = "Create" }
);



app.Run();

using ClubApp.Backend.Data;
using ClubApp.Backend.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Stores.MaxLengthForKeys = 128;
})
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI();

var clientBuilder = new ClientBuilder();

var a = new ClientBuilder().WithClientId("ClubApp.Application")
    .WithApplicationProfile(ApplicationProfiles.SPA)
    .WithRedirectUri("https://localhost:3000/authentication/login-callback")
    .WithLogoutRedirectUri("https://localhost:3000/authentication/logout-callback")
    .WithScopes("BackendApi", "openid", "profile")
    .WithoutClientSecrets()
    .Build();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
    {
        options.Clients.AddSPA("ClubApp.Application", options =>
            options.WithRedirectUri("https://localhost:3000/authentication/login-callback")
            .WithLogoutRedirectUri("https://localhost:3000/authentication/logout-callback")
            .WithScopes("openid", "profile")
            .WithoutClientSecrets());
    });

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapFallbackToFile("index.html"); ;

app.Run();

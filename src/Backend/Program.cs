using ClubApp.Backend;
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

string reactClientDomain = builder.Configuration["HostingDomains:ReactClient"]
    ?? throw new ArgumentNullException("reactClientDomain", " Hosting Domain of React Client is null");

var reactClient = ClientBuilder.SPA("ClubApp.Application")
    .WithRedirectUri($"{reactClientDomain}/authentication/login-callback")
    .WithLogoutRedirectUri($"{reactClientDomain}/authentication/logout-callback")
    .AllowOfflineAccess()
    .WithCorsOrigins(reactClientDomain)
    .Build();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
    {
        options.Clients.Add(reactClient);
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
else
{
    app.UseExceptionHandler("/Error");

    if (app.Environment.IsStaging())
    {
        using (var context = app.Services.CreateScope()
            .ServiceProvider.GetRequiredService<ApplicationDbContext>())
        {
            context.Database.Migrate();
        }
    }
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

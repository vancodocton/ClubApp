using ClubApp.Backend.Infrastructure.Identity;
using ClubApp.Backend.Options;
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

string dbProvider = builder.Configuration.GetValue("DatabaseProvider", "Sqlite")
    ?? throw new ArgumentNullException(nameof(dbProvider), "Database provider is not configured.");

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
{
    _ = dbProvider switch
    {
        "Sqlite" => options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection"),
            x => x.MigrationsAssembly("Backend.Infrastructure.Sqlite")),
        "Postgre" => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreConnection"),
            x => x.MigrationsAssembly("Backend.Infrastructure.Postgre")),
        _ => throw new Exception($"Unsupported database provider: {dbProvider}")
    };
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Stores.MaxLengthForKeys = 128;
})
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultUI();

IdentityServerConfig.ReactClientOrigins = builder.Configuration.GetSection("Origins:ReactClient").Get<string[]>()
    ?? throw new ArgumentNullException("reactClientDomain", "React Client Origin is null");

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, AppIdentityDbContext>(options =>
    {
        options.Clients.AddRange(IdentityServerConfig.Clients.ToArray());
    });

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();

    if (app.Environment.IsStaging())
    {
        using var context = app.Services.CreateScope().ServiceProvider
            .GetRequiredService<AppIdentityDbContext>();
        context.Database.Migrate();
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

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080);
});

// 1. Connection string ophalen uit appsettings of secrets
string dbConnectionString = builder.Configuration.GetConnectionString("SqlConnectionString");
if (string.IsNullOrWhiteSpace(dbConnectionString))
    throw new InvalidOperationException("The connection string has not been initialized.");

// 2. Identity en Dapper stores toevoegen
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddDapperStores(options => options.ConnectionString = dbConnectionString);

// Nodig voor het aanmaken van gebruikers en login
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();
app.UseAuthorization();
app.MapControllers();

// Root check
app.MapGet("/", () => "API is running!");

app.Run();

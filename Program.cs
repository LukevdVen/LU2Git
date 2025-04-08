using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using System;

var builder = WebApplication.CreateBuilder(args);

// Haal de connection string op uit User Secrets
string dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Debug: Controleer of de connection string correct is opgehaald
Console.WriteLine("Retrieving Connection String...");
if (string.IsNullOrWhiteSpace(dbConnectionString))
{
    Console.WriteLine("Connection string is empty or null!");
    throw new InvalidOperationException("The connection string has not been initialized.");
}
else
{
    // Log een gedeelte van de connection string (voor de veiligheid maar een deel tonen)
    Console.WriteLine($"Connection string retrieved successfully: {dbConnectionString.Substring(0, Math.Min(dbConnectionString.Length, 20))}...");
}

// Voeg controllers toe aan de services
builder.Services.AddControllers();

// Voeg autorisatie toe aan de services
builder.Services.AddAuthorization();

// Debug: Toevoegen van Identity en Dapper stores
Console.WriteLine("Adding Identity and Dapper stores...");

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>() // Default identity met rollen
    .AddDapperStores(options =>
    {
        options.ConnectionString = dbConnectionString; // Gebruik de connection string uit User Secrets
    });

var app = builder.Build();

// Debug: Start het bouwen van de app
Console.WriteLine("Building application...");
app.UseAuthorization();

// Map controllers en Identity API
Console.WriteLine("Mapping controllers and Identity API...");
app.MapControllers();

app.MapGroup("/account")
    .MapIdentityApi<IdentityUser>();

// Debug: Root endpoint om de status van de API te controleren
app.MapGet("/", () =>
{
    var connectionStatus = string.IsNullOrWhiteSpace(dbConnectionString) ? "No" : "Yes";
    Console.WriteLine($"Root endpoint hit. Connection string found: {connectionStatus}");
    return $"The API is up. Connection string found: {connectionStatus}";
});

try
{
    // Run de app
    Console.WriteLine("Running the application...");
    app.Run();
}
catch (Exception ex)
{
    // Debug: Foutafhandeling en loggen van fouten
    Console.WriteLine($"An error occurred while running the app: {ex.Message}");
    throw;
}

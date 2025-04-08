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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
=======
>>>>>>> Stashed changes
else
{
    Console.WriteLine($"Connection string retrieved successfully: {dbConnectionString.Substring(0, Math.Min(dbConnectionString.Length, 20))}..."); // Log only part of the connection string for security
}
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

builder.Services.AddControllers();

<<<<<<< Updated upstream
<<<<<<< Updated upstream
builder.Services.AddAuthorization();
=======
// Debug: Adding Identity services
Console.WriteLine("Adding Identity and Dapper stores...");
>>>>>>> Stashed changes
=======
// Debug: Adding Identity services
Console.WriteLine("Adding Identity and Dapper stores...");
>>>>>>> Stashed changes
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>() // Default identity with roles
    .AddDapperStores(options =>
    {
        options.ConnectionString = dbConnectionString;
    });

var app = builder.Build();

// Debug: Start building the app
Console.WriteLine("Building application...");
app.UseAuthorization();

// Map controllers and Identity API
Console.WriteLine("Mapping controllers and Identity API...");
app.MapControllers();

app.MapGroup("/account")
    .MapIdentityApi<IdentityUser>();

// Debug: Root endpoint to confirm API status
app.MapGet("/", () =>
{
    var connectionStatus = string.IsNullOrWhiteSpace(dbConnectionString) ? "No" : "Yes";
    Console.WriteLine($"Root endpoint hit. Connection string found: {connectionStatus}");
    return $"The API is up. Connection string found: {connectionStatus}";
});

try
{
    // Run the app
    Console.WriteLine("Running the application...");
    app.Run();
}
catch (Exception ex)
{
    // Debug: Catch any errors and log them
    Console.WriteLine($"An error occurred while running the app: {ex.Message}");
    throw;
}

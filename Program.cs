using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Get the logger
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Haal de connection string op uit User Secrets
string dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Log de connection string voor debuggen (let op, stel in productie geen gevoelige data bloot)
logger.LogInformation("Connection string loaded: {ConnectionString}", dbConnectionString);

// Controleer of de connection string gevonden is
if (string.IsNullOrWhiteSpace(dbConnectionString))
{
    logger.LogError("Connection string is null or empty.");
    throw new InvalidOperationException("The connection string has not been initialized.");
}
else
{
    logger.LogInformation("Connection string is initialized.");
}

builder.Services.AddControllers();

builder.Services.AddAuthorization();
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddDapperStores(options =>
    {
        options.ConnectionString = dbConnectionString;
    });

// Ensure the database connection works
using (var connection = new SqlConnection(dbConnectionString))
{
    try
    {
        await connection.OpenAsync();
        logger.LogInformation("Database connection successful.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to connect to the database.");
        throw;
    }
}

app.UseAuthorization();
app.MapControllers();

app.MapGroup("/account")
    .MapIdentityApi<IdentityUser>();

app.MapGet("/", () =>
{
    return $"The API is up. Connection string found: {(string.IsNullOrWhiteSpace(dbConnectionString) ? "No" : "Yes")}";
});

app.Run();

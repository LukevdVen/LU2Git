using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Haal de connection string op uit User Secrets
string dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Controleer of de connection string gevonden is
if (string.IsNullOrWhiteSpace(dbConnectionString))
{
    throw new InvalidOperationException("The connection string has not been initialized.");
}

builder.Services.AddControllers();

builder.Services.AddAuthorization();
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddDapperStores(options =>
    {
        options.ConnectionString = dbConnectionString;
    });

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.MapGroup("/account")
    .MapIdentityApi<IdentityUser>();

app.MapGet("/", () =>
{
    return $"The API is up. Connection string found: {(string.IsNullOrWhiteSpace(dbConnectionString) ? "No" : "Yes")}";
});

app.Run();

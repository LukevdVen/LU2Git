var builder = WebApplication.CreateBuilder(args);

// Fase 1: Service Builder
// Controleer of de SqlConnectionString aanwezig is in de configuratie
bool sqlConnectionStringFound = !string.IsNullOrWhiteSpace(builder.Configuration.GetValue<string>("SqlConnectionString"));

// Fase 2: App Builder
var app = builder.Build();

// Map een eenvoudige diagnostische homepage
app.MapGet("/", () =>
{
    // Haal de SqlConnectionString op
    var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");

    // Toon een bericht met of de connection string gevonden is
    return $"The API is up. Connection string found: {(sqlConnectionStringFound ? "Yes" : "No")}";
});

// Start de applicatie
app.Run();

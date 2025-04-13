using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Haal de connectiestring op uit de configuratie
var dbConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");
if (string.IsNullOrWhiteSpace(dbConnectionString))
    throw new InvalidOperationException("Connection string missing.");

// Voeg services toe voor Identity, Dapper en HttpClient
builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    }); // Add NewtonsoftJson configuration here
builder.Services.AddHttpContextAccessor();

// Register HttpClient for dependency injection
builder.Services.AddHttpClient();

// Registreer de repository en configuraties
builder.Services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
builder.Services.AddScoped<IObjectRepository, ObjectRepository>();

// Register Identity with Dapper stores
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddDapperStores(options => options.ConnectionString = dbConnectionString);

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
});

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// API Endpoints
app.MapGroup("/account").MapIdentityApi<IdentityUser>();
app.MapControllers().RequireAuthorization();

app.MapGet("/", () => "API is running!");

app.Run();

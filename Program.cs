using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Haal de connectiestring op uit de configuratie
string dbConnectionString = builder.Configuration.GetConnectionString("SqlConnectionString");
if (string.IsNullOrWhiteSpace(dbConnectionString))
    throw new InvalidOperationException("Connection string missing.");

// Voeg services toe voor Identity en Dapper
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Registreer de repository en configuraties
builder.Services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();


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

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/account").MapIdentityApi<IdentityUser>();
app.MapControllers().RequireAuthorization();

app.MapGet("/", () => "API is running!");

app.Run();

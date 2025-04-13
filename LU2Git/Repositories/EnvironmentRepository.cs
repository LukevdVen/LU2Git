using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EnvironmentRepository : IEnvironmentRepository
{
    private readonly string _connectionString;
    private readonly ILogger<EnvironmentRepository> _logger;

    // Constructor that takes IConfiguration and ILogger<EnvironmentRepository>
    public EnvironmentRepository(IConfiguration configuration, ILogger<EnvironmentRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("SqlConnectionString");

        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection string missing.");
        }

        _logger = logger;
    }

    // Constructor with just IConfiguration (optional, but you can remove it)
    public EnvironmentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlConnectionString");

        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection string missing.");
        }
    }

    // Get all environments for a specific user
    public async Task<List<Environment2D>> GetAll(string userName)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "SELECT * FROM Environment2D WHERE Username = @UserName";
        var environments = await connection.QueryAsync<Environment2D>(query, new { UserName = userName });
        return environments.ToList();
    }

    // Get environment by its Id
    public async Task<Environment2D> GetById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "SELECT * FROM Environment2D WHERE Id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Environment2D>(query, new { Id = id });
    }

    // Add a new environment
    public async Task<Environment2D> Add(Environment2D environment)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();  // Open the connection explicitly
        var query = @"
        INSERT INTO Environment2D (Naam, MaxHeight, MaxWidth, Username) 
        VALUES (@Naam, @MaxHeight, @MaxWidth, @Username);
        SELECT * FROM Environment2D WHERE Id = SCOPE_IDENTITY();";  // Use SCOPE_IDENTITY() to get the latest ID

        _logger.LogInformation($"Inserting Environment: Naam={environment.Naam}, MaxHeight={environment.MaxHeight}, MaxWidth={environment.MaxWidth}, Username={environment.UserName}");

        // Execute the query and log the results
        var inserted = await connection.QuerySingleAsync<Environment2D>(query, environment);

        // Log the ID of the inserted environment
        _logger.LogInformation($"Inserted Environment ID: {inserted.Id}");

        return inserted;
    }

    // Update an existing environment
    public async Task<Environment2D> Update(Environment2D environment)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            UPDATE Environment2D
            SET Naam = @Naam, MaxHeight = @MaxHeight, MaxWidth = @MaxWidth
            WHERE Id = @Id;
            SELECT * FROM Environment2D WHERE Id = @Id;";

        return await connection.QuerySingleOrDefaultAsync<Environment2D>(query, environment);
    }

    // Delete an environment by its ID
    public async Task<bool> Delete(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "DELETE FROM Environment2D WHERE Id = @Id";
        return await connection.ExecuteAsync(query, new { Id = id }) > 0;
    }
}

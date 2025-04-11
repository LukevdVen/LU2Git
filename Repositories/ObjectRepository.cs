using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class ObjectRepository : IObjectRepository
{
    private readonly string _connectionString;
    private readonly ILogger<ObjectRepository> _logger;
    private readonly HttpClient _httpClient;

    // Constructor that takes IConfiguration, ILogger<ObjectRepository>, and HttpClient
    public ObjectRepository(IConfiguration configuration, ILogger<ObjectRepository> logger, HttpClient httpClient)
    {
        _connectionString = configuration.GetConnectionString("SqlConnectionString");

        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection string missing.");
        }

        _logger = logger;
        _httpClient = httpClient;
    }

    // Get all objects for a specific environment
    public async Task<List<ObjectDTO>> GetObjectsByEnvironment(int environmentId)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "SELECT * FROM dbo.Objects WHERE EnvironmentId = @EnvironmentId";
        var objects = await connection.QueryAsync<ObjectDTO>(query, new { EnvironmentId = environmentId });
        return objects.ToList();
    }

    // Get object by its Id
    public async Task<ObjectDTO> GetObjectById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "SELECT * FROM dbo.Objects WHERE ObjectId = @Id";
        return await connection.QuerySingleOrDefaultAsync<ObjectDTO>(query, new { Id = id });
    }

    // Add a new object
    public async Task<ObjectDTO> Add(ObjectDTO objectDto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(); // Open the connection explicitly

        var query = @"
        INSERT INTO dbo.Objects (EnvironmentId, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ) 
        VALUES (@EnvironmentId, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ);
        SELECT * FROM dbo.Objects WHERE ObjectId = SCOPE_IDENTITY();"; // Use SCOPE_IDENTITY() to get the latest ID

        _logger.LogInformation($"Inserting Object: EnvironmentId={objectDto.EnvironmentId}, PrefabId={objectDto.PrefabId}, PositionX={objectDto.PositionX}, PositionY={objectDto.PositionY}, ScaleX={objectDto.ScaleX}, ScaleY={objectDto.ScaleY}, RotationZ={objectDto.RotationZ}");

        var inserted = await connection.QuerySingleAsync<ObjectDTO>(query, objectDto);

        return inserted;
    }

    // Update an existing object
    public async Task<ObjectDTO> Update(ObjectDTO objectDto)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            UPDATE dbo.Objects
            SET PrefabId = @PrefabId, PositionX = @PositionX, PositionY = @PositionY, 
                ScaleX = @ScaleX, ScaleY = @ScaleY, RotationZ = @RotationZ
            WHERE ObjectId = @Id;
            SELECT * FROM dbo.Objects WHERE ObjectId = @Id;";

        return await connection.QuerySingleOrDefaultAsync<ObjectDTO>(query, objectDto);
    }

    // Delete an object by its ID
    public async Task<bool> Delete(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "DELETE FROM dbo.Objects WHERE ObjectId = @Id";
        return await connection.ExecuteAsync(query, new { Id = id }) > 0;
    }

    // Delete all objects in a specific environment
    public async Task<bool> DeleteAllObjectsInWorld(int environmentId)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "DELETE FROM dbo.Objects WHERE EnvironmentId = @EnvironmentId";
        return await connection.ExecuteAsync(query, new { EnvironmentId = environmentId }) > 0;
    }

    // Save placed objects to API
    public async Task SavePlacedObjects(List<ObjectDTO> placedObjects)
    {
        var url = "yourApiUrl"; // Replace with your actual API URL
        var content = new StringContent(JsonConvert.SerializeObject(placedObjects), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();

            try
            {
                // Deserialize the response string to a list of ObjectDTOs
                var savedObjects = JsonConvert.DeserializeObject<List<ObjectDTO>>(responseString);

                // Handle saved objects if necessary
            }
            catch (JsonSerializationException ex)
            {
                // Handle the error in case deserialization fails
                _logger.LogError($"Failed to deserialize response: {ex.Message}");
            }
        }
        else
        {
            _logger.LogError("Failed to save placed objects.");
        }
    }

    public async Task<bool> DeleteObjectsByEnvironment(int environmentId)
    {
        using var connection = new SqlConnection(_connectionString);

        var query = "DELETE FROM dbo.Objects WHERE EnvironmentId = @EnvironmentId";

        // Execute the query and return whether any rows were affected
        var affectedRows = await connection.ExecuteAsync(query, new { EnvironmentId = environmentId });

        return affectedRows > 0;  // If any rows were deleted, return true
    }
}

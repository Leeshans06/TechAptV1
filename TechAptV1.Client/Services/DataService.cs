// Copyright © 2025 Always Active Technologies PTY Ltd

using System.Data.Common;
using TechAptV1.Client.Models;
using System.Collections.Concurrent;
using System.Data.SQLite;

namespace TechAptV1.Client.Services;

/// <summary>
/// Data Access Service for interfacing with the SQLite Database
/// </summary>
public sealed class DataService
{
    private readonly ILogger<DataService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly SQLiteConnection _connection;
    /// <summary>
    /// Default constructor providing DI Logger and Configuration
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    public DataService(ILogger<DataService> logger, IConfiguration configuration)
    {
        this._logger = logger;
        this._configuration = configuration;
        _connectionString = configuration.GetConnectionString("Default");  
        _connection = new SQLiteConnection(_connectionString);
    }
    /// <summary>
    /// Initialize Sqlite Database Connection
    /// </summary>
    public async Task InitializeAsync()
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            await connection.OpenAsync();
            _logger.LogInformation("Database connection initialized.");
        }
    }
    /// <summary>
    /// Save the list of data to the SQLite Database
    /// Batch Insert
    /// </summary>
    /// <param name="dataList"></param>
    public async Task Save(List<Number> dataList,int batchSize = 100000)
    {
        this._logger.LogInformation("Save");
        const string insertQuery = "INSERT INTO Number (Value, IsPrime) VALUES (@Value, @IsPrime);";

        // persistent connection
        using var connection = new SQLiteConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();

        // Create table if it doesn't exist
        string createTableQuery = @"
        CREATE TABLE IF NOT EXISTS Number (
            Value INTEGER NOT NULL,
            IsPrime INTEGER NOT NULL DEFAULT 0
        );";
        using (var createCmd = new SQLiteCommand(createTableQuery, connection))
        {
            await createCmd.ExecuteNonQueryAsync();
        }

        // Start the first transaction manually
        var transaction = await connection.BeginTransactionAsync();
        using var command = new SQLiteCommand(insertQuery, connection, (SQLiteTransaction)transaction);

        int count = 0;
        int batchCount = 0;

        foreach (var num in dataList)
        {
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@Value", num.Value);
            command.Parameters.AddWithValue("@IsPrime", num.IsPrime);
            await command.ExecuteNonQueryAsync();

            count++;

            // Commit after every batch inserts
            if (count % batchSize == 0)
            {
                batchCount++;
                _logger.LogInformation("Inserting Batch: #" + batchCount);
                await transaction.CommitAsync();
                // Start a new transaction after committing (re-use transaction variable)
                transaction = await connection.BeginTransactionAsync();
            }
        }

        // Final commit if remaining records exist
        await transaction.CommitAsync();
    }

    /// <summary>
    /// Fetch N records from the SQLite Database where N is specified by the count parameter
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Number>> Get(int count)
    {
        this._logger.LogInformation("Get");
        var numbers = new List<Number>();

        using (var connection = new SQLiteConnection(_connectionString))
        {
            await connection.OpenAsync();

            string query = $"SELECT Value, IsPrime FROM Number ORDER BY Value LIMIT {count};";
            using (var cmd = new SQLiteCommand(query, connection))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    numbers.Add(new Number
                    {
                        Value = reader.GetInt32(0),
                        IsPrime = reader.GetInt32(1)
                    });
                }
            }
        }

        return numbers;
    }

    /// <summary>
    /// Fetch All the records from the SQLite Database
    /// Asynchronously streams all records from the "Number" table in the SQLite database.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Number>> GetAll()
    {
        this._logger.LogInformation("GetAll");

        // persistent connection
        using var connection = new SQLiteConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();

        var numbers = new List<Number>();
        string query = "SELECT Value, IsPrime FROM Number";

        using (var cmd = new SQLiteCommand(query, connection)) // Use the existing persistent connection
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                this._logger.LogInformation("Adding to Number List : "+reader.GetInt32(0));
                numbers.Add(new Number
                {
                    Value = reader.GetInt32(0),
                    IsPrime = reader.GetInt32(1)
                });
            }
        }

        return numbers;
    }

    /// <summary>
    /// Asynchronously streams all records from the "Number" table in the SQLite database.   
    /// </summary>
    public async IAsyncEnumerable<Number> StreamAllNumbers()
    {
        using var connection = new SQLiteConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
               
        using var command = new SQLiteCommand("SELECT Value, IsPrime FROM Number", connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            yield return new Number
            {
                Value = reader.GetInt32(0),
                IsPrime = reader.GetInt32(1)
            };
        }
    }
}

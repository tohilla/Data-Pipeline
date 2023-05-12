using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using Serilog;

class Program
{
    static void Main()
    {
        // Configure the logger
        LogConfiguration.ConfigureLogging();

        try
        {
            // Build the configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Set up the connection strings for the source and destination databases
            string sourceConnectionString = configuration.GetConnectionString("SourceDatabase");
            string destinationConnectionString = configuration.GetConnectionString("DestinationDatabase");

            using (SqlConnection sourceConnection = new SqlConnection(sourceConnectionString))
            {
                sourceConnection.Open();
                Log.Information("Connected to the source database.");

                using (SqlConnection destinationConnection = new SqlConnection(destinationConnectionString))
                {
                    destinationConnection.Open();
                    Log.Information("Connected to the destination database.");

                    // Retrieve all table names from the source database
                    List<string> tableNames = GetTableNames(sourceConnection);

                    // Data extraction and insertion for each table
                    foreach (string tableName in tableNames)
                    {
                        Log.Information($"Migrating data for table: {tableName}");

                        // Data extraction
                        DataExtractor dataExtractor = new DataExtractor(sourceConnectionString);
                        DataTable extractedData = dataExtractor.ExtractDataFromTable(sourceConnectionString, tableName);

                        // Data insertion
                        DataInserter dataInserter = new DataInserter(destinationConnectionString);
                        Dictionary<string, DataTable> dataToInsert = new Dictionary<string, DataTable>
                        {
                            { tableName, extractedData }
                        };
                        dataInserter.InsertDataIntoTables(dataToInsert);

                    }

                    Log.Information("Data migration process completed successfully.");

                    destinationConnection.Close();
                    Log.Information("Disconnected from the destination database.");
                }

                sourceConnection.Close();
                Log.Information("Disconnected from the source database.");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred during the data migration process.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    static List<string> GetTableNames(SqlConnection connection)
    {
        List<string> tableNames = new List<string>();

        using (SqlCommand command = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", connection))
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string tableName = reader.GetString(0);
                    tableNames.Add(tableName);
                }
            }
        }

        return tableNames;
    }
}

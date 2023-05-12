using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Serilog;

class Program
{
    static void Main()
    {
        // Configure the logger
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("log.txt")
            .CreateLogger();

        try
        {
            string sourceConnectionString = "Data Source=TOMMY-DANNY;Initial Catalog=Test_Excel2SQL;Integrated Security=True";
            Dictionary<string, DataTable> extractedData = new Dictionary<string, DataTable>();

            using (SqlConnection sourceConnection = new SqlConnection(sourceConnectionString))
            {
                sourceConnection.Open();
                Log.Information("Connected to the source SQL Server database.");

                DataTable tables = sourceConnection.GetSchema("Tables");
                Log.Information("Retrieved tables from the source database.");

                foreach (DataRow table in tables.Rows)
                {
                    string tableName = table["TABLE_NAME"].ToString();
                    Log.Information($"Extracting data from table: {tableName}");

                    DataTable data = ExtractDataFromTable(sourceConnection, tableName);
                    extractedData.Add(tableName, data);

                    Log.Information($"Extracted data from table: {tableName}. Rows: {data.Rows.Count}");
                }

                sourceConnection.Close();
                Log.Information("Disconnected from the source SQL Server database.");
            }

            // Connect to the source database again
            using (SqlConnection sourceConnection = new SqlConnection(sourceConnectionString))
            {
                sourceConnection.Open();
                Log.Information("Connected to the source SQL Server database.");

                // Create the ExtractedData table if it doesn't exist
                using (SqlCommand createTableCommand = new SqlCommand(
                    "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExtractedData]') AND type in (N'U')) " +
                    "BEGIN " +
                    "    CREATE TABLE [dbo].[ExtractedData] " +
                    "    (" +
                    "        " + GetTableSchemaFromData(extractedData.Values.First()) +
                    "    ) " +
                    "END",
                    sourceConnection))
                {
                    createTableCommand.ExecuteNonQuery();
                }

                // Insert the extracted data into the ExtractedData tables
                foreach (var extractedTable in extractedData)
                {
                    string tableName = extractedTable.Key;
                    DataTable data = extractedTable.Value;
                    string extractedTableName = "extracted_" + tableName;

                    // Create the extracted table if it doesn't exist
                    using (SqlCommand createTableCommand = new SqlCommand(
                        "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + extractedTableName + "]') AND type in (N'U')) " +
                        "BEGIN " +
                        "    CREATE TABLE [dbo].[" + extractedTableName + "] " +
                        "    (" +
                        "        " + GetTableSchemaFromData(data) +
                        "    ) " +
                        "END",
                        sourceConnection))
                    {
                        createTableCommand.ExecuteNonQuery();
                    }

                    // Insert the extracted data into the extracted table
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sourceConnection))
                    {
                        bulkCopy.DestinationTableName = "[dbo].[" + extractedTableName + "]"; // Destination table name
                        bulkCopy.WriteToServer(data);
                    }

                    Log.Information($"Data inserted into the {extractedTableName} table for table: {tableName}");
                }

                sourceConnection.Close();
                Log.Information("Disconnected from the source SQL Server database.");
            }

            Log.Information("Data migration process completed successfully.");
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

    static DataTable ExtractDataFromTable(SqlConnection connection, string tableName)
    {
        DataTable data = new DataTable();

        using (SqlCommand command = new SqlCommand($"SELECT * FROM {tableName}", connection))
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(data);
            }
        }

        return data;
    }

    static string GetTableSchemaFromData(DataTable data)
    {
        string tableSchema = "";

        foreach (DataColumn column in data.Columns)
        {
            string columnName = column.ColumnName;
            string dataType = GetDataTypeString(column.DataType);

            tableSchema += $"[{columnName}] {dataType}, ";
        }

        // Remove the trailing comma and space
        tableSchema = tableSchema.TrimEnd(',', ' ');

        return tableSchema;
    }

    static string GetDataTypeString(Type dataType)
    {
        if (dataType == typeof(int))
            return "INT";
        else if (dataType == typeof(string))
            return "VARCHAR(100)"; // Change the length as needed
        else if (dataType == typeof(DateTime))
            return "DATETIME";
        // Add more data type mappings as required

        // If the data type is not recognized, use a default data type
        return "NVARCHAR(100)";
    }

}

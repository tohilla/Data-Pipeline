using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class DataInserter
{
    private readonly string destinationConnectionString;

    public DataInserter(string destinationConnectionString)
    {
        this.destinationConnectionString = destinationConnectionString;
    }

    public void InsertDataIntoTables(Dictionary<string, DataTable> extractedData)
    {
        using (SqlConnection connection = new SqlConnection(destinationConnectionString))
        {
            connection.Open();

            foreach (var entry in extractedData)
            {
                string tableName = entry.Key;
                DataTable data = entry.Value;

                CreateDestinationTable(connection, tableName, data);
                BulkInsertData(connection, tableName, data);

                Console.WriteLine($"Data inserted into the destination table for table: {tableName}");
            }

            connection.Close();
        }
    }

    private void CreateDestinationTable(SqlConnection connection, string tableName, DataTable data)
    {
        // Use SqlBulkCopy to transfer data from the source table to the destination table
        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
        {
            bulkCopy.DestinationTableName = tableName;
            bulkCopy.WriteToServer(data);
        }
    }


    private void BulkInsertData(SqlConnection connection, string tableName, DataTable data)
    {
        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
        {
            bulkCopy.DestinationTableName = tableName;
            bulkCopy.WriteToServer(data);
        }
    }

    private string GetSqlType(Type dataType)
    {
        if (dataType == typeof(int))
            return "INT";
        else if (dataType == typeof(string))
            return "NVARCHAR(MAX)";
        else if (dataType == typeof(DateTime))
            return "DATETIME";
        // Add more data type mappings as needed

        return "NVARCHAR(MAX)";
    }

}

using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public static class DatabaseHelper
{
    private static string GetConnectionString(string databaseName)
    {
        return ConfigurationManager.ConnectionStrings[databaseName].ConnectionString;
    }

    public static DataTable ExecuteQuery(string databaseName, string query)
    {
        DataTable dataTable = new DataTable();

        string connectionString = GetConnectionString(databaseName);

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while executing the query: {ex.Message}");
                }
            }
        }

        return dataTable;
    }

    public static void ExecuteNonQuery(string databaseName, string query)
    {
        string connectionString = GetConnectionString(databaseName);

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while executing the query: {ex.Message}");
                }
            }
        }
    }
}

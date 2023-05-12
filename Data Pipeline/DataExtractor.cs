using System.Data.SqlClient;
using System.Data;

public class DataExtractor
{
    private readonly string sourceConnectionString;

    public DataExtractor(string sourceConnectionString)
    {
        this.sourceConnectionString = sourceConnectionString;
    }

    public Dictionary<string, DataTable> ExtractDataFromTables(List<string> tablesToExtract)
    {
        Dictionary<string, DataTable> extractedData = new Dictionary<string, DataTable>();

        using (SqlConnection connection = new SqlConnection(sourceConnectionString))
        {
            connection.Open();

            foreach (string tableName in tablesToExtract)
            {
                DataTable dataTable = ExtractDataFromTable(sourceConnectionString, tableName);
                extractedData.Add(tableName, dataTable);
            }

            connection.Close();
        }

        return extractedData;
    }

    public DataTable ExtractDataFromTable(string connectionString, string tableName)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = $"SELECT * FROM {tableName}";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }
    }
}

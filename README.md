----------------------------------------------------------------------------------------------------------------------------------
--------------------------------------UPDATES 2023-16-05----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------

# Data Pipeline

## Overview
The Data Pipeline project is designed to extract data from a source database, perform business logic manipulations, and insert the transformed data into a destination database. This documentation covers the recent updates made to the codebase.

## Code Changes

### `Program.cs`
- **`Main()` Method**: The main entry point of the application. It establishes connections to the source and destination databases, retrieves table names from the source database, and performs data extraction and insertion for each table.
- **`GetTableNames()` Method**: Retrieves all table names from the source database using the `INFORMATION_SCHEMA.TABLES` query.
- **`ManipulateAggregateData()` Method**: Performs any necessary data manipulation or transformation on the extracted aggregate data.

### `DataInserter.cs`
- **`InsertAggregates()` Method**: Inserts the list of aggregates into the destination database. It creates the destination table if it doesn't already exist, and uses `SqlBulkCopy` to efficiently insert the data.
- **`CreateDestinationTable()` Method**: Creates the destination table for aggregates if it doesn't already exist. It checks for the existence of the table using the `INFORMATION_SCHEMA.TABLES` query and creates the table using the `CREATE TABLE` statement.
- **`GenerateCreateTableCommand()` Method**: Generates the `CREATE TABLE` command dynamically based on the table name and data schema.
- **`BulkInsertAggregates()` Method**: Performs bulk insertion of aggregates using `SqlBulkCopy`. It maps the list of aggregates to a `DataTable` and writes it to the destination table.

### `DataExtractor.cs`
- **`ExtractAggregates()` Method**: Extracts the aggregate data from the source database. It establishes a connection to the source database, retrieves the data from the "aggregates" table, and maps it to a list of `Aggregate` objects.

### `Aggregate.cs`
- Represents the data structure for an aggregate, with properties for `Values` and `Unnamed1`.

## Usage
To use the Data Pipeline, follow these steps:

1. Update the connection strings in the `appsettings.json` file to point to the appropriate source and destination databases.

2. Run the application. It will establish connections to the databases, retrieve table names, extract data from each table, perform necessary data manipulations, and insert the transformed data into the destination database.

3. Monitor the console output and logs for information about the migration process. Any errors encountered during the process will be logged and displayed.

## Dependencies
- Microsoft.Extensions.Configuration: Used for configuration management.
- System.Data.SqlClient: Provides access to SQL Server database functionality.
- Serilog: Used for logging purposes.

## Conclusion
The recent code changes in the Data Pipeline project enable the extraction and insertion of aggregate data from the source to the destination database. The code now supports dynamic creation of the destination table if it doesn't exist and performs efficient bulk insertion of data. The application provides comprehensive logging to track the migration process and any encountered errors.

----------------------------------------------------------------------------------------------------------------------------------
--------------------------------------ORIGINAL DOCUMENTATION----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------

# Data Migration Tool

The Data Migration Tool is a custom-built data pipeline and migration tool developed using C# and the .NET Framework. It allows you to extract data from different databases (Microsoft SQL Server and Oracle 11g) and migrate it to another Microsoft SQL Server database.

## Prerequisites

- .NET Framework 4.5 or higher
- Microsoft SQL Server client libraries
- Oracle 11g client libraries (if using Oracle as a data source)

## Installation Instructions

1. Clone the repository from GitHub:

   ```shell
   git clone https://github.com/your-username/data-migration-tool.git
   ```

2. Open the project in Visual Studio.

3. Build the solution to restore NuGet packages.

4. Configure the application settings in the `appsettings.json` file:
   - Set the connection strings for the source SQL Server and Oracle databases.
   - Set the connection string for the destination SQL Server database.

5. Build the project to ensure all dependencies are resolved.

## Usage

To run the data migration tool:

1. Open a command-line interface.

2. Navigate to the project's output directory.

3. Run the following command:

   ```shell
   DataMigrationTool.exe
   ```

4. The tool will extract data from the source databases, create corresponding tables in the destination database (if they don't exist), and insert the extracted data.

## Configuration

The following settings can be configured in the `appsettings.json` file:

- `SourceDatabaseSettings`: Connection details for the source databases.
- `DestinationDatabaseSettings`: Connection details for the destination database.
- `Logging`: Logging configuration settings.

## Code Structure

- `Program.cs`: Main entry point of the application.
- `DatabaseHelper.cs`: Contains helper methods for connecting to databases and executing SQL commands.
- `DataExtractor.cs`: Implements data extraction logic from the source databases.
- `DataInserter.cs`: Implements data insertion logic into the destination database.
- `Models/`: Contains data models and classes for storing extracted data.
- `Logging/`: Contains logging configuration and setup.

## Logging

The data migration tool utilizes the Serilog logging library. The log messages are written to the console and a log file named `log.txt`. The log file captures information, warnings, errors, and exceptions encountered during the data migration process.

## Error Handling

Errors and exceptions during the data migration process are logged and can be found in the log file. If an error occurs, the tool will handle it gracefully and continue with the remaining tables.

## Contact Information

For any questions or inquiries, please contact:

- Aaron Tohill
- Email: aaron.tohill@gmail.com

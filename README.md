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

## License

This project is licensed under the [MIT License](LICENSE).

## Contact Information

For any questions or inquiries, please contact:

- Your Name
- Email: your-email@example.com

---

Feel free to customize this template based on your project's specific details and requirements.

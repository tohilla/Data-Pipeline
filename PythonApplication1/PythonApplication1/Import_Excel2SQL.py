import pandas as pd
import pyodbc
import numpy as np
import re

# Define a dictionary mapping pandas data types to SQL data types
DATA_TYPE_MAPPING = {
    "datetime64[ns]": "DATETIME",
    "datetime64[ns, UTC]": "DATETIME",
    "date": "DATE",
    "int": "INT",
    "float": "FLOAT",
    "bool": "BIT",
    "bytes": "VARBINARY(MAX)",
    "object": "VARCHAR(MAX)"
}

try:
    # Set up a connection to the SQL server
    with pyodbc.connect('Driver={SQL Server};'
                        'Server=(localhost);'
                        'Database=Test_Excel2SQL;'
                        'Trusted_Connection=yes;') as conn:

        # Load the Excel file into a pandas dataframe
        excel_file = pd.read_excel('C:\Test\Excel2SQL.xlsx', sheet_name=None, header=0, read_only=True, ignore_hidden=True)

        # Store table names to check for duplicates
        table_names = set()

        # Iterate over each sheet in the Excel file
        for sheet_name, df in excel_file.items():
            # Create a SQL table with the same name as the sheet
            table_name = sheet_name.replace(" ", "_").lower()

            # Add a suffix to the table name if there are multiple sheets with the same name
            if table_name in table_names:
                suffix = 1
                while f"{table_name}_{suffix}" in table_names:
                    suffix += 1
                table_name = f"{table_name}_{suffix}"
            table_names.add(table_name)

            # Fill merged cells with the value from the top-left cell
            df = df.fillna(method='ffill')

            # Infer data types for each column in the dataframe
            column_data_types = []
            for column_name, data_type in df.dtypes.to_dict().items():
                sql_data_type = DATA_TYPE_MAPPING[str(data_type)]
                if df[column_name].isnull().all():
                    # If all values in the column are null, use VARCHAR(MAX) as the SQL data type
                    sql_data_type = "VARCHAR(MAX)"
                column_data_types.append(f"[{column_name}] {sql_data_type}")
            columns = ", ".join(column_data_types)

            with conn.cursor() as cursor:
                # Check if the table already exists
                cursor.execute(f"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{table_name}'")
                if cursor.fetchone()[0] == 0:
                    # If the table does not exist, create it
                    cursor.execute(f"CREATE TABLE [{table_name}] ({columns})")

                # Insert the data from the dataframe into the SQL table
                values_placeholder = ", ".join(["?" for i in range(len(df.columns))])
                insert_query = f"INSERT INTO [{table_name}] ({', '.join([f'[{col}]' for col in df.columns])}) VALUES ({values_placeholder})"
                try:
                    for index, row in df.iterrows():
                        values = [None if pd.isna(x) else x for x in row]
                        cursor.execute(insert_query, values)
                    conn.commit()
                    print(f"{len(df)} rows written to table {table_name}")
                except pyodbc.Error as e:
                    conn.rollback()
                    print(f"Error inserting data into table {table_name}: {e}")
    print("All sheets written to database")
except pyodbc.Error as e:
    print(f"Error connecting to database: {e}")


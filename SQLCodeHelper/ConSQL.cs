using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

public class DatabaseInfo
{
    public string Name { get; set; }
    public string Size { get; set; }
    public DateTime CreateDate { get; set; }
    public int TableCount { get; set; }
    public string Collation { get; set; }
    public string RecoveryModel { get; set; }
    public string State { get; set; }
}

public class TableInfo
{
    public string Name { get; set; }
    public string Schema { get; set; }
    public string DatabaseName { get; set; }
    public string RowCount { get; set; }
    public string SizeMB { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime ModifyDate { get; set; }
    public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();
    public List<ForeignKeyInfo> ForeignKeys { get; set; } = new List<ForeignKeyInfo>();
    public List<IndexInfo> Indexes { get; set; } = new List<IndexInfo>();
}

public class ColumnInfo
{
    public string Name { get; set; }
    public string DataType { get; set; }
    public int MaxLength { get; set; }
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsIdentity { get; set; }
    public string DefaultValue { get; set; }
    public int Precision { get; set; }
    public int Scale { get; set; }
}

public class ForeignKeyInfo
{
    public string Name { get; set; }
    public string ColumnName { get; set; }
    public string ReferenceTable { get; set; }
    public string ReferenceColumn { get; set; }
}

public class IndexInfo
{
    public string Name { get; set; }
    public string ColumnName { get; set; }
    public bool IsUnique { get; set; }
    public bool IsClustered { get; set; }
}

public class StoredProcedureInfo
{
    public string Name { get; set; }
    public string Schema { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime ModifyDate { get; set; }
}

public class ConSQL
{
    private string connectionString;
    private bool isConnected = false;
    private string serverName;
    private string serverVersion;
    private List<DatabaseInfo> cachedDatabases = new List<DatabaseInfo>();
    private DateTime lastCacheUpdate = DateTime.MinValue;
    public string GetConnectionString()
    {
        return connectionString;
    }
    public void Conn(string connStr)
    {
        connectionString = connStr;
        isConnected = TestConnection(connStr);

        if (isConnected)
        {
            // Получаем информацию о сервере
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                serverName = connection.DataSource;
                serverVersion = connection.ServerVersion;
            }

            // Кэшируем информацию о базах данных
            CacheDatabases();
        }
    }

    public bool TestConnection(string connStr)
    {
        try
        {
            using (SqlConnection testConn = new SqlConnection(connStr))
            {
                testConn.Open();
                testConn.Close();
                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    public bool IsConnected()
    {
        return isConnected;
    }

    public string GetServerName()
    {
        return serverName;
    }

    public string GetServerVersion()
    {
        return serverVersion;
    }

    private void CacheDatabases()
    {
        try
        {
            cachedDatabases = GetDatabases();
            lastCacheUpdate = DateTime.Now;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при кэшировании баз данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public List<DatabaseInfo> GetDatabases()
    {
        List<DatabaseInfo> databases = new List<DatabaseInfo>();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Сначала получаем основные данные о базах
            string query = @"
            SELECT 
                d.name,
                d.create_date,
                d.collation_name,
                d.recovery_model_desc,
                d.state_desc
            FROM sys.databases d
            WHERE d.database_id > 4 AND d.state = 0
            ORDER BY d.name";

            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string dbName = reader["name"].ToString();
                    databases.Add(new DatabaseInfo
                    {
                        Name = dbName,
                        Size = "0 MB", // временно, будет заполнено ниже
                        CreateDate = Convert.ToDateTime(reader["create_date"]),
                        Collation = reader["collation_name"].ToString(),
                        RecoveryModel = reader["recovery_model_desc"].ToString(),
                        State = reader["state_desc"].ToString(),
                        TableCount = 0
                    });
                }
            }

            // Для каждой базы получаем размер и количество таблиц
            foreach (var db in databases)
            {
                // Размер базы
                string sizeQuery = $@"
                USE [{db.Name}];
                SELECT CAST(SUM(size) * 8.0 / 1024 AS DECIMAL(10,2)) AS SizeMB
                FROM sys.master_files
                WHERE database_id = DB_ID()";

                using (SqlCommand cmd = new SqlCommand(sizeQuery, connection))
                {
                    object result = cmd.ExecuteScalar();
                    db.Size = result != DBNull.Value ? $"{result} MB" : "0 MB";
                }

                // Количество таблиц
                string tablesCountQuery = $@"
                USE [{db.Name}];
                SELECT COUNT(*) FROM sys.tables WHERE is_ms_shipped = 0";

                using (SqlCommand cmd = new SqlCommand(tablesCountQuery, connection))
                {
                    db.TableCount = (int)cmd.ExecuteScalar();
                }
            }
        }
        return databases;
    }

    public int GetTableCount(string databaseName)
    {
        int tableCount = 0;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = $@"
            USE [{databaseName}];
            SELECT COUNT(*) 
            FROM sys.tables 
            WHERE is_ms_shipped = 0";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                tableCount = Convert.ToInt32(command.ExecuteScalar());
            }
        }

        return tableCount;
    }

    public DataTable GetTableData(string databaseName, string tableName, int rowLimit = 1000)
    {
        DataTable dataTable = new DataTable();

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = $@"
                USE [{databaseName}];
                SELECT TOP {rowLimit} * 
                FROM [{tableName}]";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке данных таблицы: {ex.Message}");
        }

        return dataTable;
    }

    public List<TableInfo> GetTables(string databaseName)
    {
        List<TableInfo> tables = new List<TableInfo>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = $@"
            USE [{databaseName}];
            SELECT 
                t.name,
                s.name as schema_name,
                p.rows as row_count,
                CAST(SUM(a.used_pages) * 8.0 / 1024 AS DECIMAL(10,2)) as size_mb,
                t.create_date,
                t.modify_date
            FROM sys.tables t
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            INNER JOIN sys.indexes i ON t.object_id = i.object_id AND i.index_id <= 1
            INNER JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
            INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
            WHERE t.is_ms_shipped = 0
            GROUP BY t.name, s.name, p.rows, t.create_date, t.modify_date
            ORDER BY s.name, t.name";

            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tables.Add(new TableInfo
                    {
                        Name = reader["name"].ToString(),
                        Schema = reader["schema_name"].ToString(),
                        DatabaseName = databaseName, // Заполняем имя базы данных
                        RowCount = reader["row_count"].ToString(),
                        SizeMB = $"{reader["size_mb"]} MB",
                        CreateDate = Convert.ToDateTime(reader["create_date"]),
                        ModifyDate = Convert.ToDateTime(reader["modify_date"])
                    });
                }
            }
        }

        return tables;
    }

    public List<ColumnInfo> GetColumns(string databaseName, string tableName)
    {
        List<ColumnInfo> columns = new List<ColumnInfo>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = $@"
                USE [{databaseName}];
                SELECT 
                    c.name AS ColumnName,
                    t.name AS DataType,
                    c.max_length AS MaxLength,
                    c.is_nullable AS IsNullable,
                    CASE WHEN pk.column_id IS NOT NULL THEN 1 ELSE 0 END AS IsPrimaryKey,
                    c.is_identity AS IsIdentity,
                    OBJECT_DEFINITION(c.default_object_id) AS DefaultValue,
                    c.precision AS Precision,
                    c.scale AS Scale
                FROM sys.columns c
                INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
                LEFT JOIN (
                    SELECT ic.column_id
                    FROM sys.index_columns ic
                    INNER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                    WHERE i.is_primary_key = 1 AND ic.object_id = OBJECT_ID('{tableName}')
                ) pk ON c.column_id = pk.column_id
                WHERE c.object_id = OBJECT_ID('{tableName}')
                ORDER BY c.column_id";

            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    columns.Add(new ColumnInfo
                    {
                        Name = reader["ColumnName"].ToString(),
                        DataType = reader["DataType"].ToString(),
                        MaxLength = Convert.ToInt32(reader["MaxLength"]),
                        IsNullable = Convert.ToBoolean(reader["IsNullable"]),
                        IsPrimaryKey = Convert.ToBoolean(reader["IsPrimaryKey"]),
                        IsIdentity = Convert.ToBoolean(reader["IsIdentity"]),
                        DefaultValue = reader["DefaultValue"].ToString(),
                        Precision = Convert.ToInt32(reader["Precision"]),
                        Scale = Convert.ToInt32(reader["Scale"])
                    });
                }
            }
        }

        return columns;
    }

    public List<ForeignKeyInfo> GetForeignKeys(string databaseName, string tableName)
    {
        List<ForeignKeyInfo> foreignKeys = new List<ForeignKeyInfo>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = $@"
                USE [{databaseName}];
                SELECT 
                    fk.name AS ForeignKeyName,
                    c1.name AS ColumnName,
                    OBJECT_NAME(fk.referenced_object_id) AS ReferenceTable,
                    c2.name AS ReferenceColumn
                FROM sys.foreign_keys fk
                INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                INNER JOIN sys.columns c1 ON fkc.parent_object_id = c1.object_id AND fkc.parent_column_id = c1.column_id
                INNER JOIN sys.columns c2 ON fkc.referenced_object_id = c2.object_id AND fkc.referenced_column_id = c2.column_id
                WHERE fk.parent_object_id = OBJECT_ID('{tableName}')
                ORDER BY fk.name";

            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    foreignKeys.Add(new ForeignKeyInfo
                    {
                        Name = reader["ForeignKeyName"].ToString(),
                        ColumnName = reader["ColumnName"].ToString(),
                        ReferenceTable = reader["ReferenceTable"].ToString(),
                        ReferenceColumn = reader["ReferenceColumn"].ToString()
                    });
                }
            }
        }

        return foreignKeys;
    }

    public List<IndexInfo> GetIndexes(string databaseName, string tableName)
    {
        List<IndexInfo> indexes = new List<IndexInfo>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = $@"
                USE [{databaseName}];
                SELECT 
                    i.name AS IndexName,
                    c.name AS ColumnName,
                    i.is_unique AS IsUnique,
                    i.type_desc AS IndexType
                FROM sys.indexes i
                INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                WHERE i.object_id = OBJECT_ID('{tableName}') AND i.is_primary_key = 0
                ORDER BY i.name, ic.key_ordinal";

            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    indexes.Add(new IndexInfo
                    {
                        Name = reader["IndexName"].ToString(),
                        ColumnName = reader["ColumnName"].ToString(),
                        IsUnique = Convert.ToBoolean(reader["IsUnique"]),
                        IsClustered = reader["IndexType"].ToString() == "CLUSTERED"
                    });
                }
            }
        }

        return indexes;
    }

    public List<StoredProcedureInfo> GetStoredProcedures(string databaseName)
    {
        List<StoredProcedureInfo> procedures = new List<StoredProcedureInfo>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = $@"
                USE [{databaseName}];
                SELECT 
                    name,
                    SCHEMA_NAME(schema_id) as schema_name,
                    create_date,
                    modify_date
                FROM sys.procedures 
                WHERE is_ms_shipped = 0
                ORDER BY schema_name, name";

            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    procedures.Add(new StoredProcedureInfo
                    {
                        Name = reader["name"].ToString(),
                        Schema = reader["schema_name"].ToString(),
                        CreateDate = Convert.ToDateTime(reader["create_date"]),
                        ModifyDate = Convert.ToDateTime(reader["modify_date"])
                    });
                }
            }
        }

        return procedures;
    }

    public DataTable ExecuteQuery(string databaseName, string query)
    {
        DataTable result = new DataTable();

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                if (!string.IsNullOrEmpty(databaseName))
                {
                    using (SqlCommand useCommand = new SqlCommand($"USE [{databaseName}]", connection))
                    {
                        useCommand.ExecuteNonQuery();
                    }
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка выполнения запроса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return result;
    }

    public int ExecuteNonQuery(string databaseName, string query)
    {
        int rowsAffected = 0;

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                if (!string.IsNullOrEmpty(databaseName))
                {
                    using (SqlCommand useCommand = new SqlCommand($"USE [{databaseName}]", connection))
                    {
                        useCommand.ExecuteNonQuery();
                    }
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    rowsAffected = command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка выполнения запроса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return rowsAffected;
    }

    public object ExecuteScalar(string databaseName, string query)
    {
        object result = null;

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                if (!string.IsNullOrEmpty(databaseName))
                {
                    using (SqlCommand useCommand = new SqlCommand($"USE [{databaseName}]", connection))
                    {
                        useCommand.ExecuteNonQuery();
                    }
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    result = command.ExecuteScalar();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка выполнения запроса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return result;
    }

    public DataTable GetDatabaseStructureTable()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Параметр", typeof(string));

        try
        {
            if (!isConnected) return dt;

            // Получаем информацию о базах данных
            var databases = cachedDatabases;

            // Добавляем колонки для каждой базы данных
            foreach (DatabaseInfo db in databases)
            {
                dt.Columns.Add(db.Name, typeof(string));
            }

            // Добавляем строку с размером баз данных
            DataRow sizeRow = dt.NewRow();
            sizeRow["Параметр"] = "Размер";
            for (int i = 0; i < databases.Count; i++)
            {
                sizeRow[i + 1] = databases[i].Size;
            }
            dt.Rows.Add(sizeRow);

            // Добавляем строку с количеством таблиц
            DataRow tableCountRow = dt.NewRow();
            tableCountRow["Параметр"] = "Количество таблиц";
            for (int i = 0; i < databases.Count; i++)
            {
                tableCountRow[i + 1] = databases[i].TableCount.ToString();
            }
            dt.Rows.Add(tableCountRow);

            // Добавляем строку с моделью восстановления
            DataRow recoveryRow = dt.NewRow();
            recoveryRow["Параметр"] = "Модель восстановления";
            for (int i = 0; i < databases.Count; i++)
            {
                recoveryRow[i + 1] = databases[i].RecoveryModel;
            }
            dt.Rows.Add(recoveryRow);

            // Добавляем строку с состоянием
            DataRow stateRow = dt.NewRow();
            stateRow["Параметр"] = "Состояние";
            for (int i = 0; i < databases.Count; i++)
            {
                stateRow[i + 1] = databases[i].State;
            }
            dt.Rows.Add(stateRow);

            // Добавляем строку с датой создания
            DataRow createDateRow = dt.NewRow();
            createDateRow["Параметр"] = "Дата создания";
            for (int i = 0; i < databases.Count; i++)
            {
                createDateRow[i + 1] = databases[i].CreateDate.ToString("yyyy-MM-dd");
            }
            dt.Rows.Add(createDateRow);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при создании таблицы структуры: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return dt;
    }

    public void RefreshCache()
    {
        CacheDatabases();
    }

    public DateTime GetLastCacheUpdate()
    {
        return lastCacheUpdate;
    }

    public List<DatabaseInfo> GetCachedDatabases()
    {
        return cachedDatabases;
    }

    public DatabaseInfo FindDatabase(string databaseName)
    {
        return cachedDatabases.FirstOrDefault(db => db.Name.Equals(databaseName, StringComparison.OrdinalIgnoreCase));
    }

    public TableInfo FindTable(string databaseName, string tableName)
    {
        var tables = GetTables(databaseName);
        return tables.FirstOrDefault(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
    }
}
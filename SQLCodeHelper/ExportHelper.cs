using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using ClosedXML.Excel; 

public static class ExportHelper
{
    public static void ExportToExcel(DataTable dataTable)
    {
        try
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(dataTable, "Данные");
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Сохранить как Excel файл"
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                    MessageBox.Show("Данные успешно экспортированы!");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при экспорте: {ex.Message}");
        }
    }
    public static void ExportToSQL(DataTable dataTable, string connectionString)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
        {
            MessageBox.Show("Нет данных для экспорта.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using (var dialog = new ExportToSqlDialog(connectionString))
        {
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            string databaseName = dialog.SelectedDatabase;
            string tableName = dialog.TableName;
            bool autoOverwrite = dialog.AutoOverwrite;

            if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("Необходимо указать имя базы данных и таблицы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Проверка существования БД
                    if (!DatabaseExists(conn, databaseName))
                    {
                        MessageBox.Show($"База данных '{databaseName}' не существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    conn.ChangeDatabase(databaseName);
                    bool tableExists = TableExists(conn, tableName);

                    bool proceed = false;
                    bool replaceTable = false;

                    if (tableExists)
                    {
                        if (autoOverwrite)
                        {
                            replaceTable = true;
                            proceed = true;
                        }
                        else
                        {
                            DialogResult res = MessageBox.Show(
                                $"Таблица '{tableName}' уже существует в базе '{databaseName}'. Заменить данные (удалить таблицу и создать заново)?",
                                "Подтверждение замены",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);
                            if (res == DialogResult.Yes)
                            {
                                replaceTable = true;
                                proceed = true;
                            }
                            else
                            {
                                proceed = false;
                            }
                        }
                    }
                    else
                    {
                        // Таблицы нет – можно создавать
                        proceed = true;
                        replaceTable = false; // не нужно удалять
                    }

                    if (!proceed) return;

                    // Если требуется замена – удаляем таблицу
                    if (replaceTable)
                    {
                        DropTable(conn, tableName);
                    }

                    // Создаём таблицу (если её нет или после удаления)
                    CreateTableFromDataTable(conn, tableName, dataTable);

                    // Вставляем данные
                    InsertData(conn, tableName, dataTable);

                    MessageBox.Show($"Данные успешно экспортированы в таблицу '{tableName}' базы данных '{databaseName}'.",
                                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private static bool DatabaseExists(SqlConnection connection, string databaseName)
    {
        string query = "SELECT 1 FROM sys.databases WHERE name = @dbName";
        using (SqlCommand cmd = new SqlCommand(query, connection))
        {
            cmd.Parameters.AddWithValue("@dbName", databaseName);
            return cmd.ExecuteScalar() != null;
        }
    }

    private static bool TableExists(SqlConnection connection, string tableName)
    {
        string query = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName";
        using (SqlCommand cmd = new SqlCommand(query, connection))
        {
            cmd.Parameters.AddWithValue("@tableName", tableName);
            return cmd.ExecuteScalar() != null;
        }
    }

    private static void DropTable(SqlConnection connection, string tableName)
    {
        string dropQuery = $"DROP TABLE IF EXISTS [{tableName}]";
        using (SqlCommand cmd = new SqlCommand(dropQuery, connection))
        {
            cmd.ExecuteNonQuery();
        }
    }

    private static void CreateTableFromDataTable(SqlConnection connection, string tableName, DataTable dataTable)
    {
        StringBuilder createSql = new StringBuilder($"CREATE TABLE [{tableName}] (");
        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            DataColumn col = dataTable.Columns[i];
            string columnName = col.ColumnName.Replace("]", "]]");
            string sqlType = MapTypeToSql(col, dataTable);
            string nullable = col.AllowDBNull ? "NULL" : "NOT NULL";
            createSql.Append($"[{columnName}] {sqlType} {nullable}");
            if (i < dataTable.Columns.Count - 1)
                createSql.Append(", ");
        }
        createSql.Append(")");

        using (SqlCommand cmd = new SqlCommand(createSql.ToString(), connection))
        {
            cmd.ExecuteNonQuery();
        }
    }

    private static string MapTypeToSql(DataColumn column, DataTable dataTable)
    {
        Type type = column.DataType;
        if (type == typeof(int)) return "INT";
        if (type == typeof(long)) return "BIGINT";
        if (type == typeof(short)) return "SMALLINT";
        if (type == typeof(byte)) return "TINYINT";
        if (type == typeof(decimal)) return "DECIMAL(18,2)";
        if (type == typeof(float)) return "FLOAT";
        if (type == typeof(double)) return "FLOAT";
        if (type == typeof(bool)) return "BIT";
        if (type == typeof(DateTime)) return "DATETIME";
        if (type == typeof(Guid)) return "UNIQUEIDENTIFIER";
        if (type == typeof(byte[])) return "VARBINARY(MAX)";
        if (type == typeof(string))
        {
            int maxLen = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                if (!row.IsNull(column))
                {
                    string val = row[column].ToString();
                    if (val.Length > maxLen) maxLen = val.Length;
                }
            }
            if (maxLen > 4000 || maxLen == 0)
                return "NVARCHAR(MAX)";
            else
                return $"NVARCHAR({maxLen})";
        }
        return "NVARCHAR(MAX)";
    }

    private static void InsertData(SqlConnection connection, string tableName, DataTable dataTable)
    {
        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
        {
            bulkCopy.DestinationTableName = tableName;
            bulkCopy.BatchSize = 1000;
            bulkCopy.BulkCopyTimeout = 300;
            try
            {
                bulkCopy.WriteToServer(dataTable);
            }
            catch
            {
                // Если BulkCopy не подходит – пробуем построчно
                InsertDataRowByRow(connection, tableName, dataTable);
            }
        }
    }

    private static void InsertDataRowByRow(SqlConnection connection, string tableName, DataTable dataTable)
    {
        using (SqlTransaction transaction = connection.BeginTransaction())
        {
            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    StringBuilder insertSql = new StringBuilder($"INSERT INTO [{tableName}] (");
                    StringBuilder values = new StringBuilder("VALUES (");
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        string colName = dataTable.Columns[i].ColumnName.Replace("]", "]]");
                        insertSql.Append($"[{colName}]");
                        values.Append($"@p{i}");
                        if (i < dataTable.Columns.Count - 1)
                        {
                            insertSql.Append(", ");
                            values.Append(", ");
                        }
                    }
                    insertSql.Append(") ");
                    values.Append(")");
                    string fullSql = insertSql.ToString() + values.ToString();

                    using (SqlCommand cmd = new SqlCommand(fullSql, connection, transaction))
                    {
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            object value = row[i];
                            cmd.Parameters.AddWithValue($"@p{i}", value ?? DBNull.Value);
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
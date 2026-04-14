using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

public partial class ExportToSqlDialog : Form
{
    private string connectionString;
    private ComboBox cmbDatabase;
    private ComboBox cmbTableName;  
    private CheckBox chkOverwrite;
    private Button btnOk;
    private Button btnCancel;
    private Label lblDatabase;
    private Label lblTable;
    private Label lblOverwrite;

    public string SelectedDatabase => cmbDatabase.SelectedItem?.ToString()?.Split('|')[0].Trim(); // убираем возможный суффикс
    public string TableName => cmbTableName.Text.Trim();
    public bool AutoOverwrite => chkOverwrite.Checked;

    public ExportToSqlDialog(string connectionString)
    {
        this.connectionString = connectionString;
        InitializeComponents();
        LoadDatabases();
        cmbDatabase.SelectedIndexChanged += CmbDatabase_SelectedIndexChanged;
        cmbTableName.DropDownStyle = ComboBoxStyle.DropDown; // разрешаем ввод и выбор
        cmbTableName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        cmbTableName.AutoCompleteSource = AutoCompleteSource.ListItems;
    }

    private void InitializeComponents()
    {
        this.Text = "Экспорт в SQL";
        this.Size = new Size(450, 250);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        lblDatabase = new Label() { Text = "База данных:", Left = 20, Top = 20, Width = 120 };
        cmbDatabase = new ComboBox() { Left = 150, Top = 17, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

        lblTable = new Label() { Text = "Имя таблицы:", Left = 20, Top = 60, Width = 120 };
        cmbTableName = new ComboBox() { Left = 150, Top = 57, Width = 250 };

        lblOverwrite = new Label() { Text = "Авто-перезапись:", Left = 20, Top = 100, Width = 120 };
        chkOverwrite = new CheckBox() { Text = "Перезаписывать таблицу без подтверждения", Left = 150, Top = 97, Width = 250, Checked = false };

        btnOk = new Button() { Text = "OK", Left = 230, Top = 150, Width = 80, DialogResult = DialogResult.OK };
        btnCancel = new Button() { Text = "Отмена", Left = 320, Top = 150, Width = 80, DialogResult = DialogResult.Cancel };

        this.Controls.AddRange(new Control[] { lblDatabase, cmbDatabase, lblTable, cmbTableName, lblOverwrite, chkOverwrite, btnOk, btnCancel });
        this.AcceptButton = btnOk;
        this.CancelButton = btnCancel;
    }

    private void LoadDatabases()
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Исключаем системные БД (database_id > 4)
                string query = @"
                SELECT name, state, state_desc 
                FROM sys.databases 
                WHERE database_id > 4 
                ORDER BY name";
                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                List<string> databases = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    string dbName = row["name"].ToString();
                    int stateCode = Convert.ToInt32(row["state"]);
                    string stateDesc = row["state_desc"].ToString();

                    string displayName = dbName;
                    // Состояние 6 = RESTORING (ожидает восстановления) – не зависит от языка
                    if (stateCode == 6)
                        displayName = dbName + " (ожидает восстановления)";
                    else if (stateDesc == "RECOVERING")
                        displayName = dbName + " (восстанавливается)";
                    else if (stateDesc == "OFFLINE")
                        displayName = dbName + " (OFFLINE)";

                    databases.Add(displayName);
                }
                cmbDatabase.DataSource = databases;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки списка баз данных: {ex.Message}");
        }
    }

    private string GetDatabaseState(SqlConnection connection, string dbName)
    {
        string query = "SELECT state_desc FROM sys.databases WHERE name = @dbName";
        using (SqlCommand cmd = new SqlCommand(query, connection))
        {
            cmd.Parameters.AddWithValue("@dbName", dbName);
            object result = cmd.ExecuteScalar();
            return result?.ToString().ToUpper() ?? "UNKNOWN";
        }
    }

    private void CmbDatabase_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbDatabase.SelectedItem == null) return;
        string dbName = SelectedDatabase;
        if (string.IsNullOrEmpty(dbName)) return;

        LoadTablesForDatabase(dbName);
    }

    private void LoadTablesForDatabase(string databaseName)
    {
        cmbTableName.Items.Clear();
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                conn.ChangeDatabase(databaseName);
                string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbTableName.Items.Add(reader["TABLE_NAME"].ToString());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки списка таблиц: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using static SQLCodeHelper.WorkSQL;

namespace SQLCodeHelper
{
    public partial class SQLHelper : Form
    {
        WorkRPN RPN = new WorkRPN();
        WorkSQL SQL = new WorkSQL();
        ConSQL con = new ConSQL();
        Error err = new Error();

        public SQLHelper()
        {
            InitializeComponent();
            tabControl.DrawItem += new DrawItemEventHandler(tabControl_DrawItem);
            ConfigureInterface();
            SetupGridContextMenus();
        }
        //Interface
        private void ConfigureInterface()
        {
            //настройка таблицы
            DataTable currentDataTable = new DataTable();
            currentDataTable.Columns.Add("Column1");
            TableAndKeys.DataSource = currentDataTable;
            TableAndKeys.Columns[0].HeaderText = "";

            // Настройка разделения окна
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer1.SplitterDistance = 300; // Ширина дерева
            splitContainer2.SplitterDistance = 135;
            splitContainer2.FixedPanel = FixedPanel.Panel1; // Фиксируем нижнюю панель (где кнопка)
            splitContainer2.IsSplitterFixed = true;         // Запрещаем пользователю изменять размер
            // Настройка TreeView с системными иконками
            treeView1.Dock = DockStyle.Fill;
            treeView1.ImageList = new ImageList();
            treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;
            treeView1.ImageList.ImageSize = new Size(16, 16);

            // Создаем простые иконки программно
            treeView1.ImageList.Images.Add("Server", CreateIcon(Color.Blue, "S"));
            treeView1.ImageList.Images.Add("Database", CreateIcon(Color.Green, "DB"));
            treeView1.ImageList.Images.Add("Table", CreateIcon(Color.Orange, "T"));
            treeView1.ImageList.Images.Add("Column", CreateIcon(Color.Gray, "C"));
            treeView1.ImageList.Images.Add("Key", CreateIcon(Color.Red, "K"));

            treeView1.AfterSelect += TreeView1_AfterSelect;
            treeView1.BeforeExpand += TreeView1_BeforeExpand;

            // Контекстное меню для дерева
            ContextMenuStrip treeMenu = new ContextMenuStrip();
            treeMenu.Items.Add("Удалить", null, DeleteTreeNode_Click);
            treeView1.ContextMenuStrip = treeMenu;

            // Настройка TabControl
            tabControl1.Dock = DockStyle.Fill;

            // Добавление вкладок
            tabControl1.TabPages.Add("Данные", "Данные");
            tabControl1.TabPages.Add("Структура", "Структура");
            tabControl1.TabPages.Add("Сохранённые действия", "Сохранённые действия");

            // Настройка DataGridView на вкладке "Структура"
            dgv.Dock = DockStyle.Fill;
            tabControl1.TabPages["Структура"].Controls.Add(dgv);

            // Настройка DataGridView на вкладке "Данные"
            TableAndKeys.Dock = DockStyle.Fill;
            tabControl1.TabPages["Данные"].Controls.Add(TableAndKeys);

            // Настройка DataGridView на вкладке "Данные"
            TableAndKeys.Dock = DockStyle.Fill;
            tabControl1.TabPages["Сохранённые действия"].Controls.Add(dwas);

            // Настройка панели инструментов
            toolStrip1.Dock = DockStyle.Top;
            toolStrip1.Items.Add(new ToolStripButton("Подключится", null, Connect_button_Click));
            toolStrip1.Items.Add(new ToolStripButton("Обновить", null, Refresh_Click));
            toolStrip1.Items.Add(new ToolStripButton("Экспорт данных таблиц в Excel", null, Export_Click));
            toolStrip1.Items.Add(new ToolStripButton("Экспорт данных таблиц в SQL", null, ExportSQL_Click));
            toolStrip1.Items.Add(new ToolStripButton("Создать БД", null, CreateDatabase_Click));
            toolStrip1.Items.Add(new ToolStripSeparator());

            // Перечень используемых таблиц
            dwas.Dock = DockStyle.Fill;

            // Настройка labelActionT и checkbox
            labelActionT.Text = "Активная база данных: не выбрана";

            checkBoxFix.Text = "Зафиксировать базу данных";
            checkBoxFix.AutoSize = true;
        }

        private Bitmap CreateIcon(Color color, string text)
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(color);
                using (Font font = new Font("Arial", 6, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    SizeF textSize = g.MeasureString(text, font);
                    g.DrawString(text, font, brush,
                        (16 - textSize.Width) / 2,
                        (16 - textSize.Height) / 2);
                }
            }
            return bmp;
        }
        //Work buttons
        private void GenerateSql_Click(object sender, EventArgs e)
        {
            string basename = GetActiveDatabaseName();  // новый метод
            if (string.IsNullOrEmpty(basename))
            {
                MessageBox.Show("Не выбрана активная база данных. Выберите базу в дереве.",
                                "Ошибка", MessageBoxButtons.OK);
                return;
            }

            string[][] arr = GetAllTablesStructure(basename);  // новый метод
            if (SourceValidationPoint(arr, EquationWithBase.Text.Replace(" ", ""), basename, out string str))
            {
                ExecuteQueryAndDisplay(str);
                dwas.DataSource = SQL.GetQueriesFromDatabase(); // обновляем список сохранённых запросов
            }
        }
        private void ViewSQLbutton_Click_1(object sender, EventArgs e)
        {
            string[] arrst = richTable.Text.Replace(" ", "").Split('\n');
            string[][] arr = new string[arrst.Length][]; 
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = arrst[i].Split('/', ',');
            }
            SourceValidationPoint(arr, Equation.Text.Replace(" ", ""), NameBase.Text, out string str);
            richTextBox2.Text = str;
            dwast.DataSource = SQL.GetQueriesFromDatabase();
        }
        //Logic
        private bool SourceValidationPoint(string[][] arr, string Equation,string name, out string str)
        {
            Equation = Equation.Replace(" ", "");
            str = "";
            err.Error_log(name, Equation, TableAndKeys, out string errorMessage, out bool hasError);
            if (hasError)
            {
                MessageBox.Show($"Ошибка при построении: {errorMessage}", "Предупреждение", MessageBoxButtons.OK);
                return false;
            }
            else
            {
                str = $"USE {name}\r\n" + SQL.CodeCreate(RPN.Do(Equation), arr, name);
                return true;
            }
        }
        /// <summary>
        /// Выполняет SQL-запрос на указанной базе данных и отображает результат в TableAndKeys.
        /// </summary>
        /// <param name="sql">Текст SQL-запроса</param>
        private void ExecuteQueryAndDisplay(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                MessageBox.Show("Запрос пуст!", "Ошибка", MessageBoxButtons.OK);
                return;
            }
            string basename = sql.Substring(4, sql.IndexOf("\r\n") - 4).Trim();
            if (string.IsNullOrEmpty(basename))
            {
                MessageBox.Show("Выберите базу данных!", "Ошибка", MessageBoxButtons.OK);
                return;
            }

            try
            {
                DataTable result = con.ExecuteQuery(basename, sql);
                TableAndKeys.DataSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения запроса: {ex.Message}", "Ошибка", MessageBoxButtons.OK);
            }
        }
        //быстрая работа с данными
        /// <summary>Возвращает имя активной базы из labelActionT (или из treeview)</summary>
        private string GetActiveDatabaseName()
        {
            string labelText = labelActionT.Text;
            if (labelText.StartsWith("активная база данных: "))
            {
                string dbName = labelText.Substring("активная база данных: ".Length).Trim();
                return string.IsNullOrEmpty(dbName) ? null : dbName;
            }
            return null;
        }

        /// <summary>Возвращает массив определений всех таблиц базы данных</summary>
        private string[][] GetAllTablesStructure(string databaseName)
        {
            var tables = con.GetTables(databaseName);
            var result = new List<string[]>();

            foreach (var table in tables)
            {
                var columns = con.GetColumns(databaseName, table.Name);
                var columnNames = columns.Select(c => c.Name).ToArray();
                var tableDef = new string[columnNames.Length + 1];
                tableDef[0] = table.Name;
                Array.Copy(columnNames, 0, tableDef, 1, columnNames.Length);
                result.Add(tableDef);
            }

            return result.ToArray();
        }
        //Database Conect
        private void Connect_button_Click(object sender, EventArgs e)
        {
            Extra extraForm = new Extra(con);
            if (extraForm.ShowDialog() == DialogResult.OK)
            {
                LoadDatabaseTree();
                tabControl.SelectTab(UpdatePage);
            }
        }
        private void LoadDatabaseTree()
        {
            treeView1.Nodes.Clear();

            TreeNode serverNode = new TreeNode(con.GetServerName(), 0, 0);
            treeView1.Nodes.Add(serverNode);

            foreach (var db in con.GetDatabases())
            {
                TreeNode dbNode = new TreeNode(db.Name, 1, 1);
                dbNode.Tag = db;

                // Узел "Таблицы" с фиктивным дочерним узлом, чтобы отображался знак «+»
                TreeNode tablesNode = new TreeNode("Таблицы", 2, 2);
                tablesNode.Tag = "TablesFolder";
                tablesNode.Nodes.Add(new TreeNode("Загрузка...")); // фиктивный узел
                dbNode.Nodes.Add(tablesNode);

                serverNode.Nodes.Add(dbNode);
            }

            serverNode.Expand();
        }

        private void LoadTablesForDatabase(DatabaseInfo db, TreeNode tablesNode)
        {
            tablesNode.Nodes.Clear(); // удаляем фиктивный узел

            foreach (var table in con.GetTables(db.Name))
            {
                TreeNode tableNode = new TreeNode(table.Name, 2, 2);
                tableNode.Tag = table;
                // Добавляем фиктивный узел для отложенной загрузки столбцов
                tableNode.Nodes.Add(new TreeNode("Загрузка..."));
                tablesNode.Nodes.Add(tableNode);
            }

            tablesNode.Expand(); // раскрываем папку "Таблицы" после загрузки
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Обновляем метку, если checkbox не фиксирует
            if (!checkBoxFix.Checked)
            {
                if (e.Node.Tag is DatabaseInfo db)
                {
                    labelActionT.Text = $"активная база данных: {db.Name}";
                }
                else if (e.Node.Tag is TableInfo table)
                {
                    labelActionT.Text = $"активная база данных: {table.DatabaseName}";
                }
            }

            // Обработка выбора узла
            if (e.Node.Tag is DatabaseInfo dbInfo)
            {
                ShowDatabaseInfo(dbInfo);
            }
            else if (e.Node.Tag is TableInfo tableInfo)
            {
                DataTable data = con.GetTableData(tableInfo.DatabaseName, tableInfo.Name);
                TableAndKeys.Columns.Clear();
                TableAndKeys.DataSource = data;
            }
        }

        private void TreeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // Если раскрываем узел "Таблицы" и у него есть фиктивный узел "Загрузка..."
            if (e.Node.Tag as string == "TablesFolder" && e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "Загрузка...")
            {
                LoadTablesForDatabase(e.Node.Parent.Tag as DatabaseInfo, e.Node);
            }
            // Если раскрываем узел таблицы и у него есть фиктивный узел "Загрузка..."
            else if (e.Node.Tag is TableInfo table && e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "Загрузка...")
            {
                // Удаляем фиктивный узел
                e.Node.Nodes.Clear();

                // Загружаем столбцы
                List<ColumnInfo> columns = con.GetColumns(table.DatabaseName, table.Name);

                TreeNode structureNode = new TreeNode("Столбцы");
                structureNode.ImageIndex = 4;
                structureNode.SelectedImageIndex = 4;

                foreach (ColumnInfo column in columns)
                {
                    string keyInfo = column.IsPrimaryKey ? " (PK)" : "";
                    TreeNode columnNode = new TreeNode($"{column.Name} ({column.DataType}){keyInfo}");
                    columnNode.ImageIndex = 3;
                    columnNode.SelectedImageIndex = 3;
                    structureNode.Nodes.Add(columnNode);
                }

                e.Node.Nodes.Add(structureNode);
            }
        }
        /// <summary>Создание новой базы данных</summary>
        private void CreateDatabase_Click(object sender, EventArgs e)
        {
            string dbName = Microsoft.VisualBasic.Interaction.InputBox("Введите имя новой базы данных:", "Создание БД", "");
            if (string.IsNullOrWhiteSpace(dbName))
                return;

            // Простейшая валидация имени
            if (dbName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0 || dbName.Contains(" "))
            {
                MessageBox.Show("Имя базы данных содержит недопустимые символы или пробелы.", "Ошибка", MessageBoxButtons.OK);
                return;
            }

            if (con.CreateDatabase(dbName))
            {
                LoadDatabaseTree(); // перезагружаем дерево
                MessageBox.Show($"База данных '{dbName}' успешно создана.", "Успех", MessageBoxButtons.OK);
            }
        }

        /// <summary>Обработчик контекстного меню (удаление)</summary>
        private void DeleteTreeNode_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode == null) return;

            // Удаление базы данных
            if (selectedNode.Tag is DatabaseInfo db)
            {
                DialogResult result = MessageBox.Show($"Вы действительно хотите удалить базу данных '{db.Name}'?\nЭто действие необратимо!",
                                                        "Подтверждение удаления",
                                                        MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (con.DropDatabase(db.Name))
                    {
                        LoadDatabaseTree();
                        // Очищаем отображение, если активная БД была удалена
                        if (labelActionT.Text.Contains(db.Name))
                            labelActionT.Text = "активная база данных: не выбрана";
                        MessageBox.Show($"База данных '{db.Name}' удалена.", "Успех", MessageBoxButtons.OK);
                    }
                }
            }
            // Удаление таблицы
            else if (selectedNode.Tag is TableInfo table)
            {
                DialogResult result = MessageBox.Show($"Удалить таблицу '{table.Name}' из базы '{table.DatabaseName}'?",
                                                        "Подтверждение удаления",
                                                        MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (con.DropTable(table.DatabaseName, table.Name))
                    {
                        // Обновляем дерево: перезагружаем узел базы данных
                        TreeNode dbNode = selectedNode.Parent?.Parent; // папка "Таблицы" -> узел БД
                        if (dbNode != null && dbNode.Tag is DatabaseInfo dbInfo)
                        {
                            // Находим папку "Таблицы" и перезагружаем её
                            TreeNode tablesNode = dbNode.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Tag as string == "TablesFolder");
                            if (tablesNode != null)
                            {
                                LoadTablesForDatabase(dbInfo, tablesNode);
                            }
                        }
                        // Очищаем DataGridView, если отображалась удалённая таблица
                        if (TableAndKeys.DataSource != null && TableAndKeys.DataSource is DataTable dt)
                        {
                            // Просто сбросим источник
                            TableAndKeys.DataSource = null;
                        }
                        MessageBox.Show($"Таблица '{table.Name}' удалена.", "Успех", MessageBoxButtons.OK);
                    }
                }
            }
            else
            {
                MessageBox.Show("Удаление этого элемента не поддерживается.", "Информация", MessageBoxButtons.OK);
            }
        }
        private void ShowDatabaseInfo(DatabaseInfo db)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Параметр");
            dt.Columns.Add("Значение");

            dt.Rows.Add("Имя базы данных", db.Name);
            dt.Rows.Add("Размер", db.Size);
            dt.Rows.Add("Дата создания", db.CreateDate);

            // Получаем актуальное количество таблиц
            int tableCount = con.GetTableCount(db.Name);
            dt.Rows.Add("Количество таблиц", tableCount);

            dgv.DataSource = dt;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            if (con.IsConnected())
            {
                LoadDatabaseTree();
                MessageBox.Show("Данные обновлены!");
            }
            else
            {
                MessageBox.Show("Нет активного подключения к серверу.");
            }
        }

        private void Export_Click(object sender, EventArgs e)
        {
            TableAndKeys.EndEdit();
            this.ActiveControl = null;
            // Реализация экспорта данных
            if (TableAndKeys.DataSource != null)
            {
                ExportHelper.ExportToExcel((DataTable)TableAndKeys.DataSource);
            }
        }
        private void ExportSQL_Click(object sender, EventArgs e)
        {
            TableAndKeys.EndEdit();
            this.ActiveControl = null;
            if (TableAndKeys.DataSource != null && TableAndKeys.DataSource is DataTable dt)
            {
                // Передаём строку подключения через метод GetConnectionString()
                ExportHelper.ExportToSQL(dt, con.GetConnectionString());
            }
            else
            {
                MessageBox.Show("Нет данных для экспорта.", "Информация", MessageBoxButtons.OK);
            }
        }
        private void tabControl_DrawItem(Object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush _textBrush;

            // Get the item from the collection.
            TabPage _tabPage = tabControl.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle _tabBounds = tabControl.GetTabRect(e.Index);
            _textBrush = new System.Drawing.SolidBrush(e.ForeColor);
            e.DrawBackground();

            // Use our own font.
            Font _tabFont = new Font("Arial", 13.0f, FontStyle.Bold, GraphicsUnit.Pixel);
            // Draw string. Center the text.
            StringFormat _stringFlags = new StringFormat();
            _stringFlags.Alignment = StringAlignment.Center;
            _stringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));
        }

        private void checkBoxFix_CheckedChanged(object sender, EventArgs e)
        {
            // Если галочка снята, обновляем метку на текущий выбранный элемент
            if (!checkBoxFix.Checked && treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Tag is DatabaseInfo db)
                    labelActionT.Text = $"активная база данных: {db.Name}";
                else if (treeView1.SelectedNode.Tag is TableInfo table)
                    labelActionT.Text = $"активная база данных: {table.DatabaseName}";
                else
                    labelActionT.Text = "активная база данных: не выбрана";
            }
        }

        private void dwas_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (dwas.CurrentRow != null)
            {
                var queryInfo = dwas.CurrentRow.DataBoundItem as QueryInfo;
                string sql = $"USE {queryInfo.BaseName}\r\n{queryInfo.SQL}";
                if (queryInfo != null && !string.IsNullOrEmpty(sql))
                {

                    ExecuteQueryAndDisplay(sql);
                }
            }
        }

        // При инициализации формы или после привязки данных
        private void SetupGridContextMenus()
        {
            // Меню для заголовков столбцов
            ContextMenuStrip columnMenu = new ContextMenuStrip();
            columnMenu.Items.Add("Добавить столбец", null, AddColumn_Click);
            columnMenu.Items.Add("Переименовать столбец", null, RenameColumn_Click);
            columnMenu.Items.Add("Удалить столбец", null, DeleteColumn_Click);

            TableAndKeys.ColumnHeaderMouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right && e.ColumnIndex >= 0)
                {
                    TableAndKeys.CurrentCell = TableAndKeys.Rows[0].Cells[e.ColumnIndex];
                    columnMenu.Show(TableAndKeys, e.Location);
                }
            };

            // Меню для строк (по клику на ячейку)
            ContextMenuStrip rowMenu = new ContextMenuStrip();
            rowMenu.Items.Add("Удалить выбранную строку", null, DeleteRow_Click);

            TableAndKeys.CellMouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    TableAndKeys.ClearSelection();
                    TableAndKeys.Rows[e.RowIndex].Selected = true;
                    TableAndKeys.CurrentCell = TableAndKeys.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    rowMenu.Show(TableAndKeys, e.Location);
                }
            };
        }

        private void AddColumn_Click(object sender, EventArgs e)
        {
            string newColumnName = ShowInputDialog("Введите имя нового столбца:", "Добавление столбца");
            if (string.IsNullOrWhiteSpace(newColumnName)) return;

            if (TableAndKeys.DataSource is DataTable dt)
            {
                dt.Columns.Add(newColumnName, typeof(string));
            }
            else
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
                {
                    Name = newColumnName,
                    HeaderText = newColumnName,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                TableAndKeys.Columns.Add(column);
            }
        }

        private void RenameColumn_Click(object sender, EventArgs e)
        {
            if (TableAndKeys.CurrentCell == null) return;
            DataGridViewColumn column = TableAndKeys.CurrentCell.OwningColumn;
            string newName = ShowInputDialog("Введите новое имя столбца:", "Переименование", column.HeaderText);
            if (string.IsNullOrWhiteSpace(newName)) return;

            column.HeaderText = newName;
            if (TableAndKeys.DataSource is DataTable dt && dt.Columns.Contains(column.Name))
            {
                dt.Columns[column.Name].ColumnName = newName;
            }
            column.Name = newName;
        }

        private void DeleteColumn_Click(object sender, EventArgs e)
        {
            if (TableAndKeys.CurrentCell == null) return;
            DataGridViewColumn column = TableAndKeys.CurrentCell.OwningColumn;

            // Защита от удаления важных столбцов (при необходимости)
            if (column.Name == "ID" || column.ReadOnly)
            {
                MessageBox.Show("Этот столбец нельзя удалить.", "Предупреждение", MessageBoxButtons.OK);
                return;
            }

            DialogResult result = MessageBox.Show($"Удалить столбец '{column.HeaderText}'?", "Подтверждение",
                MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (TableAndKeys.DataSource is DataTable dt && dt.Columns.Contains(column.Name))
                {
                    dt.Columns.Remove(column.Name);
                }
                else
                {
                    TableAndKeys.Columns.Remove(column);
                }
            }
        }
        private void DeleteRow_Click(object sender, EventArgs e)
        {
            if (TableAndKeys.SelectedRows.Count == 0) return;
            DataGridViewRow row = TableAndKeys.SelectedRows[0];
            if (row.IsNewRow) return; // нельзя удалить строку для новых записей

            DialogResult result = MessageBox.Show("Удалить выбранную строку?", "Подтверждение",
                MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (TableAndKeys.DataSource is DataTable dt)
                {
                    // Удаляем из источника данных
                    DataRowView drv = (DataRowView)row.DataBoundItem;
                    dt.Rows.Remove(drv.Row);
                }
                else
                {
                    TableAndKeys.Rows.Remove(row);
                }
            }
        }
        // Простой диалог ввода (если нет Microsoft.VisualBasic)
        private string ShowInputDialog(string prompt, string title, string defaultValue = "")
        {
            using (Form form = new Form())
            {
                Label label = new Label() { Text = prompt, Left = 10, Top = 10, Width = 260 };
                TextBox textBox = new TextBox() { Left = 10, Top = 35, Width = 260, Text = defaultValue };
                Button okButton = new Button() { Text = "OK", Left = 110, Top = 70, Width = 70, DialogResult = DialogResult.OK };
                Button cancelButton = new Button() { Text = "Отмена", Left = 190, Top = 70, Width = 70, DialogResult = DialogResult.Cancel };
                form.Controls.AddRange(new Control[] { label, textBox, okButton, cancelButton });
                form.ClientSize = new System.Drawing.Size(290, 110);
                form.Text = title;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.AcceptButton = okButton;
                form.CancelButton = cancelButton;

                if (form.ShowDialog() == DialogResult.OK)
                    return textBox.Text;
                return null;
            }
        }
    }
}
using SQLCodeHelper.Properties;

namespace SQLCodeHelper
{
    partial class SQLHelper
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ViewSQLbutton = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.Equation = new System.Windows.Forms.TextBox();
            this.NameBase = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.UpdatePage = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.checkBoxFix = new System.Windows.Forms.CheckBox();
            this.labelActionT = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.EquationWithBase = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.GenerateSql = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.dwas = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TableAndKeys = new System.Windows.Forms.DataGridView();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.GeneralPage = new System.Windows.Forms.TabPage();
            this.dwast = new System.Windows.Forms.DataGridView();
            this.richTable = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.HelpPage = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.UpdatePage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dwas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TableAndKeys)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.GeneralPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dwast)).BeginInit();
            this.HelpPage.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ViewSQLbutton
            // 
            this.ViewSQLbutton.Location = new System.Drawing.Point(396, 32);
            this.ViewSQLbutton.Name = "ViewSQLbutton";
            this.ViewSQLbutton.Size = new System.Drawing.Size(205, 34);
            this.ViewSQLbutton.TabIndex = 10;
            this.ViewSQLbutton.Text = "Сформировать запрос";
            this.ViewSQLbutton.UseVisualStyleBackColor = true;
            this.ViewSQLbutton.Click += new System.EventHandler(this.ViewSQLbutton_Click_1);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Font = new System.Drawing.Font("Times New Roman", 14F);
            this.richTextBox2.Location = new System.Drawing.Point(632, 36);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(535, 300);
            this.richTextBox2.TabIndex = 18;
            this.richTextBox2.Text = "";
            // 
            // Equation
            // 
            this.Equation.Font = new System.Drawing.Font("Times New Roman", 14F);
            this.Equation.Location = new System.Drawing.Point(21, 107);
            this.Equation.Name = "Equation";
            this.Equation.Size = new System.Drawing.Size(580, 34);
            this.Equation.TabIndex = 11;
            // 
            // NameBase
            // 
            this.NameBase.Font = new System.Drawing.Font("Times New Roman", 14F);
            this.NameBase.Location = new System.Drawing.Point(21, 31);
            this.NameBase.Name = "NameBase";
            this.NameBase.Size = new System.Drawing.Size(339, 34);
            this.NameBase.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.label2.Location = new System.Drawing.Point(17, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(229, 19);
            this.label2.TabIndex = 14;
            this.label2.Text = "Введите назватие базы данных.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.label6.Location = new System.Drawing.Point(17, 347);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(228, 19);
            this.label6.TabIndex = 21;
            this.label6.Text = "Отображение хранимых таблиц";
            // 
            // tabControl
            // 
            this.tabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl.Controls.Add(this.UpdatePage);
            this.tabControl.Controls.Add(this.GeneralPage);
            this.tabControl.Controls.Add(this.HelpPage);
            this.tabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl.ItemSize = new System.Drawing.Size(50, 160);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1419, 711);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 23;
            // 
            // UpdatePage
            // 
            this.UpdatePage.Controls.Add(this.splitContainer1);
            this.UpdatePage.Location = new System.Drawing.Point(160, 4);
            this.UpdatePage.Name = "UpdatePage";
            this.UpdatePage.Size = new System.Drawing.Size(1255, 703);
            this.UpdatePage.TabIndex = 1;
            this.UpdatePage.Text = "Работа с базой данных";
            this.UpdatePage.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1316, 485);
            this.splitContainer1.SplitterDistance = 438;
            this.splitContainer1.TabIndex = 36;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(-3, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(438, 527);
            this.treeView1.TabIndex = 34;
            // 
            // splitContainer2
            // 
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(18, 29);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.checkBoxFix);
            this.splitContainer2.Panel1.Controls.Add(this.labelActionT);
            this.splitContainer2.Panel1.Controls.Add(this.label7);
            this.splitContainer2.Panel1.Controls.Add(this.EquationWithBase);
            this.splitContainer2.Panel1.Controls.Add(this.label5);
            this.splitContainer2.Panel1.Controls.Add(this.GenerateSql);
            this.splitContainer2.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dwas);
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Panel2.Controls.Add(this.TableAndKeys);
            this.splitContainer2.Panel2.Controls.Add(this.dgv);
            this.splitContainer2.Size = new System.Drawing.Size(856, 456);
            this.splitContainer2.SplitterDistance = 200;
            this.splitContainer2.TabIndex = 40;
            // 
            // checkBoxFix
            // 
            this.checkBoxFix.AutoSize = true;
            this.checkBoxFix.Location = new System.Drawing.Point(6, 119);
            this.checkBoxFix.Name = "checkBoxFix";
            this.checkBoxFix.Size = new System.Drawing.Size(18, 17);
            this.checkBoxFix.TabIndex = 47;
            this.checkBoxFix.UseVisualStyleBackColor = true;
            this.checkBoxFix.CheckedChanged += new System.EventHandler(this.checkBoxFix_CheckedChanged);
            // 
            // labelActionT
            // 
            this.labelActionT.AutoSize = true;
            this.labelActionT.Location = new System.Drawing.Point(3, 86);
            this.labelActionT.Name = "labelActionT";
            this.labelActionT.Size = new System.Drawing.Size(0, 16);
            this.labelActionT.TabIndex = 46;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(227, 16);
            this.label7.TabIndex = 45;
            this.label7.Text = "введите реляционное уравнение";
            // 
            // EquationWithBase
            // 
            this.EquationWithBase.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EquationWithBase.Location = new System.Drawing.Point(3, 50);
            this.EquationWithBase.Name = "EquationWithBase";
            this.EquationWithBase.Size = new System.Drawing.Size(773, 28);
            this.EquationWithBase.TabIndex = 44;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 16);
            this.label5.TabIndex = 43;
            // 
            // GenerateSql
            // 
            this.GenerateSql.Location = new System.Drawing.Point(603, 91);
            this.GenerateSql.Name = "GenerateSql";
            this.GenerateSql.Size = new System.Drawing.Size(173, 45);
            this.GenerateSql.TabIndex = 41;
            this.GenerateSql.Text = "выполнить обработку данных";
            this.GenerateSql.UseVisualStyleBackColor = true;
            this.GenerateSql.Click += new System.EventHandler(this.GenerateSql_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(856, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // dwas
            // 
            this.dwas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dwas.Location = new System.Drawing.Point(427, 54);
            this.dwas.Name = "dwas";
            this.dwas.RowHeadersWidth = 51;
            this.dwas.RowTemplate.Height = 24;
            this.dwas.Size = new System.Drawing.Size(240, 150);
            this.dwas.TabIndex = 40;
            this.dwas.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dwas_CellClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Location = new System.Drawing.Point(58, 16);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(266, 215);
            this.tabControl1.TabIndex = 35;
            // 
            // TableAndKeys
            // 
            this.TableAndKeys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TableAndKeys.Location = new System.Drawing.Point(279, 254);
            this.TableAndKeys.Name = "TableAndKeys";
            this.TableAndKeys.RowHeadersWidth = 51;
            this.TableAndKeys.RowTemplate.Height = 24;
            this.TableAndKeys.Size = new System.Drawing.Size(250, 150);
            this.TableAndKeys.TabIndex = 39;
            // 
            // dgv
            // 
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(33, 254);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersWidth = 51;
            this.dgv.RowTemplate.Height = 24;
            this.dgv.Size = new System.Drawing.Size(240, 150);
            this.dgv.TabIndex = 38;
            // 
            // GeneralPage
            // 
            this.GeneralPage.Controls.Add(this.dwast);
            this.GeneralPage.Controls.Add(this.richTable);
            this.GeneralPage.Controls.Add(this.NameBase);
            this.GeneralPage.Controls.Add(this.label2);
            this.GeneralPage.Controls.Add(this.ViewSQLbutton);
            this.GeneralPage.Controls.Add(this.label6);
            this.GeneralPage.Controls.Add(this.richTextBox2);
            this.GeneralPage.Controls.Add(this.Equation);
            this.GeneralPage.Controls.Add(this.label3);
            this.GeneralPage.Controls.Add(this.label4);
            this.GeneralPage.Controls.Add(this.label1);
            this.GeneralPage.Location = new System.Drawing.Point(160, 4);
            this.GeneralPage.Name = "GeneralPage";
            this.GeneralPage.Padding = new System.Windows.Forms.Padding(3);
            this.GeneralPage.Size = new System.Drawing.Size(1255, 703);
            this.GeneralPage.TabIndex = 0;
            this.GeneralPage.Text = "Обработка данных без подключения";
            this.GeneralPage.UseVisualStyleBackColor = true;
            // 
            // dwast
            // 
            this.dwast.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dwast.Location = new System.Drawing.Point(21, 369);
            this.dwast.Name = "dwast";
            this.dwast.RowHeadersWidth = 51;
            this.dwast.RowTemplate.Height = 24;
            this.dwast.Size = new System.Drawing.Size(1146, 316);
            this.dwast.TabIndex = 23;
            // 
            // richTable
            // 
            this.richTable.Font = new System.Drawing.Font("Times New Roman", 14F);
            this.richTable.Location = new System.Drawing.Point(21, 190);
            this.richTable.Name = "richTable";
            this.richTable.Size = new System.Drawing.Size(580, 146);
            this.richTable.TabIndex = 22;
            this.richTable.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.label3.Location = new System.Drawing.Point(17, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(237, 19);
            this.label3.TabIndex = 15;
            this.label3.Text = "Введите реляционное уравнение";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.label4.Location = new System.Drawing.Point(17, 168);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(175, 19);
            this.label4.TabIndex = 17;
            this.label4.Text = "Укажите данные таблиц";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.label1.Location = new System.Drawing.Point(628, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 19);
            this.label1.TabIndex = 12;
            this.label1.Text = "Получившийся запрос";
            // 
            // HelpPage
            // 
            this.HelpPage.Controls.Add(this.tabControl2);
            this.HelpPage.Location = new System.Drawing.Point(160, 4);
            this.HelpPage.Name = "HelpPage";
            this.HelpPage.Padding = new System.Windows.Forms.Padding(3);
            this.HelpPage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.HelpPage.Size = new System.Drawing.Size(1255, 703);
            this.HelpPage.TabIndex = 3;
            this.HelpPage.Text = "Инструкция";
            this.HelpPage.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(3, 3);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1249, 697);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1241, 668);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // SQLHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1419, 711);
            this.Controls.Add(this.tabControl);
            this.Name = "SQLHelper";
            this.Text = "SQLHelper";
            this.tabControl.ResumeLayout(false);
            this.UpdatePage.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dwas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TableAndKeys)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.GeneralPage.ResumeLayout(false);
            this.GeneralPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dwast)).EndInit();
            this.HelpPage.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button ViewSQLbutton;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.TextBox Equation;
        private System.Windows.Forms.TextBox NameBase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage GeneralPage;
        private System.Windows.Forms.TabPage HelpPage;
        private System.Windows.Forms.TabPage UpdatePage;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.DataGridView TableAndKeys;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Button GenerateSql;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dwas;
        private System.Windows.Forms.RichTextBox richTable;
        private System.Windows.Forms.DataGridView dwast;
        private System.Windows.Forms.TextBox EquationWithBase;
        private System.Windows.Forms.Label labelActionT;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBoxFix;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage1;
    }
}


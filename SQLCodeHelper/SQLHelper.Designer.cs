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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SQLHelper));
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ViewSQLbutton = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.Equation = new System.Windows.Forms.TextBox();
            this.NameBase = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TableAndKeys = new System.Windows.Forms.RichTextBox();
            this.TableList = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.label4.Location = new System.Drawing.Point(15, 300);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(641, 152);
            this.label4.TabIndex = 17;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.label3.Location = new System.Drawing.Point(15, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(649, 152);
            this.label3.TabIndex = 15;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.label1.Location = new System.Drawing.Point(733, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(646, 152);
            this.label1.TabIndex = 12;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // ViewSQLbutton
            // 
            this.ViewSQLbutton.Location = new System.Drawing.Point(19, 743);
            this.ViewSQLbutton.Name = "ViewSQLbutton";
            this.ViewSQLbutton.Size = new System.Drawing.Size(167, 55);
            this.ViewSQLbutton.TabIndex = 10;
            this.ViewSQLbutton.Text = "Получить SQL код";
            this.ViewSQLbutton.UseVisualStyleBackColor = true;
            this.ViewSQLbutton.Click += new System.EventHandler(this.ViewSQLbutton_Click_1);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Font = new System.Drawing.Font("Times New Roman", 14F);
            this.richTextBox2.Location = new System.Drawing.Point(737, 171);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(735, 300);
            this.richTextBox2.TabIndex = 18;
            this.richTextBox2.Text = "";
            // 
            // Equation
            // 
            this.Equation.Font = new System.Drawing.Font("Times New Roman", 14F);
            this.Equation.Location = new System.Drawing.Point(19, 246);
            this.Equation.Name = "Equation";
            this.Equation.Size = new System.Drawing.Size(699, 34);
            this.Equation.TabIndex = 11;
            // 
            // NameBase
            // 
            this.NameBase.Font = new System.Drawing.Font("Times New Roman", 14F);
            this.NameBase.Location = new System.Drawing.Point(19, 34);
            this.NameBase.Name = "NameBase";
            this.NameBase.Size = new System.Drawing.Size(699, 34);
            this.NameBase.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.label2.Location = new System.Drawing.Point(15, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(229, 19);
            this.label2.TabIndex = 14;
            this.label2.Text = "Введите назватие базы данных.";
            // 
            // TableAndKeys
            // 
            this.TableAndKeys.Font = new System.Drawing.Font("Times New Roman", 14F);
            this.TableAndKeys.Location = new System.Drawing.Point(19, 471);
            this.TableAndKeys.Name = "TableAndKeys";
            this.TableAndKeys.Size = new System.Drawing.Size(699, 266);
            this.TableAndKeys.TabIndex = 16;
            this.TableAndKeys.Text = "";
            // 
            // TableList
            // 
            this.TableList.Font = new System.Drawing.Font("Times New Roman", 14F);
            this.TableList.Location = new System.Drawing.Point(737, 512);
            this.TableList.Name = "TableList";
            this.TableList.Size = new System.Drawing.Size(735, 286);
            this.TableList.TabIndex = 20;
            this.TableList.Text = "";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.label6.Location = new System.Drawing.Point(733, 471);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(379, 38);
            this.label6.TabIndex = 21;
            this.label6.Text = "Полученные таблицы в ходе работы программы\r\n(1 элемент/2 элемент/действие/получен" +
    "ная таблица)";
            // 
            // SQLHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1484, 810);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TableList);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.Equation);
            this.Controls.Add(this.TableAndKeys);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ViewSQLbutton);
            this.Controls.Add(this.NameBase);
            this.Controls.Add(this.label2);
            this.Name = "SQLHelper";
            this.Text = "SQLHelper";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ViewSQLbutton;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.TextBox Equation;
        private System.Windows.Forms.TextBox NameBase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox TableAndKeys;
        private System.Windows.Forms.RichTextBox TableList;
        private System.Windows.Forms.Label label6;
    }
}


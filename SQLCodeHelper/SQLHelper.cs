using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SQLCodeHelper
{
    public partial class SQLHelper : Form
    {
        WorkRPN RPN = new WorkRPN();
        WorkSQL SQL = new WorkSQL();
        Error err = new Error();
        public SQLHelper()
        {
            InitializeComponent();
        }
        private void ViewSQLbutton_Click_1(object sender, EventArgs e)
        {
            Equation.Text = Equation.Text.Replace(" ", "");
            Queue<string> sqlbase = new Queue<string>();
            string[] sp = TableAndKeys.Text.Split('\n');
            foreach (string s in sp) { sqlbase.Enqueue(s); }
            //переделать!!
            if (err.Error_log(NameBase.Text, Equation.Text, TableAndKeys.Text)) { }
            else
            {
                Queue<string> QueRPN = RPN.Do(Equation.Text);
                if (NameBase.Text == "")
                    NameBase.Text = "your base name";
                richTextBox2.Text = $"USE {NameBase.Text}\n\n" + SQL.CodeCreate(QueRPN, sqlbase);
                TableList.Text = SQL.GetSqlWas();
            }
        }
    }
}

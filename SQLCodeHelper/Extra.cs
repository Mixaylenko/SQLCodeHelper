using System;
using System.Drawing;
using System.Windows.Forms;

namespace SQLCodeHelper
{
    public partial class Extra : Form
    {
        private ConSQL con;

        public Extra(ConSQL connection)
        {
            con = connection;
            InitializeComponent();
        }



        private void Connect_button_Click(object sender, EventArgs e)
        {
            string connStr = $@"Server={textBox1.Text};
                 User Id={textBox2.Text};
                 Password={textBox3.Text};
                 TrustServerCertificate=True;";

            if (con.TestConnection(connStr))
            {
                con.Conn(connStr);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Не удалось подключиться к серверу.");
            }
        }

        private void Local_Connect_Click(object sender, EventArgs e)
        {
            string connStr = $@"Data Source=(local);
                Integrated Security=True;
                Trusted_Connection=True;
                TrustServerCertificate=True";

            if (con.TestConnection(connStr))
            {
                con.Conn(connStr);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Не удалось подключиться к локальному серверу.");
            }
        }
    }
}
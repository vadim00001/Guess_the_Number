using System;
using System.Windows.Forms;

namespace Guess_the_number
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 Gamers = new Form2();
            Gamers.Show();
            this.Visible = false;
        }
    }
}

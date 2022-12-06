using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Guess_the_number
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private SQLiteConnection SQLiteConn;
        private SQLiteCommand cmd;

        private string dbName;
        private int lifes;
        private int number_guess;
        private PictureBox[] mas_pictureBox = new PictureBox[7];

        private void DB()
        {
            try
            {
                SQLiteConn = new SQLiteConnection("Data Source =" + dbName + ";Version = 3;");
                SQLiteConn.Open();
                cmd.Connection = SQLiteConn;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                button1.Enabled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                button1.Enabled = true;
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            SQLiteConn = new SQLiteConnection();
            cmd = new SQLiteCommand();

            dbName = "Game";
            
            DB();

            textBox1.Enabled = false;
            textBox2.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            dataGridView1.Enabled = false;

            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.ScrollBars = ScrollBars.None;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 48);
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;

            dataGridView1.ColumnCount = 1;
            dataGridView1.RowCount = 1;

            foreach (DataGridViewRow x in dataGridView1.Rows)
            {
                x.MinimumHeight = dataGridView1.Height;
            }

            foreach (DataGridViewColumn x in dataGridView1.Columns)
            {
                x.MinimumWidth = dataGridView1.Width;
            }

            mas_pictureBox[0] = pictureBox1;
            mas_pictureBox[1] = pictureBox2;
            mas_pictureBox[2] = pictureBox3;
            mas_pictureBox[3] = pictureBox4;
            mas_pictureBox[4] = pictureBox5;
            mas_pictureBox[5] = pictureBox6;
            mas_pictureBox[6] = pictureBox7;
        }

        private void Guessing_Number()
        {
            int min = Convert.ToInt32(textBox1.Text);
            int max = Convert.ToInt32(textBox2.Text);

            if (min > max)
            {
                min = Convert.ToInt32(textBox2.Text);
                max = Convert.ToInt32(textBox1.Text);

                textBox1.Text = Convert.ToString(min);
                textBox2.Text = Convert.ToString(max);
            }

            Random random = new Random();
            number_guess = random.Next(min, max);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Guessing_Number();

            textBox1.Enabled = false;
            textBox2.Enabled = false;
            button1.Enabled = false;
            button3.Enabled = true;
            dataGridView1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows[0].Cells[0].Value != null)
            {
                int number_from_dgv = Convert.ToInt32(dataGridView1.Rows[0].Cells[0].Value);

                richTextBox1.AppendText(number_from_dgv.ToString() + "\n");

                if (number_from_dgv == number_guess)
                {
                    textBox4.Text = "ВЕРНО";
                    MessageBox.Show("ВЫ УГАДАЛИ!", "ПОБЕДА", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    string result = "INSERT INTO Players_info(Игрок, Количество_жизней, Диапазон_ОТ, Диапазон_ДО, Результат) VALUES('" + textBox3.Text + "', '" + textBox5.Text + "', '" + textBox1.Text + "', '" + textBox2.Text + "', 'Победа');";
                    cmd = new SQLiteCommand(result, SQLiteConn);
                    cmd.ExecuteNonQuery();

                    Application.Restart();
                }
                else if (number_from_dgv < number_guess)
                {
                    textBox4.Text = "БОЛЬШЕ";
                    lifes -= 1;
                }
                else
                {
                    textBox4.Text = "МЕНЬШЕ";
                    lifes -= 1;
                }

                for (int i = lifes - 1; i < lifes; i++)
                {
                    mas_pictureBox[i + 1].Image = Properties.Resources.icon2;
                    mas_pictureBox[i + 1].SizeMode = PictureBoxSizeMode.StretchImage;
                }

                if (lifes == 0)
                {
                    MessageBox.Show("ВЫ ПРОИГРАЛИ!" + "\n" + "ПРАВИЛЬНЫЙ ОТВЕТ: " + Convert.ToString(number_guess), "ПОРАЖЕНИЕ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    string result = "INSERT INTO Players_info(Игрок, Количество_жизней, Диапазон_ОТ, Диапазон_ДО, Результат) VALUES('" + textBox3.Text + "', '" + textBox5.Text + "', '" + textBox1.Text + "', '" + textBox2.Text + "', 'Поражение');";
                    cmd = new SQLiteCommand(result, SQLiteConn);
                    cmd.ExecuteNonQuery();

                    Application.Restart();
                }
            }
        }

        private void EditingControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
            {
                Control editingControl = (Control)sender;
                if (!Regex.IsMatch(editingControl.Text + e.KeyChar, "^[0-9]{0,2}$"))
                    e.Handled = true;
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            dataGridView1.EditingControl.KeyPress -= EditingControl_KeyPress;
            dataGridView1.EditingControl.KeyPress += EditingControl_KeyPress;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
            {
                Control editingControl = (Control)sender;
                if (!Regex.IsMatch(editingControl.Text + e.KeyChar, "^[0-9]{0,2}$"))
                    e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
            {
                Control editingControl = (Control)sender;
                if (!Regex.IsMatch(editingControl.Text + e.KeyChar, "^[0-9]{0,2}$"))
                    e.Handled = true;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;

            for (int i = 0; i < lifes; i++)
            {
                mas_pictureBox[i].Image = null;
            }

            lifes = 7;

            for (int i = 0; i < lifes; i++)
            {
                mas_pictureBox[i].Image = Properties.Resources.icon;
                mas_pictureBox[i].SizeMode = PictureBoxSizeMode.StretchImage;
            }

            textBox1.Enabled = true;
            textBox2.Enabled = true;

            textBox5.Text = "7";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Enabled = false;
            radioButton3.Enabled = false;

            for (int i = 0; i < lifes; i++)
            {
                mas_pictureBox[i].Image = null;
            }

            lifes = 5;

            for (int i = 0; i < lifes; i++)
            {
                mas_pictureBox[i].Image = Properties.Resources.icon;
                mas_pictureBox[i].SizeMode = PictureBoxSizeMode.StretchImage;
            }

            for (int i = lifes; i < lifes + 2; i++)
            {
                mas_pictureBox[i].Image = Properties.Resources.icon3;
                mas_pictureBox[i].SizeMode = PictureBoxSizeMode.StretchImage;
            }

            textBox1.Enabled = true;
            textBox2.Enabled = true;

            textBox5.Text = "5";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;

            for (int i = 0; i < lifes; i++)
            {
                mas_pictureBox[i].Image = null;
            }

            lifes = 3;

            for (int i = 0; i < lifes; i++)
            {
                mas_pictureBox[i].Image = Properties.Resources.icon;
                mas_pictureBox[i].SizeMode = PictureBoxSizeMode.StretchImage;
            }

            for (int i = lifes; i < lifes + 4; i++)
            {
                mas_pictureBox[i].Image = Properties.Resources.icon3;
                mas_pictureBox[i].SizeMode = PictureBoxSizeMode.StretchImage;
            }

            textBox1.Enabled = true;
            textBox2.Enabled = true;

            textBox5.Text = "3";
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[0].Cells[0].Value != null)
            {
                button2.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ВЫ СДАЛИСЬ!" + "\n" + "ПРАВИЛЬНЫЙ ОТВЕТ: " + Convert.ToString(number_guess), "ПОРАЖЕНИЕ", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string result = "INSERT INTO Players_info(Игрок, Количество_жизней, Диапазон_ОТ, Диапазон_ДО, Результат) VALUES('" + textBox3.Text + "', '" + textBox5.Text + "', '" + textBox1.Text + "', '" + textBox2.Text + "', 'Сдался');";
            cmd = new SQLiteCommand(result, SQLiteConn);
            cmd.ExecuteNonQuery();

            Application.Restart();
        }
    }
}
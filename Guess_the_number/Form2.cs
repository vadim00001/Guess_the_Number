using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace Guess_the_number
{
    public partial class Form2 : Form
    {
        private SQLiteConnection SQLiteConn;
        private DataTable dTable;
        private SQLiteCommand cmd;
        private SQLiteDataReader reader;
        private SQLiteDataAdapter adapter;

        private string selected_player;
        private string dbName;

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count != 0)
            {
                selected_player = listBox1.SelectedItem.ToString();
                Form3 Game = new Form3();
                Game.Show();
                Game.textBox3.Text = selected_player;
                this.Visible = false;
            }
            else
            {
                MessageBox.Show("ВЫБЕРИТЕ ИГРОКА", "ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void CreateDB()
        {
            try
            {
                SQLiteConn = new SQLiteConnection("Data Source =" + dbName + ";Version = 3;");
                SQLiteConn.Open();
                cmd.Connection = SQLiteConn;

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Players(nickname TEXT)";
                cmd.ExecuteNonQuery();

                string default_player = "INSERT INTO Players(nickname) VALUES('Player');";
                cmd = new SQLiteCommand(default_player, SQLiteConn);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Players_info(Игрок TEXT, Количество_жизней TEXT, Диапазон_ОТ TEXT, Диапазон_ДО TEXT, Результат TEXT)";
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void DB()
        {
            if (!File.Exists(dbName))
            {
                SQLiteConnection.CreateFile(dbName);
                CreateDB();
            }

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

        private void Form2_Load(object sender, EventArgs e)
        {
            SQLiteConn = new SQLiteConnection();
            dTable = new DataTable();
            cmd = new SQLiteCommand();
            adapter = new SQLiteDataAdapter();

            textBox1.Enabled = false;
            button3.Enabled = false;

            dbName = "Game";

            DB();

            string command = "SELECT nickname FROM Players ORDER BY nickname;";
            cmd = new SQLiteCommand(command, SQLiteConn);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                listBox1.Items.Add(reader.GetString(0));
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form4 players = new Form4();

            string players_info = "SELECT * FROM Players_info ORDER BY Игрок";
            cmd = new SQLiteCommand(players_info, SQLiteConn);

            dTable.Clear();
            adapter = new SQLiteDataAdapter(players_info, SQLiteConn);
            adapter.Fill(dTable);

            players.dataGridView1.Columns.Clear();
            players.dataGridView1.Rows.Clear();

            for (int col = 0; col < dTable.Columns.Count; col++)
            {
                string ColName = dTable.Columns[col].ColumnName;
                players.dataGridView1.Columns.Add(ColName, ColName);
                players.dataGridView1.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            for (int row = 0; row < dTable.Rows.Count; row++)
            {
                players.dataGridView1.Rows.Add(dTable.Rows[row].ItemArray);
            }

            players.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = true;
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                if (!listBox1.Items.Contains(textBox1.Text))
                {
                    string add_player = "INSERT INTO Players(nickname) VALUES('" + textBox1.Text + "');";
                    cmd = new SQLiteCommand(add_player, SQLiteConn);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    MessageBox.Show("ТАКОЙ ПРОФИЛЬ УЖЕ СУЩЕСТВУЕТ!", "ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            listBox1.Items.Clear();

            string update_listbox = "SELECT nickname FROM Players ORDER BY nickname;";
            cmd = new SQLiteCommand(update_listbox, SQLiteConn);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                listBox1.Items.Add(reader.GetString(0));
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string clean_db = "DELETE FROM Players_info;";
            cmd = new SQLiteCommand(clean_db, SQLiteConn);
            cmd.ExecuteNonQuery();
        }
    }
}

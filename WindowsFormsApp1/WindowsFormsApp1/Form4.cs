using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form4 : Form
    {
        private Form2 form2;
        public Form4()
        {
            InitializeComponent();
        }

        private void DASHBOARD_Click(object sender, EventArgs e)
        {

        }

        private void Form4_Load(object sender, EventArgs e) {
            
            dataGridView1.Columns.Add("transaction_id", "Transaction ID");
            dataGridView1.Columns.Add("customer_name", "Customer Name");
            dataGridView1.Columns.Add("date", "Date");
            dataGridView1.Columns.Add("items", "Item");
            dataGridView1.Columns.Add("amount", "Amount");
            dataGridView1.Columns.Add("status", "Status");

            LoadTransactions();
        }

        private void LoadTransactions()
        {

            string query = "SELECT transaction_id, customer_id, items, amount, status, created_at FROM Transactions";

            // Create connection to the database
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KhataDBConnection"].ConnectionString))
            {
                // Create the SQL command
                SqlCommand cmd = new SqlCommand(query, conn);

                // Open the connection
                conn.Open();

                // Execute the command and retrieve the data
                SqlDataReader reader = cmd.ExecuteReader();

                // Define DataGridView columns (if not done in the Designer)
                dataGridView1.Columns.Clear();
                dataGridView1.Columns.Add("transaction_id", "Transaction ID");
                dataGridView1.Columns.Add("customer_id", "Customer ID");
                dataGridView1.Columns.Add("items", "Items");
                dataGridView1.Columns.Add("amount", "Amount");
                dataGridView1.Columns.Add("status", "Status");
                dataGridView1.Columns.Add("created_at", "Created At");

                // Loop through the data and add rows to the DataGridView
                while (reader.Read())
                {
                    int transactionId = (int)reader["transaction_id"];
                    int customerId = (int)reader["customer_id"];
                    string items = reader["items"].ToString();
                    decimal amount = (decimal)reader["amount"];
                    string status = reader["status"].ToString();
                    DateTime createdAt = (DateTime)reader["created_at"];

                    // Add the row to the DataGridView
                    dataGridView1.Rows.Add(transactionId, customerId, items, amount, status, createdAt.ToString("yyyy-MM-dd"));
                }

                // Close the reader when done
                reader.Close();
            }
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            // Use SaveFileDialog to choose where to save the file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV file (*.csv)|*.csv";
            saveFileDialog.Title = "Save Transactions Data";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                {
                    // Get the headers
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                    {
                        sw.Write(col.HeaderText + ",");
                    }
                    sw.WriteLine();

                    // Get the rows
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            sw.Write(cell.Value?.ToString() + ",");
                        }
                        sw.WriteLine();
                    }
                }

                MessageBox.Show("Transactions data downloaded successfully.", "Success");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            form2 = new Form2();
            form2.Show();
            this.Hide();
        }

        private void Form4_Load_1(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'database1DataSet.Transactions' table. You can move, or remove it, as needed.
            this.transactionsTableAdapter.Fill(this.database1DataSet.Transactions);

        }
    }
}

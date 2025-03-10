using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {

        private Form2 form2;
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("transaction_id", "Transaction ID");
            dataGridView1.Columns.Add("items", "Item");
            dataGridView1.Columns.Add("amount", "Amount");


            string query = "SELECT customer_id, customer_name FROM Customers";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KhataDBConnection"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                listView1.Items.Clear();

                while (reader.Read())
                {
                    int customerId = (int)reader["customer_id"];
                    string customerName = reader["customer_name"].ToString();

                    // Create a ListViewItem, store customer_id in Tag
                    ListViewItem item = new ListViewItem(customerName);
                    item.Tag = customerId;

                    listView1.Items.Add(item);
                }
            }
        }

        private void DASHBOARD_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // Get customer_id from Tag
                int selectedCustomerId = (int)listView1.SelectedItems[0].Tag;

                // Clear previous dues
                dataGridView1.Rows.Clear();

                // Fetch dues from the database for the selected customer_id
                string query = "SELECT transaction_id, items, amount FROM Transactions WHERE customer_id = @customerId AND status = 'Pending'";
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KhataDBConnection"].ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@customerId", selectedCustomerId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Loop through the dues and add them to DataGridView
                    while (reader.Read())
                    {
                        int transactionId = (int)reader["transaction_id"];
                        string itemName = reader["items"].ToString();
                        decimal amount = (decimal)reader["amount"];

                        dataGridView1.Rows.Add(transactionId, itemName, amount);
                    }

                    if (dataGridView1.Rows.Count == 1)
                    {
                        dataGridView1.Visible = false;
                        panel3.Visible = true;
                    }
                    else
                    {
                        dataGridView1.Visible = true;
                        panel3.Visible = false;
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0 && dataGridView1.SelectedRows.Count > 0)
            {
                int selectedCustomerId = (int)listView1.SelectedItems[0].Tag;

                // Loop through selected dues and mark them as paid
                foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
                {
                    int transactionId = (int)selectedRow.Cells["transaction_id"].Value;

                    // Update transaction status in the database
                    string query = "UPDATE Transactions SET status = 'paid' WHERE transaction_id = @transactionId";

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KhataDBConnection"].ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@transactionId", transactionId);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    // Remove the row from the DataGridView
                    dataGridView1.Rows.Remove(selectedRow);
                }

                MessageBox.Show("Selected dues cleared.");
            }
            else
            {
                MessageBox.Show("Please select a customer and due first.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            form2 = new Form2();
            form2.Show();
            this.Hide();
            this.Hide();
        }

        private void SendEmail(string recipientEmail, string customerName)
        {
            try
            {
                // Set up the email message
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("yhingu2005@gmail.com");  // Sender's email
                mail.To.Add(recipientEmail);                            // Customer's email
                mail.Subject = "Pending Transaction Notification";
                mail.Body = $"Dear {customerName},\n\nYou have a pending transaction. Please clear it as soon as possible.";

                // Set up the SMTP client
                SmtpClient smtpClient = new SmtpClient("smtp.your-email-provider.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential("yhingu2005@gmail.com", "mzwc mivy twzl ujtw");
                smtpClient.EnableSsl = true;

                // Send the email
                smtpClient.Send(mail);

                Console.WriteLine("Notification sent to " + customerName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Database connection
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KhataDBConnection"].ConnectionString))
            {
                conn.Open();

                // SQL query to fetch customers with pending transactions
                string query = "SELECT c.customer_name, c.email FROM Customers c " +
                               "JOIN Transactions t ON c.customer_id = t.customer_id " +
                               "WHERE t.status = 'Pending'";

                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader reader = command.ExecuteReader();

                // Loop through customers and send notification
                while (reader.Read())
                {
                    string customerName = reader["customer_name"].ToString();
                    string email = reader["email"].ToString();

                    // Send email
                    SendEmail(email, customerName);
                }
            }
        }
    }
}
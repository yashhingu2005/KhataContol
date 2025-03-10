using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form5 : Form
    {
        private Form2 form2;
        public Form5()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Get customer details from textboxes
            string customerName = customerNameTextBox.Text.Trim();
            string customerAddress = customerAddressTextBox.Text.Trim();
            string customerNumber = customerNumberTextBox.Text.Trim();
            string customerEmail = textBox1.Text.Trim();  // Get email input

            // Validate input (you can add more validation if needed)
            if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(customerAddress) || string.IsNullOrEmpty(customerNumber) || string.IsNullOrEmpty(customerEmail))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Validate email format using regular expressions
            if (!IsValidEmail(customerEmail))
            {
                MessageBox.Show("Please enter a valid email address.");
                return;
            }

            // SQL query to insert customer data into the Customers table
            string query = "INSERT INTO Customers (customer_name, customer_address, customer_number, email) VALUES (@customerName, @customerAddress, @customerNumber, @customerEmail)";

            // Insert data into the database
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KhataDBConnection"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@customerName", customerName);
                cmd.Parameters.AddWithValue("@customerAddress", customerAddress);
                cmd.Parameters.AddWithValue("@customerNumber", customerNumber);
                cmd.Parameters.AddWithValue("@customerEmail", customerEmail);  // Insert email parameter

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Customer added successfully!");

                    // Optionally clear the form fields after successful submission
                    customerNameTextBox.Clear();
                    customerAddressTextBox.Clear();
                    customerNumberTextBox.Clear();
                    textBox1.Clear();  // Clear email input
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding customer: " + ex.Message);
                }
            }

        }

        // Validate email format using regex
        private bool IsValidEmail(string email)
        {
            // Regular expression for validating email format
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            form2 = new Form2();
            form2.Show();
            this.Hide();
        }
    }
}

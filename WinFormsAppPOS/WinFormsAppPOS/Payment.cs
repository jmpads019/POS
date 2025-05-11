using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsAppPOS
{
    public partial class Payment : Form
    {
        public event Action DataUpdated;
        string filePathOrder = Path.Combine(Application.StartupPath, "orders.txt");
        public Payment()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            this.Load += Payment_Load;
            txtPayment.TextChanged += txtPayment_TextChanged;
        }

        private void txtPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow control keys like Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Ignore the key press
            }
        }

        private void Payment_Load(object sender, EventArgs e)
        {
            if (File.Exists(filePathOrder))
            {
                string[] lines = File.ReadAllLines(filePathOrder);
                decimal totalAmount = CalculateTotalAmount(lines);
                txtTotal.Text = totalAmount.ToString("N2"); // Load into a TextBox named txtTotal
            }
            else
            {
                txtTotal.Text = "0.00";
            }
        }

        private decimal CalculateTotalAmount(string[] lines)
        {
            decimal grandTotal = 0;

            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length >= 9)
                {
                    if (decimal.TryParse(parts[8], out decimal total))
                    {
                        grandTotal += total;
                    }
                }
            }

            return grandTotal;
        }

        private void txtPayment_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtTotal.Text, out decimal total) &&
                decimal.TryParse(txtPayment.Text, out decimal payment))
            {
                decimal balance = total - payment;
                txtBalance.Text = balance.ToString("N2");
            }
            else
            {
                txtBalance.Text = total.ToString("N2"); // Show full total if payment input is invalid
            }
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            if (File.Exists(filePathOrder))
            {
                File.WriteAllText(filePathOrder, string.Empty);
            }

            DataUpdated?.Invoke();

            MessageBox.Show("Payment successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

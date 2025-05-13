using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WinFormsAppPOS
{
    public partial class POS : Form
    {
        private SearchProducts searchProductsForm;
        private Payment paymentForm;
        string filePath = Path.Combine(Application.StartupPath, "products.txt");
        string filePathOrder = Path.Combine(Application.StartupPath, "orders.txt");
        int stock;
        int quantity;
        decimal price;
        decimal subTotal;
        decimal vatRate = 0.12m;
        decimal txtdiscountAmount = 0;
        public POS()
        {
            InitializeComponent();
            InitializeDataGrid();
            LoadData();
            searchProductsForm = new SearchProducts();
            searchProductsForm.DataUpdated += RefreshDataGrid;
        }

        private void RefreshDataGrid()
        {
            LoadOrders();
            LoadData();
        }

        private void LoadOrders()
        {
            if (File.Exists(filePathOrder))
            {
                var lines = File.ReadAllLines(filePathOrder);
                dataGridView1.Rows.Clear();

                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 11)
                    {
                        dataGridView1.Rows.Add(parts[1], parts[2], parts[3], parts[4], parts[5], parts[6], parts[7], parts[8], parts[9]);
                    }
                }
            }
            else
            {
                MessageBox.Show("Orders file not found.");
            }
        }


        private void POS_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void InitializeDataGrid()
        {
            dataGridView1.Columns.Add("DateTime", "Date");
            dataGridView1.Columns.Add("Id", "Product ID");
            dataGridView1.Columns.Add("Name", "Product Name");
            dataGridView1.Columns.Add("Price", "Price");
            dataGridView1.Columns.Add("Quantity", "Quantity");
            dataGridView1.Columns.Add("Subtotal", "Subtotal");
            dataGridView1.Columns.Add("Discount", "Discount %");
            dataGridView1.Columns.Add("Discount Amount", "Discount");
            dataGridView1.Columns.Add("VAT", "VAT");
            dataGridView1.Columns.Add("Total", "Total");

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void txtProductID_TextChanged(object sender, EventArgs e)
        {
            string typedId = txtProductID.Text.Trim();
            if (!string.IsNullOrEmpty(typedId))
            {
                LoadProductById(typedId);
            }
            else
            {
                txtProductName.Clear();
                txtPrice.Clear();
                txtQuality.Clear();
                pictureBox1.ImageLocation = null;
            }
        }

        private void LoadProductById(string productId)
        {
            if (!File.Exists(filePath)) return;

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length >= 5 && parts[0] == productId)
                {
                    txtProductName.Text = parts[1];
                    txtPrice.Text = parts[2];   
                    stock = Convert.ToInt32(parts[3]);
                    pictureBox1.ImageLocation = parts[4];
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    return;
                }
            }

            // If no match found, clear fields
            txtProductName.Clear();
            txtPrice.Clear();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtQuality_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(txtQuality.Text, out quantity))
            {
                MessageBox.Show("Please enter a valid quantity.");
                txtQuality.Clear();
                return;
            }

            if (quantity > stock)
            {
                MessageBox.Show("Available stock: " + stock);
                txtQuality.Clear();
            }

            bool priceValid = decimal.TryParse(txtPrice.Text, out price);
            bool quantityValid = int.TryParse(txtQuality.Text, out quantity);
            
            subTotal = price * quantity;
            txtSubTotal.Text = subTotal.ToString("F2");

            decimal discountPercent = 0;

            if (!string.IsNullOrWhiteSpace(txtDiscount.Text) && decimal.TryParse(txtDiscount.Text, out decimal parsedDiscount))
            {
                discountPercent = parsedDiscount;
            }

            decimal discountAmount = subTotal * (discountPercent / 100);
            decimal discountedPrice = subTotal - discountAmount;

            decimal vatAmount = discountedPrice * vatRate;
            decimal totalWithVAT = discountedPrice + vatAmount;

            txtTotal.Text = discountedPrice.ToString("F2");
            txtVAT.Text = vatRate * 100 + "%";
        }

        private void txtSubTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnNewTran_Click(object sender, EventArgs e)
        {
            // Display confirmation dialog
            DialogResult result = MessageBox.Show("Are you sure you want to delete all order data?",
                                                  "Confirm Delete",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            // If the user clicked "Yes"
            if (result == DialogResult.Yes)
            {
                // Clear text fields
                txtProductID.Clear();
                txtPrice.Clear();
                txtQuality.Clear();
                txtSubTotal.Clear();
                txtProductName.Clear();

                // Clear the contents of orders.txt by writing an empty string
                string filePath = Path.Combine(Application.StartupPath, "orders.txt");

                if (File.Exists(filePath))
                {
                    File.WriteAllText(filePath, string.Empty); // This will delete all content in the file
                }

                // After clearing the file, reload the DataGridView to reflect changes
                LoadData();

                // Inform the user that the data has been cleared
                MessageBox.Show("Order data has been deleted and the DataGridView has been refreshed.",
                                "Data Cleared",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
            {
                // If the user clicked "No", do nothing and just exit the method
                return;
            }
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            if (txtQuality.Text != null)
            {
                if (quantity > stock)
                {
                    MessageBox.Show("Available stock: " + stock);
                    txtQuality.Clear();
                }

                if (string.IsNullOrWhiteSpace(txtProductID.Text) ||
                    string.IsNullOrWhiteSpace(txtProductName.Text) ||
                    string.IsNullOrWhiteSpace(txtPrice.Text) ||
                    string.IsNullOrWhiteSpace(txtQuality.Text) ||
                    string.IsNullOrWhiteSpace(txtSubTotal.Text) ||
                    string.IsNullOrWhiteSpace(txtVAT.Text) ||
                    string.IsNullOrWhiteSpace(txtTotal.Text))
                {
                    MessageBox.Show("Please fill in all product fields.");
                    return;
                }

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                dataGridView1.Rows.Add(
                timestamp,
                txtProductID.Text.Trim(),
                    txtProductName.Text.Trim(),
                    txtPrice.Text.Trim(),
                    txtQuality.Text.Trim(),
                    txtSubTotal.Text.Trim(),
                    txtDiscount.Text.Trim() + "%",
                    txtdiscountAmount.ToString(),
                    txtVAT.Text.Trim(),
                    txtTotal.Text.Trim(),
                    pictureBox1.ImageLocation ?? ""
                );

                UpdateCartTotals();

                MessageBox.Show("Successfully Added to Cart.");

                UpdateStock(filePath, txtProductID.Text);
                SaveData();
                RefreshDataGrid();
            }
            else
            {
                MessageBox.Show("Please input quantity.");
            }
        }

        void UpdateStock(string filePath, string productId)
        {
            var lines = File.ReadAllLines(filePath).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split('|');
                if (parts.Length < 5) continue; // ensure proper format

                if (parts[0] == productId)
                {
                    int newStock = Convert.ToInt32(parts[3]) - quantity;

                    parts[3] = Convert.ToString(newStock); // index 3 = Quantity
                    lines[i] = string.Join("|", parts);
                    break;
                }
            }

            File.WriteAllLines(filePath, lines);
        }

        private void SaveData()
        {
            if (!File.Exists(filePathOrder))
            {
                File.Create(filePathOrder).Close(); // Creates the file if it doesn't exist
            }

            List<string> lines = new List<string>();

            string dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string id = txtProductID.Text;
            string name = txtProductName.Text;
            string price = txtPrice.Text;
            string quality = txtQuality.Text;
            string subtotal = txtSubTotal.Text;
            string discount = txtDiscount.Text + "%";
            string strDiscountAmount = txtdiscountAmount.ToString();
            string vat = txtVAT.Text;
            string total = txtTotal.Text;   
            string imagePath = pictureBox1.ImageLocation;

            string line = string.Join("|", dateTimeNow, id, name, price, quality, subTotal, discount, strDiscountAmount, vat, total,imagePath);
            lines.Add(line);

            File.AppendAllText(filePathOrder, line + Environment.NewLine);
        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            decimal discountPercent = 0;

            if (!string.IsNullOrWhiteSpace(txtDiscount.Text) && decimal.TryParse(txtDiscount.Text, out decimal parsedDiscount))
            {
                discountPercent = parsedDiscount;
            }

            decimal discountAmount = subTotal * (discountPercent / 100);
            decimal discountedPrice = subTotal - discountAmount;

            decimal vatAmount = discountedPrice * vatRate;
            decimal totalWithVAT = discountedPrice + vatAmount;

            txtdiscountAmount = discountAmount;
            txtTotal.Text = discountedPrice.ToString("F2");
        }

        private void btnSearchProduct_Click(object sender, EventArgs e)
        {
            searchProductsForm = new SearchProducts();
            searchProductsForm.DataUpdated += RefreshDataGrid;
            searchProductsForm.ShowDialog();
        }

        private void LoadData()
        {
            if (!File.Exists(filePathOrder))
            {
                // This creates an empty file
                File.Create(filePathOrder).Close();
            }

            dataGridView1.Rows.Clear();

            decimal cartTotal = 0;
            decimal totalDiscountAmount = 0;

            var lines = File.ReadAllLines(filePathOrder);
            foreach (string line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 11)
                {
                    dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5], parts[6], parts[7], parts[8], parts[9]);

                    if (decimal.TryParse(parts[9], out decimal itemTotal))
                    {
                        cartTotal += itemTotal;
                    }

                    if (decimal.TryParse(parts[7], out decimal itemTotalDiscount))
                    {
                        totalDiscountAmount += itemTotalDiscount;
                    }
                }
            }

            // Calculate VAT and Vatable amount
            decimal vat = cartTotal * 0.12m;
            decimal vatable = cartTotal - vat;
            decimal discount = totalDiscountAmount;
            decimal grandTotal = cartTotal - totalDiscountAmount;

            // Update labels
            lblCartTotal.Text = cartTotal.ToString("N2");
            lblTotalVat.Text = vat.ToString("N2");
            lblVatable.Text = vatable.ToString("N2");
            lblDiscountAmount.Text = discount.ToString("N2");
            lblGrandTotal.Text = grandTotal.ToString("N2");

            dataGridView1.Refresh();
        }

        private void UpdateCartTotals()
        {
            decimal cartTotal = 0;
            decimal totalDiscountAmount = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[5].Value != null &&
                    decimal.TryParse(row.Cells[5].Value.ToString(), out decimal itemTotal))
                {
                    cartTotal += itemTotal;
                }

                if (row.Cells[7].Value != null && 
                    decimal.TryParse(row.Cells[7].Value.ToString(), out decimal itemTotalDiscount))
                {
                    totalDiscountAmount += itemTotalDiscount;
                }
            }

            decimal vat = cartTotal * 0.12m;
            decimal vatable = cartTotal - vat;
            decimal discount = totalDiscountAmount;
            decimal grandTotal = cartTotal - totalDiscountAmount;

            lblCartTotal.Text = cartTotal.ToString("N2");
            lblTotalVat.Text = vat.ToString("N2");
            lblVatable.Text = vatable.ToString("N2");
            lblDiscountAmount.Text = discount.ToString("N2");
            lblGrandTotal.Text = grandTotal.ToString("N2");
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            Payment paymentForm = new Payment();
            paymentForm.DataUpdated += RefreshOrderGrid;
            paymentForm.ShowDialog();
        }

        private void RefreshOrderGrid()
        {
            // Reload the data from the (now empty or updated) file
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();

            string filePath = Path.Combine(Application.StartupPath, "orders.txt");

            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length >= 9)
                    {
                        dataGridView1.Rows.Add(parts); // or custom logic for selecting specific columns
                    }
                }
            }

            UpdateCartTotals();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirm removal
            DialogResult result = MessageBox.Show("Are you sure you want to remove the selected item?", "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            // Get selected row
            var selectedRow = dataGridView1.SelectedRows[0];
            string date = selectedRow.Cells[0].Value.ToString();
            string productId = selectedRow.Cells[1].Value.ToString();

            // Remove from DataGridView
            dataGridView1.Rows.Remove(selectedRow);

            // Remove from orders.txt
            string[] allLines = File.ReadAllLines(filePathOrder);
            List<string> updatedLines = new List<string>();

            foreach (var line in allLines)
            {
                var parts = line.Split('|');
                if (parts.Length >= 2 && !(parts[0] == date && parts[1] == productId))
                {
                    updatedLines.Add(line);
                }
            }

            File.WriteAllLines(filePathOrder, updatedLines);

            MessageBox.Show("Item removed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Recalculate totals
            UpdateCartTotals();
        }
    }
}

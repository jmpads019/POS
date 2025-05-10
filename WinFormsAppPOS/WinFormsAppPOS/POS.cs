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
        string filePath = Path.Combine(Application.StartupPath, "products.txt");
        string filePathOrder = Path.Combine(Application.StartupPath, "orders.txt");
        int stock;
        int quantity;
        decimal price;
        decimal subTotal;
        decimal vatRate = 0.12m;
        public POS()
        {
            InitializeComponent();
            InitializeDataGrid();
            LoadData();
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
            dataGridView1.Columns.Add("Discount", "Discount");
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

            txtTotal.Text = totalWithVAT.ToString("F2");
            txtVAT.Text = vatRate * 100 + "%";
        }

        private void txtSubTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnNewTran_Click(object sender, EventArgs e)
        {
            txtProductID.Clear();
            txtPrice.Clear();
            txtQuality.Clear();
            txtSubTotal.Clear(); 
            txtProductName.Clear();    
        }

        private void btnOrder_Click(object sender, EventArgs e)
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

            dataGridView1.Rows.Add(
            txtProductID.Text.Trim(),
                txtProductName.Text.Trim(),
                txtPrice.Text.Trim(),
                txtQuality.Text.Trim(),
                txtSubTotal.Text.Trim(),
                txtDiscount.Text.Trim() + "%",
                txtVAT.Text.Trim(),
                txtTotal.Text.Trim(),   
                pictureBox1.ImageLocation ?? ""
            );


            UpdateStock(filePath, txtProductID.Text);
            SaveData();
            txtQuality.Clear();
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
            string vat = txtVAT.Text;
            string total = txtTotal.Text;   
            string imagePath = pictureBox1.ImageLocation;

            string line = string.Join("|", dateTimeNow, id, name, price, quality, subTotal, discount, vat, total,imagePath);
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

            txtTotal.Text = totalWithVAT.ToString("F2");
        }

        private void btnSearchProduct_Click(object sender, EventArgs e)
        {
            SearchProducts obj = new SearchProducts();
            obj.Dock = DockStyle.Fill;
            obj.StartPosition = FormStartPosition.CenterScreen;
            obj.Show();
        }

        private void LoadData()
        {
            if (!File.Exists(filePathOrder))
            {
                // This creates an empty file
                File.Create(filePathOrder).Close();
            }

            var lines = File.ReadAllLines(filePathOrder);
            foreach (string line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 10)
                {
                    dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5], parts[6], parts[7], parts[8]);
                }
            }
        }
    }
}

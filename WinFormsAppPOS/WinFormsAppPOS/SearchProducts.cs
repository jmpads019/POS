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
    public partial class SearchProducts : Form
    {
        public event Action DataUpdated;
        string filePath = Path.Combine(Application.StartupPath, "products.txt");
        string filePathOrder = Path.Combine(Application.StartupPath, "orders.txt");
        public SearchProducts()
        {
            InitializeComponent();
            InitializeDataGrid();
            LoadData();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void InitializeDataGrid()
        {
            dataGridView1.Columns.Add("Id", "Product ID");
            dataGridView1.Columns.Add("Name", "Product Name");
            dataGridView1.Columns.Add("Price", "Price");
            dataGridView1.Columns.Add("Stock", "Stock");
            dataGridView1.Columns.Add("Image", "Image Path");

            string imagePath = Path.Combine(Application.StartupPath, "add_shopping_cart_16px.png");
            Image cartImage = Image.FromFile(imagePath);

            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.HeaderText = "Action";
            imageColumn.Image = cartImage;
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            dataGridView1.Columns.Add(imageColumn);

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void LoadData()
        {
            if (!File.Exists(filePath))
            {
                // This creates an empty file
                File.Create(filePath).Close();
            }

            dataGridView1.Rows.Clear();

            var lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 5)
                {
                    dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3], parts[4]);
                }
            }

            dataGridView1.Refresh();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5) // Assuming "Add to Cart" is in the 5th column (index 4)
            {
                // Get the selected product information
                string productId = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                string productName = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string price = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                string stock = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();

                // Get the current timestamp
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Set the quantity (1 for now)
                int quantity = 0;
                if (txtQuantity.Text != null)
                {
                    if (!int.TryParse(txtQuantity.Text, out quantity))
                    {
                        MessageBox.Show("Please enter a valid numeric quantity.");
                        txtQuantity.Clear();
                        txtQuantity.Focus();
                        return;
                    }
                    quantity = Convert.ToInt32(txtQuantity.Text);
                }
                else
                {
                    quantity = 1;
                }

                decimal discountAmount = 0;
                decimal productPrice = decimal.Parse(price);
                decimal subtotal = productPrice * quantity;

                string discount = "%"; // No discount for now
                decimal taxRate = 0.12M; // 12% tax
                decimal taxAmount = subtotal * taxRate;

                decimal totalPrice = subtotal + taxAmount;

                UpdateStock(filePath, productId, quantity);
                LoadData();

                string imagePath = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();

                AddToOrderFile(timestamp, productId, productName, price, quantity.ToString(), subtotal.ToString(), discount, discountAmount.ToString(), "12.00%", totalPrice.ToString(), imagePath);

                DataUpdated?.Invoke();
            }
        }

        private void AddToOrderFile(string timestamp, string productId, string productName, string price, string quantity, string subtotal, string discount, string discountAmount, string tax, string totalPrice, string imagePath)
        {
            string orderDetails = $"{timestamp}|{productId}|{productName}|{price}|{quantity}|{subtotal}|{discount}|{discountAmount}|{tax}|{totalPrice}|{imagePath}";

            try
            {
                File.AppendAllText(filePathOrder, orderDetails + Environment.NewLine);
                MessageBox.Show("Product added to cart!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding to the cart: " + ex.Message);
            }
        }

        void UpdateStock(string filePath, string productId, int quantity)
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

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
   
        }
    }
}

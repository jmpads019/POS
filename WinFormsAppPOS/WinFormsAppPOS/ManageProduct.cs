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
    public partial class ManageProduct : Form
    {
        string filePath = Path.Combine(Application.StartupPath, "products.txt");
        public ManageProduct()
        {
            InitializeComponent();
            InitializeDataGrid();
            LoadData();
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        private void ManageProduct_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged; // Redundant safety hookup
        }

        private void InitializeDataGrid()
        {
            dataGridView1.Columns.Add("Id", "Product ID");
            dataGridView1.Columns.Add("Name", "Product Name");
            dataGridView1.Columns.Add("Price", "Price");
            dataGridView1.Columns.Add("Stock", "Stock");
            dataGridView1.Columns.Add("ImagePath", "Image Path");

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void LoadData()
        {
            if (!File.Exists(filePath))
            {
                // This creates an empty file
                File.Create(filePath).Close();
            }

            var lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 5)
                {
                    dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3], parts[4]);
                }
            }
        }

        private void SaveData()
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close(); // Creates the file if it doesn't exist
            }

            List<string> lines = new List<string>();

            string id = txtId.Text;
            string name = txtName.Text;
            string price = txtPrice.Text;
            string stock = txtStock.Text;
            string imagePath = pictureBox1.ImageLocation;

            string line = string.Join("|", id, name, price, stock, imagePath);
            lines.Add(line);


            File.AppendAllText(filePath, line + Environment.NewLine);
        }

        private void btnBrowseImage_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.ImageLocation = openFileDialog1.FileName;
            }
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text) ||
                string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text) ||
                string.IsNullOrWhiteSpace(txtStock.Text))
            {
                MessageBox.Show("Please fill in all product fields.");
                return;
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow && row.Cells[0].Value?.ToString() == txtId.Text.Trim())
                {
                    MessageBox.Show("Product ID already exists. Use a unique ID.");
                    return;
                }
            }

            dataGridView1.Rows.Add(
                txtId.Text.Trim(),
                txtName.Text.Trim(),
                txtPrice.Text.Trim(),
                txtStock.Text.Trim(),
                pictureBox1.ImageLocation ?? ""
            );

            MessageBox.Show("Product Added Successfully.");
            SaveData();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];

                txtId.Text = row.Cells[0].Value?.ToString() ?? "";
                txtName.Text = row.Cells[1].Value?.ToString() ?? "";
                txtPrice.Text = row.Cells[2].Value?.ToString() ?? "";
                txtStock.Text = row.Cells[3].Value?.ToString() ?? "";

                string imagePath = row.Cells[4].Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    pictureBox1.ImageLocation = imagePath;
                }
                else
                {
                    pictureBox1.Image = null;  // Clear the image if not found
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                SaveAllRowsToFile(); // update the file after deletion
                MessageBox.Show("Product Deleted.");
            }
            else
            {
                MessageBox.Show("Please select a product to delete.");
            }
        }

        private void SaveAllRowsToFile()
        {
            List<string> lines = new List<string>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    string id = row.Cells[0].Value?.ToString();
                    string name = row.Cells[1].Value?.ToString();
                    string price = row.Cells[2].Value?.ToString();
                    string stock = row.Cells[3].Value?.ToString();
                    string imagePath = row.Cells[4].Value?.ToString();

                    string line = string.Join("|", id, name, price, stock, imagePath);
                    lines.Add(line);
                }
            }

            File.WriteAllLines(filePath, lines);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtId.Clear();
            txtName.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            pictureBox1.ImageLocation = null;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];

                if (!row.IsNewRow)
                {
                    row.Cells[0].Value = txtId.Text.Trim();
                    row.Cells[1].Value = txtName.Text.Trim();
                    row.Cells[2].Value = txtPrice.Text.Trim();
                    row.Cells[3].Value = txtStock.Text.Trim();
                    row.Cells[4].Value = pictureBox1.ImageLocation ?? "";

                    SaveAllRowsToFile();
                    MessageBox.Show("Product updated successfully.");
                }
            }
            else
            {
                MessageBox.Show("Please select a product to update.");
            }
        }
    }
}

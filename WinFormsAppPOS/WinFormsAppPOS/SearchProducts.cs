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
        string filePath = Path.Combine(Application.StartupPath, "products.txt");
        public SearchProducts()
        {
            InitializeComponent();
            InitializeDataGrid();
            LoadData();
        }

        private void InitializeDataGrid()
        {
            dataGridView1.Columns.Add("Id", "Product ID");
            dataGridView1.Columns.Add("Name", "Product Name");
            dataGridView1.Columns.Add("Price", "Price");
            dataGridView1.Columns.Add("Stock", "Stock");

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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
   
        }
    }
}

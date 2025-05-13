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
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
            DisplayTotalSales();
            DisplayTotalItems();
            DisplayCriticalStockCount();
            DisplayItemsSold();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void DisplayTotalSales()
        {
            string filePathOrder = Path.Combine(Application.StartupPath, "itemsSold.txt");
            decimal totalSales = 0;

            try
            {
                if (File.Exists(filePathOrder))
                {
                    string[] lines = File.ReadAllLines(filePathOrder);

                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] parts = line.Split('|');
                            if (parts.Length >= 10 && decimal.TryParse(parts[9], out decimal saleAmount))
                            {
                                totalSales += saleAmount;
                            }
                        }
                    }

                    labelTotalSales.Text = $"₱{totalSales:N2}";
                }
                else
                {
                    labelTotalSales.Text = "Data file not found.";
                }
            }
            catch (Exception ex)
            {
                labelTotalSales.Text = "Error reading data.";
            }
        }

        private void DisplayTotalItems()
        {
            string filePath = Path.Combine(Application.StartupPath, "products.txt");

            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    int totalItems = lines.Count(line => !string.IsNullOrWhiteSpace(line));

                    labelTotalItems.Text = totalItems.ToString();
                }
                else
                {
                    labelTotalItems.Text = "File not found";
                }
            }
            catch (Exception)
            {
                labelTotalItems.Text = "Error reading data";
            }
        }


        private void DisplayCriticalStockCount()
        {
            string filePath = Path.Combine(Application.StartupPath, "products.txt");
            int criticalThreshold = 5;
            int criticalCount = 0;

            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);

                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] parts = line.Split('|');
                            if (parts.Length >= 4 && int.TryParse(parts[3], out int quantity))
                            {
                                if (quantity < criticalThreshold)
                                {
                                    criticalCount++;
                                }
                            }
                        }
                    }

                    labelCriticalStock.Text =  criticalCount.ToString();
                }
                else
                {
                    labelCriticalStock.Text = "Inventory file not found.";
                }
            }
            catch
            {
                labelCriticalStock.Text = "Error reading inventory.";
            }
        }

        private void DisplayItemsSold()
        {
            string filePath = Path.Combine(Application.StartupPath, "itemsSold.txt");
            int totalItems = 0;

            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);

                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] parts = line.Split('|');
                            if (parts.Length >= 5 && int.TryParse(parts[4], out int quantity))
                            {
                                totalItems += quantity;
                            }
                        }
                    }

                    labelItemsSold.Text = totalItems.ToString();
                }
                else
                {
                    labelItemsSold.Text = "File not found";
                }
            }
            catch (Exception ex)
            {
                labelItemsSold.Text = "Error reading data";
            }
        }
    }
}

using PdfSharp.Drawing;
using PdfSharp.Pdf;
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
    public partial class InventoryReport : Form
    {
        public InventoryReport()
        {
            InitializeComponent();
            LoadData();
        }

        private void InventoryReport_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        public class Inventory
        {
            public string Product { get; set; }
            public string Quantity { get; set; }
            public string UnitPrice { get; set; }
            public string Total { get; set; }
        }

        public void LoadData()
        {
            string filePath = Path.Combine(Application.StartupPath, "products.txt");
            List<Inventory> orders = new List<Inventory>();

            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var data = line.Split('|');

                    string product = data[1];
                    bool isUnitPriceValid = int.TryParse(data[2], out int unitPrice);
                    bool isQuantityValid = int.TryParse(data[3], out int quantity);

                    if (!isUnitPriceValid || !isQuantityValid)
                    {
                        MessageBox.Show($"Invalid data for unit price or quantity in line:\n{line}");
                        continue; // skip this line
                    }

                    int totalValue = unitPrice * quantity;

                    string formattedUnitPrice = $"₱{unitPrice:N2}";
                    string formattedFinalPrice = $"₱{totalValue:N2}";

                    orders.Add(new Inventory
                    {
                        Product = product,
                        Quantity = quantity.ToString(),
                        UnitPrice = formattedUnitPrice,
                        Total = formattedFinalPrice
                    });
                }

                // Bind to DataGridView
                dataGridView1.DataSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void btnGenerateReports_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(Application.StartupPath, "products.txt");
            try
            {
                var inventoryLines = File.ReadAllLines(filePath); // Use same path to your inventory.txt
                PdfDocument inventoryDoc = new PdfDocument();

                // Get current date and time
                string currentDateTime = DateTime.Now.ToString("MMMM dd, yyyy hh:mm:ss tt");
                inventoryDoc.Info.Title = $"Inventory Report - {currentDateTime}";

                // Create a page
                PdfPage invPage = inventoryDoc.AddPage();
                XGraphics invGfx = XGraphics.FromPdfPage(invPage);

                // Fonts
                XFont invHeaderFont = new XFont("Arial", 14);
                XFont invFont = new XFont("Arial", 12);
                double invY = 40;

                // Title
                invGfx.DrawString($"Inventory Report - {currentDateTime}", invHeaderFont, XBrushes.Black, new XPoint(100, invY));
                invY += 30;

                // Column headers
                invGfx.DrawString("Product | Stock Quantity | Unit Price | Total Value", invFont, XBrushes.Black, new XPoint(100, invY));
                invY += 20;

                // Data rows
                foreach (var line in inventoryLines)
                {
                    var data = line.Split('|');

                    string product = data[1];
                    int unitPrice = int.Parse(data[2]);
                    int quantity = int.Parse(data[3]);
                    int totalValue = unitPrice * quantity;

                    string formattedUnitPrice = $"₱{unitPrice:N2}";
                    string formattedTotalValue = $"₱{totalValue:N2}";

                    string row = $"{product} | {quantity} | {formattedUnitPrice} | {formattedTotalValue}";

                    invGfx.DrawString(row, invFont, XBrushes.Black, new XPoint(100, invY));
                    invY += 20;

                    if (invY > invPage.Height - 50)
                    {
                        invPage = inventoryDoc.AddPage();
                        invGfx = XGraphics.FromPdfPage(invPage);
                        invY = 40;
                    }
                }

                // File name
                string currentDateTimeInv = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputPath = $@"C:\Reports\InventoryReport_{currentDateTimeInv}.pdf";

                // Save
                inventoryDoc.Save(outputPath);

                MessageBox.Show("Done generating inventory report! File Path: " + outputPath, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Inventory PDF: " + ex.Message);
            }
        }
    }
}

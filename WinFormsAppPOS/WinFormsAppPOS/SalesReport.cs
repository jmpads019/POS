using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace WinFormsAppPOS
{
    public partial class SalesReport : Form
    {
        public SalesReport()
        {
            InitializeComponent();
            LoadData();
        }

        private void SalesReport_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        public class SalesOrder
        {
            public string ID { get; set; }
            public string Product { get; set; }
            public string Price { get; set; }
            public string Quantity { get; set; }
            public string Total { get; set; }
            public string Discount { get; set; }
            public string FinalPrice { get; set; }
        }

        public void LoadData()
        {
            string filePath = Path.Combine(Application.StartupPath, "orders.txt");
            List<SalesOrder> orders = new List<SalesOrder>();

            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var data = line.Split('|');
                    if (data.Length >= 9) // Ensure there's enough data
                    {
                        orders.Add(new SalesOrder
                        {
                            ID = data[1],
                            Product = data[2],
                            Price = data[3],
                            Quantity = data[4],
                            Total = data[5],
                            Discount = data[6],
                            FinalPrice = data[8]
                        });
                    }
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
            string filePath = Path.Combine(Application.StartupPath, "orders.txt");
            try
            {
                // Read all lines from the file
                var lines = File.ReadAllLines(filePath);

                // Create a new PDF document
                PdfDocument doc = new PdfDocument();
                string currentDateTimeTitle = DateTime.Now.ToString("MMMM dd, yyyy hh:mm:ss tt");
                doc.Info.Title = $"Sales Report - {currentDateTimeTitle}";

                // Create a page
                PdfPage page = doc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Set up the font
                XFont font = new XFont("Arial", 12);
                XFont headerFont = new XFont("Arial", 14);

                // Set up the initial position on the page
                double yPosition = 40;

                // Draw the report title
                gfx.DrawString($"Sales Report - {currentDateTimeTitle}", headerFont, XBrushes.Black, new XPoint(100, yPosition));
                yPosition += 30; // Move the y position down after the title

                // Draw column headers
                gfx.DrawString("ID | Product | Price | Quantity | Total | Discount | Final Price", font, XBrushes.Black, new XPoint(100, yPosition));
                yPosition += 20; // Move the y position down after the headers

                // Process each line of the file
                foreach (var line in lines)
                {
                    // Split the line into parts based on '|'
                    var data = line.Split('|');

                    // Format the data into a string to display in the PDF
                    string row = $"{data[1]} | {data[2]} | {data[3]} | {data[4]} | {data[5]} | {data[6]} | {data[8]}";

                    // Draw the data on the PDF
                    gfx.DrawString(row, font, XBrushes.Black, new XPoint(100, yPosition));
                    yPosition += 20; // Move the y position down after each row

                    // Check if the page is full and if so, add a new page
                    if (yPosition > page.Height - 50)
                    {
                        page = doc.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPosition = 40; // Reset the y position
                    }
                }

                // Save the document
                string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputPath = $@"C:\Reports\SalesReport_{currentDateTime}.pdf"; // Set your desired output path
                doc.Save(outputPath);

                MessageBox.Show("Done generating sales report! File Path: " + outputPath, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating PDF: " + ex.Message);
            }
        }
    }
}

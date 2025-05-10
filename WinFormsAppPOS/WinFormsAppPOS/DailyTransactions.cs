using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WinFormsAppPOS.SalesReport;

namespace WinFormsAppPOS
{
    public partial class DailyTransactions : Form
    {
        public DailyTransactions()
        {
            InitializeComponent();
            LoadData();
        }

        public class DailyReport
        {
            public string Date { get; set; }
            public string ID { get; set; }
            public string Product { get; set; }
            public string Price { get; set; }
            public string Quantity { get; set; }
            public string SubTotal { get; set; }
            public string Discount { get; set; }
            public string VAT { get; set; }
            public string Total { get; set; }
        }

        public void LoadData()
        {
            string filePath = Path.Combine(Application.StartupPath, "orders.txt");
            List<DailyReport> orders = new List<DailyReport>();

            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var data = line.Split('|');
                    if (data.Length >= 9) // Ensure there's enough data
                    {
                        orders.Add(new DailyReport
                        {
                            Date = data[0],
                            ID = data[1],
                            Product = data[2],
                            Price = data[3],
                            Quantity = data[4],
                            SubTotal = data[5],
                            Discount = data[6],
                            VAT = data[7],
                            Total = data[8],
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
                // Read all lines from file
                var lines = File.ReadAllLines(filePath);

                // Get today's date (no time)
                var today = DateTime.Today;

                // Filter lines with today's date
                List<string[]> todayTransactions = new List<string[]>();

                foreach (var line in lines)
                {
                    var data = line.Split('|');
                    if (DateTime.TryParseExact(data[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime entryDate))
                    {
                        if (entryDate.Date == today)
                        {
                            todayTransactions.Add(data);
                        }
                    }
                }

                if (todayTransactions.Count == 0)
                {
                    MessageBox.Show("No transactions found for today.");
                    return;
                }

                // Create PDF
                PdfDocument doc = new PdfDocument();
                doc.Info.Title = $"Daily Transactions - {today:MMMM dd, yyyy}";

                PdfPage page = doc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont headerFont = new XFont("Arial", 14);
                XFont textFont = new XFont("Arial", 12);
                double y = 40;

                // Title
                gfx.DrawString($"Daily Transactions - {today:MMMM dd, yyyy}", headerFont, XBrushes.Black, new XPoint(100, y));
                y += 30;

                // Column headers
                gfx.DrawString("Date | Product | Price | Qty | Subtotal | Discount | VAT | Total", textFont, XBrushes.Black, new XPoint(40, y));
                y += 20;

                foreach (var data in todayTransactions)
                {
                    string time = DateTime.Parse(data[0]).ToString("yyyy-MM-dd HH:mm:ss");
                    string product = data[2];
                    string price = $"₱{int.Parse(data[3]):N2}";
                    string qty = data[4];
                    string subtotal = $"₱{int.Parse(data[5]):N2}";
                    string discount = data[6];
                    string vat = data[7];
                    string total = $"₱{double.Parse(data[8]):N2}";

                    string row = $"{time} | {product} | {price} | {qty} | {subtotal} | {discount} | {vat} | {total}";

                    gfx.DrawString(row, textFont, XBrushes.Black, new XPoint(40, y));
                    y += 20;

                    if (y > page.Height - 50)
                    {
                        page = doc.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 40;
                    }
                }

                // Ensure reports folder exists
                string folderPath = @"C:\Reports";
                Directory.CreateDirectory(folderPath);

                // Generate safe file name
                string fileDate = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputPath = $@"C:\Reports\DailyTransactions_{fileDate}.pdf";

                doc.Save(outputPath);

                MessageBox.Show("Done generating daily report!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating daily transactions PDF:\n" + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DailyTransactions_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }
    }
}

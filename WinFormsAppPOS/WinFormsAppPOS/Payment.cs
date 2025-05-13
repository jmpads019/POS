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
    public partial class Payment : Form
    {
        public event Action DataUpdated;
        string filePathOrder = Path.Combine(Application.StartupPath, "orders.txt");
        string filePathItemsSold = Path.Combine(Application.StartupPath, "itemsSold.txt");
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
            decimal subTotal = 0;
            decimal totalDiscountAmount = 0;

            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length >= 10)
                {
                    if (decimal.TryParse(parts[9], out decimal total))
                    {
                        subTotal += total;
                    }

                    if (decimal.TryParse(parts[7], out decimal totalDiscount))
                    {
                        totalDiscountAmount += totalDiscount;
                    }
                }
            }

            decimal grandTotal = subTotal - totalDiscountAmount;

            return grandTotal;
        }

        private void txtPayment_TextChanged(object sender, EventArgs e)
        {
            txtPayment.TextChanged -= txtPayment_TextChanged; // Prevent recursive calls

            string rawInput = txtPayment.Text.Replace(",", "");
            int cursorPosition = txtPayment.SelectionStart;

            if (decimal.TryParse(rawInput, out decimal payment))
            {
                // Split into integer and fractional parts
                string[] parts = rawInput.Split('.');

                // Format integer part with commas
                string formatted = string.Format("{0:N0}", decimal.Parse(parts[0]));

                // Add decimal part back, if present
                if (parts.Length > 1)
                {
                    formatted += "." + parts[1];
                }

                // Set formatted text and restore cursor
                txtPayment.Text = formatted;

                // Calculate new cursor position
                int diff = txtPayment.Text.Length - rawInput.Length;
                txtPayment.SelectionStart = Math.Max(0, cursorPosition + diff);
            }

            txtPayment.TextChanged += txtPayment_TextChanged; // Reattach event

            // Update balance
            if (decimal.TryParse(txtTotal.Text.Replace(",", ""), out decimal total) &&
                decimal.TryParse(txtPayment.Text.Replace(",", ""), out payment))
            {
                decimal change = payment - total;
                txtBalance.Text = change.ToString("N2");
            }
            else
            {
                txtBalance.Text = txtTotal.Text;
            }
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtTotal.Text, out decimal total) &&
                decimal.TryParse(txtPayment.Text, out decimal payment))
            {
                if (payment < total)
                {
                    MessageBox.Show("Insufficient Payment");
                }
                else
                {
                    MessageBox.Show("Payment successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    string filePath = Path.Combine(Application.StartupPath, "orders.txt");
                    try
                    {
                        var lines = File.ReadAllLines(filePath);

                        PdfDocument doc = new PdfDocument();
                        doc.Info.Title = "Receipt";

                        PdfPage page = doc.AddPage();
                        XGraphics gfx = XGraphics.FromPdfPage(page);

                        // Fonts
                        XFont titleFont = new XFont("Courier New", 14, XFontStyleEx.Bold);
                        XFont bodyFont = new XFont("Courier New", 10, XFontStyleEx.Bold);
                        XFont boldFont = new XFont("Courier New", 10, XFontStyleEx.Bold);

                        double y = 20;

                        // Header - Store Info
                        gfx.DrawString("Falcon Mart", titleFont, XBrushes.Black, new XPoint(20, y));
                        y += 20;
                        gfx.DrawString("San Dionisio in Parañaque", bodyFont, XBrushes.Black, new XPoint(20, y));
                        y += 15;
                        gfx.DrawString("Date: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"), bodyFont, XBrushes.Black, new XPoint(20, y));
                        y += 25;

                        gfx.DrawString("Qty  Item                Price   Total", boldFont, XBrushes.Black, new XPoint(20, y));
                        y += 15;

                        decimal cartTotal = 0, vat = 0, vatable = 0, discount = 0, grandTotal = 0;

                        foreach (var line in lines)
                        {
                            var data = line.Split('|');
                            if (data.Length < 10) continue;

                            string qty = data[4].PadRight(4);
                            string product = data[2].PadRight(18).Substring(0, 18);
                            string price = Convert.ToDecimal(data[3]).ToString("N2").PadLeft(7);
                            string totalAmount = Convert.ToDecimal(data[5]).ToString("N2").PadLeft(7);

                            gfx.DrawString($"{qty} {product} {price} {totalAmount}", bodyFont, XBrushes.Black, new XPoint(20, y));
                            y += 15;

                            cartTotal += Convert.ToDecimal(data[5]);
                            discount += Convert.ToDecimal(data[7]);
                            grandTotal += Convert.ToDecimal(data[9]);


                            if (!File.Exists(filePathItemsSold))
                            {
                                File.Create(filePathItemsSold).Close();
                            }

                            List<string> lines2 = new List<string>();

                            string line2 = string.Join("|", data[0].ToString(), data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9]);
                            lines2.Add(line2);

                            File.AppendAllText(filePathItemsSold, line2 + Environment.NewLine);
                        }

                        // VAT Calculations
                        vat = grandTotal * 0.12m; // Example 12% VAT
                        vatable = grandTotal - vat;

                        // Footer
                        y += 20;
                        gfx.DrawLine(XPens.Black, 20, y, 180, y);
                        y += 10;

                        gfx.DrawString($"Subtotal:     {cartTotal.ToString("N2")}", bodyFont, XBrushes.Black, new XPoint(20, y)); y += 15;
                        gfx.DrawString($"VAT (12%):    {vat.ToString("N2")}", bodyFont, XBrushes.Black, new XPoint(20, y)); y += 15;
                        gfx.DrawString($"Vatable:      {vatable.ToString("N2")}", bodyFont, XBrushes.Black, new XPoint(20, y)); y += 15;
                        gfx.DrawString($"Discount:     -{discount.ToString("N2")}", bodyFont, XBrushes.Black, new XPoint(20, y)); y += 15;

                        gfx.DrawLine(XPens.Black, 20, y, 180, y); y += 10;
                        gfx.DrawString($"TOTAL:        {grandTotal.ToString("N2")}", boldFont, XBrushes.Black, new XPoint(20, y)); y += 25;                        
                        gfx.DrawString($"PAYMENT:        {payment.ToString("N2")}", boldFont, XBrushes.Black, new XPoint(20, y)); y += 15;

                        decimal change = 0;
                        decimal.TryParse(txtBalance.Text, out change);
                        gfx.DrawString($"CHANGE:        {change.ToString("N2")}", boldFont, XBrushes.Black, new XPoint(20, y)); y += 25;

                        gfx.DrawString("Thank you for shopping!", bodyFont, XBrushes.Black, new XPoint(20, y));

                        string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        string outputPath = $@"C:\Reports\Receipt_{currentDateTime}.pdf";
                        doc.Save(outputPath);

                        MessageBox.Show("Receipt generated successfully:\n" + outputPath, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (File.Exists(filePathOrder))
                        {
                            File.WriteAllText(filePathOrder, string.Empty);
                        }

                        DataUpdated?.Invoke();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error generating receipt: " + ex.Message);
                    }
                }
            }
        }
    }
}

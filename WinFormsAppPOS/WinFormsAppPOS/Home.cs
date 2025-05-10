using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace WinFormsAppPOS
{
    public partial class HomeForm : Form
    {
        private string loggedInUsername;
        public HomeForm(string username)
        {
            InitializeComponent();
            loggedInUsername = username;
        }

        private void Home_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;

            labelUsername.Text = "Welcome, " + loggedInUsername.ToString() + "!";
            timer1.Interval = 1000; // Update every second
            timer1.Tick += timer1_Tick; // Attach the Tick event
            timer1.Start(); // Start the timer

            Dashboard obj = new Dashboard();
            obj.MdiParent = this;
            obj.Dock = DockStyle.Fill;
            obj.StartPosition = FormStartPosition.CenterScreen;
            obj.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelDateTime.Text = DateTime.Now.ToString("dddd, MMM dd, yyyy hh:mm:ss tt");
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            this.Close(); // Hide the current home page
            LoginForm login = new LoginForm(); // Create a new instance of the login form
            login.Show(); // Show the login form
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            Dashboard obj = new Dashboard();
            obj.MdiParent = this;
            obj.Dock = DockStyle.Fill;
            obj.StartPosition = FormStartPosition.CenterScreen;
            obj.Show();
        }

        bool expand = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (expand == false)
            {
                controlPanel.Height += 109;
                expand = true;
            }
            else
            {
                controlPanel.Height -= 109;
                expand = false;
            }
        }

        private void btnManageProduct_Click(object sender, EventArgs e)
        {
            ManageProduct obj = new ManageProduct();
            obj.MdiParent = this;
            obj.Dock = DockStyle.Fill;
            obj.StartPosition = FormStartPosition.CenterScreen;
            obj.Show();
        }

        private void btnManageStaff_Click(object sender, EventArgs e)
        {

        }

        private void btnPOS_Click(object sender, EventArgs e)
        {
            POS obj = new POS();
            obj.MdiParent = this;
            obj.Dock = DockStyle.Fill;
            obj.StartPosition = FormStartPosition.CenterScreen;
            obj.Show();
        }

        bool expandReports = false;
        private void btnGenerateReport_Click_1(object sender, EventArgs e)
        {
            if (expandReports == false)
            {
                panelReports.Height += 180;
                expandReports = true;
            }
            else
            {
                panelReports.Height -= 180;
                expandReports = false;
            }
        }

        private void btnSalesReport_Click(object sender, EventArgs e)
        {
            SalesReport obj = new SalesReport();
            obj.MdiParent = this;
            obj.Dock = DockStyle.Fill;
            obj.StartPosition = FormStartPosition.CenterScreen;
            obj.Show();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            InventoryReport obj = new InventoryReport();
            obj.MdiParent = this;
            obj.Dock = DockStyle.Fill;
            obj.StartPosition = FormStartPosition.CenterScreen;
            obj.Show();
        }

        private void btnDailyTransaction_Click(object sender, EventArgs e)
        {
            DailyTransactions obj = new DailyTransactions();
            obj.MdiParent = this;
            obj.Dock = DockStyle.Fill;
            obj.StartPosition = FormStartPosition.CenterScreen;
            obj.Show();
        }

        //private void btnMaintenance_Click(object sender, EventArgs e)
        //{
        //    Maintenance obj = new Maintenance();
        //    obj.MdiParent = this;
        //    obj.Dock = DockStyle.Fill;
        //    obj.StartPosition = FormStartPosition.CenterScreen;
        //    obj.Show();

        //    ManageProduct manageProduct= new ManageProduct();
        //    manageProduct.Hide();
        //}
    }
}

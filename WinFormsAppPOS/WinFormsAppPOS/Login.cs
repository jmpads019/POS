using Microsoft.VisualBasic.ApplicationServices;

namespace WinFormsAppPOS
{
    public partial class LoginForm : Form
    {
        private List<User> users;
        public LoginForm()
        {
            InitializeComponent();
            txtUserName.Focus();

            users = new List<User>
            {
                new User { Username = "admin", Password = "admin123" },
                new User { Username = "cashier", Password = "cashier" }
            };
        }

        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUserName.Text;
                string password = txtPassword.Text;

                // Check the credentials against the list of users
                User user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

                if (user != null)
                {
                    MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide(); // Hide login form
                    new HomeForm(username).Show(); // Open main application form
                }
                else
                {
                    MessageBox.Show("Login failed!", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
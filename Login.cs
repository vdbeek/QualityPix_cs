using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace QP
{
    public partial class Login : Form
    {
        private RoundedForm _main;
        public Login(RoundedForm inRef)
        {
            InitializeComponent();
            _main = inRef;
        }

        protected override void OnLoad(EventArgs e)
        {
            txtUsername.BackColor = Color.FromArgb(255, 228, 151, 88);
            txtUsername.ForeColor = Color.Black;
            txtUsername.Text = @"Username";

            txtPassword.BackColor = Color.FromArgb(255, 228, 151, 88);
            txtPassword.ForeColor = Color.Black;
            txtPassword.Text = @"Password";
            txtPassword.UseSystemPasswordChar = true;
            
            btnLogin.BackColor = Color.FromArgb(255, 228, 151, 88);
            btnLogin.Text = @"Login";
            btnRegister.BackColor = Color.FromArgb(255, 228, 151, 88);
            btnRegister.Text = @"Register";
            base.OnLoad(e);
        }

        private void txtUsername_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (txtUsername.Text == @"Username") txtUsername.Text = "";
        }

        private void txtPassword_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (txtPassword.Text == @"Password") txtPassword.Text = "";
        }

        private void btnRegister_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtUsername.Text.Length > 1 && txtPassword.Text.Length > 1)
            {
                
            }
            // get whatever is in the boxes, connect to db and create account
        }

        private void btnLogin_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtUsername.Text.Length > 1 && txtPassword.Text.Length > 1)
            {
                string q =
                    $"Select * from db_users where UserName = '{txtUsername.Text}' and Password = '{txtPassword.Text}'";
                cSQL v = new cSQL();
                DataSet r = v.Query(q);

                if (r.Tables[0].Rows.Count >= 1)
                {
                    if (r.Tables[0].Rows[0].ItemArray[0].ToString() == "0")
                    {
                        _main._adm = true;
                    }
                    else { _main._adm = false; }

                    _main._user = r.Tables[0].Rows[0].ItemArray[1].ToString();
                    _main.LogIn();
                }
            }
        }
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return){btnLogin_MouseClick(this,new MouseEventArgs(MouseButtons.Left,1,0,0,0));}
        }
    }
}
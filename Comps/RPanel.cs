using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QP
{
    public partial class RPanel : Panel
    {
        //private Region _client;
        public int _movieId;
        public string _userString;
        public RPanel()
        {
            InitializeComponent();
        }

        public RPanel(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            // drop down for stars
            base.OnHandleCreated(e);
            
            ComboBox stars = new ComboBox();
            stars.Location = new Point(ClientRectangle.X+10, ClientRectangle.Y+25);
            stars.ValueMember = "Value";
            stars.DisplayMember = "Text";

            var list = new[]
            {
                new { Text = "1", Value = 1 },
                new { Text = "2", Value = 2 },
                new { Text = "3", Value = 3 },
                new { Text = "4", Value = 4 },
                new { Text = "5", Value = 5 }
            };

            stars.DataSource = list;
            stars.Name = "cmbStars";

            Controls.Add(stars);
            
            // textbox for review body
            TextBox box = new TextBox();
            box.Multiline = true;
            box.MaxLength = 249;
            box.Size = new Size(200, 200);
            box.Location = new Point(stars.Location.X, stars.Location.Y + stars.ClientRectangle.Height + 25);
            box.Name = "txtReview";
            Controls.Add(box);
            
            // button for submit
            Button submit = new Button();
            submit.BackColor = Color.DarkOrange;
            submit.ForeColor = Color.Black;
            submit.Location = new Point(stars.Location.X, box.Location.Y + box.Height + 25);
            submit.FlatStyle = FlatStyle.Flat;
            submit.FlatAppearance.BorderSize = 0;
            submit.FlatAppearance.MouseDownBackColor = BackColor;
            submit.FlatAppearance.MouseOverBackColor = BackColor;
            submit.Text = @"Submit";
            
            submit.MouseClick += SubmitOnMouseClick;
            
            Controls.Add(submit);
            
            // button to close this menu
            Button close = new Button();
            close.Location = new Point(submit.Location.X + close.Width + 10, submit.Location.Y);
            close.BackColor = Color.DarkOrange;
            close.ForeColor = Color.Black;
            close.FlatStyle = FlatStyle.Flat;
            close.FlatAppearance.BorderSize = 0;
            close.FlatAppearance.MouseDownBackColor = BackColor;
            close.FlatAppearance.MouseOverBackColor = BackColor;
            close.Text = @"Cancel";
            
            close.MouseClick += CloseOnMouseClick;
            
            Controls.Add(close);
        }

        private void CloseOnMouseClick(object sender, MouseEventArgs e)
        {
            Dispose();
        }

        private void SubmitOnMouseClick(object sender, MouseEventArgs e)
        {
            var l = Controls.Find("cmbStars", true);
            var l2 = Controls.Find("txtReview", true);

            int numStar = Convert.ToInt16(l[0].Text);
            var reviewBody = l2[0].Text;

            cSQL sql = new cSQL();
            var ds = sql.Query($"select * from db_users where UserName = '{_userString}'");

            var userId = Convert.ToInt16(ds.Tables[0].Rows[0].ItemArray[0].ToString());

            sql.Query(
                $"insert into db_reviews (userId,movieId,stars,reviewBody) VALUES ('{userId}','{_movieId}','{numStar}','{reviewBody}') ");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            BackColor = Color.FromArgb(255, 49, 39, 71);
            e.Graphics.DrawString("Number of Stars:",new Font("Segoe UI",9,FontStyle.Bold),new SolidBrush(Color.DarkOrange),this.ClientRectangle.X+10,ClientRectangle.Y+10);
            e.Graphics.DrawString("Review:",new Font("Segoe UI",9,FontStyle.Bold),new SolidBrush(Color.DarkOrange),this.ClientRectangle.X+10,ClientRectangle.Y+50);
        }
    }
}
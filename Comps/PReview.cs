using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QP
{
    public partial class PReview : Panel
    {
        public SReview Review;
        public string UserName { get; set; }
        private Label reviewBodyPanel;

        public PReview()
        {
            InitializeComponent();
        }

        public PReview(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // reviewBody
            reviewBodyPanel = new Label();
            //reviewBodyPanel.Size = new Size(300, 100);
            reviewBodyPanel.AutoSize = true;
            reviewBodyPanel.MinimumSize = new Size(750, 80);
            reviewBodyPanel.MaximumSize = new Size(750, 350);
            reviewBodyPanel.Location = new Point(ClientRectangle.X + 10, ClientRectangle.Y + 35);

            // note: debug
            //reviewBodyPanel.BorderStyle = BorderStyle.FixedSingle;
            
            reviewBodyPanel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            reviewBodyPanel.Text = Review.ReviewBody;
            reviewBodyPanel.ForeColor = Color.DarkOrange;

            Controls.Add(reviewBodyPanel);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            
            Rectangle bounds = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);
            var b = new SolidBrush(Color.FromArgb(255, 49, 39, 71));
            
            RoundedRectangle.FillRoundedRectangle(e.Graphics,b,bounds,15);

            // usericon
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            String[] splitter = { "bin" };
            var strings = exePath.Split(splitter, StringSplitOptions.None);
            String dir = strings[0] + @"\content\";
                
            Image u = Image.FromFile(dir + @"profile-user-alt.png");
            Bitmap map = new Bitmap(u, new Size(13, 13));
            e.Graphics.DrawImage(map,ClientRectangle.X+15,ClientRectangle.Y+10);
            
            // draw user name
            var font = new Font("Consolas", 10, FontStyle.Bold | FontStyle.Italic | FontStyle.Underline);
            e.Graphics.DrawString(UserName,font,new SolidBrush(Color.FloralWhite),ClientRectangle.X+35,ClientRectangle.Y+8f);
            var fontM = e.Graphics.MeasureString(UserName, font);
            
            // stars
            // TODO: 
            /*
            * find value in range review.stars for each var < .stars is full else not 
            */
            int emptyStars = Review.Stars;
            emptyStars = 5 - emptyStars;
            PointF offset = new PointF(ClientRectangle.X + 40 + fontM.Width + 5, ClientRectangle.Y + 9);
            for (int star = 0; star < Review.Stars; star++)
            {
                Bitmap fillStar = new Bitmap(Image.FromFile(dir + @"starfill_o.png"), new Size(13, 13));
                // offset 15+13+
                e.Graphics.DrawImage(fillStar,offset);
                offset = new PointF(offset.X + 15, offset.Y);
            }
            if (emptyStars > 0)
            {
                for (int empty = 0; empty < emptyStars; empty++)
                {
                    Bitmap emptyStar = new Bitmap(Image.FromFile(dir + @"star_o.png"), new Size(13, 13));
                    e.Graphics.DrawImage(emptyStar,offset);
                    offset = new PointF(offset.X + 15, offset.Y);
                }
            }
        }
    }
}
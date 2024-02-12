using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CefSharp.WinForms;

namespace QP
{
    public partial class InfoPanel : Panel
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );
        
        private Region _client;
        public Movie Movie { get; set; }
        public Font _Font { get; set; }

        private ChromiumWebBrowser chrome;
        
        public InfoPanel()
        {
            InitializeComponent();
        }

        public InfoPanel(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _client = Region.FromHrgn(CreateRoundRectRgn(0, 0, ClientRectangle.Width,
                ClientRectangle.Height, 20, 20));
            /*
            <html>
            <head>
            <title>My Win Forms App<title>
            </head>
            <body>
            <script>There code</script>
            </body>
            </html>
            */
            Panel container = new Panel();
            container.Size = new Size(560, 315);
            container.Location = new Point(ClientRectangle.X+35, ClientRectangle.Bottom-container.Height-35);
            
            Controls.Add(container);
            
            string address = $@"https://www.youtube.com/embed/{Movie.Trailer}?controls=1&mute=1&showinfo=0&rel=0&autoplay=1";
            chrome = new ChromiumWebBrowser(address);
            chrome.Location = ClientRectangle.Location;
            chrome.Size = new Size(200, 200);
            
            container.Controls.Add(chrome);
            
            var synopsisFont = new Font("Segoe UI", 9, FontStyle.Bold);
            
            Label synopsis = new Label();
            synopsis.Location = new Point(ClientRectangle.X + 30, ClientRectangle.Y + 50);
            synopsis.Size = new Size(550, 50);
            synopsis.Text = Movie.Synopsis;
            synopsis.Font = synopsisFont;
            ForeColor = Color.DarkOrange;
            BackColor = Color.Transparent;

            Controls.Add(synopsis);
            
            // TODO: ////////////////////////////////////////////////////////////////////////////////////////////////////////
            // TODO: Wrap this around its own panel, so we can change the content of this panel to a form for adding new reviews

            FlowLayoutPanel flowPanel = new FlowLayoutPanel();
            flowPanel.Name = @"flowPanel";
            flowPanel.AutoScroll = false;
            flowPanel.VerticalScroll.Maximum = 0;
            flowPanel.VerticalScroll.Visible = false;
            flowPanel.HorizontalScroll.Maximum = 0;
            flowPanel.HorizontalScroll.Visible = false;
            flowPanel.AutoScroll = true;
            flowPanel.FlowDirection = FlowDirection.TopDown;
            flowPanel.WrapContents = false;

            flowPanel.Location = new Point(container.ClientRectangle.X + container.Size.Width + 90, ClientRectangle.Y + 15);
            flowPanel.Size = new Size(840, 375);
            
            //flowPanel.VerticalScroll.Maximum
            
            // Note: debug
            //flowPanel.BorderStyle = BorderStyle.FixedSingle;
            
            Controls.Add(flowPanel);
            
            // NOTE: /////////////////////////////////////
            // NOTE: BUILD REVIEWS IN LOOP USING SREVIEW
            // NOTE: /////////////////////////////////////

            cSQL sql = new cSQL();
            var reviewsDB = sql.Query($"select * from db_reviews where movieID = '{Movie.Id}'");
            
            for (int X = 0; X < reviewsDB.Tables[0].Rows.Count; X++)
            {
                // measure rough size of the reviewbody
                string rb = Convert.ToString(reviewsDB.Tables[0].Rows[X].ItemArray[4]);
                //int numL = Convert.ToInt16(rb.Length / 100);
                
                PReview localReview = new PReview();
                localReview.AutoSize = true;
                //localReview.MinimumSize = new Size(800, localReview.ClientRectangle.Bottom+35);

                if (reviewsDB.Tables[0].Rows.Count > 0)
                {
                    // convert userID to string
                    var u = sql.Query($"select * from db_users where userID = '{reviewsDB.Tables[0].Rows[X].ItemArray[1]}'");
                    
                    var localUser = Convert.ToString(u.Tables[0].Rows[0].ItemArray[1]);
                    
                    localReview.UserName = localUser;
                    
                    localReview.Review = new SReview(
                        Convert.ToInt16(reviewsDB.Tables[0].Rows[X].ItemArray[0]),
                        Convert.ToInt16(reviewsDB.Tables[0].Rows[X].ItemArray[1]),
                        Convert.ToInt16(reviewsDB.Tables[0].Rows[X].ItemArray[2]),
                        Convert.ToInt16(reviewsDB.Tables[0].Rows[X].ItemArray[3]),
                        Convert.ToString(reviewsDB.Tables[0].Rows[X].ItemArray[4])
                        );
                }
                // note: debug
                //localReview.BorderStyle = BorderStyle.FixedSingle;
                
                //localReview.MinimumSize = new Size(800, localReview.Text.Length*2);
                flowPanel.Controls.Add(localReview);
            }
        }
        

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            // Todo: Maybe gradient?
            Brush t = new SolidBrush(Color.FromArgb(128, 128, 128, 128));
            Color bot = Color.FromArgb(255, 49, 39, 71);
            Color top = Color.FromArgb(255,67,65,119);
            LinearGradientBrush gradient = new LinearGradientBrush(ClientRectangle, top, bot, -89.0f);
            e.Graphics.FillRegion(gradient,_client);
            
            Rectangle trailerRectangle = new Rectangle(ClientRectangle.X+25,
                ClientRectangle.Bottom - 355,
                chrome.ClientRectangle.Width+20, 
                chrome.ClientRectangle.Height+10
                );
            
            RoundedRectangle.FillRoundedRectangle(e.Graphics,new SolidBrush(Color.Black),trailerRectangle,15);
            
            // Todo: write string with Movie.Info
            // Movie Name, year, age rating goes here
            int width = 500;
            
            _Font = new Font("Segoe UI", 18,FontStyle.Bold);
            
            string movieTitleInfo = $"{Movie.Title} ({Movie.Year})";
            
            Rectangle movieInfoRec = new Rectangle(ClientRectangle.X+25,
                                                   ClientRectangle.Y + 5, width,70);
            
            // Note: debug
            //e.Graphics.FillRectangle(new SolidBrush(Color.Black),movieInfoRec);
            
            e.Graphics.DrawString(movieTitleInfo,_Font,new SolidBrush(Color.FromArgb(255,255,150,0)),movieInfoRec.X,movieInfoRec.Y);
            
            // rating 
            var m = e.Graphics.MeasureString(movieTitleInfo, _Font);
            Rectangle ratingRec = new Rectangle();
            if (Movie.Rating.Length > 3)
            {
                ratingRec = new Rectangle(movieInfoRec.X + Convert.ToInt16(m.Width),
                    movieInfoRec.Y + Convert.ToInt16((m.Height - 18) / 2),50,20);
            }
            else
            {
                ratingRec = new Rectangle(movieInfoRec.X + Convert.ToInt16(m.Width),
                    movieInfoRec.Y + Convert.ToInt16((m.Height - 18) / 2),30,20);
            }
            
            RoundedRectangle.FillRoundedRectangle(e.Graphics,new SolidBrush(Color.FromArgb(100,255,255,255)),ratingRec,5);
            var ratingFont = new Font("Segoe UI", 8, FontStyle.Bold);
            e.Graphics.DrawString(Movie.Rating,ratingFont,new SolidBrush(Color.FromArgb(255,49,39,71)), ratingRec.X+ratingRec.Width/6, ratingRec.Y + ratingRec.Height/6);
           
            var p = new Pen(Color.DarkOrange, 1);
            e.Graphics.DrawLine(p,ClientRectangle.X+30,ratingRec.Y+ratingRec.Height+10,ClientRectangle.X+600,ratingRec.Y+ratingRec.Height+10);
            
            // Runtime
            string runTime = Movie.RunTime + " Min.";
            e.Graphics.DrawString(runTime,ratingFont,new SolidBrush(Color.FromArgb(255,255,150,0)),ratingRec.X+ratingRec.Width+5,ratingRec.Y+3.5f);
        }
    }
}
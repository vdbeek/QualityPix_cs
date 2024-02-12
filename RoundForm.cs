using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace QP
{
    public struct Movie
    {
        public Movie(int inId, string inTitle, string inYear, int inRunTime, string inRating, string inTrailer, string inSynopsis)
        {
            Id = inId;
            Title = inTitle;
            Year = inYear;
            RunTime = inRunTime;
            Rating = inRating;
            Trailer = inTrailer;
            Synopsis = inSynopsis;
        }
        
        public int Id { get; }
        public string Title { get; }
        public string Year { get; }
        public int RunTime { get; }
        public string Rating { get; }
        public string Trailer { get; }
        public string Synopsis { get; }

        public override string ToString()
        {
            String r = $@"Id:: {Id.ToString()} | Title:: {Title} | Year:: {Year} | RunTime:: {RunTime.ToString()} | Rating:: {Rating} | Trailer:: {Trailer} | Synopsis:: {Synopsis}";
            return r;
        }
    }

    public struct SReview
    {
        public SReview(int inId, int inUserId, int inMovieId, int inStars, string inReviewBody)
        {
            Id = inId;
            UserId = inUserId;
            MovieId = inMovieId;
            Stars = inStars;
            ReviewBody = inReviewBody;
        }
        public int Id { get; }
        public int UserId { get; }
        public int MovieId { get; }
        public int Stars { get; }
        public string ReviewBody { get; }
    }
    
    public partial class RoundedForm : Form
    {
        private readonly Timer _drawTimer = new Timer();
        private readonly Size _res = SystemInformation.PrimaryMonitorSize;
        private string _contentdir;
        
        // cef
        private CefSettings _settings;
        
        // Panels we need to store
        private InfoPanel _movieDetail;
        private int _scrollLocation = 0;
        private Panel _info;
        private Panel _carousel;
        private RPanel _addReviewPanel;
        
        // Todo: wip
        public bool _adm { get; set; }
        public string _user { get; set; }
        
        public RoundedForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            String[] splitter = { "bin" };
            var strings = exePath.Split(splitter, StringSplitOptions.None);
            String dir = strings[0];
            _contentdir = dir + @"content\movies\";
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!DesignMode)
                {
                    cp.ExStyle |= 0x00080000;
                }
                return cp;
            }
        }

        public void LogIn()
        {
            var panel1 = Controls.Find("loginPanel", true);
            if (panel1[0] != null)
            {
                panel1[0].Dispose();
            }
            RenderLogin();
            
           
        }
        
#region EVENT
        
        protected override void OnLoad(EventArgs e)
        {
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            if (!DesignMode)
            {
                _drawTimer.Interval = 1000 / 60;
                _drawTimer.Tick += DrawForm;
                _drawTimer.Start();
            }
            base.OnLoad(e);
            
            _settings = new CefSettings();
            _settings.CefCommandLineArgs["autoplay-policy"] = "no-user-gesture-required";
            Cef.Initialize(_settings);

            Location = new Point(_res.Width/2-Bounds.Width/2,
                                 _res.Height/2-Bounds.Height/2);

            BuildHandle();
            BuildLoginPanel();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                Graphics graphics = e.Graphics;

                Rectangle gradientRectangle = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

                Brush b = new LinearGradientBrush(gradientRectangle, Color.DarkSlateBlue, Color.MediumPurple, 0.0f);

                graphics.SmoothingMode = SmoothingMode.HighQuality;

                RoundedRectangle.FillRoundedRectangle(graphics, b, gradientRectangle, 35);
            }
            base.OnPaint(e);
        }
        
        private void DrawForm(object pSender, EventArgs pE)
        {
            using (Bitmap backImage = new Bitmap(this.Width, this.Height))
            {
                using (Graphics graphics = Graphics.FromImage(backImage))
                {
                    Rectangle gradientRectangle = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                    Rectangle contentRectangle = new Rectangle(gradientRectangle.Width / 2 - Convert.ToInt16(ClientRectangle.Width * 0.80f) / 2,
                        gradientRectangle.Height / 2 - Convert.ToInt16(ClientRectangle.Height * 0.80f) / 2, Convert.ToInt16(ClientRectangle.Width * 0.80f),
                        Convert.ToInt16(ClientRectangle.Height * 0.80f));

                    Rectangle handleRectangle = new Rectangle(ClientRectangle.Left+90, 35/2*-1, 
                                                           ClientRectangle.Right-180, 55);
                    
                    //TODO: /////////////////////////////////////////////////////////////
                    //TODO: ///////// ADD QUALITY PIX LOGO ABOVE MOVIE CAROUSEL /////////
                    //TODO: /////////////////////////////////////////////////////////////
                    
                    using (Brush b = new LinearGradientBrush(gradientRectangle, 
                                                        Color.FromArgb(255,30,29,53),
                                                        Color.FromArgb(255,158,78,98), -89.0f))
                    {
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        
                        Brush bColor = new SolidBrush(Color.FromArgb(255, 30, 29, 53));
                        RoundedRectangle.FillRoundedRectangle(graphics, b, gradientRectangle, 35);
                        RoundedRectangle.FillRoundedRectangle(graphics,bColor,contentRectangle,20);
                        RoundedRectangle.FillRoundedRectangle(graphics,bColor,handleRectangle,20);
                        //graphics.FillRectangle(new SolidBrush(Color.FromArgb(255,30,29,53)),contentRectangle);

                        foreach (Control ctrl in this.Controls)
                        {
                            using (Bitmap bmp = new Bitmap(ctrl.Width, ctrl.Height))
                            {
                                Rectangle rect = new Rectangle(0, 0, ctrl.Width, ctrl.Height);
                                ctrl.DrawToBitmap(bmp, rect);
                                graphics.DrawImage(bmp, ctrl.Location);
                            }
                        }
                        /*
                        if (_carousel is { Visible: true })
                        {
                            string exePath = AppDomain.CurrentDomain.BaseDirectory;
                            String[] splitter = { "bin" };
                            var strings = exePath.Split(splitter, StringSplitOptions.None);
                            String dir = strings[0];
                            Bitmap bm = new Bitmap(Image.FromFile(dir + "content/logo.png"));

                            graphics.DrawImage(bm,new PointF(ClientRectangle.Width/2 - bm.Width/2,ClientRectangle.Top+75));
                        }
                        */
                        PerPixelAlphaBlend.SetBitmap(backImage, Left, Top, Handle);
                    }
                }
            }
        }
        
        private void BtnCloseOnMouseClick(object sender, MouseEventArgs e)
        {
            Environment.Exit(0);
        }

        private void LblControlOnMouseDown(object sender, MouseEventArgs e)
        {
            Win32.ReleaseCapture();
            Win32.SendMessage(Handle,0x112,0xf012,0);
        }
        
        /*
         * Drag Window Override
         */
        protected override void WndProc(ref Message m)
        {
            const int wmCalcSize = 0x0082;
            if (m.Msg == wmCalcSize && m.WParam.ToInt32() == 1)
            {
                return;
            }
            base.WndProc(ref m);
        }
        
#endregion
        
#region GFX

        private void BuildHandle()
        {
            // Build a handle for grab control at the top of the screen
            Label lblControl = new Label();
            lblControl.Location = new Point(ClientRectangle.Left + 100, 0);
            //lblControl.Width = _res.Width-1;
            lblControl.Height = 35;
            lblControl.Bounds = new Rectangle(ClientRectangle.Left + 100, 0, ClientRectangle.Right-200, lblControl.Height);
            lblControl.BackColor = Color.FromArgb(255,30,29,53);
            lblControl.ForeColor = Color.FromArgb(255, 228, 151, 88);
            lblControl.TextAlign = ContentAlignment.MiddleCenter;
            lblControl.Font = new Font("MS Gothic", 9,FontStyle.Bold);
            lblControl.Text = @"QualityPix";

            lblControl.MouseDown += LblControlOnMouseDown;

            Controls.Add(lblControl);
            
            //^ HANDLE ^
            
            // Close and Minimize buttons
            RButton btnClose = new RButton();
            btnClose._inset = true;
            btnClose.Size = new Size(20, 20);
            btnClose.Location = new Point(ClientRectangle.Right - 230, (lblControl.Height / 2 - btnClose.Height / 2));
            btnClose.Name = @"btnClose";
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnClose._Color = Color.Red;
            btnClose.MouseClick += BtnCloseOnMouseClick;

            lblControl.Controls.Add(btnClose);
        }
        private void BuildLoginPanel()
        {
            // roundabout way to do this
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            String[] splitter = { "bin" };
            var strings = exePath.Split(splitter, StringSplitOptions.None);
            String dir = strings[0];
            
            Image p = Image.FromFile(dir+@"\content\logo.png");
            _contentdir = dir + @"content\movies\";

            Panel loginPanel = new Panel();
            loginPanel.Name = @"loginPanel";
            loginPanel.Size = new Size(400, 700);
            loginPanel.Location = new Point(Bounds.Width / 2 - loginPanel.Size.Width/2, 
                Bounds.Height / 2 - loginPanel.Size.Height/2);
            loginPanel.BackColor = Color.FromArgb(255, 30, 29, 53);

            Controls.Add(loginPanel);

            PictureBox pic = new PictureBox();
            pic.Size = p.Size;
            pic.Image = p;
            pic.BackColor = Color.Transparent;

            loginPanel.Controls.Add(pic);

            Login login = new Login(this) { Dock = DockStyle.Bottom, TopLevel = false, TopMost = true };
            login.FormBorderStyle = FormBorderStyle.None;
            login.Name = @"login";
            login.BackColor = Color.FromArgb(255, 30, 29, 53);
            
            loginPanel.Controls.Add(login);
            login.Show();
        }
        private void RenderLogin()
        {
            Panel movieCarousel = new Panel();
            movieCarousel.AutoScroll = false;
            movieCarousel.AutoSize = false;
            movieCarousel.BackColor = Color.FromArgb(255, 30, 29, 53);

            // Size the carousel so 6 movies fit in
            movieCarousel.Size = new Size(1290, 500);
            movieCarousel.Location = new Point(ClientRectangle.Width / 2 - movieCarousel.ClientRectangle.Width / 2, 
                                               Convert.ToInt16(ClientRectangle.Top+ClientRectangle.Height*0.10f));
            
            _carousel = movieCarousel;
            Controls.Add(movieCarousel);
            
            // NOTE: ADM
            if (_adm)
            {
                RButton adm = new RButton();
                adm._rectangle = true;
                adm.BackColor = Color.FromArgb(255, 30, 29, 53);
                adm.Size = new Size(100, 25);
                adm.Location = new Point(250, 135);
                adm.Text = @"AdminPanel";
                Controls.Add(adm);
                adm.MouseClick += AdmOnMouseClick;
            }

            // Login
            cSQL sql = new cSQL();
            var r = sql.Query("select * from db_movies");
            
            if (r.Tables.Count >= 1)
            {
                int i = 0;
                Point offset = new Point(movieCarousel.ClientRectangle.X+90, (movieCarousel.ClientRectangle.Height / 2) - 230/2);
                
                foreach (DataRow row in r.Tables[0].Rows)
                {
                    Image image = Image.FromFile(_contentdir+$"{row.ItemArray[0]}.png");
                    Blutton btn = new Blutton(offset,new Size(180,230),Convert.ToInt16(row.ItemArray[0]));
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackgroundImage = image;
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                    btn.Size = new Size(180, 230);
                    // this creates an artificial gap every 6 movies
                    if (i == 6)
                    {
                        i = 0;
                        offset = new Point(offset.X + 185, offset.Y);
                    }
                    
                    btn.Location = offset;
                    offset = new Point(offset.X + 185,movieCarousel.ClientRectangle.Height / 2 - btn.ClientRectangle.Height / 2);
                    
                    btn.Movie = new Movie(
                        Convert.ToInt16(row.ItemArray[0]),
                        Convert.ToString(row.ItemArray[1]),
                        Convert.ToString(row.ItemArray[2]),
                        Convert.ToInt16(row.ItemArray[3]),
                        Convert.ToString(row.ItemArray[4]),
                        Convert.ToString(row.ItemArray[5]),
                        Convert.ToString(row.ItemArray[6])
                        );

                    movieCarousel.Controls.Add(btn);
                    i++;
                }
                // add dead button to the end for proper offset
                Button btnd = new Button();
                btnd.FlatStyle = FlatStyle.Flat;
                btnd.FlatAppearance.BorderSize = 0;
                btnd.FlatAppearance.MouseDownBackColor = Color.Transparent;
                btnd.Size = new Size(180, 230);
                btnd.Location = offset;
                movieCarousel.Controls.Add(btnd);
            }
            // set max lenght of the scroll bar
            movieCarousel.HorizontalScroll.Maximum = r.Tables[0].Rows.Count*185;

            // build scroll buttons on both sides
            RButton btnLeft = new RButton();
            btnLeft._innerGradient = true;
            btnLeft._inset = true;
            btnLeft._Color = Color.Black;
            btnLeft.FlatStyle = FlatStyle.Flat;
            btnLeft.FlatAppearance.BorderSize = 0;
            btnLeft.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 30, 29, 53);
            btnLeft.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 30, 29, 53);
            btnLeft.Size = new Size(100, 100);
            btnLeft.BackColor = Color.FromArgb(255, 30, 29, 53);

            btnLeft.Location = new Point(movieCarousel.Location.X-btnLeft.Width-50,
                                         movieCarousel.Height/2+btnLeft.ClientRectangle.Height/2);

            btnLeft.Text = @"<<";

            RButton btnRight = new RButton();
            btnRight._innerGradient = true;
            btnRight._inset = true;
            btnRight._Color = Color.Black;
            btnRight.FlatStyle = FlatStyle.Flat;
            btnRight.FlatAppearance.BorderSize = 0;
            btnRight.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 30, 29, 53);
            btnRight.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 30, 29, 53);
            btnRight.Size = new Size(100, 100);
            btnRight.BackColor = Color.FromArgb(255, 30, 29, 53);

            btnRight.Location = new Point((movieCarousel.Location.X + movieCarousel.Width) + btnLeft.Width-50,
                                           movieCarousel.Height/2+btnLeft.ClientRectangle.Height/2);;

            btnRight.Text = @">>";
            
            Controls.Add(btnLeft);
            Controls.Add(btnRight);

            btnLeft.Click += BtnLeftOnClick;
            btnRight.Click += BtnRightOnClick;
        }

        private void AdmOnMouseClick(object sender, MouseEventArgs e)
        {
            adm adm = new adm();
            adm.AutoSize = true;
            adm.Show();
        }

        public void RenderInfo(Movie inMovie)
        {
            // start by adding a panel
            if (_movieDetail != null)
            {  
                if (_addReviewPanel is {Visible:true}) { _addReviewPanel.Dispose(); }
                _info.Dispose();
            }

            _info = new Panel();
            _info.Size = new Size(Convert.ToInt16(_carousel.ClientRectangle.Width + (_carousel.ClientRectangle.Width*0.20f)), _carousel.Height);
            _info.Location = new Point(ClientRectangle.Width/2 - _info.ClientRectangle.Width / 2, Convert.ToInt16(ClientRectangle.Bottom - ClientRectangle.Height*0.13f - _info.ClientRectangle.Height));
            _info.BackColor = Color.FromArgb(255, 30, 29, 53);
            
            Controls.Add(_info);
            
            // into panel add custom panel
            _movieDetail = new InfoPanel();
            _movieDetail.Movie = inMovie;
            _movieDetail.Size = new Size(Convert.ToInt16(_carousel.ClientRectangle.Width + (_carousel.ClientRectangle.Width*0.20f)), 450);
            _movieDetail.Location = new Point(_info.ClientRectangle.Width / 2 - _movieDetail.ClientRectangle.Width / 2,
                _info.ClientRectangle.Height / 2 - _movieDetail.ClientRectangle.Height / 2);
            
            _info.Controls.Add(_movieDetail);
            
            // add a review button
            RButton btnAddReview = new RButton();
            btnAddReview.Name = @"btnAddReview";
            btnAddReview.Text = @"Write Review";
            btnAddReview._rectangle = true;
            btnAddReview.Size = new Size(110, 35);

            btnAddReview.Location = new Point(_info.ClientRectangle.X+_info.Width - btnAddReview.Width - 50,_info.Bottom-5);
            btnAddReview.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 30, 29, 53);
            btnAddReview.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 30, 29, 53);
            btnAddReview.FlatAppearance.BorderSize = 0;
            btnAddReview.BackColor = Color.FromArgb(255, 30, 29, 53);
            btnAddReview.FlatStyle = FlatStyle.Flat;
            btnAddReview.Click += BtnAddReviewOnClick;
            
            Controls.Add(btnAddReview);
        }

        private void BtnAddReviewOnClick(object sender, EventArgs e)
        {
            _addReviewPanel = new RPanel();
            _addReviewPanel.Location = new Point(_info.Location.X+_info.Width+10, _info.Bounds.Y+50);
            _addReviewPanel.Size = new Size(300, 500);
            
            // TODO: send some data with this
            if (_movieDetail.Visible)
            {
                _addReviewPanel._movieId = _movieDetail.Movie.Id;
                _addReviewPanel._userString = _user;
            }
            Controls.Add(_addReviewPanel);
        }

        private void BtnRightOnClick(object sender, EventArgs e)
        {
            if (_addReviewPanel is {Visible:true}) { _addReviewPanel.Dispose(); }
            if (_scrollLocation + 185 * 7 < _carousel.HorizontalScroll.Maximum)
            {
                _scrollLocation += 185 * 7;
                _carousel.HorizontalScroll.Value = _scrollLocation;
            }
            else
            {
                _scrollLocation = 0;
                _carousel.AutoScrollPosition = new Point(0, 0);
            }
        }

        private void BtnLeftOnClick(object sender, EventArgs e)
        {
            if (_addReviewPanel is {Visible:true}) { _addReviewPanel.Dispose(); }
            // First left click, essentially
            if (_carousel.AutoScrollPosition == new Point())
            {
                _scrollLocation = _carousel.HorizontalScroll.Maximum - 185*5;
                _carousel.HorizontalScroll.Value = _scrollLocation;
            }
            else
            {
                _scrollLocation = 0;
                _carousel.AutoScrollPosition = new Point();
            }
        }

#endregion
    }

    public static class RoundedRectangle
    {
        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public static void FillRoundedRectangle(Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (brush == null)
                throw new ArgumentNullException("brush");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }
    }

    internal static class PerPixelAlphaBlend
    {
        public static void SetBitmap(Bitmap bitmap, int left, int top, IntPtr handle)
        {
            SetBitmap(bitmap, 255, left, top, handle);
        }

        public static void SetBitmap(Bitmap bitmap, byte opacity, int left, int top, IntPtr handle)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");


            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr memDc = Win32.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                oldBitmap = Win32.SelectObject(memDc, hBitmap);

                Win32.Size size = new Win32.Size(bitmap.Width, bitmap.Height);
                Win32.Point pointSource = new Win32.Point(0, 0);
                Win32.Point topPos = new Win32.Point(left, top);
                Win32.BLENDFUNCTION blend = new Win32.BLENDFUNCTION();
                blend.BlendOp = Win32.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = opacity;
                blend.AlphaFormat = Win32.AC_SRC_ALPHA;

                Win32.UpdateLayeredWindow(handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, Win32.ULW_ALPHA);
            }
            finally
            {
                Win32.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBitmap);
                    Win32.DeleteObject(hBitmap);
                }

                Win32.DeleteDC(memDc);
            }
        }
    }

    internal class Win32
    {
        public enum Bool
        {
            False = 0,
            True
        };


        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public Int32 x;
            public Int32 y;

            public Point(Int32 x, Int32 y) { this.x = x; this.y = y; }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            public Int32 cx;
            public Int32 cy;

            public Size(Int32 cx, Int32 cy) { this.cx = cx; this.cy = cy; }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct ARGB
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }


        public const Int32 ULW_COLORKEY = 0x00000001;
        public const Int32 ULW_ALPHA = 0x00000002;
        public const Int32 ULW_OPAQUE = 0x00000004;

        public const byte AC_SRC_OVER = 0x00;
        public const byte AC_SRC_ALPHA = 0x01;


        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteObject(IntPtr hObject);
        
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        public static extern void ReleaseCapture();
        
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        public static extern void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        }

    internal class cSQL
    {
        private SqlConnection _conn;

        public SqlConnection Connect()
        {
            _conn = new SqlConnection(@"Data Source=.;Initial Catalog=DB_QP;Integrated Security=True");
            _conn.Open();
            return _conn;
        }

        private void Disconnect()
        {
            _conn.Close();
        }
        
        public DataSet Query(string query)
        {
            Connect();
            if (_conn.State == ConnectionState.Open)
            {
                SqlDataAdapter sda = new SqlDataAdapter(query, _conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                Disconnect();
                return ds;
            }
            Disconnect();
            return null;
        }
    }
}



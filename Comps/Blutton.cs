using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace QP
{
    public partial class Blutton : Button
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
        
        protected override bool ShowFocusCues => false;

        private Point _initP;
        private Size _initS;
        private int _i;

        private Region _client;
        private TextureBrush _tBrush;

        private int _movieId { get; set; }

        public Movie Movie;
        
        public Blutton(Point inLocation, Size inSize, int inMovieId)
        {
            InitializeComponent();
            _initP = inLocation;
            _initS = inSize;
            _movieId = inMovieId;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 30, 29, 53);
        }
        
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _i = Parent.Controls.GetChildIndex(this, false);
            _client = Region.FromHrgn(CreateRoundRectRgn(0, 0, 180, 230, 20, 20));
            var t = BackgroundImage;
            _tBrush = new TextureBrush(t);
            _tBrush.ScaleTransform(0.26f,0.25f);
            BackgroundImage = null;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            pevent.Graphics.FillRegion(_tBrush, _client);
            
            // NOTE: Debug
            //pevent.Graphics.DrawString(_movieId.ToString(),new Font("MS Gothic",12),new SolidBrush(Color.Aqua),0,0);
        }

        public Blutton(IContainer container)
        {
            container.Add(this);
            //InitializeComponent();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _initP = Location;
            Grow(this,e);
        }

        
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Shrink(this,e);
        }
        
        private void Grow(object sender, EventArgs e)
        {
            Size = new Size(_initS.Width * 2, _initS.Height * 2);
            Location = new Point(_initP.X - (Size.Width/4), _initP.Y - (Size.Height/4));
            _client = Region.FromHrgn(CreateRoundRectRgn(0,0,180*2,230*2,20,20));
            _tBrush.Transform = new Matrix(
                0.515f, 0, 0, 0.495f, 0, 0);
            Parent.Controls.SetChildIndex(this,9999);
        }

        private void Shrink(object sender, EventArgs e)
        {
            Size = _initS;
            Location = _initP;
            _client = Region.FromHrgn(CreateRoundRectRgn(0,0,ClientRectangle.Width-1,ClientRectangle.Height-1,20,20));
            _tBrush.Transform = new Matrix(
                0.26f, 0, 0, 0.245f, 0, 0);
            Parent.Controls.SetChildIndex(this,_i);
        }
        
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Program.Form.RenderInfo(Movie);
        }
    }
}
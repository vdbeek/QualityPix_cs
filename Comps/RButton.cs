using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QP
{
    public partial class RButton : Button
    {
        private Region _client;
        public Color _Color { get; set; }
        public bool _inset { get; set; }
        public Font _font { get; set; }
        public bool _innerGradient { get; set; }
        private Brush _gradient { get; set; }
        private bool _hover { get; set; }

        public bool _rectangle = false;

        protected override bool ShowFocusCues => false;

        public RButton()
        {
            InitializeComponent();
        }

        public RButton(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            _hover = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hover = false;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            pevent.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            if (_rectangle)
            {
                // draw as rectangle instead
                if (!_hover)
                {
                    RoundedRectangle.FillRoundedRectangle(pevent.Graphics,new SolidBrush(Color.FromArgb(255, 228, 151, 88)),ClientRectangle,15);
                }
                else
                {
                    RoundedRectangle.FillRoundedRectangle(pevent.Graphics,new SolidBrush(Color.FromArgb(255, 206, 121, 0)),ClientRectangle,15);
                }

                Brush b = new SolidBrush(Color.FromArgb(255,49,39,71));
                var font = new Font("Segoe UI", 11,FontStyle.Bold);
                var fontM = pevent.Graphics.MeasureString(Text, font);
                pevent.Graphics.DrawString(Text,font,b,ClientRectangle.Width/2 - fontM.Width / 2,ClientRectangle.Height/2 - fontM.Height / 2);
                
                return;
            }
            GraphicsPath path = new GraphicsPath();
            // inset
            if (_inset)
            {
                Color a = Color.FromArgb(255,49,39,71);
                Color b = Color.FromArgb(255,67,65,119);
                Color c = Color.FromArgb(255, 30, 29, 53);
           
                GraphicsPath inset = new GraphicsPath();
                inset.AddEllipse(0,0,Width,Height);
                if (_hover)
                {
                    _gradient = new LinearGradientBrush(inset.GetBounds(), b, a, -89.0f);
                }
                else
                {
                    _gradient = new LinearGradientBrush(inset.GetBounds(), a, b,-89.0f);
                }
                
                pevent.Graphics.FillPath(_gradient, inset);
                
                if (_innerGradient)
                {
                    GraphicsPath gp = new GraphicsPath();
                    gp.AddEllipse(inset.GetBounds().X+4,inset.GetBounds().Y+4,Width - 8,Height - 8);

                    PathGradientBrush pathGradientBrush = new PathGradientBrush(gp);
                    pathGradientBrush.CenterPoint = new PointF(gp.GetBounds().Width/2,gp.GetBounds().Height/2);
                    pathGradientBrush.CenterColor = c;
                    pathGradientBrush.SurroundColors = new [] { a };
                    pevent.Graphics.FillPath(pathGradientBrush,gp);
                    _client = new Region(gp);
                    /*
                    pathGradientBrush.Dispose();
                    gp.Dispose();
                    */
                }
                else
                {
                    path.AddEllipse(inset.GetBounds().X+4,inset.GetBounds().Y+4,Width - 8,Height - 8);
            
                    pevent.Graphics.FillPath(new SolidBrush(_Color),path);
                    _client = new Region(path);
                }
            }
            else
            {
                // button itself
                path.AddEllipse(1,1,Width-4,Height-4);
                pevent.Graphics.FillPath(new SolidBrush(_Color),path);
                _client = new Region(path);
            }
            
            // draw string 
            _font = new Font("Segoe UI", 24,FontStyle.Bold);
            Rectangle bounds = new Rectangle(0, 0, Width, Height);
            Brush brush = new SolidBrush(Color.FromArgb(185,255,165,0));
            /*
            pevent.Graphics.DrawString(Text,_font,brush,path.GetBounds().Width / 2 - path.GetBounds().Width / 4 ,path.GetBounds().Height/2 - path.GetBounds().Height/4);
            */
            pevent.Graphics.DrawString(Text,_font,brush,bounds.Width/4,bounds.Height/4);
            
            brush.Dispose();
            path.Dispose();
        }
    }
}
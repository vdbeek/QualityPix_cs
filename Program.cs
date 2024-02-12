using System;
using System.Drawing;
using System.Windows.Forms;

namespace QP
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static RoundedForm Form;
        static class Globals
        {
            public static Size Res { get; set; }
        }

        [STAThread] 

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // set default vars for generation
            Globals.Res = SystemInformation.PrimaryMonitorSize;
            Size desiredSize = new Size(Convert.ToInt16(Math.Round(Globals.Res.Width * 0.90f)),
                Convert.ToInt16(Math.Round(Globals.Res.Height * 0.90f)));
            RoundedForm mainForm = new RoundedForm();
            mainForm.Size = desiredSize;
            Form = mainForm;
            Application.Run(mainForm);
        }
    }
}
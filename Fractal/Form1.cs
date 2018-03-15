using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fractal
{
    public partial class Fractal : Form
    {

        //variables
        private int MAX = 256;      // max iterations
        private double sX = -2.025; // start value real
        private double sY = -1.125; // start value imaginary
        private double eX = 0.6;    // end value real
        private double eY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xend, yend, xzoom, yzoom;
        private static bool action, rectangle, finished;
        private static float xy;
        private Image picture;
        private Graphics g;
        private Cursor c1, c2;
        private HSB HSBcol = new HSB();
        private bool isRunning = true;


        //starts when form loads
        public void init() // all instances will be prepared
        {
            //HSBcol = new HSB();
            finished = false;
            c1 = Cursors.WaitCursor;
            c2 = Cursors.Cross;
            x1 = panel.Size.Width;
            y1 = panel.Size.Height;
            xy = (float)x1 / (float)y1;
            picture = new Bitmap(x1, y1);
            g = Graphics.FromImage(picture);
            finished = true;
        }


        // This method must be called after the <code>init()</code> method
        public void start()
        {
            action = false;
            rectangle = false;
            xzoom = (xend - xstart) / (double)x1;
            yzoom = (yend - ystart) / (double)y1;
        }

        private void panel_MouseMove(object sender, MouseEventArgs e)
        {

            if (action)
            {
                xe = e.X;
                ye = e.Y;

                Graphics g = panel.CreateGraphics();
                update(g);
            }
        }

        private void panel_MouseUp(object sender, MouseEventArgs e)
        {
            int z, w;

            if (action)
            {
                xe = e.X;
                ye = e.Y;
                if (xs > xe)
                {
                    z = xs;
                    xs = xe;
                    xe = z;
                }
                if (ys > ye)
                {
                    z = ys;
                    ys = ye;
                    ye = z;
                }
                w = (xe - xs);
                z = (ye - ys);
                if ((w < 2) && (z < 2)) initvalues();
                else
                {
                    if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                    else xe = (int)((float)xs + (float)z * xy);
                    xend = xstart + xzoom * (double)xe;
                    yend = ystart + yzoom * (double)ye;
                    xstart += xzoom * (double)xs;
                    ystart += yzoom * (double)ys;
                }
                xzoom = (xend - xstart) / (double)x1;
                yzoom = (yend - ystart) / (double)y1;
                rectangle = false;
            }
        }

        private void panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (action)
            {
                xs = e.X;
                ys = e.Y;
                rectangle = true;
            }
        }

        public Fractal()
        {
            InitializeComponent();
        }


        private void Fractal_Load(object sender, EventArgs e)
        {
            init();
            start();
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.DrawImage(picture, 0, 0);
        }

        private void initvalues() // reset start values
        {
            string[] state = Values();

            if (isRunning)
            {
                isRunning = false;
                xstart = float.Parse(state[6]);
                ystart = float.Parse(state[7]);
                xend = float.Parse(state[8]);
                yend = float.Parse(state[9]);
            }
            else
            {
                xstart = sX;
                ystart = sY;
                xend = sX;
                yend = sY;
                if ((float)((xend - xstart) / (yend - ystart)) != xy)
                    xstart = xend - (yend - ystart) * (double)xy;
            }
        }
        private string[] Values()
        {
            string line = " ";
            string temp = "";
            string[] list = { };
            int counter = 1;
            System.IO.StreamReader file = new System.IO.StreamReader("config.txt");
            while ((line = file.ReadLine()) != null)
            {
                temp += (line + ",");
            }
            list = temp.Split(',');
            file.Close();

            return list;
        }

        // The algorithm
       

        private float PixelColour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;
            int j = 0;

            while ((j < MAX) && (m < 4.0))
            {
                j++;
                m = r * r - i * i;
                i = 2.0 * r * i + ywert;
                r = m + xwert;
            }
            return (float)j / (float)MAX;
        }

        public void update(Graphics g)
        {
            Pen pen = new Pen(Color.White, 2);

            g.DrawImage(picture, 0, 0);

            if (rectangle)
            {
                if (xs < xe)
                {
                    if (ys < ye) g.DrawRectangle(pen, xs, ys, (xe - xs), (ye - ys));
                    else g.DrawRectangle(pen, xs, ye, (xe - xs), (ys - ye));
                }
                else
                {
                    if (ys < ye) g.DrawRectangle(pen, xe, ys, (xs - xe), (ye - ys));
                    else g.DrawRectangle(pen, xe, ye, (xs - xe), (ys - ye));
                }
            }


        }


    }
}

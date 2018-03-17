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
            Mandelbrot();
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
                Mandelbrot();
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
            xstart = sX;
            ystart = sY;
            xend = eX;
            yend = eY;
            if ((float)((xend - xstart) / (yend - ystart)) != xy)
                xstart = xend - (yend - ystart) * (double)xy;
        }


        // Algorithm for Mandelbrot Calculation
        private void Mandelbrot() // calculates all possible points
        {
            int x, y;
            float h, b, alt = 0.0f;

            action = false;
            this.Cursor = c1;

            Pen pen = null;

            for (x = 0; x < x1; x += 2)
                for (y = 0; y < y1; y++)
                {
                    h = PixelColour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // color value
                    if (h != alt)
                    {
                        b = 1.0f - h * h; // brightnes
                                          ///djm added
                        Color col = HSB.ToRGB(h, 0.8f, b);
                        pen = new Pen(col);
                        alt = h;
                    }
                    g.DrawLine(pen, x, y, x + 1, y);
                }
            this.Cursor = c2;
            action = true;
        }

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

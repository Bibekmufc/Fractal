using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        private bool cColour;
        Random rd = new Random();
        Color[] change = new Color[6];


        private HSB HSBcol = new HSB();
        private bool clicked = false;


        public Fractal()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

        }

        //loads form
        private void Fractal_Load(object sender, EventArgs e)
        {
            init();
            Colour();
            start();
        }

        //starts when form loads
        public void init() // all instances will be prepared
        {
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
            initvalues(); //missing out initvalues() made the image to be all zoomed when the form loaded
            xzoom = (xend - xstart) / (double)x1;
            yzoom = (yend - ystart) / (double)y1;
            Mandelbrot();
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

        //when right mouse button(rmb) is pressed
        private void panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (action)
            {
                xs = e.X;
                ys = e.Y;
                rectangle = true;
            }
        }

        //when rmb is pressed and moved
        private void panel_MouseMove(object sender, MouseEventArgs e)
        {

            if (action)
            {
                xe = e.X;
                ye = e.Y;

                Graphics g = panel.CreateGraphics();
                Update(g);
            }
        }

        //when rmb is released
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
      
        //paints mandlebrot in the designated panel
        private void panel_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.DrawImage(picture, 0, 0);
        }



        // Algorithm for Mandelbrot Calculation
        private void Mandelbrot() // calculates all possible points
        {
            int x, y;
            float h, b, alt = 0.0f, c;
            Pen pn = null;
            Color col;

            action = false;
            panel.Cursor = c1;
            //statusBar.Text = ("Mandelbrot-Set will be produced - please wait...");
            for (x = 0; x < x1; x += 2)
            {
                for (y = 0; y < y1; y++)
                {
                    h = PixelColour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // color value
                    if (h != alt)
                    {
                        b = 1.0f - h * h;
                        //calling method of ToHSB class(passing value into)                   
                        col = HSB.ToRGB(h, 0.8f, b, change);
                        pn = new Pen(col);
                        //djm 
                        alt = h;
                    }
                    g.DrawLine(pn, x, y, x + 1, y);
                }
            }
            //statusBar.Text = ("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
            panel.Cursor = c2;
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

        //the rectangle box formed when rmb is pressed and dragged to zoom in on the image
        public void Update(Graphics g)
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
        private void saveImageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Submenu which allows users to save the image formed in the file format that they desire   
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "jpeg|*.jpg|Bitmap|*.bmp|Gif|*.gif |Png|*.png";
            sd.Title = "Save Image File";
            sd.ShowDialog();

            // If condition to save the file given that the name is not empty  
            if (sd.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.  
                FileStream fs =
                   (FileStream)sd.OpenFile();
                // Prompts a dialogue box from which the users can save the image from the image format given below
                // NOTE that the FilterIndex property is one-based.  
                switch (sd.FilterIndex)
                {
                    case 1:
                        picture.Save(fs, ImageFormat.Jpeg);
                        break;

                    case 2:
                        picture.Save(fs, ImageFormat.Bmp);
                        break;

                    case 3:
                        picture.Save(fs, ImageFormat.Gif);
                        break;

                    case 4:
                        picture.Save(fs, ImageFormat.Png);
                        break;
                }

                fs.Close();
            }
        }

        //when "Change Color" is pressed
        private void changeColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Colour();
            Mandelbrot();
        }

        //generates random colours which get displayed
        private void Colour()
        {
            if (!cColour)
            {
                change[0] = Color.FromArgb(255, 255, 255);
                change[1] = Color.FromArgb(255, 255, 255);
                change[2] = Color.FromArgb(255, 255, 255);
                change[3] = Color.FromArgb(255, 255, 255);
                change[4] = Color.FromArgb(255, 255, 255);
                change[5] = Color.FromArgb(255, 255, 255);
                cColour = true;
            }
            else
            {
                change[0] = Color.FromArgb(rd.Next(255), rd.Next(255), rd.Next(255));
                change[1] = Color.FromArgb(rd.Next(255), rd.Next(255), rd.Next(255));
                change[2] = Color.FromArgb(rd.Next(255), rd.Next(255), rd.Next(255));
                change[3] = Color.FromArgb(rd.Next(255), rd.Next(255), rd.Next(255));
                change[4] = Color.FromArgb(rd.Next(255), rd.Next(255), rd.Next(255));
                change[5] = Color.FromArgb(rd.Next(255), rd.Next(255), rd.Next(255));
            }
        }

    }
}

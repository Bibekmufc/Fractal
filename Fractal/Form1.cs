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
        private HSB HSBcol = new HSB();
        Random rd = new Random();
        Color[] change = new Color[6];
        String[] settings = new String[4];
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

        //paints mandlebrot in the designated panel
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

        // Algorithm for Mandelbrot Calculation
        private void Mandelbrot() // calculates all possible points
        {
            int x, y;
            float h, b, alt = 0.0f, c;
            Pen pn = null;
            Color col;

            action = false;
            panel.Cursor = c1;
            status.Text = ("Mandelbrot-Set will be produced - please wait...");
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
            status.Text = ("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
            panel.Cursor = c2;
            action = true;
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
            clicked = false;
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
                Mandelbrot();
                rectangle = false;

            }
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

        //exit in the menustrip
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        //start colour cycle
        private void cycleColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        //stop colour cycle
        private void stopCyclingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        //timer to initiate the colour cycle
        private void timer1_Tick(object sender, EventArgs e)
        {
            Colour();
            Mandelbrot();
            panel.Refresh();
        }

        //when save state is clicked
        private void saveStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SState();
        }

        //when load state is clicked
        private void loadStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LState();
        }

        //read values from the file to load state
        private String[] ReadFile()
        {
            String line = "";
            String tempStrore = "";
            OpenFileDialog od = new OpenFileDialog();
            od.Title = "Open File";
            // Shows dialogue box and checks iff the user clicked OK
            if (od.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = (FileStream)od.OpenFile();
                StreamReader sr = new StreamReader(fs);
                while ((line = sr.ReadLine()) != null)
                {
                    tempStrore += (line + ",");
                }
                settings = tempStrore.Split(',');
                sr.Close();
            }
            return settings;
        }

        //method to save state
        private void SState()
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "Text File|*.txt";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = (FileStream)sd.OpenFile();
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(xstart);
                sw.WriteLine(ystart);
                sw.WriteLine(xend);
                sw.WriteLine(yend);
                sw.WriteLine(change[0].R);
                sw.WriteLine(change[0].G);
                sw.WriteLine(change[0].B);
                sw.WriteLine(change[1].R);
                sw.WriteLine(change[1].G);
                sw.WriteLine(change[1].B);
                sw.WriteLine(change[2].R);
                sw.WriteLine(change[2].G);
                sw.WriteLine(change[2].B);
                sw.WriteLine(change[3].R);
                sw.WriteLine(change[3].G);
                sw.WriteLine(change[3].B);
                sw.WriteLine(change[4].R);
                sw.WriteLine(change[4].G);
                sw.WriteLine(change[4].B);
                sw.WriteLine(change[5].R);
                sw.WriteLine(change[5].G);
                sw.WriteLine(change[5].B);
                sw.Close();
            }
        }

        //method to load state
        private void LState()
        {
            string[] set = ReadFile();
            xstart = Convert.ToDouble(set[0]);
            ystart = Convert.ToDouble(set[1]);
            xend = Convert.ToDouble(set[2]);
            yend = Convert.ToDouble(set[3]);
            xzoom = (xend - xstart) / (double)x1;
            yzoom = (yend - ystart) / (double)y1;
            change[0] = Color.FromArgb(Convert.ToInt32(set[4]), Convert.ToInt32(set[5]), Convert.ToInt32(set[6]));
            change[1] = Color.FromArgb(Convert.ToInt32(set[7]), Convert.ToInt32(set[8]), Convert.ToInt32(set[9]));
            change[2] = Color.FromArgb(Convert.ToInt32(set[10]), Convert.ToInt32(set[11]), Convert.ToInt32(set[12]));
            change[3] = Color.FromArgb(Convert.ToInt32(set[13]), Convert.ToInt32(set[14]), Convert.ToInt32(set[15]));
            change[4] = Color.FromArgb(Convert.ToInt32(set[16]), Convert.ToInt32(set[17]), Convert.ToInt32(set[18]));
            change[5] = Color.FromArgb(Convert.ToInt32(set[19]), Convert.ToInt32(set[20]), Convert.ToInt32(set[21]));
            Mandelbrot();
        }
    }
}

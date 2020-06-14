using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //-----------------------------------НАТУРАЛЬНЫЙ РИСОВАНИЕ ЛИНИЙ-------------------------------------------\\

        public void NaturalLine(double x, double y, double X, double Y, Bitmap bmp, Color color)
        {
            double x0, y0, x1, y1;
            bool flag = false;
            if ((X - x) == 0)
            {
                y0 = (y < Y) ? y : Y;
                y1 = (y < Y) ? Y : y;
                for (int i = Convert.ToInt32(y0); i <= y1; i++)
                {
                    bmp.SetPixel(Convert.ToInt32(x), i, color);
                }
            }
            else
            {
                double a = (Y - y) / (X - x), b = y - a * x;
                if (Math.Abs(X - x) < Math.Abs(Y - y))
                {
                    x0 = y;
                    x1 = Y;
                    y0 = x;
                    y1 = X;
                    flag = true;
                }
                else
                {
                    x0 = x;
                    x1 = X;
                    y0 = y;
                    y1 = Y;
                }

                if (x1 < x0)
                {
                    double v = x1; x1 = x0; x0 = v;
                }

                if (flag)
                {
                    for (int i = Convert.ToInt32(x0); i <= x1; i++)
                    {
                        bmp.SetPixel(Convert.ToInt32((i - b) / a), i, color);
                    }
                }
                else
                {
                    for (int i = Convert.ToInt32(x0); i <= x1; i++)
                    {
                        bmp.SetPixel(i, Convert.ToInt32(i * a + b), color);
                    }
                }
            }
        }

        //-------------------------------------БРЕЗЕНХАМ РИСОВАНИЕ ЛИНИЙ---------------------------------------------\\

        private void Change(ref int x, ref int y)
        {
            int tmp = x;
            x = y;
            y = tmp;
        }
       
        public void BresenhamLine(int x0, int y0, int x1, int y1, Bitmap bmp, Color color)
        {
            bool check = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (check)
            {
                Change(ref x0, ref y0);
                Change(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Change(ref x0, ref x1);
                Change(ref y0, ref y1);
            }
            double deltax = x1 - x0;
            double deltay = Math.Abs(y1 - y0);
            double m = deltay / deltax;
            double error = m - 0.5;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                bmp.SetPixel(check ? y : x, check ? x : y, color);
                if (error >= 0)
                {
                    y += ystep;
                    error += m - 1;
                }
                else
                {
                    error += m;
                }
            }
        }

        //------------------------------РИСОВАНИЕ ОКРУЖНОСТИ---------------------------------------------\\

        public void Draw8Pixels(int x, int y, int x0, int y0, Bitmap bmp, Color color)
        {
            bmp.SetPixel(x + x0, y + y0, color);
            bmp.SetPixel(x + x0, -y + y0, color);
            bmp.SetPixel(-x + x0, y + y0, color);
            bmp.SetPixel(-x + x0, -y + y0, color);
            bmp.SetPixel(y + x0, x + y0, color);
            bmp.SetPixel(y + x0, -x + y0, color);
            bmp.SetPixel(-y + x0, x + y0, color);
            bmp.SetPixel(-y + x0, -x + y0, color);
        }

        public void BrezenhamCircle(int x0, int y0, int R, Bitmap bmp, Color color)
        {
            int x = 0;
            int y = R;
            int d = 3 - 2 * R;
            while (y >= x)
            {
                Draw8Pixels(x, y, x0, y0, bmp, color);
                if (d <= 0)
                {
                    d += 4 * x + 6;
                }
                else
                {
                    d += 4 * (x - y) + 10;
                    y--;
                }
                x++;
            }
        }
        
        
        public void Seeding(int x, int y, Bitmap bmp, Color newcolor)
        {
            Color backcolor = bmp.GetPixel(x, y);
            int xl = x;
            int xr = x + 1;                        
            while ((xl >= 0) && (bmp.GetPixel(xl, y) == backcolor))
            {
                bmp.SetPixel(xl, y, newcolor);
                xl--;
            }            
            xl++;           
            while ((xr < bmp.Width - 1) && (bmp.GetPixel(xr, y) == backcolor))
            {
                bmp.SetPixel(xr, y, newcolor);
                xr++;
            }
            xr--;
            int extreme = xl;
            while ((extreme <= xr) && (y != 0))
            {           
                while ((extreme <= xr) && (bmp.GetPixel(extreme, y - 1) != backcolor))
                {
                    extreme++;
                }
                if (extreme <= xr)
                {
                    Seeding(extreme, y - 1, bmp, newcolor);
                }
                extreme++;
            }
            extreme = xl;
            while ((extreme <= xr) && (y + 1 != bmp.Height))
            {                
                while ((extreme <= xr) && (bmp.GetPixel(extreme, y + 1) != backcolor))
                {
                    extreme++;
                }
                if (extreme <= xr)
                {
                    Seeding(extreme, y + 1, bmp, newcolor);
                }
                extreme++;
            }
        }
        
        //---------------------------ЗАТРАВКА НЕ МОДИФИЦИРОВАННАЯ---------------------------------\\

        public void Seeding0 (int x, int y, Bitmap bmp, Color newcolor)
        {
            Color center = bmp.GetPixel(x, y);
            bmp.SetPixel(x, y, newcolor);
            if ((x > 1) && (x < bmp.Width - 1) && (y > 1) && (y < bmp.Height - 1))
            {
                if(bmp.GetPixel(x + 1,y) == center)
                {
                    Seeding0(x + 1, y, bmp, newcolor);
                }
                if (bmp.GetPixel(x - 1, y) == center)
                {
                    Seeding0(x - 1, y, bmp, newcolor);
                }
                if (bmp.GetPixel(x, y + 1) == center)
                {
                    Seeding0(x, y + 1, bmp, newcolor);
                }             
                if (bmp.GetPixel(x , y - 1) == center)
                {
                    Seeding0(x, y - 1, bmp, newcolor);
                }
            }
        }

        //--------------------------------ЗАКРАСКА УЗОРОМ-------------------------------------------\\

        public void Pattern(int x, int y, Bitmap bmp, Color[,] pattern, int w, int h)
        {
            Color backcolor = bmp.GetPixel(x, y);
            int xl = x;
            int xr = x + 1;                 
            while ((xl >= 0) && (bmp.GetPixel(xl, y) == backcolor))
            {
                bmp.SetPixel(xl, y, pattern[xl % w, y % h]);
                xl--;
            }
            xl++; 
            while ((xr < bmp.Width - 1) && (bmp.GetPixel(xr, y) == backcolor))
            {
                bmp.SetPixel(xr, y, pattern[xr % w, y % h]);
                xr++;
            }
            xr--;
            int extreme = xl;
            while ((extreme <= xr) && (y != 0))
            {
                while ((extreme <= xr) && (bmp.GetPixel(extreme, y - 1) != backcolor))
                {
                    extreme++;
                }

                if (extreme <= xr)
                {
                    Pattern(extreme, y - 1, bmp, pattern, w, h);
                }
                extreme++;
            }
            extreme = xl;
            while ((extreme <= xr) && (y + 1 != bmp.Height))
            {
                while ((extreme <= xr) && (bmp.GetPixel(extreme, y + 1) != backcolor))
                {
                    extreme++;
                }
                if (extreme <= xr)
                {
                    Pattern(extreme, y + 1, bmp, pattern, w, h);
                }                
                extreme++;
            }
        }


        //-------------------------------------------------------------------------------------------\\

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(9000, 9000); 
            BrezenhamCircle(300, 350, 50, bmp, Color.Brown);            
            Color[,] col = new Color[3, 3];
            col[0, 0] = Color.Green;
            col[0, 1] = Color.Green;
            col[0, 2] = Color.Green;
            col[1, 0] = Color.Black;
            col[1, 1] = Color.Black;
            col[1, 2] = Color.Black;
            col[2, 0] = Color.Green;
            col[2, 1] = Color.Green;
            col[2, 2] = Color.Green;
            Pattern(300, 350, bmp, col, 3, 3);
            BrezenhamCircle(410, 350, 50, bmp, Color.Black);
            Seeding0(410, 350, bmp, Color.BurlyWood);
            NaturalLine(0, 400, 800, 400, bmp, Color.Gray);
            NaturalLine(0, 401, 800, 401, bmp, Color.Gray);
            NaturalLine(0, 402, 800, 402, bmp, Color.DarkGray);
            NaturalLine(0, 403, 800, 403, bmp, Color.DarkGray);
            NaturalLine(0, 404, 800, 404, bmp, Color.Gray);
            NaturalLine(0, 405, 800, 405, bmp, Color.Gray);
            NaturalLine(0, 406, 800, 406, bmp, Color.DarkGray);
            NaturalLine(0, 407, 800, 407, bmp, Color.DarkGray);
            NaturalLine(0, 408, 800, 408, bmp, Color.Gray);
            NaturalLine(0, 409, 800, 409, bmp, Color.Gray);
            NaturalLine(0, 410, 800, 410, bmp, Color.DarkGray);
            NaturalLine(0, 411, 800, 411, bmp, Color.DarkGray);
          
            BresenhamLine(250, 300, 440, 300, bmp, Color.Red);
            BresenhamLine(440, 300, 440, 250, bmp, Color.Red);
            BresenhamLine(440, 250, 250, 250, bmp, Color.Red);
            BresenhamLine(250, 250, 250, 300, bmp, Color.Red);
            Seeding(300, 270, bmp, Color.White);
            pictureBox1.Image = bmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }
    }
}

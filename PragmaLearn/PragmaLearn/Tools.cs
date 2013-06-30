using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;



namespace PragmaLearn
{
    public class Tools
    {
        const double noise = 0.2;
        static string applicatonPath { get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); } }
        public static readonly string DataPath =  Path.GetFullPath((Path.Combine(applicatonPath, @"..\..\Data\")));

        private static int seed = Environment.TickCount;

        private static ThreadLocal<Random> randomWrapper = new ThreadLocal<Random>(() =>
            new Random(Interlocked.Increment(ref seed))
        );

        public static Random GetThreadRandom()
        {
            return randomWrapper.Value;
        }
        public static Random rnd { get { return randomWrapper.Value; } }
        
        public static double[] bmp_to_double(Bitmap bmp)
        {

            double[] pixels = new double[bmp.Width * bmp.Height];

            int n = 0;
            for (int j = 0; j < bmp.Height; j++)
            {
                for (int i = 0; i < bmp.Width; i++)
                {
                    var b = bmp.GetPixel(i, j).GetBrightness();
                    // if (b == 0) b = -1.0f;
                    double f = b + (Tools.rnd.NextDouble()-0.5) * noise;
                    pixels[n] = f;
                    n++;
                }
            }

            return pixels;
        }

        internal static Color getColor(double h)
        {
            if (h > 1) h = 1;
            if (h < 0)
            {
                h = Math.Abs(h);
                if (h > 1) h = 1;
                byte n = (byte)(h * 255);
                return Color.FromArgb(0, 0, n);
            }
            byte b = (byte)(h * 255);
            return Color.FromArgb(b, 0, 0);
        }

        /// <summary>
        /// slow slow slow. as hell.
        /// </summary>
        public static Bitmap double_to_bmp(double[] pixels, int sx = 16, int sy = 16)
        {
            Bitmap bmp = new Bitmap(sx, sy);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Black);

                int n = 0;
                for (int j = 0; j < bmp.Height; j++)
                {
                    for (int i = 0; i < bmp.Width; i++)
                    {
                        bmp.SetPixel(i, j, getColor(pixels[n]));
                        n++;
                    }
                }

            }

            return bmp;
        }
    }
}

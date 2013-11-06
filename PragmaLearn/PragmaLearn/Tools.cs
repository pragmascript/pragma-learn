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
       //  public static Random rnd { get { return randomWrapper.Value; } }


    

        public static RandomOps.ThreadSafe.CMWC4096 rnd = new RandomOps.ThreadSafe.CMWC4096();



        public static float[] bmp_to_float(Bitmap bmp, float noise = 0.0f)
        {

            float[] pixels = new float[bmp.Width * bmp.Height];

            int n = 0;
            for (int j = 0; j < bmp.Height; j++)
            {
                for (int i = 0; i < bmp.Width; i++)
                {
                    var b = bmp.GetPixel(i, j).GetBrightness();
                    // if (b == 0) b = -1.0f;
                    var f = b + (Tools.rnd.Uniform() - 0.5f) * noise;
                    pixels[n] = (float)f;
                    n++;
                }
            }

            return pixels;
        }

        internal static Color getColor(float h)
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

        static bool alreadyOnce = false;
        public static void Once(Action a)
        {
            if (!alreadyOnce)
                a();
            alreadyOnce = true;
        }

        /// <summary>
        /// slow slow slow. as hell.
        /// </summary>
        public static Bitmap float_to_bmp(float[] pixels, int sx = 16, int sy = 16)
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

        public static float Clamp(float x, float min, float max)
        {
            return Math.Max(min, Math.Min(max, x));
        }

    }
}

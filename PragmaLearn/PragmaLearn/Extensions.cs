using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PragmaLearn
{
    public static class Extensions
    {

        public static double Uniform(this Random rnd)
        {
            return rnd.NextDouble();
        }

        public static int Index(this Random rnd, int max)
        {
            return rnd.Next(max);
        }


        public static Bitmap ScaleTo(this Bitmap bmp, int nx, int ny)
        {
            var result = new Bitmap(nx, ny);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, result.Width, result.Height), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
            }

            return result;
        }

        public static Bitmap GetPatch(this Bitmap bmp, Rectangle src, int destSize)
        {
            var result = new Bitmap(destSize, destSize);


            using (var g = Graphics.FromImage(result))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, result.Width, result.Height), src, GraphicsUnit.Pixel);
            }

            return result;
        }

        public static void Clear(this Bitmap bmp, Color color)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(color);
            }
        }

        public static string Print<T>(this T[] a)
        {
            string result = "{ ";
            for (int i = 0; i < a.Length; ++i)
            {
                result += a[i];
                if (i < a.Length - 1)
                    result += ", ";
            }
            result += " }";

            return result;
        }

        public static int MaxIndex<T>(this IEnumerable<T> sequence)
    where T : IComparable<T>
        {
            int maxIndex = -1;
            T maxValue = default(T); // Immediately overwritten anyway

            int index = 0;
            foreach (T value in sequence)
            {
                if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }

        public static void Invoke(this Control Control, Action Action)
        {
            Control.Invoke(Action);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            foreach (var item in source.OrderBy(i => Guid.NewGuid()))
            {
                yield return item;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PragmaLearn
{
    public static class Extensions
    {
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
    }
}

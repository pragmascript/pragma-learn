using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PragmaLearn
{
    public class Dataset
    {
        public List<double[]> input;
        public List<double[]> output;

        public Dataset()
        {
            input = new List<double[]>();
            output = new List<double[]>();
        }

        public void AddPair(double[] inp, double[] outp)
        {
            input.Add(inp);
            output.Add(outp);
        }

        public int GetInputDimension()
        {
            if (input.Count == 0)
                throw new Exception("Empty Dataset!");
            return input[0].Length;
        }

        public int GetOutputDimension()
        {
            if (output.Count == 0)
                throw new Exception("Empty Dataset!");
            return output[0].Length;
        }

        public Func<double[], Bitmap> VisualizeInput = null;
        public Func<double[], Bitmap> VisualizeOutput = null;
    }
}

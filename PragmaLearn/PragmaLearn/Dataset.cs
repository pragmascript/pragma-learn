using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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

        public void AddPairSync(double[] inp, double[] outp)
        {
            Monitor.Enter(this);
            AddPair(inp, outp);
            Monitor.Exit(this);
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

        public void RandomSplit(double ratio, out Dataset trainData, out Dataset testData)
        {
            if (ratio < 0 || ratio > 1)
                throw new ArgumentException("ratio must be in [0, 1]");

            trainData = new Dataset();
            testData = new Dataset();

            var indices = Enumerable.Range(0, input.Count).ToList();

            for (int i = 0; i < input.Count; ++i)
            {
                var d = default(Dataset);
                if (i < (int)(ratio * input.Count))
                    d = trainData;
                else
                    d = testData;

                var pos = Tools.rnd.Next(indices.Count);
                var x = indices[pos];
                indices.RemoveAt(pos);
                d.AddPair(input[x], output[x]);
            }
        }

        public Func<double[], Bitmap> VisualizeInput = null;
        public Func<double[], Bitmap> VisualizeOutput = null;
    }
}

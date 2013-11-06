using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PragmaLearn
{
    public class Dataset
    {
        public List<float[]> input;
        public List<float[]> output;

        public Dataset()
        {
            input = new List<float[]>();
            output = new List<float[]>();
        }

        public void AddPair(float[] inp, float[] outp)
        {
            input.Add(inp);
            output.Add(outp);
        }

        public void AddPairSync(float[] inp, float[] outp)
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

        public void RandomSplit(float ratio, out Dataset trainData, out Dataset testData)
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

                var pos = Tools.rnd.Index(indices.Count);
                var x = indices[pos];
                indices.RemoveAt(pos);
                d.AddPair(input[x], output[x]);
            }
        }

        public void Save(string filename)
        {
            using (FileStream f = new FileStream(filename, FileMode.Create))
            {
                Save(f);
            }
        }

        public void Save(Stream stream)
        {
            if (input.Count <= 0)
                return;

            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(input.Count);
            writer.Write(input[0] == null ? 0 : input[0].Length);
            writer.Write(output[0] == null ? 0 : output[0].Length);

            for (int i = 0; i < input.Count; ++i)
            {
                if (input[i] != null)
                {
                    foreach (var x in input[i])
                    {
                        writer.Write(x);
                    }
                }
                if (output[i] != null)
                {
                    foreach (var y in output[i])
                    {
                        writer.Write(y);
                    }
                }
            }
        }

        public void Open(string filename)
        {
            using (FileStream f = new FileStream(filename, FileMode.Open))
            {
                Open(f);
            }
        }

        public void Open(Stream stream)
        {
            input.Clear();
            output.Clear();
            BinaryReader reader = new BinaryReader(stream);
            var count = reader.ReadInt32();
            var inpc = reader.ReadInt32();
            var outc = reader.ReadInt32();

            for (int i = 0; i < count; ++i)
            {
                var inpa = new float[inpc];
                for (int x = 0; x < inpc; ++x)
                {
                    inpa[x] = (float)reader.ReadSingle();
                }

                var outpa = new float[outc];
                for (int y = 0; y < outc; ++y)
                {
                    outpa[y] = (float)reader.ReadSingle();
                }

                AddPair(inpa, outpa);
            }
        }

        public Func<float[], Bitmap> VisualizeInput = null;
        public Func<float[], Bitmap> VisualizeOutput = null;
    }
}

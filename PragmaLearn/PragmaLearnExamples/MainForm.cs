using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PragmaLearn.Examples
{
    public partial class MainForm : Form
    {
        PragmaLearn.Learner.BackpropNeuralNetwork network = new PragmaLearn.Learner.BackpropNeuralNetwork();
        Dataset data = new Dataset();

        public MainForm()
        {
            InitializeComponent();
        }



        void replaceInput(Bitmap bmp)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }

            pictureBox1.Image = bmp;
            pictureBox1.Refresh();
        }

        void replaceOutput(Bitmap bmp)
        {
            if (pictureBox2.Image != null)
            {
                pictureBox2.Image.Dispose();
            }

            pictureBox2.Image = bmp;
            pictureBox2.Refresh();
        }

        int pos = 0;
        void test()
        {
            if (data.VisualizeInput != null)
            {
                //  replaceInput(data.VisualizeInput((learner as BackpropNeuralNetwork).PredictReversed(data.output[pos % data.input.Count])));

                replaceInput(data.VisualizeInput(data.input[pos % data.input.Count]));
                // replaceInput(data.VisualizeOutput(data.output[pos % data.output.Count]));
            }

            if (data.VisualizeOutput != null)
            {
                replaceOutput(data.VisualizeOutput(network.Predict(data.input[pos % data.input.Count])));
            }
            pos++;
            Refresh();
        }

        private void train(Dataset data, int batchSize = 100)
        {
            var batch = new int[batchSize];
            for (int t = 0; t < 50000; ++t)
            {
                Console.WriteLine(t);
                int pos = Tools.rnd.Next(data.input.Count);
                for (int i = 0; i < batchSize; ++i)
                {
                    batch[i] = (pos + i) % data.input.Count;
                }
                network.TrainMiniBatch(data, batch);

                Console.WriteLine("LEARNING RATE: " + network.learningRate);
                network.learningRate *= 0.9998;
                if (t % 10 == 0)
                {
                    test();
                }
            }
        }

        private void bTrainOCR_Click(object sender, EventArgs e)
        {
            data = PragmaLearn.Exampels.Datasets.OCR.Create();
            var hidden = data.GetInputDimension();
            network.Init(data.GetInputDimension(), hidden / 2, hidden / 3, hidden / 4, data.GetOutputDimension());
            
            train(data, batchSize:100);

        }

        private void bTrainLines_Click(object sender, EventArgs e)
        {
            data = PragmaLearn.Exampels.Datasets.Lines.Create(10000);
            var hidden = data.GetInputDimension();
            network.Init(data.GetInputDimension(), hidden / 2, hidden / 2, data.GetOutputDimension());

            train(data, batchSize: 100);
        }

    }
}

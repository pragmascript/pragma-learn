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
        
        void replaceNetworkOutput(Bitmap bmp)
        {
            if (pictureBox3.Image != null)
            {
                pictureBox3.Image.Dispose();
            }

            pictureBox3.Image = bmp;
            pictureBox3.Refresh();
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
                replaceOutput(data.VisualizeOutput(data.output[pos % data.output.Count]));
                replaceNetworkOutput(data.VisualizeOutput(network.Predict(data.input[pos % data.input.Count])));
            }

            
            pos++;
            Refresh();
        }


        private int[] genMiniBatch(int batchSize)
        {
            var batch = new int[batchSize];
            
            for (int i = 0; i < batchSize; ++i)
            {
                int pos = Tools.rnd.Next(data.input.Count);
                batch[i] = pos % data.input.Count;
            }

            return batch;
        }

        volatile bool running;
        private void train(Dataset data, int batchSize = 100, int testModulo = 100)
        {
            Task.Run(() =>
                {
                    running = true;
                    
                    int t = 0;
                    while (running)
                    {
                        t++;
                        var batch = genMiniBatch(batchSize);
                        network.TrainMiniBatch(data, batch);

                        Console.WriteLine("LEARNING RATE: " + network.learningRate);
                        if (network.learningRate > 0.0001)
                            network.learningRate *= 0.9998;
                        if (t % testModulo == 0)
                        {
                            this.Invoke(test);
                        }
                    }
                });
        }

        private void bTrainOCR_Click(object sender, EventArgs e)
        {
            data = PragmaLearn.Exampels.Datasets.OCR.Create();
            var hidden = data.GetInputDimension();
            if (network.GetInputs() != data.GetInputDimension() || network.GetOutputs() != data.GetOutputDimension())
                network.Init(data.GetInputDimension(), hidden / 2, hidden / 3, hidden / 4, data.GetOutputDimension());
            
            train(data, batchSize:100);

        }

        private void bTrainLines_Click(object sender, EventArgs e)
        {
            data = PragmaLearn.Exampels.Datasets.Lines.Create(10000);
            var hidden = data.GetInputDimension();
            if (network.GetInputs() != data.GetInputDimension() || network.GetOutputs() != data.GetOutputDimension())
                network.Init(data.GetInputDimension(), hidden, data.GetOutputDimension());

            train(data, batchSize: 100, testModulo:100);
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            running = false;
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            network.Save("network.dat");
        }

        private void bLoad_Click(object sender, EventArgs e)
        {
            network.Open("network.dat");
        }

    }
}

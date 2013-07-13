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


        void replaceImage(PictureBox p, Bitmap bmp)
        {
            if (p.Image != null)
            {
                p.Image.Dispose();
            }

            p.Image = bmp;
            p.Refresh();
        }
        void replaceInput(Bitmap bmp)
        {
            replaceImage(pictureBox1, bmp);
        }

        void replaceOutput(Bitmap bmp)
        {
            replaceImage(pictureBox2, bmp);
        }
        
        void replaceNetworkOutput(Bitmap bmp)
        {
            replaceImage(pictureBox3, bmp);
        }

        void replaceNetworkInput(Bitmap bmp)
        {
            replaceImage(pictureBox4, bmp);
        }

        int pos = 0;
        void test()
        {
            var idx = pos % data.input.Count;
            double[] output = null;
            if (data.VisualizeOutput != null)
            {
                replaceOutput(data.VisualizeOutput(data.output[idx].Take(data.GetOutputDimension()).ToArray()));
                output = network.Predict(data.input[idx]);
                replaceNetworkOutput(data.VisualizeOutput(output.Take(data.GetOutputDimension()).ToArray()));

            }
            if (data.VisualizeInput != null)
            {
                replaceInput(data.VisualizeInput(data.input[idx]));
                if (output != null)
                    replaceNetworkInput(data.VisualizeInput(network.PredictReversed(output)));
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
                network.Init(data.GetInputDimension(), hidden, data.GetOutputDimension());
          
            network.learningRate = 0.0001;
            network.lambda = 0.0;
            
            train(data, batchSize:100);

        }

        private void bTrainLines_Click(object sender, EventArgs e)
        {
            data = PragmaLearn.Exampels.Datasets.Lines.Create(100000);
            var hidden = data.GetInputDimension();
            if (network.GetInputs() != data.GetInputDimension() || network.GetOutputs() != data.GetOutputDimension())
                network.Init(data.GetInputDimension(), hidden, hidden, data.GetOutputDimension());

            network.learningRate = 0.0001;
            network.lambda = 0.0;

            Task.Run(() =>
            {
                running = true;

                int t = 0;
                while (running)
                {
                    t++;
                    // network.Train(data);
                    
                    var batch = genMiniBatch(100);
                    network.TrainMiniBatch(data, batch);

                    Console.WriteLine("LEARNING RATE: " + network.learningRate);
                    //if (network.learningRate > 0.0001)
                    //    network.learningRate *= 0.9998;
                    if (t % 10 == 0)
                    {
                        this.Invoke(test);

                    }

                   
                }
            });
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

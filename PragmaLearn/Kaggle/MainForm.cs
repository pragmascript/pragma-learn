using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PragmaLearn;

namespace Kaggle
{
    public partial class MainForm : Form
    {
        PragmaLearn.Learner.BackpropNeuralNetwork network = new PragmaLearn.Learner.BackpropNeuralNetwork();
        PragmaLearn.Learner.BackpropNeuralNetwork clonedNet = new PragmaLearn.Learner.BackpropNeuralNetwork();
        
        Dataset trainData;
        Dataset resultData;
        Dataset testData;

        ZedGraph.GraphPane pane;
        ZedGraph.PointPairList zedTrain, zedTest;

        public MainForm()
        {
            InitializeComponent();

            pane = zedGraphControl1.GraphPane;

            pane.Title.Text = "Kaggle (RMSE)";
            pane.XAxis.Title.Text = "Iterations";
            pane.YAxis.Title.Text = "RMSE";

            zedTrain = new ZedGraph.PointPairList();
            zedTest = new ZedGraph.PointPairList();

            pane.AddCurve("train", zedTrain, Color.Green);
            pane.AddCurve("test", zedTest, Color.Red);
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

        Bitmap vizKeypoints(float[] input, float[] output)
        {
            var s = KeypointData.imgSize;
            var bmp = Tools.float_to_bmp(input, s, s);


            for (int ix = 0; ix < output.Length; ix += 2)
            {
                try
                {
                    var x = (int)Math.Round(output[ix] * bmp.Width);
                    var y = (int)Math.Round(output[ix + 1] * bmp.Height);

                    bmp.SetPixel(x, y, Color.Pink);
                }
                catch
                {
                }
            }

            return bmp;
        }

        void displayResult(int pos)
        {
            if (clonedNet == null)
                return;
            var input = resultData.input[pos];
            var s = KeypointData.imgSize;
            var p = clonedNet.Predict(input);
            replaceNetworkOutput(vizKeypoints(input, p));
            replaceInput(Tools.float_to_bmp(input, s, s));
        }

        void displayTrain(int pos)
        {
            display(pos, trainData);
        }

        void displayTest(int pos)
        {
            display(pos, testData);
        }

        void display(int pos, Dataset data)
        {
            if (clonedNet == null)
                return;
            var input = data.input[pos];
            var s = KeypointData.imgSize;
            var p = clonedNet.Predict(input);
            replaceNetworkOutput(vizKeypoints(input, p));
            replaceOutput(vizKeypoints(data.input[pos], data.output[pos]));
            replaceInput(Tools.float_to_bmp(input, s, s));

            clonedNet.sampleDown();
            replaceNetworkInput(Tools.float_to_bmp(clonedNet.GetInputLayer(), s, s));
        }

        int pos = 0;

        public double CalcRMSE(PragmaLearn.Learner.BackpropNeuralNetwork net, Dataset data, int count)
        {
            var error = 0.0;
            var c = 0;
            for (int i = 0; i < count; ++i)
            {
                var x = Tools.rnd.Index(data.input.Count);
                var p = net.Predict(data.input[x]);
                var y = data.output[x];
                var se = 0.0;
                for (int j = 0; j < p.Length; ++j)
                {
                    if (y[j] != -1)
                    {
                        se += (p[j] - y[j]) * (p[j] - y[j]);
                        c++;
                    }
                }
                error += se;
            }

            error /= c;

            return Math.Sqrt(error);
        }

        //async void test(int iter)
        //{
        //    var idx = pos % resultData.input.Count;

        //    var net = network.Clone();
        //    net.learningRate = network.learningRate;
        //    await Task.Run(() =>
        //    {
        //        try
        //        {
        //            var rmseTrain = CalcRMSE(net, trainData, 250) * KeypointData.origImgSize;
        //            var rmseTest = CalcRMSE(net, testData, 250) * KeypointData.origImgSize;

        //            zedTrain.Add(iter, rmseTrain);
        //            zedTrain.Sort((a, b) => a.X.CompareTo(b.X));
        //            zedTest.Add(iter, rmseTest);
        //            zedTest.Sort((a, b) => a.X.CompareTo(b.X));
        //            Console.WriteLine();
        //            Console.WriteLine("iteration: " + idx);
        //            Console.WriteLine("learning rate: " + net.learningRate);
        //            Console.WriteLine("train: " + rmseTrain);
        //            Console.WriteLine("test: " + rmseTest);
        //            Console.WriteLine();
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine();
        //            Console.WriteLine(e);
        //            Console.WriteLine();
        //        }
        //    });



        //    // net.Save("network_" + iter + ".dat");
        //    clonedNet = net;


        //    zedGraphControl1.AxisChange();

        //    pos++;
        //    Refresh();
        //}

        void test(int iter)
        {
            var idx = pos % resultData.input.Count;

            var net = network.Clone();
            net.learningRate = network.learningRate;
           
            try
            {
                var rmseTrain = CalcRMSE(net, trainData, 250) * KeypointData.origImgSize;
                var rmseTest = CalcRMSE(net, testData, 250) * KeypointData.origImgSize;

                zedTrain.Add(iter, rmseTrain);
                zedTrain.Sort((a, b) => a.X.CompareTo(b.X));
                zedTest.Add(iter, rmseTest);
                zedTest.Sort((a, b) => a.X.CompareTo(b.X));
                Console.WriteLine();
                Console.WriteLine("iteration: " + idx);
                Console.WriteLine("learning rate: " + net.learningRate);
                Console.WriteLine("train: " + rmseTrain);
                Console.WriteLine("test: " + rmseTest);
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e);
                Console.WriteLine();
            }
          



            // net.Save("network_" + iter + ".dat");
            clonedNet = net;


            zedGraphControl1.AxisChange();

            pos++;
            Refresh();
        }

        private int[] genMiniBatch(int batchSize)
        {
            var batch = new int[batchSize];

            for (int i = 0; i < batchSize; ++i)
            {
                int pos = Tools.rnd.Index(trainData.input.Count);
                batch[i] = pos % trainData.input.Count;
            }

            return batch;
        }

        volatile bool running;
        private void train(Dataset data, int batchSize = 100, int testModulo = 250)
        {
            Task.Run(() =>
            {
                running = true;

                int t = 1;
                while (running)
                {
                    Console.Write(".");
                    var batch = genMiniBatch(batchSize);
                    network.TrainMiniBatch(data, batch);

                    //if (network.learningRate > 0.00001)
                    //    network.learningRate *= 0.9995;
                    if (t % testModulo == 0)
                    {
                        Console.WriteLine("test begin");
                        this.Invoke(() => test(t));
                        Console.WriteLine("test end");
                    }
                    t++;
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
            clonedNet = network.Clone();
        }

        private void bLoadData_Click(object sender, EventArgs e)
        {
            resultData = KeypointData.LoadResultData();
            var data = KeypointData.LoadTrainData();
            data.RandomSplit(0.9f, out trainData, out testData);


            var hidden = trainData.GetInputDimension();
            initNet();
            clonedNet = network.Clone();
        }

        private void bTrain_Click(object sender, EventArgs e)
        {
            network.learningRate = 0.00001f;// 0.001;
            network.lambda = 1;
            train(trainData, batchSize: 50, testModulo: 250);
        }

        private void bExport_Click(object sender, EventArgs e)
        {
            KeypointData.ExportResult(resultData, network);
            Console.WriteLine("exported.");
        }

        private void bTest_Click(object sender, EventArgs e)
        {
            displayTrain(Tools.rnd.Index(trainData.input.Count));
        }

        private void bTestTest_Click(object sender, EventArgs e)
        {
            displayTest(Tools.rnd.Index(testData.input.Count));
        }

        private void bTestResult_Click(object sender, EventArgs e)
        {
            displayResult(Tools.rnd.Index(resultData.input.Count));
        }

        private void bOpenData_Click(object sender, EventArgs e)
        {
            trainData = new Dataset();
            trainData.Open("train.data");
            testData = new Dataset();
            testData.Open("test.data");
            resultData = new Dataset();
            resultData.Open("result.data");
            initNet();
            
            Console.WriteLine("opened");
        }

        private void bSaveData_Click(object sender, EventArgs e)
        {
            trainData.Save("train.data");
            testData.Save("test.data");
            resultData.Save("result.data");
        }


        void initNet()
        {
            network.Init(trainData.GetInputDimension(), 1024, 1024, 1024, trainData.GetOutputDimension());
        }

        private void bTrainCascades_Click(object sender, EventArgs e)
        {
            Console.WriteLine(Tools.rnd.Index(2));
        }
    }
}

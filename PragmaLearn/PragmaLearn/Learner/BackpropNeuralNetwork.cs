﻿#define DROPOUT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace PragmaLearn.Learner
{
    public class BackpropNeuralNetwork : SupervisedLearner
    {
        List<double[,]> weights, deltaWeights, lastDeltaWeights, meanSquareAvg;
        List<double[]> layers;
        List<double[]> errors;
        List<double[]> bias, deltaBias;

        public double learningRate = 0.0003;
        public double momentum = 0.9;
        public double lambda = 0;

        public BackpropNeuralNetwork()
        {
            weights = new List<double[,]>();
            deltaWeights = new List<double[,]>();
            layers = new List<double[]>();
            errors = new List<double[]>();
            bias = new List<double[]>();
            deltaBias = new List<double[]>();
            meanSquareAvg = new List<double[,]>();
            lastDeltaWeights = new List<double[,]>();
        }


        public void Init(params int[] layers)
        {
            for (int i = 0; i < layers.Length; ++i)
            {
                addLayer(layers[i]);
            }
            initRandomWeights(1);
            // initStepSizes();
        }


        public override void Open(string filename)
        {
            using (FileStream f = new FileStream(filename, FileMode.Open))
            {
                Open(f);
            }
        }

        public void Open(Stream stream)
        {
            clear();
            BinaryReader reader = new BinaryReader(stream);
            var wc = reader.ReadInt32();
            Debug.Assert(wc == weights.Count);
            for (int wx = 0; wx < wc; ++wx)
            {
                var ix = reader.ReadInt32();
                var jx = reader.ReadInt32();

                if (layers.Count >= 2)
                {
                    if (ix != layers[layers.Count - 1].Length)
                        throw new FileLoadException();
                    addLayer(jx);
                }
                else
                {
                    addLayer(ix);
                    addLayer(jx);
                }

                var w = weights[weights.Count - 1];

                for (int i = 0; i < ix; ++i)
                {
                    for (int j = 0; j < jx; ++j)
                    {
                        w[i, j] = reader.ReadDouble();
                    }
                }
            }

            foreach (var b in bias)
            {
                var ix = b.Length;
                if (ix != reader.ReadInt32())
                    throw new FileLoadException();
                for (int i = 0; i < ix; ++i)
                {
                    b[i] = reader.ReadDouble();
                }

            }
        }
        public override void Save(string filename)
        {
            using (FileStream f = new FileStream(filename, FileMode.Create))
            {
                Save(f);
            }
        }

        public void Save(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(weights.Count);
            foreach (var w in weights)
            {
                var ix = w.GetLength(0);
                var jx = w.GetLength(1);
                writer.Write(ix);
                writer.Write(jx);
                for (int i = 0; i < ix; ++i)
                {
                    for (int j = 0; j < jx; ++j)
                    {
                        writer.Write(w[i, j]);
                    }
                }
            }

            foreach (var b in bias)
            {
                writer.Write(b.Length);
                for (int i = 0; i < b.Length; ++i)
                {
                    writer.Write(b[i]);
                }
            }
        }

        public BackpropNeuralNetwork Clone()
        {
            var result = new BackpropNeuralNetwork();
            var ms = new MemoryStream();
            Save(ms);
            ms.Seek(0, SeekOrigin.Begin);
            result.Open(ms);
            return result;
        }

        public override double[] Predict(double[] input)
        {
            const int samples = 10;
#if DROPOUT
            var output = layers.Last();
            var result = new double[output.Length];
            
            for (int i = 0; i < samples; ++i)
            {
                setInput(input);
                sampleUpDropout();
                for (int x = 0; x < result.Length; ++x)
                {
                    result[x] += output[x];
                }
            }
            for (int x = 0; x < result.Length; ++x)
            {
                result[x] /= samples;
            }

            return result;
            
#else
            setInput(input);
            sampleUp();
            return layers[layers.Count - 1];
#endif

        }
   
        public double[] PredictReversed(double[] output)
        {
            setOutput(output);
            sampleDown();
            return layers[0];
        }

        public override void Test(Dataset data)
        {
            Console.WriteLine("forward pass");
            for (int i = 0; i < data.input.Count; ++i)
            {
                Console.WriteLine("{0} -> {1},  net = {2}", data.input[i].Print(), data.output[i].Print(), Predict(data.input[i]).Print());
            }
            Console.WriteLine("backward pass");
            for (int i = 0; i < data.output.Count; ++i)
            {
                Console.WriteLine("{0} -> {1},  net = {2}", data.output[i].Print(), data.input[i].Print(), PredictReversed(data.output[i]).Print());
            }
        }

        public float TrainMiniBatch(Dataset data, IEnumerable<int> indices)
        {
            var mse = 0.0;
            clearDeltaWeights();

            mse += trainBackprop(data, indices);
            // mse += trainReversed(data, indices);

            //Console.WriteLine(weights[0][0, 0]);
            //Console.WriteLine(weights[1][0, 0]);
            //Console.WriteLine("mse: " + mse / 2);

            applyDeltaWeights(indices.Count());

            return (float)(mse / 2);
        }

        public int[] GetLayers()
        {
            return layers.Select(x => x.Length).ToArray();
        }

        public int GetInputs()
        {
            if (layers == null || layers.Count == 0)
                return 0;
            return layers[0].Length;
        }

        public int GetOutputs()
        {
            if (layers == null || layers.Count == 0)
                return 0;
            return layers.Last().Length;
        }

        public double CalcRMSE(Dataset data)
        {
            var error = 0.0;
            for (int i = 0; i < data.input.Count; ++i)
            {
                var p = Predict(data.input[i]);
                var y = data.output[i];
                var se = 0.0;
                for (int j = 0; j < p.Length; ++j)
                {
                    se += (p[j] - y[j]) * (p[j] - y[j]);
                }
                error += se;
            }

            error /= (data.output.Count * data.GetOutputDimension());

            return Math.Sqrt(error);
        }

   

        public double[] GetInputLayer()
        {
            return layers.First();
        }

        public double[] GetOutputLayer()
        {
            return layers.Last();
        }

        public double[] GetLayer(int layer)
        {
            return layers[layer];
        }

        void addLayer(int dim)
        {
            layers.Add(new double[dim]);
            errors.Add(new double[dim]);
            bias.Add(new double[dim]);
            deltaBias.Add(new double[dim]);

            if (layers.Count >= 2)
            {
                var wi = layers[layers.Count - 2].Length;
                var wj = dim;
                weights.Add(new double[wi, wj]);
                deltaWeights.Add(new double[wi, wj]);
                lastDeltaWeights.Add(new double[wi, wj]);
                meanSquareAvg.Add(new double[wi, wj]);
            }
        }

        void clear()
        {
            layers.Clear();
            errors.Clear();
            bias.Clear();
            deltaBias.Clear();
            weights.Clear();
            deltaWeights.Clear();
        }

        protected override float train(Dataset data)
        {
            var mse = 0.0;

            clearDeltaWeights();

            mse += trainBackprop(data);
            // mse += trainReversed(data);

            Console.WriteLine(weights[0][0, 0]);
            Console.WriteLine(weights[1][0, 0]);
            Console.WriteLine("mse: " + mse);
            applyDeltaWeights(data.input.Count);

            return (float)(mse / 2);
        }

        float trainBackprop(Dataset data, IEnumerable<int> indices)
        {
            var mse = 0.0;
            foreach (var t in indices)
            {
                setInput(data.input[t]);
#if !DROPOUT
                sampleUp();
#else
                sampleUpDropout();
#endif
                // mse += CalcMSE(data.output[t], layers[layers.Count - 1]);
                backprop(data.output[t]);
                accumulateDeltaWeights();
            }

            return (float)(mse / indices.Count());
        }

        float trainReversed(Dataset data, IEnumerable<int> indices)
        {
            var mse = 0.0;
            foreach (var t in indices)
            {
                //setInput(data.input[t]);
                //sampleUp();
                //normalizeFrom(data.output[t].Length, layers.Last());
                setSparseOutput(data.output[t]);
                sampleDown();
                mse += CalcMSE(data.input[t], layers[0]);
                backpropReversed(data.input[t]);
                accumulateDeltaWeightsReversed();
            }

            return (float)(mse / indices.Count());
        }

        float trainBackprop(Dataset data)
        {
            // clearErrors();

            var mse = 0.0;
            for (int t = 0; t < data.input.Count; ++t)
            {
                // clearDeltaWeights(); 

                setInput(data.input[t]);
#if !DROPOUT
                sampleUp();
#else
                sampleUpDropout();
#endif
                mse += CalcMSE(data.output[t], layers[layers.Count - 1]);
                backprop(data.output[t]);
                accumulateDeltaWeights();

                // applyDeltaWeights();
            }


            return (float)(mse / data.input.Count);
        }

        float trainReversed(Dataset data)
        {
            // clearErrors();
            var mse = 0.0;
            for (int t = 0; t < data.output.Count; ++t)
            {
                setInput(data.input[t]);
                sampleUp();
                setSparseOutput(data.output[t]);
                sampleDown();
                mse += CalcMSE(data.input[t], layers[0]);
                backpropReversed(data.input[t]);
                accumulateDeltaWeightsReversed();
            }

            return (float)(mse / data.input.Count);
        }



        static double activation(double x)
        {
            return rec(x);
            // return sigmoid(x);
        }

        static double activation_diff(double x)
        {
            return rec_diff(x);
            // return sigmoid_diff(x);
        }

        static double sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        static double sigmoid_stochastic(double x)
        {
            var s = sigmoid(x);
            if (Tools.rnd.NextDouble() <= s)
                return 1.0;
            else
                return 0.0;
        }

        static double sigmoid_diff(double x)
        {
            return x * (1.0 - x);
        }

        static double rec(double x)
        {
            return Math.Max(0, x);
        }

        static double rec_diff(double x)
        {
            return x > 0 ? 1.0 : 0.0;
        }


        /// <summary>
        /// inits weights to uniformly random between [-size, size]
        /// </summary>
        void initRandomWeights(double size = 0.1)
        {
            foreach (var w in weights)
            {
                for (int i = 0; i < w.GetLength(0); ++i)
                {
                    for (int j = 0; j < w.GetLength(1); ++j)
                    {
                        w[i, j] = (Tools.rnd.NextDouble() - 0.5) * 2.0 * (1.0f / Math.Sqrt(w.GetLength(0)));
                    }
                }
            }
            foreach (var b in bias)
            {
                for (int i = 0; i < b.Length; ++i)
                {
                    b[i] = (Tools.rnd.NextDouble() - 0.5) * 2.0 * (1.0f / Math.Sqrt(b.Length));
                }
            }
        }

        void addUniformNoise(int from, double[] layer, double noise)
        {
            for (int i = from; i < layer.Length; ++i)
            {
                layer[i] += (Tools.rnd.NextDouble() - 0.5) * 2 * noise;
            }
        }

        void normalizeFrom(int from, double[] layer)
        {
            var length = 0.0;
            for (int i = from; i < layer.Length; ++i)
            {
                length += Math.Abs(layer[i]);
            }
            length += 0.00001;
            for (int i = from; i < layer.Length; ++i)
            {
                layer[i] *= 1.0 / length;
            }
        }


        void setInput(double[] input)
        {
            input.CopyTo(layers[0], 0);
        }

        void setOutput(double[] output)
        {
            output.CopyTo(layers[layers.Count - 1], 0);
        }


        void setSparseOutput(double[] output)
        {
            var y = layers[layers.Count - 1];
            for (int i = 0; i < output.Length; ++i)
            {
                if (output[i] != 0)
                    y[i] = output[i];
            }
        }

        public void sampleUp()
        {
            for (int l = 0; l < layers.Count - 1; ++l)
            {
                var x = layers[l];
                var y = layers[l + 1];
                var w = weights[l];
                var b = bias[l + 1];

                // for (int j = 0; j < b.Length; ++j)
                Parallel.For(0, b.Length, j =>
                {
                    var sum = b[j] * 1.0;
                    for (int i = 0; i < x.Length; ++i)
                    {
                        sum += w[i, j] * x[i];
                    }

#if DROPOUT
                    if (l >= 1 && l < layers.Count - 2)
                        sum *= 0.5f;
#endif
                    y[j] = activation(sum);
                }
                );
            }
        }

        [Conditional("DROPOUT")]
        void sampleUpDropout()
        {
            for (int l = 0; l < layers.Count - 1; ++l)
            {
                var x = layers[l];
                var y = layers[l + 1];
                var w = weights[l];
                var b = bias[l + 1];

                //for (int j = 0; j < b.Length; ++j)
                Parallel.For(0, b.Length, j =>
                {
                    if (l < layers.Count - 2 && Tools.rnd.NextDouble() > 0.5)
                    {
                        y[j] = 0;
                        return;
                        // continue;
                    }

                    var sum = b[j] * 1.0;

                    for (int i = 0; i < x.Length; ++i)
                    {
                        sum += w[i, j] * x[i];
                    }
                    y[j] = activation(sum);
                }
                );
            }
        }

        public void sampleDown()
        {
            for (int l = layers.Count - 1; l > 0; --l)
            {
                var x = layers[l];
                var y = layers[l - 1];
                var w = weights[l - 1];
                var b = bias[l - 1];

                // for (int i = 0; i < w.GetLength(0); ++i)
                Parallel.For(0, b.Length, i =>
                {
                    var sum = b[i] * 1.0;
                    for (int j = 0; j < x.Length; ++j)
                    {
                        sum += w[i, j] * x[j];
                    }
                    y[i] = activation(sum);
                }
                );
            }
        }



        void calcErrors(double[] ty)
        {
            var ey = errors[layers.Count - 1];
            var y = layers[layers.Count - 1];
            // calculate error
            for (int j = 0; j < ty.Length; ++j)
            {
                // hack: 
                if (ty[j] == -1)
                    ey[j] = 0;
                else
                    ey[j] = y[j] - ty[j];
            }

            for (int l = layers.Count - 1; l > 1; --l)
            {
                ey = errors[l];
                var ex = errors[l - 1];
                var x = layers[l - 1];
                var w = weights[l - 1];

                // for (int i = 0; i < x.Length; ++i)
                Parallel.For(0, x.Length, i =>
                {
                    var sum = 0.0;
                    for (int j = 0; j < ey.Length; ++j)
                    {
                        sum += w[i, j] * ey[j];
                    }

                    ex[i] = sum * activation_diff(x[i]);
                }
                );
            }
        }

        void calcErrorsReversed(double[] ty)
        {
            var ey = errors[0];
            var y = layers[0];
            // calculate error
            for (int j = 0; j < y.Length; ++j)
            {
                ey[j] = y[j] - ty[j];
            }

            for (int l = 0; l < layers.Count - 1; ++l)
            {
                ey = errors[l];
                var ex = errors[l + 1];
                var x = layers[l + 1];
                var w = weights[l];
                // for (int j = 0; j < x.Length; ++j)
                Parallel.For(0, x.Length, j =>
                {
                    var sum = 0.0;
                    for (int i = 0; i < ey.Length; ++i)
                    {
                        sum += w[i, j] * ey[i];
                    }

                    ex[j] = sum * activation_diff(x[j]);
                }
                );
            }

        }

        void accumulateDeltaWeights()
        {
            for (int l = 0; l < deltaWeights.Count; ++l)
            //Parallel.For(0, deltaWeights.Count, l =>
            {
                var dw = deltaWeights[l];
                var x = layers[l];
                var ey = errors[l + 1];
                // for (int i = 0; i < x.Length; ++i)
                Parallel.For(0, x.Length, i =>
                {
                    for (int j = 0; j < ey.Length; ++j)
                    {
                        dw[i, j] += x[i] * ey[j];
                    }
                }
                );
            }
            //);


            // for (int l = 0; l < deltaBias.Count; ++l)
            Parallel.For(0, deltaBias.Count, l =>
            {
                var ey = errors[l];
                var db = deltaBias[l];
                for (int j = 0; j < ey.Length; ++j)
                {
                    db[j] += 1.0 * ey[j];
                }
            }
            );
        }


        void accumulateDeltaWeightsReversed()
        {
            // for (int l = 0; l < deltaWeights.Count; ++l)
            Parallel.For(0, deltaWeights.Count, l =>
            {
                var dw = deltaWeights[l];
                var y = layers[l + 1];
                var ex = errors[l];
                for (int i = 0; i < ex.Length; ++i)
                {
                    for (int j = 0; j < y.Length; ++j)
                    {
                        dw[i, j] += ex[i] * y[j];
                    }
                }
            }
            );

            // for (int l = 0; l < deltaBias.Count; ++l)
            Parallel.For(0, deltaBias.Count, l =>
            {
                var ey = errors[l];
                var db = deltaBias[l];
                for (int j = 0; j < ey.Length; ++j)
                {
                    db[j] += 1.0 * ey[j];
                }
            }
            );
        }




        void clearDeltaWeights()
        {
            // foreach (var w in deltaWeights)
            for (int i = 0; i < deltaWeights.Count; ++i)
            {
                var w = deltaWeights[i];
                deltaWeights[i] = lastDeltaWeights[i];
                deltaWeights[i].Initialize();
                lastDeltaWeights[i] = w;
            }

            foreach (var b in deltaBias)
            {
                b.Initialize();
            }
        }

        void clearErrors()
        {
            foreach (var e in errors)
            {
                e.Initialize();
            }
        }

        void applyDeltaWeights(int count = 1)
        {
            var lr = learningRate * (1.0 / count);

            for (int l = 0; l < weights.Count; ++l)
            {
                var w = weights[l];
                var dw = deltaWeights[l];
                var lastdw = lastDeltaWeights[l];
                //var step = stepSize[l];
                var msa = meanSquareAvg[l];

                var w0 = w.GetLength(0);
                var w1 = w.GetLength(1);


                // for (int i = 0; i < w0; ++i)
                Parallel.For(0, w0, i =>
                {
                    for (int j = 0; j < w1; ++j)
                    {
                        if (msa[i, j] == 0.0)
                        {
                            msa[i, j] = dw[i, j] * dw[i, j];
                        }
                        else
                        {
                            // calculate running average of squared gradient
                            msa[i, j] = 0.9 * msa[i, j] + 0.1 * dw[i, j] * dw[i, j];
                        }

                        // add L1 regularization
                        dw[i, j] += lambda * w[i, j] * learningRate;

                        // divide by sqrt of moving squared average of the squared gradient (RMSPROP)
                        var s1 = learningRate * dw[i, j] / Math.Sqrt(msa[i, j] + 0.0001);

                        // gradient descent (correction step)
                        w[i, j] -= s1;

                        // apply nesterov momentum
                        dw[i, j] = (s1 + lastdw[i, j]) * momentum;
                        w[i, j] -= dw[i, j];

                    }
                }
                );
            }

            for (int l = 0; l < bias.Count; ++l)
            // Parallel
            {
                var b = bias[l];
                var db = deltaBias[l];
                for (int j = 0; j < b.Length; ++j)
                {
                    b[j] -= db[j] * lr;
                }
            }
        }

        void backprop(double[] ty)
        {
            calcErrors(ty);
        }

        void backpropReversed(double[] ty)
        {
            calcErrorsReversed(ty);
        }

        double J(double[] input, double[] output)
        {
            var result = 0.0;

            setInput(input);
            sampleUp();

            var y = layers[layers.Count];

            for (int i = 0; i < y.Length; ++i)
            {
                result += (y[i] - output[i]) * (y[i] - output[i]);
            }

            return result;
        }

        void checkGradient(double[] input, double[] output)
        {
            clearDeltaWeights();

            setInput(input);
            sampleUp();
            backprop(output);
            accumulateDeltaWeights();
        }

    }
}

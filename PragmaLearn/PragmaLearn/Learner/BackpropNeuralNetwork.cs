// #define DROPOUT

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
        List<double[,]> weights, deltaWeights, meanSquareAvg;
        List<double[]> layers;
        List<double[]> errors;
        List<double[]> bias, deltaBias;

        const double learningRate = 0.001;
        const double lambda = 1;

        public BackpropNeuralNetwork()
	    {
            weights = new List<double[,]>();
            deltaWeights = new List<double[,]>();
            layers = new List<double[]>();
            errors = new List<double[]>();
            bias = new List<double[]>();
            deltaBias = new List<double[]>();
            meanSquareAvg = new List<double[,]>();
	    }


        public void Init(params int[] layers)
        {
            for (int i = 0; i < layers.Length; ++i)
            {
                addLayer(layers[i]);
            }
            initRandomWeights(1);
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

        public override double[] Predict(double[] input)
        {
            setInput(input);
            sampleUp();
            return layers[layers.Count - 1];
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

            Console.WriteLine(weights[0][0, 0]);
            Console.WriteLine(weights[1][0, 0]);
            Console.WriteLine("mse: " + mse / 2);
            applyDeltaWeights(indices.Count());

            return (float)(mse / 2);
        }

        protected override float train(Dataset data)
        {
            var mse = 0.0;

            clearDeltaWeights();
            
            mse += trainBackprop(data);
            // mse += trainReversed(data);

            Console.WriteLine(weights[0][0,0]);
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
                mse += CalcMSE(layers[layers.Count - 1], data.output[t]);
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
                sampleUp();
                setSparseOutput(data.output[t]);
                sampleDown();
                mse += CalcMSE(layers[0], data.input[t]);
                backpropReversed(data.input[t]);
                accumulateDeltaWeightsReversed();
            }

            return (float)(mse / data.input.Count);
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
                mse += CalcMSE(layers[layers.Count - 1], data.output[t]);
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
                mse += CalcMSE(layers[0], data.input[t]);
                backpropReversed(data.input[t]);
                accumulateDeltaWeightsReversed();
            }

            return (float)(mse / data.input.Count);
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
                        w[i, j] = (Tools.rnd.NextDouble() - 0.5) * 2.0 * size;
                    }
                }
            }
            foreach (var b in bias)
            {
                for (int i = 0; i < b.Length; ++i)
                {
                    b[i] = (Tools.rnd.NextDouble() - 0.5) * 2.0 * size;
                }
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
        void sampleUp()
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
                    if (l >= 1)
                        sum *= 0.5f;
#endif
                    y[j] = sigmoid(sum);
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

                for (int j = 0; j < b.Length; ++j)
                //Parallel.For(0, b.Length, j =>
                {
                    if (l < layers.Count - 2 && Tools.rnd.NextDouble() > 0.5)
                    {
                        y[j] = 0;
                        // return;
                        continue;
                    }

                    var sum = b[j] * 1.0;

                    for (int i = 0; i < x.Length; ++i)
                    {
                        sum += w[i, j] * x[i];
                    }
                    y[j] = sigmoid(sum);
                }
                 //);
            }
        }

        void sampleDown()
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
                    y[i] = sigmoid(sum);
                }
                );
            }
        }

        void calcErrors(double[] ty)
        {
            var ey = errors[layers.Count - 1];
            var y = layers[layers.Count - 1];
            // calculate error
            for (int j = 0; j < y.Length; ++j)
            {
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
                   
                    ex[i] = sum * sigmoid_diff(x[i]);
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

                    ex[j] = sum * sigmoid_diff(x[j]);
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
            foreach (var w in deltaWeights)
            {
                w.Initialize();
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
                var msa = meanSquareAvg[l];

                var w0 = w.GetLength(0);
                var w1 = w.GetLength(1);

                
                // for (int i = 0; i < w0; ++i)
                Parallel.For(0, w0, i =>
                {
                    for (int j = 0; j < w1; ++j)
                    {
                        msa[i, j] = 0.9 * msa[i, j] + 0.1 * dw[i, j] * dw[i, j];
                        // w[i, j] -= ( (1.0/count) * dw[i, j] + lambda * w[i, j]) * learningRate;
                        w[i, j] -= learningRate * dw[i, j] / Math.Sqrt(msa[i, j]);
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

        public override void Open(string filename)
        {
            clear();
            using (FileStream f = new FileStream(filename, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(f))
                {
                    var wc = reader.ReadInt32();
                    Debug.Assert(wc == weights.Count);
                    for (int wx = 0; wx < wc; ++wx )
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
            }
        }

        public override void Save(string filename)
        {
            using (FileStream f = new FileStream(filename, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(f))
                {
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
            }
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

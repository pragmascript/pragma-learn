using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragmaLearn.Learner
{
    public abstract class SupervisedLearner
    {
        double lastMSE;
        public abstract double[] Predict(double[] input);

        public virtual double Train(Dataset data, int maxIterations = 1, double stopMSE = 0.0, Action test = null)
        {
            for (int i = 0; i < maxIterations; ++i)
            {
                lastMSE = train(data);
                if (i % 100 == 0 && test != null) 
                    test();
                if (lastMSE <= stopMSE)
                    break;
            }

            return lastMSE;
        }

        public double CalcMSE(Dataset data)
        {
            double se = 0;
            for (int i = 0; i < data.input.Count; ++i)
            {
                se += CalcMSE(data.output[i], Predict(data.input[i]));
            }

            return se / data.input.Count;
        }

        public virtual void Test(Dataset data)
        {
            for (int i = 0; i < data.input.Count; ++i)
            {
                Console.WriteLine("{0} -> {1},  net = {2}", data.input[i].Print(), data.output[i].Print(), Predict(data.input[i]).Print());
            }
        }
        
        public static double CalcMSE(double[] x, double[] y)
        {
            double se = 0;

            for (int i = 0; i < x.Length; ++i)
            {
                se += (x[i] - y[i]) * (x[i] - y[i]);
            }

            return se / x.Length;
        }

        /// <summary>
        /// Performs a single training iteration on the dataset and returns the achieved MSE
        /// </summary>
        /// <returns>MSE</returns>
        protected abstract float train(Dataset data);

        public abstract void Save(string filename);

        public abstract void Open(string filename);

        
    }
}

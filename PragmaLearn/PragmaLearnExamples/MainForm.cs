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

        private void bTrainOCR_Click(object sender, EventArgs e)
        {
            data = PragmaLearn.Exampels.Datasets.OCR.Create();
            network.Init(data.GetInputDimension(), data.GetOutputDimension());
            network.Train(data, maxIterations: 100);
        }
    }
}

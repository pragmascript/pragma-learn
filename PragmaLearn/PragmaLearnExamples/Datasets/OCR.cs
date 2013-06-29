using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragmaLearn.Exampels.Datasets
{
    class OCR
    {
        static readonly string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        static readonly string[] fontsToTrain = { "Arial", "Consolas", "Courier New", "Times New Roman", "Verdana" };

        const int width = 16;
        const int height = 16;

        public static Dataset Create()
        {

            Dataset result = new Dataset();
            Bitmap bmp = new Bitmap(width, height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            float size = 8;

            Rectangle rect = new Rectangle(0, 0, width, height);



            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                for (int x = 0; x < 1; ++x)
                {
                    foreach (var f in fontsToTrain)
                    {
                        for (int i = 0; i < alphabet.Length; ++i)
                        {
                            size = 9.0f + (float)Tools.rnd.NextDouble() * 2.0f;
                            using (Font font = new Font(f, size))
                            {
                                var a = alphabet[i];
                                g.Clear(Color.Black);
                                g.TranslateTransform(width / 2, height / 2);
                                g.RotateTransform(-5.0f + (float)Tools.rnd.NextDouble() * 10.0f);
                                g.TranslateTransform(-width / 2, -height / 2);
                                g.DrawString(a, font, Brushes.White, rect, stringFormat);
                                
                                g.ResetTransform();
                                var inp = Tools.bmp_to_double(bmp);
                                var outp = new double[alphabet.Length];
                                outp[i] = 1.0;
                                result.AddPair(inp, outp);
                            }
                        }
                    }
                }
            }
            result.VisualizeInput = visualizeInput;
            result.VisualizeOutput = visualizeOutput;
            return result;
        }

        static Bitmap visualizeInput(double[] input)
        {
            return Tools.double_to_bmp(input, width, height);
        }


        static Bitmap visualizeOutput(double[] output)
        {
            var max = output.MaxIndex();

            Bitmap result = new Bitmap(width, height);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            const int size = 8;

            Rectangle rect = new Rectangle(0, 0, width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                using (Font font = new Font("Arial", size))
                {
                    var a = alphabet[max];
                    Console.WriteLine("out: " + a);
                    g.Clear(Color.Black);
                    g.DrawString(a, font, Brushes.White, rect, stringFormat);
                }
            }
            return result;
        }


    }
}

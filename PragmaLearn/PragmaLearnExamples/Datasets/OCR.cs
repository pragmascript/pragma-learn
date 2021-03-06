﻿using System;
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
        static readonly string[] alphabet = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", ",", "!"};
        static readonly string[] fontsToTrain = { "Arial", "Consolas", "Courier New", "Times New Roman", "Verdana", "Calibri Light", "Segoe Script",  "Segoe Print", "Liberation Sans", "Cambria", "Impact Standard"};

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
                for (int x = 0; x < 100; ++x)
                {
                    // var fonts = fontsToTrain.Shuffle().ToList();
                   //  foreach (var f in fonts)
                    {
                        var ai = Enumerable.Range(0, alphabet.Length).Shuffle().ToList();
                        // for (int i = 0; i < alphabet.Length; ++i)
                        foreach (var i in ai)
                        {
                            var f = fontsToTrain[Tools.rnd.Next(fontsToTrain.Length)];
                            size = 9.0f + (float)Tools.rnd.NextDouble() * 4.0f;
                            using (Font font = new Font(f, size))
                            {
                                var a = alphabet[i];
                                g.Clear(Color.Black);
                                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                g.TranslateTransform(width / 2, height / 2);
                                g.RotateTransform(((float)Tools.rnd.NextDouble()-0.5f) * 45);
                                g.TranslateTransform(-width / 2, -height / 2);
                                var dx = ((float)Tools.rnd.NextDouble() - 0.5f) * 2.0f;
                                var dy = ((float)Tools.rnd.NextDouble() - 0.5f) * 2.0f;
                                g.TranslateTransform(dx, dy);
                                g.DrawString(a, font, Brushes.White, rect, stringFormat);
                                
                                g.ResetTransform();
                                var inp = Tools.bmp_to_float(bmp);
                                var outp = new float[alphabet.Length];
                                outp[i] = 1.0f;
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

        static Bitmap visualizeInput(float[] input)
        {
            return Tools.float_to_bmp(input, width, height);
        }


        static Bitmap visualizeOutput(float[] output)
        {
            var max = output.MaxIndex();

            Bitmap result = new Bitmap(256, 256);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            const int size = 80;

            Rectangle rect = new Rectangle(0, 0, 256, 256);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
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

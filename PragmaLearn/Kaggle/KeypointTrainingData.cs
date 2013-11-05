using PragmaLearn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using PragmaLearn;
using System.Drawing;
using AForge.Imaging.Filters;
using PragmaLearn.Learner;

namespace Kaggle
{
    class KeypointData
    {
        const string dataPath = @"..\..\data\";
        const int outputSize = 15 * 2;
        public const int origImgSize = 96;
        public const int imgSize = 32;
        const int inputSize = origImgSize * origImgSize;

        static AForge.Imaging.Filters.ResizeBicubic resize = new ResizeBicubic(imgSize, imgSize);
        static Grayscale gray = Grayscale.CommonAlgorithms.BT709;
        static HistogramEqualization norm = new HistogramEqualization();
        static Edges edges = new Edges();
        static Sharpen sharpen = new Sharpen();
        
        static FiltersSequence filters = new FiltersSequence(gray, norm, resize);
        static Bitmap applyFilters(Bitmap bmp)
        {
            return filters.Apply(bmp);
        }


 
        static Bitmap transformRandom(Bitmap bmp, PointF[] points)
        {
            const float maxTranslation = 0;
            const float maxRotation = 0;
            const float maxScale = 0.30f;

            var ps = points.ToArray();

            Bitmap result = new Bitmap(origImgSize, origImgSize);
            
            using (Graphics g = Graphics.FromImage(result))
            {   
                var scl = 1.0f +(float)(Tools.rnd.Uniform() * maxScale);
                
                var imh = origImgSize / 2.0f;
                g.TranslateTransform(-origImgSize / 2.0f, -origImgSize / 2.0f, System.Drawing.Drawing2D.MatrixOrder.Append);

                g.ScaleTransform(scl, scl, System.Drawing.Drawing2D.MatrixOrder.Append);

                var rot = ((float)Tools.rnd.Uniform() - 0.5f) * 2 * maxRotation;
                g.RotateTransform(rot, System.Drawing.Drawing2D.MatrixOrder.Append);
                g.TranslateTransform(+imh, +imh, System.Drawing.Drawing2D.MatrixOrder.Append);

                var tx = ((float)Tools.rnd.Uniform() - 0.5f) * 2 * maxTranslation;
                var ty = ((float)Tools.rnd.Uniform() - 0.5f) * 2 * maxTranslation;
                //tx = 10;
                //ty = 20;
                g.TranslateTransform(tx, ty);
                
                g.DrawImage(bmp, 0, 0, result.Width, result.Height);
               
                g.Transform.TransformPoints(ps);
            }

            for (int i = 0; i < ps.Length; ++i)
            {
                if (points[i].X != -1)
                    points[i].X = ps[i].X;
                if (points[i].Y != -1)
                    points[i].Y = ps[i].Y;
            }

            for (int i = 0; i < ps.Length; ++i)
            {
                if (points[i].X < 0 || points[i].X > origImgSize ||
                    points[i].Y < 0 || points[i].Y > origImgSize)
                {
                    points[i].X = -1;
                    points[i].Y = -1;
                }
            }

        
            return result;
        }


        public static Dataset LoadTrainData()
        {
            Dataset result = new Dataset();
            var net = new PragmaLearn.Learner.BackpropNeuralNetwork();
            net.Open("network.dat");
            var reader = new StreamReader(Path.Combine(dataPath, "training.csv"));

            // ignore first line
            var line = reader.ReadLine();

            while ((line = reader.ReadLine()) != null)
            {
                var ls = line.Split(',');

                bool skipped = false;
                var keypoints = new double[outputSize];
                for (int i = 0; i < outputSize; ++i)
                {
                    if (string.IsNullOrEmpty(ls[i]))
                    {
                        Console.Write(".");
                        keypoints[i] = -1;
                        // output[i] = Tools.rnd.NextDouble();
                        skipped = true;
                        // break;
                    }
                    else
                    {
                        keypoints[i] = double.Parse(ls[i]);
                    }
                }

                //if (skipped)
                //    continue;
   
                var pic = ls.Last().Split(' ');
                var img = new double[inputSize];
                for (int j = 0; j < inputSize; ++j)
                {
                    img[j] = double.Parse(pic[j]) / 255.0;
                }

         
                //if (skipped)
                //{
                //    using (var bmp = Tools.double_to_bmp(img, origImgSize, origImgSize))
                //    {
                //        using (var filtered = applyFilters(bmp))
                //        {
                //            var input = Tools.bmp_to_double(filtered);
                //            var p = net.Predict(input);

                //            for (int i = 0; i < keypoints.Length; ++i)
                //            {
                //                if (keypoints[i] == -1)
                //                    keypoints[i] = p[i] * origImgSize;
                //            }
                //        }
                //    }
                //}

                for (int x = 0; x < 20; ++x)
                {

                    var points = new PointF[keypoints.Length / 2];
                    for (int i = 0; i < keypoints.Length; i += 2)
                    {
                        points[i / 2] = new PointF((float)keypoints[i], (float)keypoints[i + 1]);
                    }

                    var input = default(double[]);

                    using (var bmp = Tools.double_to_bmp(img, origImgSize, origImgSize))
                    {
                        if (x != 0)
                        {
                            using (var transformed = transformRandom(bmp, points))
                            {
                                using (var filtered = applyFilters(transformed))
                                {
                                    input = Tools.bmp_to_double(filtered);
                                }
                            }
                        }
                        else
                        {
                            using (var filtered = applyFilters(bmp))
                            {
                                input = Tools.bmp_to_double(filtered);
                            }
                         
                        }
                    }
                    var output = new double[keypoints.Length];
                    for (int i = 0; i < points.Length; i++)
                    {
                        output[2 * i] = points[i].X != -1 ? points[i].X / origImgSize : -1;
                        output[2 * i + 1] = points[i].Y != -1 ? points[i].Y / origImgSize : -1;
                    }

                    result.AddPair(input, output);
                }

                //if (skipped)
                //{
                //    var p = net.Predict(input);

                //    for (int i = 0; i < output.Length; ++i)
                //    {
                //        if (output[i] == -1)
                //            output[i] = p[i];
                //    }
                //}

                
            }

            reader.Dispose();
            return result;


        }

        public static Dataset LoadResultData()
        {
            Dataset result = new Dataset();

            var reader = new StreamReader(Path.Combine(dataPath, "test.csv"));

            // ignore first line
            var line = reader.ReadLine();

            while ((line = reader.ReadLine()) != null)
            {
                var ls = line.Split(',');

                int pos = 0;

                var output = default(double[]);


                var pic = ls.Last().Split(' ');
                var img = new double[inputSize];
                for (int j = 0; j < inputSize; ++j)
                {
                    img[j] = double.Parse(pic[j]) / 255.0;
                }


                var input = default(double[]);
                using (var bmp = Tools.double_to_bmp(img, origImgSize, origImgSize))
                {
                    using (var filtered = applyFilters(bmp))
                    {
                        input = Tools.bmp_to_double(filtered);
                    }
                }

                result.AddPair(input, output);
            }

            return result;


        }


        public static void ExportResult(Dataset testData, BackpropNeuralNetwork net)
        {
            var reader = new StreamReader(Path.Combine(dataPath, "IdLookupTable.csv"));
            var writer = new StreamWriter(Path.Combine(dataPath, "result.csv"));

        

            var featureNames = new List<string>() 
            {               
                "left_eye_center_x", "left_eye_center_y",
                "right_eye_center_x", "right_eye_center_y",
                "left_eye_inner_corner_x", "left_eye_inner_corner_y",
                "left_eye_outer_corner_x", "left_eye_outer_corner_y",
                "right_eye_inner_corner_x", "right_eye_inner_corner_y",
                "right_eye_outer_corner_x", "right_eye_outer_corner_y",
                "left_eyebrow_inner_end_x", "left_eyebrow_inner_end_y",
                "left_eyebrow_outer_end_x", "left_eyebrow_outer_end_y",
                "right_eyebrow_inner_end_x", "right_eyebrow_inner_end_y", 
                "right_eyebrow_outer_end_x", "right_eyebrow_outer_end_y", 
                "nose_tip_x", "nose_tip_y",
                "mouth_left_corner_x", "mouth_left_corner_y",
                "mouth_right_corner_x", "mouth_right_corner_y",
                "mouth_center_top_lip_x", "mouth_center_top_lip_y",
                "mouth_center_bottom_lip_x", "mouth_center_bottom_lip_y"
            };

            var featureLookup = featureNames.Select((f, i) => new { key = f, value = i }).ToDictionary(t => t.key, t => t.value);

            Console.WriteLine();

            var image = -1;
            var p = default(double[]);
            var s = reader.ReadLine();
            writer.WriteLine("RowId,Location");
            int row = 1;
            while((s = reader.ReadLine()) != null)
            {
                var v = s.Split(',');
                var img = int.Parse(v[1]);
                if (img != image)
                {
                    image = img;
                    p = net.Predict(testData.input[image-1]);
                }
                                    
                var idx = featureLookup[v[2]];
                var value = Tools.Clamp(p[idx] * origImgSize, 0, origImgSize);
                var rl = row + "," + value;
                row++;
                writer.WriteLine(rl);
                Console.Write(".");
            }
            Console.WriteLine();
            reader.Dispose();
            writer.Dispose();
        }

    }
}

using PragmaLearn;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragmaLearn.Exampels.Datasets
{
    public struct Vec2
    {
        public float X;
        public float Y;

        public int Xi { get { return (int)Math.Round(X); } }
        public int Yi { get { return (int)Math.Round(Y); } }

        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }
        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }
        public Vec2 Normalized()
        {
            var l = 1.0f / Length();
            return this * l;
        }
        public void Normalize()
        {
            var l = 1.0f / Length();
            X *= l;
            Y *= l;
        }
        public static float Dot(Vec2 v1, Vec2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }
        public static Vec2 operator +(Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static Vec2 operator -(Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.X - v2.X, v1.Y - v2.Y);
        }
        public static Vec2 operator *(Vec2 v1, float s)
        {
            return new Vec2(v1.X * s, v1.Y * s);
        }
        public static implicit operator PointF(Vec2 v1)
        {
            return new PointF(v1.X, v1.Y);
        }
    }

    class LineSegment
    {
        public Vec2 pos1, pos2;
    }

    class Lines
    {
        const int minLineCount = 1;
        const int maxLineCount = 6;
        const float minLineLength = 2.0f;
        const int size = 128;
        const int s_size = 32;
        const float lineDensity = 0.5f;
        const int whiteNoise = 16;
        const float epsilon = 1.5f;

        static IEnumerable<Vec2> generatePointsOnLine(LineSegment l, float epsilon)
        {
            var linePoints = (int)(lineDensity * (l.pos1 - l.pos2).Length());
            for (int i = 0; i < linePoints; ++i)
            {
                var d = l.pos2 - l.pos1;
                var a = (float)Tools.rnd.NextDouble();

                var x = l.pos1.X + d.X * a;
                var y = l.pos1.Y + d.Y * a;
                var p = l.pos1 + (d * a);
                /// HACK: replace with normal dist 
                p.X += epsilon * (float)(2.0 * (Tools.rnd.NextDouble() - 0.5));
                p.Y += epsilon * (float)(2.0 * (Tools.rnd.NextDouble() - 0.5));

                yield return p;
            }
        }

        static LineSegment generateRandomLine()
        {
            var p1 = new Vec2();
            var p2 = new Vec2();
            do
            {
                p1.X = Tools.rnd.Next(size);
                p1.Y = Tools.rnd.Next(size);

                p2.X = Tools.rnd.Next(size);
                p2.Y = Tools.rnd.Next(size);
            }
            while ((p1 - p2).Length() < minLineLength);


            var l = new LineSegment() { pos1 = p1, pos2 = p2 };

            return l;
        }

        static IEnumerable<Vec2> generateWhiteNoise(int noiseParticleCount)
        {
            for (int i = 0; i < noiseParticleCount; ++i)
            {
                var n = new Vec2();
                n.X = Tools.rnd.Next(size);
                n.Y = Tools.rnd.Next(size);
                yield return n;
            }

        }


        static void drawLines(List<LineSegment> lines, Bitmap bmp)
        {
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                foreach (var l in lines)
                {
                    g.DrawLine(Pens.White, l.pos1, l.pos2);
                }
            }
        }



        static Rectangle getRandomPatch()
        {
            var result = new Rectangle();

            result.X = Tools.rnd.Next(size - s_size);
            result.Y = Tools.rnd.Next(size - s_size);
            result.Width = s_size;
            result.Height = s_size;

            return result;
        }

        static void addSample(Dataset data)
        {
            var input = default(double[]);
            var output = default(double[]);
            var count = minLineCount + Tools.rnd.Next(maxLineCount - minLineCount + 1);
            var points = new List<Vec2>();
            var lines = new List<LineSegment>();

            for (int i = 0; i < count; ++i)
            {
                var l = generateRandomLine();
                lines.Add(l);
                points.AddRange(generatePointsOnLine(l, epsilon));
                points.AddRange(generateWhiteNoise(whiteNoise));
            }


            var patch = getRandomPatch();

            using (var i_bmp = new Bitmap(size, size))
            {
                i_bmp.Clear(Color.Black);
                foreach (var p in points)
                {
                    var x = p.Xi;
                    var y = p.Yi;
                    if (x >= 0 && x < i_bmp.Width && y >= 0 && y < i_bmp.Height)
                        i_bmp.SetPixel(x, y, Color.White);


                }
                Tools.Once(() => i_bmp.Save("input.png"));

                using (var sbmp = i_bmp.GetPatch(patch))
                {
                    input = Tools.bmp_to_double(sbmp);
                }
            }

            using (var o_bmp = new Bitmap(size, size))
            {
                o_bmp.Clear(Color.Black);
                drawLines(lines, o_bmp);
                using (var sbmp = o_bmp.GetPatch(patch))
                {
                    output = Tools.bmp_to_double(sbmp);
                }
            }

            data.AddPair(input, output);
        }

        public static Dataset Create(int samples)
        {
            Dataset result = new Dataset();
            for (int i = 0; i < samples; ++i)
            {
                addSample(result);
            }

            result.VisualizeInput = (input) => Tools.double_to_bmp(input, s_size, s_size);
            result.VisualizeOutput = (output) => Tools.double_to_bmp(output, s_size, s_size);

            return result;
        }

    }
}

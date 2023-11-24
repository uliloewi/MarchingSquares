using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.LinkLabel;


// implementation of marching squares algorithm
// (cf. https://en.wikipedia.org/wiki/Marching_squares)


namespace WindowsFormsApp1
{
    public static class StaticRandomNumber
    {
        private static Random rnd = new Random();
        public static int GetRandom(int min, int max)
        {
            return rnd.Next(min, max);
        }
    }

    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public class Line
    {
        public Point Start { get; set; }
        public Point End { get; set; }

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }
    }

    public class LinesRectangle
    {
        public Graphics Graphics { get; set; }
        public Pen Pen { get; set; }

        public LinesRectangle()
        {
            Pen = new Pen(Color.Blue, 2);
        }

        public void DrawLine(Line line)
        {
            Graphics.DrawLine(Pen, (int)line.Start.X, (int)line.Start.Y, (int)line.End.X, (int)line.End.Y);
        }
    }

    public partial class Form1 : System.Windows.Forms.Form
    {

        // create binary image: value == 1 if and only if value >= isovalue
        public int[,] ToBinary(double[,] dataDouble, int xn, int yn, double isovalue)
        {
            int[,] data = new int[xn, yn];

            for (int i = 0; i < xn; i++)
            {
                for (int j = 0; j < yn; j++)
                {
                    if (dataDouble[i, j] < isovalue)
                    {
                        data[i, j] = 0;
                    }
                    else
                    {
                        data[i, j] = 1;
                    }
                }
            }

            return data;
        }

        // linear interpolation between p1 and p2 based on v1, v2 and isovalue
        public double interpolate(double p1, double v1, double p2, double v2, double isovalue)
        {
            if (v2 == v1)
            {
                return (p1 + p2) / 2;
            }
            else
            {
                return p1 + (p2 - p1) * (isovalue - v1) / (v2 - v1);
            }
        }

        // linear interpolation between points based on z-values and isovalue
        public Point GetMidPoint(Point thisp, Point other, double isovalue)
        {
            return new Point(
                interpolate(thisp.X, thisp.Z, other.X, other.Z, isovalue),
                interpolate(thisp.Y, thisp.Z, other.Y, other.Z, isovalue), 
                isovalue
                );
        }

        // create lines acc. to table on wikipedia
        private IEnumerable<Line> GetLines(int casevalue, double isovalue, double[,] imageDataDouble, double[] xVector, double[] yVector, int i, int j)
        {
            List<Line> linesList = new List<Line>();

            Point topLeft = new Point(xVector[i], yVector[j], imageDataDouble[i, j]);
            Point topRight = new Point(xVector[i + 1], yVector[j], imageDataDouble[i + 1, j]);
            Point bottomLeft = new Point(xVector[i], yVector[j + 1], imageDataDouble[i, j + 1]);
            Point bottomRight = new Point(xVector[i + 1], yVector[j + 1], imageDataDouble[i + 1, j + 1]);

            switch (casevalue)
            {
                case 0:
                case 15:
                    /*do nothing*/
                    break;

                case 1:
                case 14:
                    {
                        Point start = GetMidPoint(topLeft, bottomLeft, isovalue);
                        Point end = GetMidPoint(bottomLeft, bottomRight, isovalue);
                        linesList.Add(new Line(start, end));
                    }
                    break;

                case 2:
                case 13:
                    {
                        Point start = GetMidPoint(bottomLeft, bottomRight, isovalue);
                        Point end = GetMidPoint(topRight, bottomRight, isovalue);
                        linesList.Add(new Line(start, end));
                    }
                    break;

                case 3:
                case 12:
                    {
                        Point start = GetMidPoint(topLeft, bottomLeft, isovalue);
                        Point end = GetMidPoint(topRight, bottomRight, isovalue);
                        linesList.Add(new Line(start, end));
                    }
                    break;

                case 4:
                case 11:
                    {
                        Point start = GetMidPoint(topLeft, topRight, isovalue);
                        Point end = GetMidPoint(topRight, bottomRight, isovalue);
                        linesList.Add(new Line(start, end));
                    }
                    break;


                case 6:
                case 9:
                    {
                        Point start = GetMidPoint(topLeft, topRight, isovalue);
                        Point end = GetMidPoint(bottomLeft, bottomRight, isovalue);
                        linesList.Add(new Line(start, end));
                    }
                    break;

                case 7:
                case 8:
                    {
                        Point start = GetMidPoint(topLeft, topRight, isovalue);
                        Point end = GetMidPoint(topLeft, bottomLeft, isovalue);
                        linesList.Add(new Line(start, end));
                    }
                    break;

                case 5:
                case 10:
                    {
                        double average = (topLeft.Z + bottomLeft.Z + topRight.Z + bottomRight.Z) / 4; 

                        if ((average > isovalue) ^ (topLeft.Z > isovalue))
                        {
                            // draw caseValue = 5;

                            Point start1 = GetMidPoint(topLeft, bottomLeft, isovalue);
                            Point end1 = GetMidPoint(topLeft, topRight, isovalue);
                            Line line1 = new Line(start1, end1);

                            Point start2 = GetMidPoint(bottomLeft, bottomRight, isovalue);
                            Point end2 = GetMidPoint(topRight, bottomRight, isovalue);
                            Line line2 = new Line(start2, end2);

                            linesList.Add(line1);
                            linesList.Add(line2);
                        }
                        else
                        {
                            // draw caseValue = 10;

                            Point start1 = GetMidPoint(topLeft, topRight, isovalue);
                            Point end1 = GetMidPoint(topRight, bottomRight, isovalue);
                            Line line1 = new Line(start1, end1);

                            Point start2 = GetMidPoint(topLeft, bottomLeft, isovalue);
                            Point end2 = GetMidPoint(bottomLeft, bottomRight, isovalue  );
                            Line line2 = new Line(start2, end2);

                            linesList.Add(line1);
                            linesList.Add(line2);
                        }
                    }
                    break;
            }

            return linesList;
        }

        // main algorithm
        public List<Line> marching_square(double[] xVector, double[] yVector, double[,] imageDataDouble, double isovalue)
        {
            List<Line> linesList = new List<Line>();

            int dataWidth = imageDataDouble.GetLength(0);
            int dataHeight = imageDataDouble.GetLength(1);

            if (dataWidth == xVector.Length && dataHeight == yVector.Length)
            {
                int [,] imageData = ToBinary(imageDataDouble, dataWidth, dataHeight, isovalue);

                for (int i = 0; i < dataWidth - 1; i++)
                {
                    for (int j = 0; j < dataHeight - 1; j++)
                    {
                        // calculate index for contour-lines lookup table (acc. to wikipedia)
                        int casevalue = imageData[i, j];
                        casevalue *= 2;
                        casevalue += imageData[i + 1, j];
                        casevalue *= 2;
                        casevalue += imageData[i + 1, j + 1];
                        casevalue *= 2;
                        casevalue += imageData[i, j + 1];

                        IEnumerable<Line> list = GetLines(casevalue, isovalue, imageDataDouble, xVector, yVector, i, j);

                        linesList.AddRange(list);
                    }
                }
            }
            else
            {
                throw new Exception("dimension mismatch!");
            }

            return linesList;
        }

        private void MainForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            //
            double[] x = new double[] { 0, 100, 200, 300, 400, 500, 600 };

            double[] y = new double[] { 0, 100, 200, 300, 400 };


            //double[,] data = new double[,]  {
            //                            { 2,1,2,1,2,2,2 },
            //                            { 1,2,1,2,1,2,2 },
            //                            { 2,1,2,1,2,2,2 },
            //                            { 1,2,1,2,1,2,2 },
            //                            { 2,1,2,1,2,2,2 }
            //                          };


            double[,] data = new double[,]  {
                                        { 0,1,1,1,1 },
                                        { 1,9,3,2,1 },
                                        { 1,3,1,2,1 },
                                        { 1,3,3,2,1 },
                                        { 1,3,1.5,2,1 },
                                        { 1,9,2,1.5,1 },
                                        { 1,1,1,1,1 }
                                      };


            int width = data.GetLength(0);
            int height = data.GetLength(1);

            Color[] colors = new Color[10]
            {
                Color.Blue,
                Color.Green,
                Color.Red,
                Color.Yellow,
                Color.Orange,
                Color.Orchid,
                Color.Magenta,
                Color.Coral,
                Color.DarkRed,
                Color.DarkOrange
            };

            Graphics g = this.CreateGraphics();
            {
                LinesRectangle rect = new LinesRectangle();
                rect.Graphics = g;
                rect.Pen.Color = Color.Red;
                rect.Pen.Width = 1;
                for (int i = 0; i < width; i++)
                {
                    Line line = new Line(new Point(x[i], y[0], 0), new Point(x[i], y[height - 1], 0));
                    rect.DrawLine(line);
                }
                for (int j = 0; j < height; j++)
                {
                    Line line = new Line(new Point(x[0], y[j], 0), new Point(x[width - 1], y[j], 0));
                    rect.DrawLine(line);
                }
            }

            for (int offset = 0; offset < 20; offset++)
            {

                //double xMaxCoord = x[6];
                //double yMaxCoord = y[4];
                //double[,] adjacencyMatrix = new double[xMaxCoord, yMaxCoord];

                List<Line> collection = marching_square(x, y, data, 1.1 + 0.2 * offset);




                LinesRectangle rect = new LinesRectangle();
                rect.Graphics = g;
                rect.Pen.Color = colors[offset % 10];
                foreach (var item in collection)
                {
                    rect.DrawLine(item);
                }



                System.Threading.Thread.Sleep(200);
            }
        }
    }
}
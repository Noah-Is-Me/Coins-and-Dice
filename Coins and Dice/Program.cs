using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Weasel_Program
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            Random random = new Random();
            Stopwatch stopwatch = new Stopwatch();

            int generationMax = 1000000000;
            int sampleInterval = 1000;

            int attempts = 10;

            for (int z = 0; z < attempts; z++)
            {

                double mutationRate = 0.2;
                double mutationRateStdv = 0.0001;
                double[] mutationRateData = new double[generationMax/sampleInterval];

                stopwatch.Start();

                for (int i = 0; i < generationMax; i++)
                {
                    if (i % sampleInterval == 0)
                        mutationRateData[i / sampleInterval] = mutationRate;

                    //if (i % 1000000 == 0) 
                    //  Debug.WriteLine("Generation " + i + "; " + stopwatch.ElapsedMilliseconds + " ms");

                    if (random.NextDouble() < mutationRate)
                    {
                        mutationRate += normalDistribution(0, mutationRateStdv);
                        mutationRate = Math.Max(Math.Min(mutationRate, 1), 0);
                    }
                }

                stopwatch.Stop();

                CreateGraph(mutationRateData);
            }


            double normalDistribution(double mean, double stdDev)
            {
                double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
                double u2 = 1.0 - random.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                double randNormal = mean + stdDev * randStdNormal;
                return randNormal;
            }


            void CreateGraph(double[] data)
            {
                Chart chart = new Chart();

                ChartArea CA = chart.ChartAreas.Add("A1");
                Series mutationRates = chart.Series.Add("Mutation Rates");
                mutationRates.ChartType = SeriesChartType.FastLine;

                chart.BackColor = Color.White;
                CA.BackColor = Color.White;

                CA.AxisY.Title = "Mutation Rates";
                CA.AxisX.Title = "Generations";

                CA.AxisX.TitleAlignment = StringAlignment.Center;
                CA.AxisY.TitleAlignment = StringAlignment.Center;

                CA.AxisX.TitleFont = new Font("Ariel", 15, FontStyle.Bold);
                CA.AxisY.TitleFont = new Font("Ariel", 15, FontStyle.Bold);

                CA.AxisY.Minimum = 0;
                CA.AxisY.Maximum = 1;
                CA.AxisY.Interval = 0.1;

                chart.Titles.Add("The Evolution of the Mutation Rate Without Selection");
                chart.Titles.ElementAt(0).Font = new Font("Ariel", 15, FontStyle.Bold);
                chart.Size = new Size(1920, 1080);
                chart.Series["Mutation Rates"].BorderWidth = 4;
                chart.Series["Mutation Rates"].Color = Color.Blue;

                chart.AntiAliasing = AntiAliasingStyles.Graphics;
                chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;

                if (sampleInterval > 1)
                {
                    CA.AxisX.Title = $"Generations ({sampleInterval}s)";
                }
                else
                {
                    CA.AxisX.Title = "Generations";
                }

                CA.AxisX.Interval = (double)generationMax / (sampleInterval * 10);


                for (int i=0; i<data.Length; i++)
                {
                    chart.Series["Mutation Rates"].Points.AddXY(i + 1, data[i]);
                }


                string graphInfo = $"Coins and Dice {DateTime.Now.ToString("MMM-dd-yyyy hh-mm-ss-fff tt")}";
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(baseDirectory).FullName).FullName).FullName;
                string graphsFolderPath = Path.Combine(projectDirectory, "Graphs");
                string imagePath = Path.Combine(graphsFolderPath, graphInfo + ".png");

                Debug.WriteLine("Image Path: " + imagePath);

                chart.SaveImage(imagePath, ChartImageFormat.Png);
            }
        }
    }
}

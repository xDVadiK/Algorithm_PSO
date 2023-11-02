using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace algorithmPSO
{
    public partial class Form1 : Form
    {
        private int dimension;
        private int swarmSize;
        private int maxIterations;
        private double inertiaWeight;
        private double cognitiveWeight;
        private double socialWeight;
        private int fuctionIndex;

        private Func<double[], double> fitnessFunction;
        private double[] lowerBound;
        private double[] upperBound;
        private double[] globalBestPosition;
        private double globalBestValue;

        private PSO particleSwarmOptimization;

        public Form1()
        {
            InitializeComponent();

            label1.Text = "Function";
            label2.Text = "SwarmSize";
            label3.Text = "MaxIterations";
            label4.Text = "InertiaWeight";
            label5.Text = "CognitiveWeight";
            label6.Text = "SocialWeight";
            button1.Text = "Run";
            label10.Text = "Result will be here...";
            label8.Text = "Lower Bounds";
            label9.Text = "Upper Bounds";

            comboBox1.SelectedIndex = 0;

            textBox2.Text = "50";
            textBox3.Text = "1000";
            textBox4.Text = "1";
            textBox5.Text = "4";
            textBox6.Text = "4";

            textBox1.Text = "-5 -5 -5";
            textBox7.Text = "5 5 5";
        }

        private Func<double[], double> SelectFunction(int index)
        {
            switch (index)
            {
                case 0:
                    dimension = 2;
                    label7.Text = "f(x,y) = (y - x^2)^2 + 100(1 - x)^2\n";
                    return x => (x[1] - x[0] * x[0]) * (x[1] - x[0] * x[0]) + 100 * (1 - x[0]) * (1 - x[0]);

                case 1:
                    dimension = 2;
                    label7.Text = "f(x,y) = (x - 2)^4 + (x - 2y)^2\n";
                    return x => (x[0] - 2) * (x[0] - 2) * (x[0] - 2) * (x[0] - 2) + (x[0] - 2 * x[1]) * (x[0] - 2 * x[1]);
                case 2:
                    dimension = 3;
                    label7.Text = "f(x,y,z) = (x - 1)^2 + (y - 3)^2 + 4(z + 5)^2\n";
                    return x => (x[0] - 1) * (x[0] - 1) + (x[1] - 3) * (x[1] - 3) + 4 * (x[2] + 5) * (x[2] + 5);
                default:
                    dimension = 1;
                    return x => x[0];
            }
        }

        private static double[] ParseBounds(string input)
        {
            string[] boundsStrings = input.Split(' ');

            double[] bounds = new double[boundsStrings.Length];

            for (int i = 0; i < boundsStrings.Length; i++)
            {
                if (double.TryParse(boundsStrings[i], out double number))
                {
                    bounds[i] = number;
                }
                else
                {
                    throw new FormatException($"Invalid number: {boundsStrings[i]}");
                }
            }
            return bounds;
        }

        public double[] Optimize(Func<double[], double> fitnessFunction, int dimension, double[] lowerBound, double[] upperBound)
        {
            Random rand = new Random();
            List<Particle> particles = new List<Particle>();

            // Initialize particles
            for (int i = 0; i < swarmSize; i++)
            {
                Particle particle = new Particle
                {
                    Position = new double[dimension],
                    Velocity = new double[dimension],
                    PersonalBestPosition = new double[dimension],
                    PersonalBestValue = double.MaxValue
                };

                for (int j = 0; j < dimension; j++)
                {
                    particle.Position[j] = rand.NextDouble() * (upperBound[j] - lowerBound[j]) + lowerBound[j];
                    particle.Velocity[j] = rand.NextDouble() * 2 - 1; // Random initial velocity
                }

                particles.Add(particle);
            }

            globalBestPosition = new double[dimension];
            globalBestValue = double.MaxValue;

            // Main PSO loop
            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                foreach (var particle in particles)
                {
                    double fitness = fitnessFunction(particle.Position);

                    // Update personal best
                    if (fitness < particle.PersonalBestValue)
                    {
                        particle.PersonalBestValue = fitness;
                        Array.Copy(particle.Position, particle.PersonalBestPosition, dimension);
                    }

                    // Update global best
                    if (fitness < globalBestValue)
                    {
                        globalBestValue = fitness;
                        Array.Copy(particle.Position, globalBestPosition, dimension);
                    }

                    // Update velocity and position
                    for (int j = 0; j < dimension; j++)
                    {
                        double r1 = rand.NextDouble();
                        double r2 = rand.NextDouble();

                        particle.Velocity[j] = inertiaWeight * particle.Velocity[j] +
                                               cognitiveWeight * r1 * (particle.PersonalBestPosition[j] - particle.Position[j]) +
                                               socialWeight * r2 * (globalBestPosition[j] - particle.Position[j]);

                        // Update position within bounds
                        particle.Position[j] += particle.Velocity[j];
                        if (particle.Position[j] < lowerBound[j])
                            particle.Position[j] = lowerBound[j];
                        if (particle.Position[j] > upperBound[j])
                            particle.Position[j] = upperBound[j];
                    }
                }
            }

            return globalBestPosition;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int.TryParse(textBox2.Text, out swarmSize);
            int.TryParse(textBox3.Text, out maxIterations);
            double.TryParse(textBox4.Text, out inertiaWeight);
            double.TryParse(textBox5.Text, out cognitiveWeight);
            double.TryParse(textBox6.Text, out socialWeight);
            lowerBound = ParseBounds(textBox1.Text);
            upperBound = ParseBounds(textBox7.Text);
            fitnessFunction = SelectFunction(fuctionIndex);

            particleSwarmOptimization = new PSO(dimension, swarmSize, maxIterations, inertiaWeight,
            cognitiveWeight, socialWeight, fitnessFunction,
            lowerBound, upperBound);

            particleSwarmOptimization.Optimize();

            label10.Text = particleSwarmOptimization.ResultToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fuctionIndex = comboBox1.SelectedIndex;
            fitnessFunction = SelectFunction(fuctionIndex);
        }
    }
}
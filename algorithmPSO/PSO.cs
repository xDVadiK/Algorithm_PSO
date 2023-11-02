using System;
using System.Collections.Generic;

namespace algorithmPSO
{
    internal class PSO
    {
        private readonly int dimension;
        private readonly int swarmSize;
        private readonly int maxIterations;
        private readonly double inertiaWeight;
        private readonly double cognitiveWeight;
        private readonly double socialWeight;

        private readonly Func<double[], double> fitnessFunction;
        private readonly double[] lowerBound;
        private readonly double[] upperBound;
        private double[] globalBestPosition;
        private double globalBestValue;

        public PSO(int dimension, int swarmSize, int maxIterations, double inertiaWeight,
            double cognitiveWeight, double socialWeight, Func<double[], double> fitnessFunction,
            double[] lowerBound, double[] upperBound)
        {
            this.dimension = dimension;
            this.swarmSize = swarmSize;
            this.maxIterations = maxIterations;
            this.inertiaWeight = inertiaWeight;
            this.cognitiveWeight = cognitiveWeight;
            this.socialWeight = socialWeight;
            this.fitnessFunction = fitnessFunction;
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;

        }

        internal Particle Particle
        {
            get => default;
            set
            {
            }
        }

        public double[] Optimize()
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
                    particle.Velocity[j] = rand.NextDouble() * 2 - 1;
                }

                particles.Add(particle);
            }

            globalBestPosition = new double[dimension];
            globalBestValue = double.MaxValue;

            // Main loop
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

        public string ResultToString()
        {
            string answer = "Result:\n";
            for (var i = 0; i < globalBestPosition.Length; i++)
            {
                answer += "x" + i.ToString() + "= " + globalBestPosition[i].ToString() + '\n';
            }
            return answer;
        }

    }
}

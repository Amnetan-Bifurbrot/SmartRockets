using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SmartRockets {
	class Rocket : Image {
		public static int lifetime = 200;
		public double fitness = -1;
		public Point pos;
		public Vector[] genes;
		string workingDirectory = Environment.CurrentDirectory;


		public Rocket() {
			string workingDirectory = Environment.CurrentDirectory;
			Source = new BitmapImage(new Uri(Directory.GetParent(workingDirectory).Parent.FullName + @"\Resources\rocket.png", UriKind.Relative));
			Width = 10;
			Height = 45;
			genes = new Vector[lifetime];
			//@"C:\Users\Adam Wißniewski\source\repos\Amnetan-Bifurbrot\SmartRockets\rocket.png"
		}

		public Rocket(Random r) {
			double angle;
			//obrazek rakiety
			Source = new BitmapImage(new Uri("/rocket.png", UriKind.Relative));
			Width = 20;
			Height = 90;
			genes = new Vector[lifetime];
			for (int i = 0; i < lifetime; i++) {
				angle = r.NextDouble() * 2 * Math.PI;   //losujemy geny do chromosomu (kąty od 0 do 2pi)
				genes[i] = new Vector(5 * Math.Sin(angle), 5 * Math.Cos(angle));
			}
		}

		//rozmnażanie
		public void Cross(Rocket parentA, Rocket parentB, Random r) {
			int midPoint = r.Next(lifetime); //losujemy punkt, w którym przedzielimy chromosom
			for (int i = 0; i < lifetime; i++) {
				if (i < midPoint)
					genes[i] = parentA.genes[i];
				else
					genes[i] = parentB.genes[i];
			}
		}

		public void Mutate(double chance, Random r) {
			double angle;
			for (int i = 0; i < lifetime; i++) {
				if (r.NextDouble() < chance) {
					Console.WriteLine("Mutation!");
					angle = r.NextDouble() * 2 * Math.PI;
					genes[i] = new Vector(5 * Math.Sin(angle), 5 * Math.Cos(angle));
				}
			}
		}

		public static Rocket Procreate(Rocket[] matingpool, double mutationChance, Random r) {
			Rocket child = new Rocket(), parentA, parentB;
			int matingpoolSize = matingpool.Length;		
			
			while (true) {
				int idA = r.Next(matingpoolSize);
				parentA = matingpool[idA];

				int idB = r.Next(matingpoolSize);
				parentB = matingpool[r.Next(matingpoolSize)];

				if (idA != idB)
					break;
				else
					continue;

			}
			
			//krzyżowanie
			child.Cross(parentA, parentB, r);
			//mutacja
			child.Mutate(mutationChance, r);

			return child;
		}

		public void Fitness(Point target) {
			//zwracamy 1/odległość do targetu

			double dx = pos.X - target.X, dy = pos.Y - target.Y;
			fitness = 1.0 / Math.Sqrt(dx * dx + dy * dy);
		}

		public static Rocket[] CreateMatingPool(Rocket[] population) {
			List<Rocket> matingpool = new List<Rocket>();
			double[] fitnesses = new double[population.Length];

			//tutaj jest proces sortowania tablicy z rakietami
			for (int i = 0; i < population.Length; i++)
				fitnesses[i] = population[i].fitness;

			Array.Sort(fitnesses, population);  //sortowanie rosnąco
			Array.Reverse(population);  //odwrócenie tablicy, żeby była posortowana malejąco

			//za Danielem Shiffmanem, ale bierzemy tylko połowę najlepszych
			double sumOfFs = 0;
			for(int i = 0; i < population.Length / 2; i++) 
				sumOfFs += population[i].fitness;
			
			for (int i = 0; i < population.Length / 2; i++) {
				double n = Math.Floor(population[i].fitness * 100 / sumOfFs); // liczba rakiet w polu rozrodczym reprezentujace population[i]
				for (int j = 0; j < n; j++)     //dodajemy rakietę o danym fitnessie odpowiednią ilość razy
					matingpool.Add(population[i]);
			}
			return matingpool.ToArray();
		}
	}
}

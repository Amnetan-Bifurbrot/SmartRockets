using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SmartRockets {
	class Rocket : Image {
		public static int lifetime = 100;
		public double fitness = -1;
		public Point pos;
		public Vector vel;
		public Vector[] genes;
		string workingDirectory = Environment.CurrentDirectory;
		public bool crashed = false;

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
			Source = new BitmapImage(new Uri(Directory.GetParent(workingDirectory).Parent.FullName + @"\Resources\rocket.png", UriKind.Relative));

			//Source = new BitmapImage(new Uri("/rocket.png", UriKind.Relative));
			Width = 20;
			Height = 90;
			genes = new Vector[lifetime];
			for (int i = 0; i < lifetime; i++) {
				int step = 4;
				angle = r.Next(0 / step, 360 / step) * step * Math.PI / 5; //losujemy geny do chromosomu (kąty od 0 do 2pi) - z dokladnoscia do stepu
				genes[i] = new Vector(1 * Math.Sin(angle), 1 * Math.Cos(angle)); // odlegosc w pixelach jaka pokona w jedym kroku czasowym rakieta dla bezwzglednych
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
					int step = 4;
					angle = r.Next(0 / step, 360 / step) * step * Math.PI / 180;
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

		public void Fitness(Point target, double targetWidth, double targetHeight) {
			//zwracamy 1/odległość do targetu

			double dx = pos.X - (target.X + targetWidth / 2), dy = pos.Y - (target.Y + targetHeight / 2);
			
			fitness = 1.0 / Math.Sqrt(dx * dx + dy * dy);

			// jak zderzony z przeszkoda to zmniejszamy fitness
			if (crashed)
				fitness = fitness / 10;
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
			double sumOfFs = 0; // suma wszystkich fitnessuf braych pod uwage - polowy 
			for(int i = 0; i < population.Length / 2; i++) 
				sumOfFs += population[i].fitness;
			// od polowy najlepszych
			for (int i = 0; i < population.Length / 2; i++) {
				// procent wylosowania danego osobnika
				double n = Math.Floor(population[i].fitness * 100 / sumOfFs); //procentowa zawartosc rakiet w polu rozrodczym reprezentujace population[i]
				for (int j = 0; j < n; j++)     //dodajemy rakietę o danym fitnessie odpowiednią ilość razy
					matingpool.Add(population[i]);
			}
			// im wiekszy procent, tym wiecej osobnikuf
			return matingpool.ToArray();
		}
	}
}

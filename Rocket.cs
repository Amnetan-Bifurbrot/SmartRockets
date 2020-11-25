using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SmartRockets {
	class Rocket : Image {
		int lifetime = 200;
		double fitness = -1;
		public Point pos;
		public Vector[] genes;


		public Rocket() {
			double angle;
			//obrazek rakiety
			Source = new BitmapImage(new Uri("/rocket.png", UriKind.Relative));
			Width = 20;
			Height = 90;

			Random r = new Random();
			for (int i = 0; i < lifetime; i++) {
				angle = r.NextDouble() * 2 * Math.PI;   //losujemy geny do chromosomu (kąty od 0 do 2pi)
				genes[i] = new Vector(Math.Sin(angle), Math.Cos(angle));
			}
			//jeszcze trzeba odpowiednio obrócić rakietę jakimś Transformem :) ale to pewnie już w dispatcherTimer_Tick()
		}

		//rozmnażanie
		public void Cross(Rocket parentA, Rocket parentB) {
			int midPoint = new Random().Next(lifetime); //losujemy punkt, w którym przedzielimy chromosom
			for (int i = 0; i < lifetime; i++) {
				if (i < midPoint)
					genes[i] = parentA.genes[i];
				else
					genes[i] = parentB.genes[i];
			}
		}

		public void Mutate(double chance) {
			if (new Random().NextDouble() < chance) {
				//tutaj będzie mutowanie
			}
		}
		public Rocket Procreate(Rocket[] matingpool, double mutationChance) {
			Rocket child = new Rocket(), parentA, parentB;
			int matingpoolSize = matingpool.Length;
			//wybór rodziców z matingpool (trzeba dodać zabezpieczenie, żeby się nie losował ten sam rodzic 2 razy)
			parentA = matingpool[new Random().Next(matingpoolSize)];
			parentB = matingpool[new Random().Next(matingpoolSize)];

			//krzyżowanie
			child.Cross(parentA, parentB);
			//mutacja
			child.Mutate(mutationChance);

			return child;
		}

		public void Fitness(Point target) {
			//zwracamy 1/odległość do targetu
			double dx = pos.X - target.X, dy = pos.Y - target.Y;
			fitness = 1.0 / Math.Sqrt(dx * dx + dy * dy);
		}

		public static Rocket[] CreateMatingPool(Rocket[] population) {
			List<Rocket> matingpool = new List<Rocket>();
			//za Danielem Shiffmanem
			for(int i = 0; i < population.Length; i++) {
				double n = Math.Floor(population[i].fitness * 100);
				for (int j = 0; j < n; j++)		//dodajemy rakietę o danym fitnessie odpowiednią ilość razy
					matingpool.Add(population[i]);
			}
			return matingpool.ToArray();
		}
	}
}

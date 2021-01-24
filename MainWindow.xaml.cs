using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartRockets {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		System.Windows.Threading.DispatcherTimer dispatcherTimer;
		double width = 450, height = 430, obstacleX, obstacleY;
		int pop_size = 25; // ilosc rakiet
		Vector[] velWhenCrashed;    //tablica do przechowywania informacji o prędkości rakiety po zderzeniu z przeszkodą; kwestia estetyczna
		Vector[] velWhenOutOfFuel;  //tablica do przechowywania informacji o prędkości rakiety po skończeniu się paliwa; kwestia estetyczna
		Vector velDifference;       //wielkość do przechowywania informacji o różnicy wektora prędkości
		Rocket[] rockets; // docelowo bedzie duze N, wiec tablica bedzie szybsza niz lista  
		Point target = new Point(450 / 2 - 10, 90);
		int generation = 1;
		Random r = new Random();
		Image target_img = new Image();
		double precisionToStop = 20, x_coord = 50, y_coord = 250, obstacleWidth = 200, obstacleHeight = 10; // x_coor i y_coord to lewy gorny rog przeszkody
		bool showLastRocket = false;
		int bestRocketIndex = 0;


		public MainWindow() {
			InitializeComponent();

			dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			dispatcherTimer.Tick += dispatcherTimer_Tick;
			dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10);

			obstacleX = width / 2 - 100;
			obstacleY = height / 2 - 55;


			SetTarget();
			SetObstacle(x_coord, y_coord, obstacleWidth, obstacleHeight);

			rockets = new Rocket[pop_size];
			velWhenCrashed = new Vector[pop_size];
			velWhenOutOfFuel = new Vector[pop_size];
		}

		private void StartBtn_Click(object sender, RoutedEventArgs e) {
			dispatcherTimer.Start();
		}

		private void StopBtn_Click(object sender, RoutedEventArgs e) {
			dispatcherTimer.Stop();
		}

		int counter = 0; // liczy ilosc tikuf

		private void dispatcherTimer_Tick(object sender, EventArgs e) {
			// rysowanie rakiet
			generationLabel.Content = "Generation: " + generation;
			if (counter == 0 && generation == 1) {
				// 1. generacja rakiet
				if (!showLastRocket) {
					for (int i = 0; i < pop_size; i++) {
						rockets[i] = new Rocket(r);  //gotowa rakieta z genami
						rockets[i].pos = new Point(canvas.Width / 2 - 10, canvas.Height - 90);

						canvas.Children.Add(rockets[i]);

						Canvas.SetLeft(rockets[i], rockets[i].pos.X);
						Canvas.SetTop(rockets[i], rockets[i].pos.Y);

					}
				} else { // jezeli rysujemy najlepsza rakiete

					canvas.Children.Clear();
					SetTarget(); // bo clearowaie usuwa cel
					SetObstacle(x_coord, y_coord, obstacleWidth, obstacleHeight);

					rockets[bestRocketIndex].pos = new Point(canvas.Width / 2 - 10, canvas.Height - 90);

					canvas.Children.Add(rockets[bestRocketIndex]);
					Canvas.SetLeft(rockets[bestRocketIndex], rockets[bestRocketIndex].pos.X);
					Canvas.SetTop(rockets[bestRocketIndex], rockets[bestRocketIndex].pos.Y);
				}
			}

			if (counter < Rocket.lifetime) {
				canvas.Children.Clear();
				SetTarget(); // bo clearowaie usuwa cel
				SetObstacle(x_coord, y_coord, obstacleWidth, obstacleHeight);

				if (!showLastRocket) {
					for (int j = 0; j < pop_size; j++) {
						// jezeli walnie w przeszkode to nie dodajemy do predkosci
						// sprawdzenie czy walniemy w przeszkode
						if (rockets[j].pos.X > x_coord && rockets[j].pos.X < (x_coord + obstacleWidth) && rockets[j].pos.Y > y_coord && rockets[j].pos.Y < (y_coord + obstacleHeight) && !rockets[j].crashed) {
							//MessageBox.Show("Crashed!");
							rockets[j].crashed = true;
							velWhenCrashed[j] = rockets[j].vel; //zapisujemy wektor prędkości, z którą rakieta walnęła w kupę, żeby potem narysować, że faktycznie walnęła :)
						}

						if (rockets[j].crashed != true) {
							//skonczylo sie paliwo?
							if (rockets[j].outOfFuel) {
								rockets[j].pos += velWhenOutOfFuel[j];  //leci dalej z prędkością, przy której skończyło się paliwo
							} else {
								if (counter == 0)   //bo pierwsza zmiana wektora prędkości to pierwszy wektor prędkości
									velDifference = rockets[j].genes[counter];
								else
									velDifference = rockets[j].genes[counter] - rockets[j].genes[counter - 1];
								rockets[j].vel += rockets[j].genes[counter];        // dodajemy do aktualnej predkosci to co jest w tablicy z genami w danym kroku czasowym
								rockets[j].pos += rockets[j].vel;   // dodajemy do aktualnej pozycji aktualną wartość prędkości
								rockets[j].fuel -= velDifference.Length;
								//jeśli skończy się paliwo, to oznaczamy rakietę, że skończyło jej się paliwo i będzie sobie dalej lecieć bezwładnie (patrz około 9 linijek wcześniej)
								if (rockets[j].fuel < 0) {
									rockets[j].outOfFuel = true;
									velWhenOutOfFuel[j] = rockets[j].vel;
								}
							}
						} else {
							rockets[j].Fitness(target); // troche nie ciaua szyyypko jakbym chciaua
						}


						Rectangle rect = new Rectangle();
						rect.Width = 8;
						rect.Height = 20;

						rect.Fill = new ImageBrush {
							ImageSource = rockets[j].Source
						};

						// obrot rakiety o odpowiedni kat
						if (!rockets[j].crashed) {
							double angle = Math.Atan2(rockets[j].vel.Y, rockets[j].vel.X);
							rect.RenderTransform = new RotateTransform((angle * 180 / Math.PI + 90));
						} else {
							double angle = Math.Atan2(velWhenCrashed[j].Y, velWhenCrashed[j].X);
							rect.RenderTransform = new RotateTransform((angle * 180 / Math.PI + 90));
						}
						canvas.Children.Add(rect);

						Canvas.SetLeft(rect, rockets[j].pos.X);
						Canvas.SetTop(rect, rockets[j].pos.Y);

					}

				} else {
					// jezeli rysujemy najlepsza rakiete
					//skopiowałem to co dopisałem w if(!showLastRocket)
					//tak ogólnie to gdyby było więcej czasu to należałoby solidnie posprzątać w tym kodzie, bo wszystko jest wszędzie
					if (rockets[bestRocketIndex].outOfFuel)
						rockets[bestRocketIndex].pos += velWhenOutOfFuel[bestRocketIndex];
					else {
						if (rockets[bestRocketIndex].outOfFuel) {
							rockets[bestRocketIndex].pos += velWhenOutOfFuel[bestRocketIndex];
						} else {
							if (counter == 0)
								velDifference = rockets[bestRocketIndex].genes[counter];
							else
								velDifference = rockets[bestRocketIndex].genes[counter] - rockets[bestRocketIndex].genes[counter - 1];

							rockets[bestRocketIndex].vel += rockets[bestRocketIndex].genes[counter];        // dodajemy do aktualnej predkosci to co jest w tablicy z genami w danym kroku czasowym
							rockets[bestRocketIndex].pos += rockets[bestRocketIndex].vel;
							if (rockets[bestRocketIndex].fuel < 0)
								rockets[bestRocketIndex].outOfFuel = true;
						}
					}
					Rectangle rect = new Rectangle();
					rect.Width = 8;
					rect.Height = 20;

					rect.Fill = new ImageBrush {
						ImageSource = rockets[bestRocketIndex].Source
					};

					// obrot rakiety o odpowiedni kat
					double angle = Math.Atan2(rockets[bestRocketIndex].vel.Y, rockets[bestRocketIndex].vel.X);
					rect.RenderTransform = new RotateTransform((angle * 180 / Math.PI + 90));

					canvas.Children.Add(rect);

					Canvas.SetLeft(rect, rockets[bestRocketIndex].pos.X);
					Canvas.SetTop(rect, rockets[bestRocketIndex].pos.Y);
				}
				counter++;

			} else {
				if (showLastRocket) {
					dispatcherTimer.Stop();
				}
				// wyliczamy najkrotszy distance do celu sposrod calej generacji
				double dist = 400;

				for (int i = 0; i < pop_size; i++) {
					double dx = rockets[i].pos.X - target.X - target_img.Height / 2, dy = rockets[i].pos.Y - target.Y - target_img.Width;
					double new_dist = Math.Sqrt(dx * dx + dy * dy);
					if (dist > new_dist) {
						dist = new_dist;
						bestRocketIndex = i;
					}
				}
				distanceLabel.Content = "Best distance: " + string.Format("{0:N5}", dist);

				// stopujemy symulacje jezeli rakieta z pewna precyzja dobiegla do celu
				if (dist < precisionToStop) {

					const string message = "Target has beeen reached. \n Do you want to see the best rocket?";
					const string caption = "Success";
					var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

					if (result == MessageBoxResult.No) {
						dispatcherTimer.Stop();
					} else {
						rockets[bestRocketIndex].outOfFuel = rockets[bestRocketIndex].crashed = false;
						showLastRocket = true; // rysujemy poejdynczy ruch rakiety tej najlepszej
					}
				}

				counter = 0; // konczymy zabawe
				double maxFit = 0;
				for (int i = 0; i < pop_size; i++) { // szukanie maxFita sposrod wszystkich rakiet
					rockets[i].Fitness(target);
					if (maxFit < rockets[i].fitness)
						maxFit = rockets[i].fitness;
					Console.WriteLine("Rakieta " + i + ": " + rockets[i].fitness);
				}

				// basen do rosmnaszanja cieciuf
				Rocket[] matingPool = Rocket.CreateMatingPool(rockets);
				Console.WriteLine(matingPool.Length);
				for (int i = 0; i < pop_size; i++) {
					// kazdom rakiete zastempujemy nofom ragjedom
					rockets[i] = Rocket.Procreate(matingPool, 0.001, r);
					rockets[i].pos = new Point(canvas.Width / 2 - 10, canvas.Height - 90); // pozycja poczatkowa
				}
				generation++;

				maxFitLabel.Content = "Max fitness: " + string.Format("{0:N5}", maxFit);
			}

		}

		private void SetObstacle(double x_coord, double y_coord, double width, double height) { // tworzy przeszkode do omijania
			Rectangle rect;
			rect = new Rectangle();

			rect.Stroke = new SolidColorBrush(Colors.Black);
			rect.Fill = new SolidColorBrush(Colors.Black);
			rect.Width = width;
			rect.Height = height;
			Canvas.SetLeft(rect, x_coord);
			Canvas.SetTop(rect, y_coord);
			canvas.Children.Add(rect);
		}

		private void SetTarget() {
			target_img.Source = new BitmapImage(new Uri("/Resources/target.png", UriKind.Relative));
			target_img.Width = 30;
			target_img.Height = 30;
			//ustawiam target tak jak poniżej, żeby potem zrekompensować w funkcji Fitness()
			Canvas.SetLeft(target_img, target.X);
			Canvas.SetTop(target_img, target.Y);
			canvas.Children.Add(target_img);

		}
	}
}

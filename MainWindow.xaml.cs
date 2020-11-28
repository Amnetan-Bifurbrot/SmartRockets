using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SmartRockets {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        double width = 450, height = 430, obstacleX, obstacleY;
        int pop_size = 10; // ilosc rakiet
        Rocket[] rockets; // docelowo bedzie duze N, wiec tablica bedzie szybsza niz lista  
        Point target = new Point(450 / 2 - 10, 90);
        int generation = 1;
        Random r = new Random();

        public MainWindow() {
            InitializeComponent();

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(5);

            obstacleX = width / 2 - 100;
            obstacleY = height / 2 - 55;

            //SetObstacle(obstacleX, obstacleY, 200, 10);

            rockets = new Rocket[pop_size];
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e) {
            dispatcherTimer.Start();

            // generacja rakiet
            for (int i = 0; i < pop_size; i++) {
                //Thread.Sleep(20);
                rockets[i] = new Rocket(r);  //gotowa rakieta z genami
                rockets[i].pos = new Point(canvas.Width / 2 - 10, canvas.Height - 90);

                canvas.Children.Add(rockets[i]);

                Canvas.SetLeft(rockets[i], rockets[i].pos.X);
                Canvas.SetTop(rockets[i], rockets[i].pos.Y);
                
            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e) {
            dispatcherTimer.Stop();
            //canvas.Children.Clear();
            //SetObstacle(obstacleX, obstacleY, 200, 10);
        }

        int counter = 0;

        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            // rysowanie rakiet
            generationLabel.Content = "Generation: " + generation;
            if (counter < Rocket.lifetime) {
                canvas.Children.Clear();
                //SetObstacle(obstacleX,obstacleY,200,10);
                for (int j = 0; j < pop_size; j++) {
                    rockets[j].pos.X += rockets[j].genes[counter].X;
                    rockets[j].pos.Y += rockets[j].genes[counter].Y;
                    canvas.Children.Add(rockets[j]);
                    Canvas.SetLeft(rockets[j], rockets[j].pos.X);
                    Canvas.SetTop(rockets[j], rockets[j].pos.Y);
                }
                counter++;
			} else {
                counter = 0;
                double maxFit = 0;
                for(int i = 0; i < pop_size; i++) {
                    rockets[i].Fitness(target);
                    if (maxFit < rockets[i].fitness)
                        maxFit = rockets[i].fitness;
					Console.WriteLine("Rakieta " + i + ": " + rockets[i].fitness);
				}
                //dispatcherTimer.Stop();
                Rocket[] matingPool = Rocket.CreateMatingPool(rockets);
				Console.WriteLine(matingPool.Length);
                for(int i = 0; i < pop_size; i++) {
                    //Thread.Sleep(20);
                    rockets[i] = Rocket.Procreate(matingPool, 0.001, r);
                    rockets[i].pos = new Point(canvas.Width / 2 - 10, canvas.Height - 90);
				}
                generation++;
                maxFitLabel.Content = "Max fitness: " + maxFit;
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
    }
}

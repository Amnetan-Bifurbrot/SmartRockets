using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartRockets {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        double width = 450, height = 430, obstacleX, obstacleY;
        int pop_size = 5; // ilosc rakiet
        int time_max = 200; // maksymalny czas ruchu rakiety
        Image[] rockets; // docelowo bedzie duze N, wiec tablica bedzie szybsza niz lista  
        double[][] x_coords, y_coords; // tablice lewego gornego rogu imiga w kojenych przedzialach czasowych // row x col = time_max x rocket
        double[] angle; // kat do kierunku ruchu rakiety
        public MainWindow() {
            InitializeComponent();

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10);

            obstacleX = width / 2 - 100;
            obstacleY = height / 2 - 55;

            SetObstacke(obstacleX, obstacleY, 200, 10);

            rockets = new Image[pop_size];
            x_coords = new double[time_max][];
            y_coords = new double[time_max][];
            angle = new double[pop_size];

            for(int i = 0; i < time_max; i++) {
                x_coords[i] = new double[pop_size];
                y_coords[i] = new double[pop_size];
            }
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e) {
            dispatcherTimer.Start();

            Random random = new Random();
            // generacja rakiet
            for (int i = 0; i < pop_size; i++) {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("/rocket.png", UriKind.Relative));
                img.Width = 20;
                img.Height = 90;
                rockets[i] = img;
               
                canvas.Children.Add(rockets[i]);

                double min = 0.0;
                double max = 180.0;

                
                angle[i] = random.NextDouble() * (max - min) + min;
                
                x_coords[0][i] = canvas.ActualWidth / 2 - img.Width / 2; // [t = 0][numer rakiety]
                y_coords[0][i] = canvas.ActualHeight - img.Height;

                x_coords[0][i] = x_coords[0][i] * Math.Sin(angle[i]); // pewnie mozna inaczej, pewno tablica 1D i ja modyfikowac w kazdym okresie czasowym
                y_coords[0][i] = y_coords[0][i] * Math.Cos(angle[i]); // jak wyzej


                Canvas.SetLeft(img, canvas.ActualWidth / 2 - img.Width / 2);
                Canvas.SetTop(img, canvas.ActualHeight - img.Height);

            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e) {
            dispatcherTimer.Stop();
            canvas.Children.Clear();
            SetObstacke(obstacleX, obstacleY, 200, 10);

        }
        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            
            // rysowanie rakiet
            for(int i = 0; i < pop_size; i++) {
                x_coords[0][i] -= 2;
                y_coords[0][i] -= 2;
                
                Canvas.SetLeft(rockets[i], x_coords[0][i]);
                Canvas.SetTop(rockets[i], y_coords[0][i]);
            }
        }

        private void SetObstacke(double x_coord, double y_coord, double width, double height) { // tworzy przeszkode do omijania
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

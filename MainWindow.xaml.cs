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
        Rocket[] rockets; // docelowo bedzie duze N, wiec tablica bedzie szybsza niz lista  
 
        public MainWindow() {
            InitializeComponent();

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10);

            obstacleX = width / 2 - 100;
            obstacleY = height / 2 - 55;

            SetObstacke(obstacleX, obstacleY, 200, 10);

            rockets = new Rocket[pop_size];
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e) {
            dispatcherTimer.Start();

            Random random = new Random();
            // generacja rakiet
            for (int i = 0; i < pop_size; i++) {
                rockets[i] = new Rocket();  //gotowa rakieta z genami
                rockets[i].pos = new Point(canvas.Width / 2 - 10, canvas.Height - 90);

                canvas.Children.Add(rockets[i]);

                Canvas.SetLeft(rockets[i], rockets[i].pos.X);
                Canvas.SetTop(rockets[i], rockets[i].pos.Y);

            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e) {
            dispatcherTimer.Stop();
            canvas.Children.Clear();
            SetObstacke(obstacleX, obstacleY, 200, 10);

        }
        private void dispatcherTimer_Tick(object sender, EventArgs e) {           
            // rysowanie rakiet

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

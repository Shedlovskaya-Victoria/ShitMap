using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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

namespace ShitMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Shape> controls = new List<Shape>();
        Timer timer;
        public MainWindow()
        {
            InitializeComponent();
            timer = new Timer(TimerRun, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }
        Random random = new Random();
        private void TimerRun(object? state)
        {
            var data = MethodFromAPI();
            // удаляем прошлые кружочки
            Dispatcher.Invoke(() =>
            {
                foreach (var control in controls)
                    map.Children.Remove(control);

                foreach (var user in data)
                {
                    if (user.LastSecurityPointDirection == "out")
                        continue;
                    var border = FindName($"b{user.LastSecurityPointNumber}") as Border;
                    int width = (int)border.Width;
                    int height = (int)border.Height;
                    int left = (int)Canvas.GetLeft(border);
                    int top = (int)Canvas.GetTop(border);
                    if (top < 0)
                        top = 0;
                    Ellipse ellipse = new Ellipse();
                    ellipse.Fill = user.PersonRole == 0 ? Brushes.Blue : Brushes.Green;
                    ellipse.Width = 10;
                    ellipse.Height = 10;
                    map.Children.Add(ellipse);

                    double newTop = 0, newLeft = 0;
                    int tryCunt = 50;
                    do
                    {
                        tryCunt--;
                        newTop = random.Next(top + 10, top + height - 10);
                        newLeft = random.Next(left + 10, left + width - 10);
                        Canvas.SetLeft(ellipse, newLeft);
                        Canvas.SetTop(ellipse, newTop);
                        Canvas.SetZIndex(ellipse, 100);
                        if (tryCunt == 0)
                            throw new Exception("НЕЕТ");
                    }
                    while (tryCunt > 0 && TestIntersect(ellipse, controls));
                    controls.Add(ellipse);
                }
            });
        }

        private bool TestIntersect(Ellipse ellipse, List<Shape> controls)
        {
            double left1 = (int)Canvas.GetLeft(ellipse) + 5;
            double top1 = (int)Canvas.GetTop(ellipse) + 5;
            double l = 0;

            foreach (Shape control in controls)
            {
                double left2 = (int)Canvas.GetLeft(control) + 5;
                double top2 = (int)Canvas.GetTop(control) + 5;

                if (top2 == top1 || left1 == left2)
                    return true;
                l = Math.Sqrt(Math.Pow(left2 - left1, 2) + Math.Pow(top2 - top1, 2));
                if (l <= 0)
                    throw new Exception("Какого хрена");
                if (l < 15)
                    return true;
            }
            return false;
        }

        List<PersonLocation> MethodFromAPI()
        {
            List<PersonLocation> result = new List<PersonLocation>(10);
            for (int i = 0; i < 20; i++)
            {
                PersonLocation personLocation = new PersonLocation
                {
                    PersonRole = random.Next(0, 2),
                    LastSecurityPointNumber = random.Next(0, 23),
                    LastSecurityPointDirection = "in"
                };
                result.Add(personLocation);
            }
            return result;

        }

    }

    class PersonLocation
    {
        public int PersonCode { get; set; }
        public int PersonRole { get; set; }
        public int LastSecurityPointNumber { get; set; }
        public string LastSecurityPointDirection { get; set; }
        public DateTime LastSecurityPointTime { get; set; }
    }
}

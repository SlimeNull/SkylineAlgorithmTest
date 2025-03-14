using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SkylineAlgorithmTest;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private RectSpace rectSpace = new RectSpace();

    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
    }

    public int RectMinWidth { get; set; } = 50;
    public int RectMaxWidth { get; set; } = 100;
    public int RectMinHeight { get; set; } = 50;
    public int RectMaxHeight { get; set; } = 100;

    private void UpdateSkylines()
    {
        skylinesCanvas.Children.Clear();

        foreach (var horizontalSkyline in rectSpace.HorizontalSkylines)
        {
            var newRect = new Border()
            {
                Width = horizontalSkyline.Length,
                Height = 2,
                Background = Brushes.Purple,
                Opacity = 0.5,
            };

            Canvas.SetLeft(newRect, horizontalSkyline.Start);
            Canvas.SetTop(newRect, horizontalSkyline.Height);

            skylinesCanvas.Children.Add(newRect);
        }

        foreach (var verticalSkyline in rectSpace.VerticalSkylines)
        {
            var newRect = new Border()
            {
                Width = 2,
                Height = verticalSkyline.Length,
                Background = Brushes.Purple,
                Opacity = 0.5,
            };

            Canvas.SetLeft(newRect, verticalSkyline.Height);
            Canvas.SetTop(newRect, verticalSkyline.Start);

            skylinesCanvas.Children.Add(newRect);
        }
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        int width = Random.Shared.Next(RectMinWidth, RectMaxWidth);
        int height = Random.Shared.Next(RectMinHeight, RectMaxHeight);

        rectSpace.Layout(width, height, out var x, out var y);
        var newRect = new Border()
        {
            Width = width,
            Height = height,
            BorderThickness = new Thickness(2),
            BorderBrush = Brushes.Pink,
        };

        Canvas.SetLeft(newRect, x);
        Canvas.SetTop(newRect, y);

        testCanvas.Children.Add(newRect);

        UpdateSkylines();
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        testCanvas.Children.Clear();
        rectSpace.Reset();

        UpdateSkylines();
    }
}
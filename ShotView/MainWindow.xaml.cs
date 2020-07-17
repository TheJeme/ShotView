using System.Windows;
using System.Windows.Input;

namespace ShotView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            imageView = new ImageView(this, MainImage);

            isFullScreen = false;
        }

        private bool isFullScreen;

        ImageView imageView;

        private void ToggleFullScreen(KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                if (!isFullScreen)
                {
                    WindowState = WindowState.Maximized;
                    WindowStyle = WindowStyle.None;
                }
                else
                {
                    WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.SingleBorderWindow;
                }
                isFullScreen = !isFullScreen;
            }
        }

        private void OpenImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageView.OpenImage();         
        }
    

        private void Slideshow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }


        private void Delete_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageView.DeleteImage();
        }

        private void RotateLeft_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageView.RotateImage(-90);
        }

        private void RotateRight_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageView.RotateImage(90);
        }

        private void FlipHorizontal_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageView.FlipImage("horizontal");
        }

        private void FlipVertical_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageView.FlipImage("vertical");
        }

        private void Zoom_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ZoomBorder.Visibility == Visibility.Hidden)
                ZoomBorder.Visibility = Visibility.Visible;
            else
                ZoomBorder.Visibility = Visibility.Hidden;
        }

        private void Stretch_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageView.StretchImage();
        }

        private void Previous_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageView.PreviousImage();
        }

        private void Next_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageView.NextImage();
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (imageView == null) return;

            imageView.Zoom(sender);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            imageView.Resize();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            ToggleFullScreen(e);
        }
    }
}

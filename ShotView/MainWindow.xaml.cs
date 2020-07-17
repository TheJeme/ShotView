using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

            isStretchMode = false;

            imageRotationValue = 0;

            imageScaleX = 1;
            imageScaleY = 1;

            imagesInFolder = new List<string>();
        }

        private bool isStretchMode;

        private double imageRotationValue;
        private double imageScaleX;
        private double imageScaleY;

        private string filePath;
        private string folderPath;

        private List<string> imagesInFolder;
        private int indexOfImageFolder;

        private ImageSource currentImageSource;

        public static ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        private double ClampDimensions(double value, double min, double max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        private void OpenImage()
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

            Nullable<bool> result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                filePath = dlg.FileName;
                folderPath = Path.GetDirectoryName(filePath);

                currentImageSource = BitmapFromUri(new Uri(filePath));
                MainImage.Source = currentImageSource;

                MainImage.Width = ClampDimensions(currentImageSource.Width, 1, this.ActualWidth);
                MainImage.Height = ClampDimensions(currentImageSource.Height, 1, this.ActualHeight - 40);

                string[] extensions = { ".jpg", ".jpeg", ".bmp", ".jpe", ".jfif", ".png" };


                foreach (var file in Directory.GetFiles(folderPath))
                {
                    string extension = file.Substring(file.LastIndexOf('.'));

                    if (extensions.Any(x => extension.ToLower().Contains(x)))
                    {
                        imagesInFolder.Add(file);
                    }
                }
                if (File.Exists(filePath))
                {
                    indexOfImageFolder = imagesInFolder.IndexOf(filePath);
                }
            }
        }

        private void FlipImage(string direction)
        {
            if (direction == "horizontal")
            {
                imageScaleX *= -1;
                ScaleTransform scaleTransform = new ScaleTransform(imageScaleX, imageScaleY);
                MainImage.RenderTransform = scaleTransform;
            }
            else if (direction == "vertical")
            {
                imageScaleY *= -1;
                ScaleTransform scaleTransform = new ScaleTransform(imageScaleX, imageScaleY);
                MainImage.RenderTransform = scaleTransform;
            }
        }

        private void RotateImage(int degrees)
        {
            imageRotationValue = imageRotationValue + degrees;
            RotateTransform rotateTransform = new RotateTransform(imageRotationValue);
            MainImage.RenderTransform = rotateTransform;
        }

        private void OpenImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenImage();         
        }
    

        private void Slideshow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }


        private void Delete_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (imagesInFolder.Count == 0) return;

            MessageBoxResult result = MessageBox.Show("Do you want to delete this image?", "Confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No || result == MessageBoxResult.None) return;

            MainImage.Source = null;
            MainImage.Width = Double.NaN;
            MainImage.Height = Double.NaN;

            imagesInFolder.Remove(filePath);
            File.Delete(filePath);

            if (indexOfImageFolder != 0 && imagesInFolder.Count > 1)
            {
                indexOfImageFolder--;
                filePath = imagesInFolder[indexOfImageFolder];
                currentImageSource = BitmapFromUri(new Uri(filePath));
                MainImage.Source = currentImageSource;

                MainImage.Width = ClampDimensions(currentImageSource.Width, 1, this.ActualWidth);
                MainImage.Height = ClampDimensions(currentImageSource.Height, 1, this.ActualHeight - 40);
            }
            else if (indexOfImageFolder != imagesInFolder.Count - 1 && imagesInFolder.Count > 1)
            {
                indexOfImageFolder++;
                filePath = imagesInFolder[indexOfImageFolder];
                currentImageSource = BitmapFromUri(new Uri(filePath));
                MainImage.Source = currentImageSource;

                MainImage.Width = ClampDimensions(currentImageSource.Width, 1, this.ActualWidth);
                MainImage.Height = ClampDimensions(currentImageSource.Height, 1, this.ActualHeight - 40);
            }

        }

        private void RotateLeft_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RotateImage(-90);
        }

        private void RotateRight_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RotateImage(90);
        }

        private void FlipHorizontal_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FlipImage("horizontal");
        }

        private void FlipVertical_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FlipImage("vertical");
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
            isStretchMode = !isStretchMode;

            if (isStretchMode)
            {
                MainImage.Width = Double.NaN;
                MainImage.Height = Double.NaN;
            }
            else
            {
                MainImage.Width = 200;
                MainImage.Height = 200;
            }
        }

        private void Previous_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (indexOfImageFolder == 0) return;

            indexOfImageFolder--;
            filePath = imagesInFolder[indexOfImageFolder];
            currentImageSource = BitmapFromUri(new Uri(filePath));
            MainImage.Source = currentImageSource;

            MainImage.Width = ClampDimensions(currentImageSource.Width, 1, this.ActualWidth);
            MainImage.Height = ClampDimensions(currentImageSource.Height, 1, this.ActualHeight - 40);
        }

        private void Next_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (indexOfImageFolder == imagesInFolder.Count - 1 && imagesInFolder.Count > 0) return;

            indexOfImageFolder++;
            filePath = imagesInFolder[indexOfImageFolder];
            currentImageSource = BitmapFromUri(new Uri(filePath));
            MainImage.Source = currentImageSource;

            MainImage.Width = ClampDimensions(currentImageSource.Width, 1, this.ActualWidth);
            MainImage.Height = ClampDimensions(currentImageSource.Height, 1, this.ActualHeight - 40);
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;

            imageScaleX = slider.Value;
            imageScaleY = slider.Value;

            ScaleTransform scaleTransform = new ScaleTransform(imageScaleX, imageScaleY);
            MainImage.RenderTransform = scaleTransform;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (currentImageSource == null) return;

            MainImage.Width = ClampDimensions(currentImageSource.Width, 1, this.ActualWidth);
            MainImage.Height = ClampDimensions(currentImageSource.Height, 1, this.ActualHeight - 40);
        }
    }
}

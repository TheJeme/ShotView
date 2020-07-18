using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShotView
{
    public class ImageView
    {

        private double imageRotationValue;
        private double imageScaleX;
        private double imageScaleY;

        private bool isStretchMode;

        private string filePath;
        private string folderPath;

        private List<string> imagesInFolder;
        private int indexOfImageFolder;

        private ImageSource currentImageSource;

        private Window mainWindow;
        private Image mainImage;

        public ImageView(Window mainWindow, Image mainImage)
        {
            this.mainWindow = mainWindow;
            this.mainImage = mainImage;

            isStretchMode = false;

            imageRotationValue = 0;

            imageScaleX = 1;
            imageScaleY = 1;

            imagesInFolder = new List<string>();
        }

        private static ImageSource BitmapFromUri(Uri source)
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

        public bool getImageExists()
        {
            return File.Exists(filePath);
        }

        public string GetImageDimensions()
        {
            BitmapImage img = new BitmapImage(new Uri(filePath));
            return img.PixelWidth + "x" + img.PixelHeight;
        }

        public string GetFolderPath()
        {
            return folderPath;
        }

        public string GetFileName()
        {
            return Path.GetFileName(filePath);
        }

        public string GetFileSize()
        {
            return GetFileSize(new FileInfo(filePath).Length);
        }

        
        public static string GetFileSize(double fileLength)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (fileLength >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                fileLength = fileLength / 1024;
            }
            string result = String.Format("{0:0.##} {1}", fileLength, sizes[order]);
            return result;
        }

        public void StretchImage()
        {
            if (currentImageSource == null) return;

            isStretchMode = !isStretchMode;

            Resize();
        }

        public void Zoom(object sender)
        {
            var slider = sender as Slider;

            if (imageScaleX >= 0)
            {
                imageScaleX = slider.Value;
            }
            else
            {
                imageScaleX = slider.Value * -1;
            }

            if (imageScaleY >= 0)
            {
                imageScaleY = slider.Value;
            }
            else
            {
                imageScaleY = slider.Value * -1;
            }


            ScaleTransform scaleTransform = new ScaleTransform(imageScaleX, imageScaleY);
            mainImage.RenderTransform = scaleTransform;
        }

        public void OpenImageFileDialog()
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

            Nullable<bool> result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                OpenImage(dlg.FileName);
            }
        }

        public void OpenImage(string _filePath)
        {
            filePath = _filePath;

            currentImageSource = BitmapFromUri(new Uri(filePath));
            mainImage.Source = currentImageSource;

            Resize();
            addImagesToList(filePath);
        }

        public void addImagesToList(string filePath)
        {
            folderPath = Path.GetDirectoryName(filePath);

            string[] extensions = { ".jpg", ".jpeg", ".bmp", ".jpe", ".jfif", ".png" };

            imagesInFolder.Clear();
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

        public void FlipImage(string direction)
        {
            if (direction == "horizontal")
            {
                imageScaleX *= -1;
                ScaleTransform scaleTransform = new ScaleTransform(imageScaleX, imageScaleY);
                mainImage.RenderTransform = scaleTransform;
            }
            else if (direction == "vertical")
            {
                imageScaleY *= -1;
                ScaleTransform scaleTransform = new ScaleTransform(imageScaleX, imageScaleY);
                mainImage.RenderTransform = scaleTransform;
            }
        }

        public void RotateImage(int degrees)
        {
            imageRotationValue = imageRotationValue + degrees;
            RotateTransform rotateTransform = new RotateTransform(imageRotationValue);
            mainImage.RenderTransform = rotateTransform;
        }

        public void NextImage()
        {
            if (indexOfImageFolder == imagesInFolder.Count - 1 || imagesInFolder.Count == 0) return;

            indexOfImageFolder++;
            filePath = imagesInFolder[indexOfImageFolder];

            if (!File.Exists(filePath))
            {
                mainImage.Source = null;
                mainImage.Width = Double.NaN;
                mainImage.Height = Double.NaN;
            }
            else
            {
                currentImageSource = BitmapFromUri(new Uri(filePath));
                mainImage.Source = currentImageSource;

                Resize();
            }
        }

        public void PreviousImage()
        {
            if (indexOfImageFolder == 0) return;

            indexOfImageFolder--;
            filePath = imagesInFolder[indexOfImageFolder];

            if (!File.Exists(filePath))
            {
                mainImage.Source = null;
                mainImage.Width = Double.NaN;
                mainImage.Height = Double.NaN;
            }
            else
            {
                currentImageSource = BitmapFromUri(new Uri(filePath));
                mainImage.Source = currentImageSource;

                Resize();
            }
        }

        public void Resize()
        {
            if (currentImageSource == null) return;

            if (isStretchMode)
            {
                mainImage.Width = Double.NaN;
                mainImage.Height = Double.NaN;
            }
            else
            {
                if (mainWindow.ActualWidth == 0)
                {
                    mainImage.Width = ClampDimensions(currentImageSource.Width, 1, mainWindow.Width);
                    mainImage.Height = ClampDimensions(currentImageSource.Height, 1, mainWindow.Height - 40);
                }
                else
                {
                    mainImage.Width = ClampDimensions(currentImageSource.Width, 1, mainWindow.ActualWidth);
                    mainImage.Height = ClampDimensions(currentImageSource.Height, 1, mainWindow.ActualHeight - 40);
                }
            }
        }

        public void DeleteImage()
        {
            if (imagesInFolder.Count == 0) return;

            MessageBoxResult result = MessageBox.Show("Do you want to delete this image?", "Confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No || result == MessageBoxResult.None) return;

            mainImage.Source = null;
            mainImage.Width = Double.NaN;
            mainImage.Height = Double.NaN;

            if (File.Exists(filePath))
            {
                imagesInFolder.Remove(filePath);
                File.Delete(filePath);
            }

            if (indexOfImageFolder != 0 && imagesInFolder.Count > 1)
            {
                indexOfImageFolder--;
                filePath = imagesInFolder[indexOfImageFolder];
                currentImageSource = BitmapFromUri(new Uri(filePath));
                mainImage.Source = currentImageSource;

                mainImage.Width = ClampDimensions(currentImageSource.Width, 1, mainWindow.ActualWidth);
                mainImage.Height = ClampDimensions(currentImageSource.Height, 1, mainWindow.ActualHeight - 40);
            }
            else if (indexOfImageFolder != imagesInFolder.Count - 1 && imagesInFolder.Count > 1)
            {
                indexOfImageFolder++;
                filePath = imagesInFolder[indexOfImageFolder];
                currentImageSource = BitmapFromUri(new Uri(filePath));
                mainImage.Source = currentImageSource;

                mainImage.Width = ClampDimensions(currentImageSource.Width, 1, mainWindow.ActualWidth);
                mainImage.Height = ClampDimensions(currentImageSource.Height, 1, mainWindow.ActualHeight - 40);
            }
        }
    }
}

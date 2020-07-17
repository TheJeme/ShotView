﻿using Microsoft.Win32;
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

        public void StretchImage()
        {
            if (currentImageSource == null) return;

            isStretchMode = !isStretchMode;

            if (isStretchMode)
            {
                mainImage.Width = Double.NaN;
                mainImage.Height = Double.NaN;
            }
            else
            {
                mainImage.Width = ClampDimensions(currentImageSource.Width, 1, mainWindow.ActualWidth);
                mainImage.Height = ClampDimensions(currentImageSource.Height, 1, mainWindow.ActualHeight - 40);
            }
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

        public void OpenImage()
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

            Nullable<bool> result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                filePath = dlg.FileName;
                folderPath = Path.GetDirectoryName(filePath);

                currentImageSource = BitmapFromUri(new Uri(filePath));
                mainImage.Source = currentImageSource;

                mainImage.Width = ClampDimensions(currentImageSource.Width, 1, mainWindow.ActualWidth);
                mainImage.Height = ClampDimensions(currentImageSource.Height, 1, mainWindow.ActualHeight - 40);

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
            if (indexOfImageFolder == imagesInFolder.Count - 1 && imagesInFolder.Count > 0) return;

            indexOfImageFolder++;
            filePath = imagesInFolder[indexOfImageFolder];
            currentImageSource = BitmapFromUri(new Uri(filePath));
            mainImage.Source = currentImageSource;

            mainImage.Width = ClampDimensions(currentImageSource.Width, 1, mainWindow.ActualWidth);
            mainImage.Height = ClampDimensions(currentImageSource.Height, 1, mainWindow.ActualHeight - 40);
        }

        public void PreviousImage()
        {
            if (indexOfImageFolder == 0) return;

            indexOfImageFolder--;
            filePath = imagesInFolder[indexOfImageFolder];
            currentImageSource = BitmapFromUri(new Uri(filePath));
            mainImage.Source = currentImageSource;

            mainImage.Width = ClampDimensions(currentImageSource.Width, 1, mainWindow.ActualWidth);
            mainImage.Height = ClampDimensions(currentImageSource.Height, 1, mainWindow.ActualHeight - 40);
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
                mainImage.Width = ClampDimensions(currentImageSource.Width, 1, mainWindow.ActualWidth);
                mainImage.Height = ClampDimensions(currentImageSource.Height, 1, mainWindow.ActualHeight - 40);
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

            imagesInFolder.Remove(filePath);
            File.Delete(filePath);

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

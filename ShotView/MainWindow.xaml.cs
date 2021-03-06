﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ShotView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string filePath)
        {
            InitializeComponent();

            imageView = new ImageView(this, MainImage);
            if (!String.IsNullOrEmpty(filePath))
            {
                string[] extensions = { ".jpg", ".jpeg", ".bmp", ".jpe", ".jfif", ".png" };

                string extension = filePath.Substring(filePath.LastIndexOf('.'));
                if (extensions.Any(x => extension.ToLower().Contains(x)))
                {
                    imageView.OpenImage(filePath);
                }
            }

            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(3);
            dt.Tick += dtTicker;            

            isFullScreen = false;
            isSlideShow = false;
        }

        private bool isFullScreen;
        private bool isSlideShow;

        DispatcherTimer dt;

        ImageView imageView;

        private void dtTicker(object sender, EventArgs e)
        {
            imageView.NextImage();
        }

        private void UpdateInfoPanel()
        {
            if (imageView.getImageExists())
            {
                FileInfoFileNameLabel.Content = imageView.GetFileName();
                FileInfoFolderPathLabel.Content = imageView.GetFolderPath();
                FileInfoDimensionsLabel.Content = imageView.GetImageDimensions();
                FileInfoSizeLabel.Content = imageView.GetFileSize();
            }
            else
            {
                FileInfoFileNameLabel.Content = "";
                FileInfoFolderPathLabel.Content = "";
                FileInfoDimensionsLabel.Content = "";
                FileInfoSizeLabel.Content = "";
            }
        }

        private void ShowInfoPanel()
        {
            if (InfoPanel.Visibility == Visibility.Visible)
            {
                InfoPanel.Visibility = Visibility.Collapsed;
            }
            else if (InfoPanel.Visibility == Visibility.Collapsed)
            {
                UpdateInfoPanel();
                InfoPanel.Visibility = Visibility.Visible;
            }
        }

        private void HideTopBar()
        {
            if (TopBarGrid.Visibility == Visibility.Visible)
            {
                TopBarGrid.Visibility = Visibility.Collapsed;
                TopBarHiderLabel.Content = "↓ ↓ ↓";
            }
            else if (TopBarGrid.Visibility == Visibility.Collapsed)
            {
                TopBarGrid.Visibility = Visibility.Visible;
                TopBarHiderLabel.Content = "↑ ↑ ↑";
            }
        }

        private void ToggleFullScreen(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                HideTopBar();
            }

            else if (e.Key == Key.Delete)
            {
                imageView.DeleteImage();
            }

            else if (e.Key == Key.Left)
            {
                imageView.PreviousImage();
            }
            else if (e.Key == Key.Right)
            {
                imageView.NextImage();
            }

            else if (e.Key == Key.F11)
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
            else if (e.Key == Key.Escape && isFullScreen)
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;

                isFullScreen = false;
            }
        }

        private void OpenImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageView.OpenImageFileDialog();
            UpdateInfoPanel();
        }
    

        private void Slideshow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isSlideShow = !isSlideShow;

            if (isSlideShow)
            {
                dt.Start();
            }
            else
            {
                dt.Stop();
            }
        }


        private void DeleteImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageView.DeleteImage();
            UpdateInfoPanel();
        }

        private void ShowInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowInfoPanel();
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
            UpdateInfoPanel();
        }

        private void Next_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageView.NextImage();
            UpdateInfoPanel();
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

        private void HideTopbar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HideTopBar();
        }
    }
}

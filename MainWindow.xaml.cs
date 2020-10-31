using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Threading;
using Gallery.Classes;
using System.IO;

namespace Gallery
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        List<string> listImages;
        readonly DispatcherTimer timer = new DispatcherTimer();
        private  BusinessLogic businessLogic;
        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Stop();
        }
        public string GetCurrentPath()
        {
            return labelPath.Content.ToString();
        }
        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            listImages = new List<string>();
            businessLogic = new BusinessLogic(listImages, ImageControl, labelPath, ButtonNext, ButtonBack) ;           
            businessLogic.OpenImage();           
        }
        private void ButtonEnableSlideShow_Click(object sender, RoutedEventArgs e)
        {
            if(labelPath.Content!=null)
            timer.Start();
        }
        private void ButtonOffSlideShow_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            businessLogic.GoToBackElement(GetCurrentPath());
        }
        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            businessLogic.GoToNextElement(GetCurrentPath());
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            businessLogic.GoToNextElement(GetCurrentPath());
        }
        private void ButtonRotate90_Click(object sender, RoutedEventArgs e)
        {
            if(labelPath.Content!=null)
            businessLogic.BitmapRotation(GetCurrentPath(), "Rotate90");
        }
        private void ButtonRotate180_Click(object sender, RoutedEventArgs e)
        {
            if (labelPath.Content != null)
                businessLogic.BitmapRotation(GetCurrentPath(), "Rotate180");
        }
        private void ButtonRotate270_Click(object sender, RoutedEventArgs e)
        {
            if (labelPath.Content != null)
                businessLogic.BitmapRotation(GetCurrentPath(),"Rotate270");
        }
        private void ContextCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetImage(new BitmapImage(new Uri(GetCurrentPath())));
        }
        private void ContextSave_Click(object sender, RoutedEventArgs e)
        {          
                MemoryStream memoryStream = new MemoryStream();
                byte[] fileBytes = File.ReadAllBytes(GetCurrentPath());
                memoryStream.Write(fileBytes, 0, fileBytes.Length);
                memoryStream.Position = 0;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = memoryStream;
                bitmap.Rotation = (Rotation)Enum.Parse(typeof(Rotation), "Rotate180");
                bitmap.Rotation = (Rotation)Enum.Parse(typeof(Rotation), "Rotate0");
                bitmap.EndInit();
                ImageControl.Source = bitmap;
                businessLogic.SaveImage(GetCurrentPath());            
        }
        private void ContextRemove_Click(object sender, RoutedEventArgs e)
        {
            businessLogic.RemoveImage(GetCurrentPath());
        }
        private void ContextZoom_Click(object sender, RoutedEventArgs e)
        {
            WindowImg formImg = new WindowImg();
            formImg.Show();
            formImg.picture.Source = ImageControl.Source;
        }
    }
}
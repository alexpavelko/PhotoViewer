using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Gallery.Classes
{
    public class BusinessLogic
    {
        private readonly List<string> listImages;
        private readonly Image ImageControl;
        private readonly Label labelPath;
        private readonly Button btnNext;
        private readonly Button btnPrev;
        private static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG", ".JPEG" };
        public BusinessLogic(List<string> listImages, Image imageControl, Label labelPath, Button btnNext, Button btnPrev)
        {
            this.listImages = listImages;
            this.ImageControl = imageControl;
            this.labelPath = labelPath;
            this.btnNext = btnNext;
            this.btnPrev = btnPrev;
        }
        public BitmapImage ImageCreate(string path)
        {
            BitmapImage image = new BitmapImage();
            MemoryStream memoryStream = new MemoryStream();
            byte[] fileBytes = File.ReadAllBytes(path);
            memoryStream.Write(fileBytes, 0, fileBytes.Length);
            memoryStream.Position = 0;

            image.BeginInit();
            image.StreamSource = memoryStream;
            image.EndInit();

            return image;
        }
        public void OpenImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JPG, PNG, BMP, GIF|*.jpg; *.png; *.bmp; *.gif;| JPG| *.jpg| PNG| *.png| BMP| *.bmp| GIF| *.gif"
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName.Length > 0)
            {
                string selectedFileName = openFileDialog.FileName;
                labelPath.Content = selectedFileName;
                if (File.Exists(selectedFileName))
                {
                    ImageControl.Source = ImageCreate(selectedFileName);
                    LoadPhoto(selectedFileName);
                    btnPrev.Visibility = Visibility.Visible;
                    btnNext.Visibility = Visibility.Visible;
                }
            }
        }
        public void LoadPhoto(string path)
        {
            var parentFolder = Directory.GetParent(path);
            foreach (var file in Directory.GetFiles(parentFolder.ToString()))
            {
                try
                {
                    if (ImageExtensions.Contains(Path.GetExtension(file).ToUpperInvariant()))
                    {
                        listImages.Add(file);
                    }
                }
                catch (NotSupportedException)
                {
                    return;
                }
            }
        }
        public void SaveImage(string path)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = Path.GetFileName(path),
                Filter = "JPG, PNG, BMP, GIF|*.jpg; *.png; *.bmp; *.gif;| JPG| *.jpg| PNG| *.png| BMP| *.bmp| GIF| *.gif"
            };
            var jpegBitmapEncoder = new JpegBitmapEncoder();
           
                jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(ImageControl.Source as BitmapSource));
                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                    jpegBitmapEncoder.Save(fileStream);
            
        }
        public void RemoveImage(string path)
        {
            if (listImages.Count > 0)
            {
                BitmapImage bitmap;
                bitmap = ImageControl.Source as BitmapImage;
                bitmap.Rotation = (Rotation)Enum.Parse(typeof(Rotation), "Rotate180");
                bitmap.Rotation = (Rotation)Enum.Parse(typeof(Rotation), "Rotate0");
                ImageControl.Source = bitmap;
                File.Delete(path);
                listImages.Remove(path);
            }

            int next = listImages.IndexOf(labelPath.Content.ToString()) + 1;
            int prev = listImages.IndexOf(labelPath.Content.ToString()) - 1;

            if (next <= listImages.Count - 1)
            {
                GoToNextElement(path);
                labelPath.Content = listImages[next];
            }
            else if (prev >= 0)
            {
                GoToBackElement(path);
                labelPath.Content = listImages[prev];
            }
            else
            {
                btnPrev.Visibility = Visibility.Hidden;
                btnNext.Visibility = Visibility.Hidden;
                labelPath.Content = null;
                ImageControl.Source = null;
            }
        }
        public void GoToNextElement(string path)
        {
            int nextIndex = listImages.IndexOf(path) + 1;
            if (nextIndex <= listImages.Count - 1)
            {
                ImageControl.Source = ImageCreate(listImages[nextIndex]);
                labelPath.Content = listImages[nextIndex];
            }
        }
        public void GoToBackElement(string path)
        {
            int prevIndex = listImages.IndexOf(path) - 1;
            if (prevIndex >= 0)
            {
                ImageControl.Source = ImageCreate(listImages[prevIndex]);
                labelPath.Content = listImages[prevIndex];
            }
        }
        public void BitmapRotation(string path, string degrees)
        {
            MemoryStream memoryStream = new MemoryStream();
            byte[] fileBytes = File.ReadAllBytes(path);
            memoryStream.Write(fileBytes, 0, fileBytes.Length);
            memoryStream.Position = 0;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = memoryStream;
            bitmap.Rotation = (Rotation)Enum.Parse(typeof(Rotation), degrees);
            bitmap.EndInit();
            ImageControl.Source = bitmap;
            SaveImage(path);
        }
    }
}
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Copper.Model
{
    public class IconModel
    {
        public IconModel()
        {

        }

        public static Border Make(FileInfo fileinfo = null)
        {
            Border icon;
            Image img;
            IconSetup(out icon, out img);
            if(fileinfo != null)
                LoadImage(fileinfo, img);

            return icon;
        }

        private static void IconSetup(out Border icon, out Image img)
        {
            icon = new Border();
            Grid iconGrid = new Grid();
            img = new Image();
            img.Width = 30;
            img.Height = 30;
            img.MouseDown += ImgTemp_MouseDown;
            //img.MouseMove += Img_MouseMove;
            //img.MouseUp += Img_MouseUp;
            iconGrid.Children.Add(img);
            icon.Child = iconGrid;
            //icon.BorderBrush = Brushes.Transparent;
            icon.BorderThickness = new Thickness(3);
            icon.BorderBrush = Brushes.Transparent;
            icon.GotFocus += Icon_GotFocus;
            icon.LostFocus += Icon_LostFocus;
            icon.Focusable = true;
        }

        private static void LoadImage(FileInfo fileinfo, Image img)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fileinfo.FullName);
            bitmap.EndInit();
            img.Source = bitmap;
        }

        private static void Icon_GotFocus(object sender, RoutedEventArgs e)
        {
            Border icon = sender as Border;
            (icon.Child as Grid).Children.Add(new Rectangle() { Width = double.NaN, Height = double.NaN, Fill = new SolidColorBrush(Color.FromArgb(30, 0, 255, 255)), IsHitTestVisible = false });
            icon.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 255, 255));
        }

        private static void Icon_LostFocus(object sender, RoutedEventArgs e)
        {
            Border icon = sender as Border;
            (icon.Child as Grid).Children.RemoveAt(1);
            icon.BorderBrush = Brushes.Transparent;
        }


        private static void ImgTemp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point offset;
            Image img = sender as Image;
            Border item = VisualTreeHelper.GetParent(img.Parent) as Border;
            item.Focus();
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Border icon = new Border();
                Grid iconGrid = new Grid();
                Image imgData = new Image();
                imgData.Source = img.Source;
                imgData.MouseDown += ImgData_MouseDown;
                imgData.MouseEnter += ImgData_MouseEnter;
                imgData.MouseLeave += ImgData_MouseLeave;
                offset = e.GetPosition(img);
                iconGrid.Children.Add(imgData);
                icon.Child = iconGrid;
                icon.BorderThickness = new Thickness(3);
                icon.BorderBrush = Brushes.Transparent;
                icon.GotFocus += Icon_GotFocus;
                icon.LostFocus += Icon_LostFocus;
                icon.Focusable = true;
                DataObject data = new DataObject();
                data.SetData("create", true);
                data.SetData("offset", offset);
                data.SetData(typeof(Border), icon);
                DragDrop.DoDragDrop(icon, data, DragDropEffects.Move);
            }
        }

        private static void ImgData_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            var x = img.Source;

            Storyboard sb = new Storyboard();
            ColorAnimation ca = new ColorAnimation();
            ca.SetValue(Storyboard.TargetProperty, img);
        }

        private static void ImgData_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private static void ImgData_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point offset;
            Image img = sender as Image;
            Border icon = VisualTreeHelper.GetParent(img.Parent) as Border;
            icon.Focus();
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                    new IconSchemeViewer(icon.MappedIconSet() as IconScheme).Show();
                else
                    AppData.ExecuteIcon(icon);
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                offset = e.GetPosition(icon);
                DataObject data = new DataObject();
                data.SetData("offset", offset);
                data.SetData(typeof(Border), icon);
                DragDrop.DoDragDrop(icon, data, DragDropEffects.Move);
            }
        }
    }
}

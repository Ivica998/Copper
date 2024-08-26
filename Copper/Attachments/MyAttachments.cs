using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Copper
{
    public class MyAttachments
    {
        public static readonly DependencyProperty AllowIconDropProperty = DependencyProperty.RegisterAttached(
            "AllowIconDrop",
            typeof(bool),
            typeof(MyAttachments),
            new PropertyMetadata(default(bool), OnLoaded));

        private static void OnLoaded(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is Panel))
                return;
            var panel = dependencyObject as Panel;
            if (panel == null || (dependencyPropertyChangedEventArgs.NewValue is bool) == false)
            {
                return;
            }
            if ((bool)dependencyPropertyChangedEventArgs.NewValue == true)
            {
                prewImages.Add(dependencyObject, new Image() { IsHitTestVisible = false,Opacity = 0.7 });
                offsets.Add(dependencyObject, new Point());
                panel.Background = new SolidColorBrush(Color.FromRgb(255, 230, 200));
                panel.AllowDrop = true;
                panel.Drop += Canv_Drop;
                panel.DragOver += Canv_DragOver;
                panel.DragEnter += Canv_DragEnter;
                panel.DragLeave += Canv_DragLeave;
            }
            else
            {
                prewImages.Remove(dependencyObject);
                offsets.Remove(dependencyObject);
                panel.Drop -= Canv_Drop;
                panel.DragOver -= Canv_DragOver;
                panel.DragEnter -= Canv_DragEnter;
                panel.DragLeave -= Canv_DragLeave;
            }

        }

        private static Dictionary<object,Image> prewImages = new Dictionary<object, Image>() { };
        private static Dictionary<object, Point> offsets = new Dictionary<object, Point>() { };
        private static void Canv_DragLeave(object sender, DragEventArgs e)
        {
            (sender as Panel).Children.Remove(prewImages[sender]);
        }

        private static void Canv_DragEnter(object sender, DragEventArgs e)
        {
            Border item = e.Data.GetData(typeof(Border)) as Border;
            Image img = (item.Child as Grid).Children[0] as Image;
            prewImages[sender].Source = img.Source;
            (sender as Panel).Children.Add(prewImages[sender]);
            offsets[sender] = (Point)e.Data.GetData("offset");
        }

        private static void Canv_DragOver(object sender, DragEventArgs e)
        {

            Point position = e.GetPosition((Canvas)sender);
            Canvas.SetLeft(prewImages[sender], position.X - offsets[sender].X + 3);
            Canvas.SetTop(prewImages[sender], position.Y - offsets[sender].Y + 3);
        }

        private static void Canv_Drop(object sender, DragEventArgs e)
        {
            Border item = e.Data.GetData(typeof(Border)) as Border;
            if(e.Data.GetDataPresent("create"))
            {
                IconScheme ish = new IconScheme(item);
                AppData.iconSetData.Add(ish.Canvas.Name, ish);
            }
            Image img = (item.Child as Grid).Children[0] as Image;
            Panel obj = (Panel)item.Parent;
            Point position = e.GetPosition((Canvas)sender);
            Canvas.SetLeft(item, position.X - offsets[sender].X);
            Canvas.SetTop(item, position.Y - offsets[sender].Y);
            obj?.MappedIconSet().Remove(item);
            (sender as Panel).Children.Remove(prewImages[sender]);
            (sender as Panel).MappedIconSet().Add(item);
        }

        public static void SetAllowIconDrop(DependencyObject element, bool value)
        {
            element.SetValue(AllowIconDropProperty, value);
        }

        public static bool GetAllowIconDrop(DependencyObject element)
        {
            return (bool)element.GetValue(AllowIconDropProperty);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Copper.Helpers;
using System.IO;

namespace Copper.Model
{
    public class IconSet
    {
        private static int counter = 0;
        public string Name { get; set; } = "unknown";
        public List<IconScheme> Components { get; set; } = new List<IconScheme>();
        public Dictionary<IconScheme, Point> Locations { get; set; } = new Dictionary<IconScheme, Point>();
        public IconSet()
        {

        }

        private Canvas canvas;

        public Canvas Canvas
        {
            get
            {
                if (canvas == null)
                    Make();
                return canvas;
            }
            private set
            {
                canvas = value;
            }
        }

        private void Make()
        {
            canvas = new Canvas();
            canvas.Name = "Canvas_" + counter.ToString();
            this.Name = "IconSet_" + counter.ToString();
            counter++;
            MyAttachments.SetAllowIconDrop(canvas, true);
            //canvas.Style = Application.Current.Resources[typeof(Panel)] as Style;
            foreach (var item in Components)
            {
                canvas.Children.Add(item.Holder);
                Canvas.SetLeft(item.Holder, Locations[item].X);
                Canvas.SetTop(item.Holder, Locations[item].Y);
            }
        }

        public void Add(Border border, Point? location = null)
        {

            foreach (var item in AppData.iconSetData.Values)
            {
                if (item is IconScheme ish && ish.Holder == border)
                {
                    Add(ish, location);
                }
            }
        }

        public void Add(IconScheme item,Point? location = null)
        {
            if(!Components.Contains(item))
            {
                Components.Add(item);
                if(location == null)
                {
                    location = new Point(Canvas.GetLeft(item.Holder), Canvas.GetTop(item.Holder));
                }
                Locations.Add(item, (Point)location);
                canvas.Children.Add(item.Holder);
            }
        }

        public bool Remove(Border border)
        {
            foreach (var item in AppData.iconSetData.Values)
            {
                if(item is IconScheme ish && ish.Holder == border)
                {
                    return Remove(ish);
                }
            }
            return false;
        }
        public bool Remove(IconScheme item)
        {
            if (!Components.Contains(item))
            {
                Components.Remove(item);
                Locations.Remove(item);
                return true;
            }
            return false;
        }

        public bool Execute()
        {
            foreach (var comp in Components)
            {
                // comp.Execute();
            }
            return true;
        }

        public void Save(string path)
        {
            SavingHelper.Save(path, this);
        }
        public void Save(FileInfo fileinfo)
        {
            SavingHelper.Save(fileinfo.FullName, this);
        }

        public static void Save<T>(FileInfo fileinfo, T obj) where T : IconSet, new()
        {
            SavingHelper.Save(fileinfo.FullName, obj);
        }

        internal static T Load<T>(FileInfo fileinfo) where T : IconSet, new()
        {
            T ics = SavingHelper.Load<T>(fileinfo.FullName);

            return ics;
        }

    }
}

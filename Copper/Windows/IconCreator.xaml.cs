using Copper.Helpers;
using Copper.Model.MyControls;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Copper
{
    /// <summary>
    /// Interaction logic for IconCreator.xaml
    /// </summary>
    public partial class IconCreator : Window, INotifyPropertyChanged
    {
        enum DrawMode
        {
            Pen = 0,
            Brush,
            Line,
            Fill,
            Eraser,
            ColorPicker,
            Circle
        }

        private Grid iconZone;
        private SolidColorBrush leftClickColor = Brushes.Black;
        private SolidColorBrush rightClickColor = Brushes.White;
        private Rectangle activeMB;
        private DrawMode activeDM = DrawMode.Pen;
        private int activeSize = 0;

        public ObservedType<int> DrawWidth { get; set; } = new ObservedType<int>();
        public ObservedType<int> DrawHeigth { get; set; } = new ObservedType<int>();
        public Viewbox IconZone { get; set; } = new Viewbox() { Stretch = Stretch.Uniform };
        public SolidColorBrush LeftClickColor
        {
            get { return leftClickColor; }
            set
            {
                leftClickColor = value;
                OnPropertyChanged();
            }
        }
        public SolidColorBrush RightClickColor
        {
            get { return rightClickColor; }
            set
            {
                rightClickColor = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public IconCreator()
        {
            Style = FindResource(typeof(Window)) as Style;

            InitializeComponent();
            Loaded += IconCreator_Loaded;
        }

        private void IconCreator_Loaded(object sender, RoutedEventArgs e)
        {
            activeMB = leftRect;
            cbColors.ItemsSource = typeof(Colors).GetProperties();
            cbColors.SelectedIndex = 7;
            cbSize.ItemsSource = GetSizeArray(8);
            cbSize.SelectedIndex = 0;

            DrawHeigth.Value = 25;
            DrawWidth.Value = 25;
            htb.Text = "#FFFFFFFF";
            CreateIconZone(DrawHeigth.Value, DrawWidth.Value);
            mainGrid.Children.Add(IconZone);
            Grid.SetRow(IconZone, 1);
            Grid.SetColumn(IconZone, 0);
        }

        private IEnumerable GetSizeArray(int n)
        {
            List<int> retVal = new List<int>();
            for (int i = 1; i <= n; i++)
            {
                retVal.Add(i);
            }
            return retVal;
        }
        private void CreateIconZone(int rows, int cols)
        {
            iconZone = new Grid();
            for (int i = 0; i < rows; i++)
            {
                iconZone.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }
            for (int i = 0; i < cols; i++)
            {
                iconZone.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            bool firstBlack = true;
            bool nextBlack;
            for (int i = 0; i < rows; i++)
            {
                nextBlack = firstBlack;
                firstBlack = !firstBlack;
                for (int j = 0; j < cols; j++)
                {
                    Viewbox vb = new Viewbox() { Stretch = Stretch.Uniform };
                    Rectangle rect = new Rectangle();
                    rect.Height = 50;
                    rect.Width = 50;
                    if (nextBlack)
                        rect.Fill = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                    else
                        rect.Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                    vb.Child = rect;
                    iconZone.Children.Add(vb);
                    Grid.SetRow(vb, i);
                    Grid.SetColumn(vb, j);
                    nextBlack = !nextBlack;

                    vb = new Viewbox() { Stretch = Stretch.Uniform };
                    rect = new Rectangle();
                    rect.Height = 50;
                    rect.Width = 50;
                    rect.Fill = Brushes.Transparent;
                    vb.Child = rect;
                    iconZone.Children.Add(vb);
                    Grid.SetRow(vb, i);
                    Grid.SetColumn(vb, j);

                    rect.MouseEnter += Draw;
                    rect.MouseDown += Draw;
                }
            }
            IconZone.Child = iconZone;
        }
        private bool firstDot;
        private double fdr;
        private double fdc;
        private void Draw(object sender, MouseEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if (activeDM == DrawMode.Pen)
            {
                if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    int row = Grid.GetRow((UIElement)rect.Parent);
                    int col = Grid.GetColumn((UIElement)rect.Parent);
                    for (int roww = -activeSize + row; roww <= activeSize + row; roww++)
                    {
                        for (int coll = -activeSize + col; coll <= activeSize + col; coll++)
                        {
                            if (coll >= 0 && coll < DrawWidth.Value && roww >= 0 && roww < DrawHeigth.Value)
                                ((Rectangle)((Viewbox)iconZone.Children[2 * (roww * DrawWidth.Value + coll) + 1]).Child).Fill
                                    = (e.LeftButton == MouseButtonState.Pressed) ? LeftClickColor : rightClickColor;
                        }
                    }
                }
            }
            if (activeDM == DrawMode.Brush)
            {
                if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    int row = Grid.GetRow((UIElement)rect.Parent);
                    int col = Grid.GetColumn((UIElement)rect.Parent);
                    for (int roww = -activeSize + row; roww <= activeSize + row; roww++)
                    {
                        for (int coll = -activeSize + col + Math.Abs(roww - row); coll <= activeSize + col - Math.Abs(roww - row); coll++)
                        {
                            if (coll >= 0 && coll < DrawWidth.Value && roww >= 0 && roww < DrawHeigth.Value)
                                ((Rectangle)((Viewbox)iconZone.Children[2 * (roww * DrawWidth.Value + coll) + 1]).Child).Fill
                                    = (e.LeftButton == MouseButtonState.Pressed) ? LeftClickColor : rightClickColor;
                        }
                    }
                }
            }
            if (activeDM == DrawMode.Line)
            {
                if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    int row = Grid.GetRow((UIElement)rect.Parent);
                    int col = Grid.GetColumn((UIElement)rect.Parent);
                    if (firstDot)
                    {
                        fdr = row; fdc = col; firstDot = false;
                    }
                    else
                    {
                        SolidColorBrush brush = (e.LeftButton == MouseButtonState.Pressed) ? LeftClickColor : rightClickColor;
                        double sdr = row, sdc = col;
                        Interpolate(fdc, fdr, sdc, sdr, PainCanvasPixel, brush);
                        fdr = sdr; fdc = sdc;
                    }

                }
            }
            if (activeDM == DrawMode.Fill)
            {
                if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    SolidColorBrush brush = (e.LeftButton == MouseButtonState.Pressed) ? LeftClickColor : rightClickColor;
                    int row = Grid.GetRow((UIElement)rect.Parent);
                    int col = Grid.GetColumn((UIElement)rect.Parent);
                    Brush fillTarget = rect.Fill;
                    List<Rectangle> toProcess = new List<Rectangle>() { rect };
                    List<Rectangle> considered = new List<Rectangle>() { rect };


                    while (toProcess.Count > 0)
                    {
                        rect = toProcess[0];
                        toProcess.RemoveAt(0);
                        row = Grid.GetRow((UIElement)rect.Parent);
                        col = Grid.GetColumn((UIElement)rect.Parent);
                        rect.Fill = brush;
                        FillProcessStep(toProcess, considered, row, col, fillTarget);

                    }
                }
            }
            if (activeDM == DrawMode.Eraser)
            {
                if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    int row = Grid.GetRow((UIElement)rect.Parent);
                    int col = Grid.GetColumn((UIElement)rect.Parent);
                    for (int roww = row - activeSize; roww <= row + activeSize; roww++)
                    {
                        for (int coll = col - activeSize; coll <= col + activeSize; coll++)
                        {
                            if (coll >= 0 && coll < DrawWidth.Value && roww >= 0 && roww < DrawHeigth.Value)
                                ((Rectangle)((Viewbox)iconZone.Children[2 * (roww * DrawWidth.Value + coll) + 1]).Child).Fill = Brushes.Transparent;
                        }
                    }
                }
            }
            if (activeDM == DrawMode.ColorPicker)
            {
                if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    htb.Text = (rect.Fill as SolidColorBrush).Color.ToString();
                    if (e.LeftButton == MouseButtonState.Pressed)
                        LeftClickColor = rect.Fill as SolidColorBrush;
                    else
                        RightClickColor = rect.Fill as SolidColorBrush;
                }
            }
        }
        private void Interpolate(double x1, double y1, double x2, double y2, Action<double, double, object> method, object param = null)
        {
            double x = Math.Abs(x1 - x2);
            double y = Math.Abs(y1 - y2);
            double xc, yc;
            int count;
            if (x > y)
            {
                xc = x1 < x2 ? 1 : -1; // X Change
                yc = (y1 - y2) / (x1 - x2) * xc; // deltaY = koef * deltaX .. koef = (y1-y2)/(x1-x2)
                if (double.IsNaN(yc) || double.IsInfinity(yc))
                    yc = 0;
                count = (int)x;
            }
            else
            {
                yc = y1 < y2 ? 1 : -1;
                xc = (x1 - x2) / (y1 - y2) * yc;
                if (double.IsNaN(xc) || double.IsInfinity(xc))
                    xc = 0;
                count = (int)y;
            }
            while (count-- >= 0)
            {
                method?.Invoke(x1, y1, param);
                x1 += xc;
                y1 += yc;
            }
        }
        private void PainCanvasPixel(double x, double y, object param)
        {
            x = Math.Round(x);
            y = Math.Round(y);
            ((Rectangle)((Viewbox)iconZone.Children[(int)(2 * (y * DrawWidth.Value + x) + 1)]).Child).Fill = param as SolidColorBrush;
        }
        private void FillProcessStep(List<Rectangle> toProcess, List<Rectangle> considered, int row, int col, Brush fillTarget)
        {
            Rectangle rect;
            for (int i = 1; i <= 7; i += 2)
            {
                int coll = col + i % 3 - 1;
                int roww = row + i / 3 - 1;
                if (coll >= 0 && coll < DrawWidth.Value && roww >= 0 && roww < DrawHeigth.Value)
                {
                    rect = (Rectangle)((Viewbox)iconZone.Children[2 * (roww * DrawWidth.Value + coll) + 1]).Child;
                    if (rect.Fill == fillTarget && !considered.Contains(rect))
                    {
                        toProcess.Add(rect);
                        considered.Add(rect);
                    }

                }
            }
        }
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            rect.Stroke = Brushes.AliceBlue;
            if (rect == leftRect)
                rightRect.Stroke = Brushes.Black;
            else
                leftRect.Stroke = Brushes.Black;
            activeMB = rect;

        }
        private void cbColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (activeMB == leftRect)
                LeftClickColor = new SolidColorBrush((Color)(cbColors.SelectedItem as PropertyInfo).GetValue(null, null));
            else
                RightClickColor = new SolidColorBrush((Color)(cbColors.SelectedItem as PropertyInfo).GetValue(null, null));
        }
        private void cbSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            activeSize = (int)((ComboBox)sender).SelectedItem - 1;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists($"Data\\Icons"))
                Directory.CreateDirectory($"Data\\Icons");
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "image"; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.InitialDirectory = Directory.GetCurrentDirectory() + "\\Data\\Icons\\";
            dlg.Filter = "PNG file (.png)|*.png"; // Filter files by extension
            if (dlg.ShowDialog() == true)
            {
                MakeImg(int.Parse(tbWidth.Text),int.Parse(tbHeight.Text), iconZone, dlg.FileName);
            }
        }
        public static void MakeImg(int width,int height, Grid data, string fileName)
        {
            if (width < 2)
                width = 2;
            if (height < 2)
                height = 2;
            int stride = (width * PixelFormats.Pbgra32.BitsPerPixel + 7) / 8;
            byte[] pixels = new byte[height * stride];

            // Try creating a new image with a custom palette.
            List<Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(Colors.Red);
            colors.Add(Colors.Green);
            colors.Add(Colors.Blue);
            BitmapPalette myPalette = new BitmapPalette(colors);

            Color color;
            int counter = 0;
            for (int i = 1; i < data.Children.Count; i += 2)
            {
                color = ((SolidColorBrush)((Rectangle)((Viewbox)data.Children[i]).Child).Fill).Color;
                pixels[counter++] = color.B;
                pixels[counter++] = color.G;
                pixels[counter++] = color.R;
                pixels[counter++] = color.A;
            }

            BitmapSource image = BitmapSource.Create(
                                        width,
                                        height,
                                        96,
                                        96,
                                        PixelFormats.Pbgra32,
                                        null,
                                        pixels,
                                        stride);

            //var image = Clipboard.GetImage();


            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
        }
        private void LoadImage()
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Title = "Open Image";
            dlg.InitialDirectory = Directory.GetCurrentDirectory() + "\\Data\\Icons\\";
            dlg.Filter = "PNG file (.png)|*.png";

            if (dlg.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dlg.FileName);
                bitmap.EndInit();
                int width = bitmap.PixelWidth;
                int height = bitmap.PixelHeight;
                int stride = (width * PixelFormats.Pbgra32.BitsPerPixel + 7) / 8;
                byte[] pixels = new byte[height * stride];

                bitmap.CopyPixels(pixels, stride, 0);

                Color color;
                int counter = 0;
                Grid data = iconZone;
                for (int i = 1; i < data.Children.Count; i += 2)
                {
                    color.B = pixels[counter++];
                    color.G = pixels[counter++];
                    color.R = pixels[counter++];
                    color.A = pixels[counter++];
                    ((Rectangle)((Viewbox)data.Children[i]).Child).Fill = new SolidColorBrush(color);
                }
            }
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            HexTextBox htb = sender as HexTextBox;
            if (e.Key == Key.Enter && htb.IsValid)
            {
                if (activeMB == leftRect)
                    LeftClickColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(htb.Text));
                else
                    RightClickColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(htb.Text));
            }
        }






        private void New_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            LoadImage();
        }
        private void Pen_Button_Click(object sender, RoutedEventArgs e)
        {
            activeDM = DrawMode.Pen;
        }
        private void Brush_Button_Click(object sender, RoutedEventArgs e)
        {
            activeDM = DrawMode.Brush;
        }
        private void Line_Button_Click(object sender, RoutedEventArgs e)
        {
            activeDM = DrawMode.Line;
            firstDot = true;
        }
        private void Fill_Button_Click(object sender, RoutedEventArgs e)
        {
            activeDM = DrawMode.Fill;
        }
        private void Eraser_Button_Click(object sender, RoutedEventArgs e)
        {
            activeDM = DrawMode.Eraser;
        }
        private void Circle_Button_Click(object sender, RoutedEventArgs e)
        {
            activeDM = DrawMode.Circle;
        }
        private void ColorPicker_Button_Click(object sender, RoutedEventArgs e)
        {
            activeDM = DrawMode.ColorPicker;
        }

        private void REDRAW_Click(object sender, RoutedEventArgs e)
        {
            CreateIconZone(DrawHeigth.Value, DrawWidth.Value);
        }
    }
}

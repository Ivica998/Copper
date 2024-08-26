using Copper.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Copper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        private static readonly double REGION_HEIGTH = 150;

        private static Dictionary<string, Grid> workspaces = new Dictionary<string, Grid>();
        private static Dictionary<string, IconSet> iconSets = new Dictionary<string, IconSet>();
        //private static ObservableCollection<IconScheme> iconSets = new ObservableCollection<IconScheme>();
        private static Grid activeWS;
        private static IconSet activeIS;

        public event PropertyChangedEventHandler PropertyChanged;
        private Grid stickerWindow;

        public Dictionary<string, Grid> Workspaces { get => workspaces;
            set
            {
                workspaces = value;
                //OnPropertyChanged();
            }
        }

        public Dictionary<string, IconSet> IconSets
        {
            get => iconSets;
            set
            {
                iconSets = value;
                OnPropertyChanged();
            }
        }
        public Grid ActiveWS { get => activeWS; set => activeWS = value; }

        /*
        internal void Refresh(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("cbWS"));
        }
         */


        public MainWindow()
        {
            Style = FindResource(typeof(Window)) as Style;
            DataContext = this;
            InitializeComponent();
            LoadData();
            miStickerWindow.IsChecked = true;

            Closed += ClosingMW;

        }

        private void LoadData()
        {
            IconSet ics = new IconSet();
            iconSets.Add(ics.Name,ics);
            AppData.iconSetData.Add(ics.Canvas.Name, ics);
            LoadWorkspaces();
            LoadIconSchemes();
        }

        public void LoadIconSchemes()
        {
            string path;
            DirectoryInfo di;

            path = Directory.GetCurrentDirectory() + "\\Data\\IconSchemes";
            if (Directory.Exists(path))
                di = new DirectoryInfo(path);
            else
                di = Directory.CreateDirectory(path);

            foreach (var iconSFile in di.GetFiles())
            {
                if (iconSFile.Extension != ".txt")
                    continue;
                IconSet iconSet = IconSet.Load<IconScheme>(iconSFile);

                AppData.iconSetData.Add(iconSet.Canvas.Name, iconSet);
            }

            path = Directory.GetCurrentDirectory() + "\\Data\\IconSets";
            if (Directory.Exists(path))
                di = new DirectoryInfo(path);
            else
                di = Directory.CreateDirectory(path);
            foreach (var iconSFile in di.GetFiles())
            {
                if (iconSFile.Extension != ".txt")
                    continue;
                IconSet iconSet = IconSet.Load<IconSet>(iconSFile);

                IconSets.Add(iconSet.Name, iconSet);
            }
            if (IconSets.Count > 0)
                cbIS.SelectedIndex = 0;
            activeIS = cbIS.SelectedValue as IconSet;
        }

        private void LoadWorkspaces()
        {
            DirectoryInfo di;
            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\Data\\WorkspacesRTF"))
                di = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Data\\WorkspacesRTF");
            else
                di = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Data\\WorkspacesRTF");
            foreach (var wsDir in di.GetDirectories())
            {
                int index = 0;
                Grid ws = new Grid();
                ws.ColumnDefinitions.Add(new ColumnDefinition());
                ws.ColumnDefinitions.Add(new ColumnDefinition());
                Workspaces.Add(wsDir.Name, ws);
                foreach (var panelFile in wsDir.GetFiles())
                {
                    if (panelFile.Extension != ".txt" && panelFile.Extension != ".rtf")
                        continue;
                    RichTextBox rtb = new RichTextBox();
                    rtb.HorizontalAlignment = index % 2 == 0 ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                    rtb.VerticalAlignment = VerticalAlignment.Top;
                    rtb.Height = 200;
                    rtb.PreviewMouseLeftButtonDown += RichTextBox_PreviewMouseLeftButtonDown;
                    //rtb.Margin = new Thickness(10, Math.Floor((double)index / 2) * REGION_HEIGTH, 10, 0);
                    rtb.Style = FindResource(typeof(TextBoxBase)) as Style;

                    if (index % 2 == 0)
                    {
                        RowDefinition rd = new RowDefinition();
                        rd.Height = new GridLength(REGION_HEIGTH, GridUnitType.Pixel);
                        ws.RowDefinitions.Add(rd);
                    }
                    ws.Children.Add(rtb);
                    Grid.SetColumn(rtb, index % 2);
                    Grid.SetRow(rtb, index / 2);
                    index++;



                    rtb.Selection.Load(panelFile.OpenRead(), DataFormats.Rtf);
                    //string text = panelFile.OpenText().ReadToEnd();
                    //TextRange tr = new TextRange(rtb.Document.ContentStart,rtb.Document.ContentEnd);
                    //tr.Text = text;
                }
            }
            if (Workspaces.Count > 0)
                cbWS.SelectedIndex = 0;
                //activeWS = Workspaces.First().Value;
        }

        private void ClosingMW(object sender, EventArgs e)
        {
            SaveData();
        }

        private void SaveData()
        {
            SaveWorkSpace();
            //SaveIconScheme();
        }

        private void SaveWorkSpace()
        {
            foreach (var wsk in Workspaces.Keys)
            {
                Directory.CreateDirectory($"Data\\WorkspacesRTF\\{wsk}");
                foreach (RichTextBox panel in workspaces[wsk].Children)
                {
                    FileStream f = File.Create($"Data\\WorkspacesRTF\\{wsk}\\{workspaces[wsk].Children.IndexOf(panel)}.rtf");
                    //StreamWriter sw = new StreamWriter(f);
                    TextRange tr = new TextRange(panel.Document.ContentStart, panel.Document.ContentEnd);
                    tr.Save(f, DataFormats.Rtf);
                    //sw.Write(tr.Text);
                    //sw.Close();
                    f.Close();

                }
            }
        }

        private void SaveIconScheme()
        {
            //Directory.CreateDirectory($"Data\\IconSchemes");
            foreach (var iconSet in IconSets.Values)
            {
                Directory.CreateDirectory($"Data\\IconSchemes\\{iconSet.Name}");
                foreach (var component in iconSet.Components)
                {
                    int counter = 1;
                    string extra = "";
                    while (File.Exists($"Data\\IconSchemes\\{iconSet.Name}\\{component.Name}" + extra + ".txt"))
                    {
                        extra = "(" + counter++.ToString() + ")";
                    }
                    component.Save($"Data\\IconSchemes\\{iconSet.Name}\\{component.Name}" + extra + ".txt");
                }
            }

            //Directory.CreateDirectory($"Data\\IconSets");
            foreach (var iconSet in IconSets.Values)
            {
                int counter = 1;
                string extra = "";
                while (File.Exists($"Data\\IconSets\\{iconSet.Name}" + extra + ".txt"))
                {
                    extra = "(" + counter++.ToString() + ")";
                }
                iconSet.Save($"Data\\IconSets\\{iconSet.Name}" + extra + ".txt");
            }
        }

        private void Copy_Button_Click(object sender, RoutedEventArgs e)
        {
            IInputElement focusedControl = FocusManager.GetFocusedElement(this);
            if(focusedControl is RichTextBox rtb)
            {
                rtb.Copy();
            }
        }

        private void Paste_Button_Click(object sender, RoutedEventArgs e)
        {
            IInputElement focusedControl = FocusManager.GetFocusedElement(this);
            if (focusedControl is RichTextBox rtb)
            {
                rtb.Paste();
            }
        }
        private void Cut_Button_Click(object sender, RoutedEventArgs e)
        {
            IInputElement focusedControl = FocusManager.GetFocusedElement(this);
            if (focusedControl is RichTextBox rtb)
            {
                rtb.Cut();
            }
        }

        private void New_Workspace_Button_Click(object sender, RoutedEventArgs e)
        {
            new WorkspaceCreator(this).ShowDialog();
            
        }

        private void New_Region_Button_Click(object sender, RoutedEventArgs e)
        {
            if(activeWS != null)
            {
                int index = activeWS.Children.Count;

                RichTextBox rtb = new RichTextBox();
                rtb.HorizontalAlignment = index % 2 == 0 ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                rtb.VerticalAlignment = VerticalAlignment.Top;
                rtb.Height = 200;
                rtb.PreviewMouseLeftButtonDown += RichTextBox_PreviewMouseLeftButtonDown;
                //rtb.Margin = new Thickness(10, Math.Floor((double)index / 2) * REGION_HEIGTH, 10, 0);
                rtb.Style = FindResource(typeof(TextBoxBase)) as Style;

                if(index % 2 == 0)
                {
                    RowDefinition rd = new RowDefinition();
                    rd.Height = new GridLength(REGION_HEIGTH, GridUnitType.Pixel);
                    activeWS.RowDefinitions.Add(rd);
                }
                activeWS.Children.Add(rtb);
                Grid.SetColumn(rtb, index % 2);
                Grid.SetRow(rtb, index / 2);
               
            }
        }

        private void Rtb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(Keyboard.Modifiers == ModifierKeys.Control)
            {
                new RegionViewer(sender as RichTextBox).ShowDialog();
            }
        }

        private void New_Panel_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void New_Group_Button_Click(object sender, RoutedEventArgs e)
        {

        }


        private void RichTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                new RegionViewer(sender as RichTextBox).ShowDialog();
            }
            e.Handled = false;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void StickerWindow_Checked(object sender, RoutedEventArgs e)
        {
            gridZone1.MinWidth = 100;
            gridSplitter1.Width = 3;

            Grid sw = GetStickerWindow();
            wHolder.Children.Add(sw);
            Grid.SetRow(sw, 0);
            Grid.SetColumn(sw, 3);
        }

        private void StickerWindow_Unchecked(object sender, RoutedEventArgs e)
        {
            gridZone1.MinWidth = 0;
            wHolder.Children.Remove(stickerWindow);
            gridZone1.Width = new GridLength(0,GridUnitType.Pixel);
            gridSplitter1.Width = 0;
        }

        private Grid GetStickerWindow()
        {
            if (stickerWindow == null)
            {
                // Create a name scope for the page.
                //NameScope.SetNameScope(this, new NameScope());

                stickerWindow = new Grid();
                stickerWindow.Name = "stickerWindow";
                this.RegisterName(stickerWindow.Name, stickerWindow);
                Binding binding = new Binding("Width") { ElementName = "gridZone1" };
                BindingOperations.SetBinding(stickerWindow, WidthProperty, binding);
                stickerWindow.Height = double.NaN;
                stickerWindow.Background = new SolidColorBrush(Color.FromRgb(0, 50, 50));
                stickerWindow.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                stickerWindow.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                stickerWindow.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                

                Grid header = new Grid();
                header.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                header.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                Button iks = new Button();
                iks.Style = FindResource(typeof(Button)) as Style;
                iks.Click += (object sender, RoutedEventArgs e) => { miStickerWindow.IsChecked = false; };
                iks.HorizontalAlignment = HorizontalAlignment.Right;
                iks.BorderBrush = Brushes.Black;
                iks.BorderThickness = new Thickness(1,1,0,0);
                iks.Content = "X";
                Binding b = new Binding("ActualHeight") { RelativeSource = new RelativeSource() { Mode = RelativeSourceMode.Self } };
                BindingOperations.SetBinding(iks, WidthProperty, b);
                header.Children.Add(iks);
                Grid.SetColumn(iks, 0);
                Grid.SetRow(iks, 1);
                stickerWindow.Children.Add(header);
                Grid.SetColumn(header, 0);
                Grid.SetRow(header, 0);

                ToolBarTray tbt = new ToolBarTray();
                //tbt.HorizontalAlignment = HorizontalAlignment.Stretch;
                tbt.VerticalAlignment = VerticalAlignment.Top;
                ToolBar tb = new ToolBar();
                tb.Band = 0;
                tb.BandIndex = 1;
                tb.Background = Brushes.Beige;
                Button but = new Button();
                but.Width = double.NaN;
                but.Height = double.NaN;
                but.Content = "Icons";
                but.Style = FindResource(typeof(Button)) as Style;
                but.Click += Icons_Click;
                Button b2 = new Button();
                b2.Width = double.NaN;
                b2.Height = double.NaN;
                b2.Content = "Maker";
                b2.Style = FindResource(typeof(Button)) as Style;
                b2.Click += (x, e) => { new IconCreator().ShowDialog(); };
                tb.Items.Add(but);
                tb.Items.Add(b2);
                tbt.ToolBars.Add(tb);
                stickerWindow.Children.Add(tbt);
                Grid.SetColumn(tbt, 0);
                Grid.SetRow(tbt, 1);

                Grid container = new Grid();
                container.Name = "container";
                this.RegisterName(container.Name, container);
                container.Children.Add(activeIS.Canvas);
                stickerWindow.Children.Add(container);
                Grid.SetColumn(container, 0);
                Grid.SetRow(container, 2);
                
            }
            
            return stickerWindow;
        }

        

        private void Icons_Click(object sender, RoutedEventArgs e)
        {
            new Icons().Show();
        }

        private void Button100_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            //Process.Start("notepad.exe", Directory.GetCurrentDirectory() + "\\Data\\Workspaces\\ws-aaaa\\2.txt");
            //psi.Arguments = "gcc sourcefile_name.cs -o outputfile.exe";
            //cmd / K ""C:\Users\Ivica\Desktop\Rad\Program attempts\Copper\Copper\bin\Debug\netcoreapp3.1\Data\Workspaces\ws-aaaa""
            // cd "C:\Users\Ivica\Desktop\Rad\Program attempts\Copper\Copper\bin\Debug\netcoreapp3.1\Data\Workspaces\ws-aaaa" && csc test.cs && test.exe
            psi.FileName = "cmd.exe";
            psi.Arguments = @"/K cd ""C:\Users\Ivica\Desktop\Rad\Program attempts\Copper\Copper\bin\Debug\netcoreapp3.1\Data\Workspaces\ws-aaaa"""
                            + @" && csc test.cs && test.exe > 1.txt && exit";
            Process.Start(psi);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            sv1.Content = activeWS;
        }

        private void cbIS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vb = FindName("container") as Grid;
            if (vb != null)
                vb.Children.Add(activeIS.Canvas);
        }
    }
}

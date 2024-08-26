using System.IO;
using System.Windows;
using System.Windows.Controls;
using Copper.Model;

namespace Copper
{
    /// <summary>
    /// Interaction logic for Icons.xaml
    /// </summary>
    public partial class Icons : Window
    {
        public Icons()
        {
            Style = FindResource(typeof(Window)) as Style;

            InitializeComponent();

            WrapPanel ugrid = new WrapPanel();
            ugrid.Style = FindResource("MyPanel") as Style;
            topGrid.Children.Add(ugrid);
            Grid.SetRow(ugrid, 1);
            //ugrid.Columns = 10;
            //ugrid.Rows = 10;
            DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Data\\Icons");
            var fileInfos = di.GetFiles("*.png");
            foreach (var fileinfo in fileInfos)
            {
                Border icon = IconModel.Make(fileinfo);
                ugrid.Children.Add(icon);
            }
        }

        

       

    }
}

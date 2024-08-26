using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Copper
{
    /// <summary>
    /// Interaction logic for IconSchemeViewer.xaml
    /// </summary>
    public partial class IconSchemeViewer : Window
    {
        private IconScheme myScheme;
        public IconSchemeViewer(IconScheme ish)
        {
            Style = FindResource(typeof(Window)) as Style;

            if (!AppData.iconSetData.ContainsKey(ish.Canvas.Name))
            {
                AppData.iconSetData.Add(ish.Canvas.Name, ish);
            }
            myScheme = ish;

            InitializeComponent();

        }

    }
}

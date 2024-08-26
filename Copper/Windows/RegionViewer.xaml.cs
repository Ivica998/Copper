using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for RegionViewer.xaml
    /// </summary>
    public partial class RegionViewer : Window
    {
        TextRange tr;
        RichTextBox rtbOld;
        public RegionViewer(RichTextBox rtbOld)
        {
            Style = FindResource(typeof(Window)) as Style;

            InitializeComponent();
            Closed += ClosingRV;

            this.rtbOld = rtbOld;
            tr = new TextRange(rtbOld.Document.ContentStart, rtbOld.Document.ContentEnd);
            using (MemoryStream ms = new MemoryStream())
            {
                tr.Save(ms, DataFormats.Rtf);
                tr = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                tr.Load(ms, DataFormats.Rtf);
            }
        }

        private void ClosingRV(object sender, EventArgs e)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                tr.Save(ms, DataFormats.Rtf);
                tr = new TextRange(rtbOld.Document.ContentStart, rtbOld.Document.ContentEnd);
                tr.Load(ms, DataFormats.Rtf);
            }
        }
    }
}

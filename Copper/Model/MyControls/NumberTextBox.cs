using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Copper.Model.MyControls
{
    public class NumberTextBox : TextBox
    {
        public bool IsValid { get; set; }
        private static readonly Regex regexChar = new Regex("^[0-9]+$$");
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (!regexChar.IsMatch(e.Text))
                e.Handled = true;
            base.OnPreviewTextInput(e);
        }
        public NumberTextBox()
        {
            TextChanged += Ntb_TextChanged;
        }
        private static readonly Regex regex = new Regex("^[0-9]+$$");
        private void Ntb_TextChanged(object sender, TextChangedEventArgs e)
        {
            NumberTextBox ntb = sender as NumberTextBox;
            if (regex.IsMatch(ntb.Text))
            {
                ntb.Foreground = Brushes.Black;
                IsValid = true;
            }
            else
            {
                ntb.Foreground = Brushes.Red;
                IsValid = false;
            }
        }
    }

}

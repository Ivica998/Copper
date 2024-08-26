using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Copper.Model.MyControls
{
    public class HexTextBox : TextBox
    {
        public bool IsValid { get; set; }
        private static readonly Regex regexChar = new Regex("^[#a-fA-F0-9]+$$");
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (!regexChar.IsMatch(e.Text))
                e.Handled = true;
            base.OnPreviewTextInput(e);
        }
        public HexTextBox()
        {
            TextChanged += Htb_TextChanged;
            MaxLength = 9;
        }
        private static readonly Regex regex = new Regex("^#[a-fA-F0-9]+$$");
        private void Htb_TextChanged(object sender, TextChangedEventArgs e)
        {
            HexTextBox htb = sender as HexTextBox;
            if (regex.IsMatch(htb.Text) && (htb.Text.Length == 9 || htb.Text.Length == 11))
            {
                htb.Foreground = Brushes.Black;
                IsValid = true;
            }
            else
            {
                htb.Foreground = Brushes.Red;
                IsValid = false;
            }
        }
    }

}

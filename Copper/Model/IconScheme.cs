using Copper.Helpers;
using Copper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace Copper
{
    public class IconScheme : IconSet
    {
        private Border holder;
        private DateTime creatinDate;
        private int id;
        public Border Holder { get => holder; set => holder = value; }
        public DateTime CreatinDate { get => creatinDate; set => creatinDate = value; }
        public int Id { get => id; set => id = value; }
        
        public IconScheme()
        {

        }
        public IconScheme(Border icon = null)
        {
            Holder = icon;
            CreatinDate = DateTime.Now;
            Id = GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }

}

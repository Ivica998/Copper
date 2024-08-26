using Copper.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Copper
{
    public static class AppData
    {
        public static Dictionary<string, IconSet> iconSetData = new Dictionary<string, IconSet>();

        public static bool ExecuteIcon(object obj)
        {
            Border icon = obj as Border;
            if (!iconSetData.ContainsKey(icon.Name))
                return false;

            IconSet myScheme = iconSetData[icon.Name];
            myScheme.Execute();

            return true;
        }

        public static IconSet MappedIconSet(this Panel panel)
        {
            if(iconSetData.ContainsKey(panel.Name))
            {
                return iconSetData[panel.Name];
            }
            return null;
        }

        public static IconSet MappedIconSet(this Border border)
        {
            foreach (var item in iconSetData.Values)
            {
                if (item is IconScheme ish && ish.Holder == border)
                {
                    return ish;
                }
            }
            return null;
        }


    }
}

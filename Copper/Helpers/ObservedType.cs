using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Copper.Helpers
{
    public class ObservedType<T> : INotifyPropertyChanged where T : new()
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private T val = new T();
        public T Value
        {
            get => val;
            set
            {
                if (!val.Equals(value))
                {
                    val = value;
                    OnPropertyChanged();
                }
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

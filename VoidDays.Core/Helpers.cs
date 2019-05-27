using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidDays.Core
{
    public static class Helpers
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }
        public static ObservableCollection<T> ToObservableCollection<T>(this ICollection<T> source)
        {
            return new ObservableCollection<T>(source);
        }
        public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> list)
        {
            foreach (var item in list)
                source.Add(item);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Li.Krkr.krkrfgformatWPF.Models
{
    public class SelectedItemWithIndexModel
    {
        public int Index { get; set; }
        public object SelectedItem { get; set; }
        public SelectedItemWithIndexModel(int c,object item)
        {
            this.Index = c;
            this.SelectedItem = item;
        }
    }
}

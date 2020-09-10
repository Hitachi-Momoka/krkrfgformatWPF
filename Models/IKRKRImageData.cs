using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Li.Krkr.krkrfgformatWPF.Models
{
    interface IKRKRImageData
    {
        string LayerType { get; }
        string Name { get; }
        string Left { get; }
        string Top { get; }
        string Width { get; }
        string Height { get; }
        string Type { get; }
        string Opacity { get; }
        string Visible { get; }
        string LayerId { get; }
        string GroupLayerId { get; }
        string Base { get; }
        string Images { get; }
    }
}

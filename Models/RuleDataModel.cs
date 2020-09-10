using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Li.Krkr.krkrfgformatWPF.Models
{
    public class RuleDataModel
    {
        public readonly static string textHead = "#layer_type	name	left	top	width	height	type	opacity	visible	layer_id	group_layer_id	base	images	";

        public string OriginalFilePath { set; get; }
        public string FileHander { get; set; }
        public LineDataModel FgLarge { get; set; }
        public List<LineDataModel> TextData { get; set; }
        public RuleDataModel()
        {
            OriginalFilePath = "";
            FileHander = "";
            FgLarge = new LineDataModel();
            TextData = new List<LineDataModel>();
        }
        #region IRuleData
        public LineDataModel GetFgLarge()
        {
            return this.FgLarge;
        }

        public List<LineDataModel> GetTextData()
        {
            return this.TextData;
        }

        public LineDataModel GetLineDataByID(int id)
        {
            return this.TextData.Find(t => t.LayerId == id.ToString());
        }
        public LineDataModel GetLineDataBySize(int w,int h)
        {
            return this.TextData.Find(t => Convert.ToInt32(t.Width) == w && Convert.ToInt32(t.Height) == h);
        }

        public List<LineDataModel> GetLineDatasByGroupLayerID(int id)
        {
            return this.TextData.FindAll(t => t.GroupLayerId == id.ToString());
        }

        public static int GetLineDataVisible(LineDataModel lineData)
        {
            return Convert.ToInt32(lineData.Visible);
        }
        #endregion
    }
}

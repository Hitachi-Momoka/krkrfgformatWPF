using Prism.Mvvm;
using Prism.Commands;
using Microsoft.Win32;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media.Imaging;

using Li.Drawing.Wpf;
using Li.Krkr.krkrfgformatWPF.Helper;
using Li.Krkr.krkrfgformatWPF.Models;
using System.Runtime.Remoting.Messaging;

namespace Li.Krkr.krkrfgformatWPF.ViewModes
{
    //public delegate ImageSource CutImageBlankHandler(BitmapSource source);
    public partial class MainWindowViewModel : BindableBase
    {
        private string GetRulePath(string imagePath)
        {
            string dir = System.IO.Path.GetDirectoryName(imagePath);
            var namePart = System.IO.Path.GetFileNameWithoutExtension(imagePath).Split('_');
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < namePart.Length - 1; i++)
            {
                sb.Append(namePart[i]);
                if (i < namePart.Length - 2)
                {
                    sb.Append('_');
                }
            }
            string ruleFileName = sb.ToString();
            foreach (var format in SupportedFormat.RuleDataFormat)
            {
                string tmpPath = $"{dir}\\{ruleFileName}{format}";
                if (File.Exists(tmpPath))
                    return tmpPath;
            }
            return null;
        }

        private string CreatDefultSavePath(string v)
        {
            var newDir = System.IO.Path.GetDirectoryName(v) + @"\合成输出\";
            if(!System.IO.Directory.Exists(newDir))
            {
                System.IO.Directory.CreateDirectory(newDir);
            }
            return newDir;
        }

        private void UpDataAllSelectedItems(SelectedItemWithIndexModel item)
        {
            if (item == null) return;
            canUpData = false;
            foreach (var i in AllSelectedItems)
            {
                if(i.Index == item.Index)
                {
                    AllSelectedItems.Remove(i);
                    break;
                }
            }
            canUpData = true;
            AllSelectedItems.Add(item);
            
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (isClearing) return;//如果在执行清理操作，不执行操作
            if (!canUpData) return;//移除集合内容时不激发合成操作。
            if (this.AllSelectedItems.Count == 0) return;//包含东西的时候不执行操作。
            if(RuleData == null)
            {
                WithoutRuleDataMode();
                return;
            }

            string strtmp = System.IO.Path.GetFileNameWithoutExtension(RulePath);
            SortedDictionary<int, string> dic = new SortedDictionary<int, string>();
            foreach (var item in this.AllSelectedItems)
            {
                dic.Add(item.Index, item.SelectedItem.ToString());
            }
            PictureMixer mixer = new PictureMixer(Convert.ToInt32(this.RuleData.FgLarge.Width), Convert.ToInt32(this.RuleData.FgLarge.Height));
            foreach (var item in dic)
            {
                strtmp += "+";
                var image = WPFPictureHelper.GreateBitmapFromFile(item.Value); //new BitmapImage(new Uri(item.Value));
                LineDataModel line;
                if(this.IsSideOnly)
                {
                    line = this.RuleData.GetLineDataBySize(image.PixelWidth, image.PixelHeight);
                }
                else
                {
                    line = this.RuleData.GetLineDataByID(Helper.Helper.GetFileCode(item.Value));
                }
                strtmp += line.LayerId;
                mixer.AddPicture(image,line.ToRect(),Convert.ToInt32(line.Opacity)); 
            }
            SaveName = strtmp;
            
            BitmapSource source = WPFPictureHelper.DrawingImageToBitmapSource(mixer.OutImage);
            this.ImageBoxSource = WPFPictureHelper.CutImageBlank(source);
            //GC.Collect();
        }

        private void WithoutRuleDataMode()
        {
            if (!messageBoxIsShow)
            {
                var result = MessageBox.Show("需要一个规则文件或者规则文件不受支持。\n是否手动选择文件？\n（否只会显示当前最后一次选择的图片。）", "错误", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    this.SelectRulePath();
                }
                messageBoxIsShow = true;
                return;
            }
            if (SelectItemTmp == null) return;
            BitmapImage image = new BitmapImage(new Uri(this.SelectItemTmp.SelectedItem.ToString()));
            ImageBoxSource = new System.Windows.Media.DrawingImage() { Drawing = new ImageDrawing(image, new Rect(0, 0, image.PixelWidth, image.PixelHeight))};
        }

        public void FormatSelected()
        {
            if (this.ImageBoxSource == null) return;
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)ImageBoxSource));
            string newname = $"{SavePath}\\{SaveName}.png";
            using (var stream = new FileStream(newname, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }
        public void SelectRulePath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "文本文档(*.txt)|*.txt|json文件(*.json)|*.json|所有文件 (*.*)|*.*",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                RulePath = openFileDialog.FileName;
            }
        }
        public void OpenSaveFloder()
        {
            if (!string.IsNullOrEmpty(SavePath))
                System.Diagnostics.Process.Start("explorer.exe", SavePath);
        }
        public void EmptyMethod()
        {

        }
        public void ClearSelected()
        {
            this.AllSelectedItems.CollectionChanged -= this.CollectionChanged;
            this.AllSelectedItems.Clear();
            this.ImageBoxSource = null;
            GC.Collect();
            this.AllSelectedItems.CollectionChanged += this.CollectionChanged;
        }
        public void ClearAll()
        {
            this.AllSelectedItems.CollectionChanged -= this.CollectionChanged;
            this.messageBoxIsShow = true;
            this.canUpData = true;
            this.isClearing = false;

            this.SelectItemTmp = null;
            this.RulePath = "";
            this.IsSideOnly = false;
            this.SavePath = "";
            this.SaveName = "";
            this.RuleData = null;
            this.ImageBoxSource = null;

            this.AllSelectedItems = new ObservableCollection<SelectedItemWithIndexModel>();
            this.AllSelectedItems.CollectionChanged += this.CollectionChanged;
            this.messageBoxIsShow = false;

            GC.Collect();
        }
    }
}

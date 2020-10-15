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
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace Li.Krkr.krkrfgformatWPF.ViewModes
{
    public partial class MainWindowViewModel : BindableBase
    {
        public ImageSource ImageBoxSourcetmp
        {
            get { return imageBoxSourcetmp; }
            set 
            { 
                imageBoxSourcetmp = value;
            }
        }

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
            foreach (var format in SupportedFileExtension.RuleDataExtension)
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
        private void UpDataAllItems(SelectedItemWithIndexModel item)
        {
            if (item == null) return;
            bool needtodelect = false;
            foreach (var i in AllItems)
            {
                if(i.Key==item.Index)
                {
                    needtodelect = true;
                    break;
                }
            }
            if (needtodelect) { AllItems.Remove(item.Index); }
            AllItems.Add(item.Index, new Tuple<string, BitmapSource>(item.SelectedItem.ToString(), WPFPictureHelper.GreateBitmapFromFile(item.SelectedItem.ToString())));
            UpdateImage();
        }
        private void UpdateImage()
        {
            if (AllItems.Count == 0) return;
            if (RuleData == null)
            {
                WithoutRuleDataMode();
                return;
            }
            string strtmp = System.IO.Path.GetFileNameWithoutExtension(RulePath);
            PictureMixer mixer = new PictureMixer();
            foreach (var item in AllItems)
            {
                strtmp += "+";
                LineDataModel line;
                if (this.IsSideOnly)
                {
                    line = this.RuleData.GetLineDataBySize(item.Value.Item2.PixelWidth, item.Value.Item2.PixelHeight);
                }
                else
                {
                    line = this.RuleData.GetLineDataByID(Helper.Helper.GetFileCode(item.Value.Item1)); ;
                }
                strtmp += line.LayerId;
                mixer.AddPicture(item.Value.Item2, line.ToRect(), Convert.ToInt32(line.Opacity));
            }
            SaveName = strtmp;
            //BitmapSource source = WPFPictureHelper.DrawingImageToBitmapSource(mixer.OutImage);
            this.ImageBoxSource = mixer.OutImage;
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
            encoder.Frames.Add(BitmapFrame.Create(((DrawingImage)ImageBoxSource).ToBitmapSource()));
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
            
            this.AllItems.Clear();
            this.ImageBoxSource = null;
            this.ImageBoxFrameSource = null;
            GC.Collect();
        }
        public void ClearAll()
        {
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
            this.ImageBoxFrameSource = null;

            this.AllItems = new SortedDictionary<int, Tuple<string, BitmapSource>>();
            this.messageBoxIsShow = false;

            GC.Collect();
        }
    }
}

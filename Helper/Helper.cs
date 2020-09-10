using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Li.Krkr.krkrfgformatWPF.Helper
{
    public static class Helper
    {
        public static int GetFileCode(string path)
        {
            var part = System.IO.Path.GetFileName(path).Split('.')[0].Split('_');
            return Convert.ToInt32(part[part.Length - 1]);
        }
    }
    public static class Extension
    {
        //Code from "https://www.it1352.com/1253670.html".
        public static BitmapSource ToBitmapSource(this DrawingImage source)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(source, new Rect(new Point(0, 0), new Size(source.Width, source.Height)));
            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)source.Width, (int)source.Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            return bmp;
        }
        public static DrawingImage ToDrawingImage(this BitmapSource source)
        {
            Rect imageRect = new Rect(0, 0, source.PixelWidth, source.PixelHeight);
            ImageDrawing drawing = new ImageDrawing(source, imageRect);
            return new DrawingImage(drawing);
        }
    }
}

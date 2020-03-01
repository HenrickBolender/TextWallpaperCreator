using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using Size = System.Windows.Size;

namespace TextWallpaperCreator
{
    public class ImageGenerator
    {
        private static readonly FontFamily fontFamily;

        static ImageGenerator()
        {
            var collection = new PrivateFontCollection();
            collection.AddFontFile(Path.Combine(Environment.CurrentDirectory, @"Fonts\TitilliumWeb-Bold.ttf"));
            fontFamily = new FontFamily("Titillium Web", collection);
        }

        public static Bitmap CreateImage(string text, Color backgroundColor, Color fontColor, float fontSize,
            Size inAppStringSize)
        {
            var screenResolution = Screen.PrimaryScreen.Bounds;
            var bmp = new Bitmap(screenResolution.Width, screenResolution.Height);
            var graphics = Graphics.FromImage(bmp);


            var font = new Font(fontFamily, fontSize, FontStyle.Bold);


            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            var imageStringSizeWidth =
                graphics.MeasureString(text, font, new SizeF(float.PositiveInfinity, float.PositiveInfinity),
                    StringFormat.GenericTypographic).Width;


            font = new Font(fontFamily, (float) (fontSize * inAppStringSize.Width / imageStringSizeWidth),
                FontStyle.Bold);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.FillRectangle(new SolidBrush(backgroundColor), screenResolution);
            Console.WriteLine($"o image {graphics.MeasureString(text, font)}");
            graphics.DrawString(text,
                font,
                new SolidBrush(fontColor),screenResolution.Width / 2, screenResolution.Height / 2,  sf);
            return bmp;
        }
    }
}
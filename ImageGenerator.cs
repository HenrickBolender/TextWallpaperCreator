using System;
using System.Collections.Generic;
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
        private static readonly List<FontFamily> families;
        private static readonly FontFamily defaultFontFamily;
        private const float wpfToWinFormsFontRatio = 91f / 72;

        static ImageGenerator()
        {
            var collection = new PrivateFontCollection();

            collection.AddFontFile(Path.Combine(Environment.CurrentDirectory, @"Fonts\TitilliumWeb-Bold.ttf"));
            defaultFontFamily = new FontFamily("Titillium Web", collection);

            var f = FontFamily.Families;
            families = new List<FontFamily>();
            families.AddRange(f);
        }

        public static Bitmap CreateImage(string text, Color backgroundColor, Color fontColor, float fontSize,
            string fontFamilyName)
        {
            var screenResolution = Screen.PrimaryScreen.Bounds;
            var bmp = new Bitmap(screenResolution.Width, screenResolution.Height);
            var graphics = Graphics.FromImage(bmp);

            var fontFamily = families.Find(ff => ff.Name == fontFamilyName) ?? defaultFontFamily;
            
            var font = new Font(fontFamily, fontSize * wpfToWinFormsFontRatio, FontStyle.Regular);

            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.FillRectangle(new SolidBrush(backgroundColor), screenResolution);
            graphics.DrawString(text,
                font,
                new SolidBrush(fontColor), screenResolution.Width / 2, screenResolution.Height / 2, sf);
            return bmp;
        }
    }
}
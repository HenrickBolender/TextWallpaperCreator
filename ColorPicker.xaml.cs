using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace TextWallpaperCreator
{
    public partial class ColorPicker : UserControl
    {
        public readonly Dictionary<ElementToColor, Color> elementColors;
        private Color selectedColor;
        private bool IsMouseDown;
        private ElementToColor currentElementToColor;

        public enum ElementToColor
        {
            TextBox,
            Background
        }

        public ColorPicker()
        {
            InitializeComponent();
            selectedColor = Color.White;
            currentElementToColor = ElementToColor.Background;
            elementColors = new Dictionary<ElementToColor, Color>
            {
                {ElementToColor.TextBox, Color.Black},
                {ElementToColor.Background, SelectedColor}
            };
        }
        public Color SelectedColor
        {
            get => selectedColor;
            private set
            {
                if (selectedColor == value) 
                    return;
                selectedColor = value;
            }
        }

        private void CanvasImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown) 
                UpdateColor();
        }

        private void CanvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            UpdateColor();
        }

        private void CanvasImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = false;
        }

        public void SwitchElementToColor(ElementToColor element)
        {
            if (element == currentElementToColor)
                return;
            elementColors[currentElementToColor] = SelectedColor;
            currentElementToColor = element;
            SelectedColor = elementColors[currentElementToColor];
            UpdateCursorEllipse(SelectedColor);
        }

        private void UpdateColor()
        {
            var imageX = (int) Mouse.GetPosition(canvasImage).X;
            var imageY = (int) Mouse.GetPosition(canvasImage).Y;
            if (imageX < 0 || imageY < 0 || imageX > ColorImage.Width - 1 || imageY > ColorImage.Height - 1) 
                return;

            var cb = new CroppedBitmap(ColorImage.Source as BitmapSource, new Int32Rect(imageX, imageY, 1, 1));
            var pixels = new byte[4];
            cb.CopyPixels(pixels, 4, 0);

            ellipsePixel.SetValue(Canvas.LeftProperty, Mouse.GetPosition(canvasImage).X - ellipsePixel.Width / 2.0);
            ellipsePixel.SetValue(Canvas.TopProperty, Mouse.GetPosition(canvasImage).Y - ellipsePixel.Width / 2.0);
            canvasImage.InvalidateVisual();

            SelectedColor = Color.FromArgb(pixels[2], pixels[1], pixels[0]);
            elementColors[currentElementToColor] = selectedColor;

            var brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(selectedColor.R, selectedColor.G, selectedColor.B));

            var mainWindow = Application.Current.MainWindow as MainWindow;
            switch (currentElementToColor)
            {
                case ElementToColor.TextBox:
                    mainWindow.WallpaperText.Foreground = brush;
                    break;

                case ElementToColor.Background:
                    mainWindow.WallpaperBackground.Fill = brush;
                    break;
            }
        }

        private void UpdateCursorEllipse(Color searchColor)
        {
            var tempColor = new Color();
            var pixels = new byte[4];
            var searchY = 0;
            var searchX = 0;
            for (searchY = 0; searchY <= canvasImage.Width - 1; searchY++)
            {
                for (searchX = 0; searchX <= canvasImage.Height - 1; searchX++)
                {
                    var croppedBitmap = 
                        new CroppedBitmap(ColorImage.Source as BitmapSource, new Int32Rect(searchX, searchY, 1, 1));
                    croppedBitmap.CopyPixels(pixels, 4, 0);
                    tempColor = Color.FromArgb(255, pixels[2], pixels[1], pixels[0]);
                    if (tempColor == searchColor) break;
                }
                if (tempColor == searchColor) break;
            }
            if (tempColor != searchColor)
            {
                searchX = 0;
                searchY = 0;
            }
            ellipsePixel.SetValue(Canvas.LeftProperty, searchX - ellipsePixel.Width / 2.0);
            ellipsePixel.SetValue(Canvas.TopProperty, searchY - ellipsePixel.Width / 2.0);
        }


    }
}
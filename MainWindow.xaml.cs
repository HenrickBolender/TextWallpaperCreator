using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Media;
using ComboBox = System.Windows.Controls.ComboBox;

namespace TextWallpaperCreator
{
    public partial class MainWindow : Window
    {
        private readonly ColorPicker colorPicker;
        private float aspectRatio;
        private double defaultHeight;
        private double defaultToolsWidth;
        private readonly bool componentsInitialized;
        private const double minFontSize = 30f;
        private const double maxFontSize = 150f;
        private double actualFontSize;

        private double ActualFontSize
        {
            get => actualFontSize;
            set
            {
                actualFontSize = value;
                UpdateWallpaperFontSize();
            }
        }

        public MainWindow()
        {
            componentsInitialized = false;
            InitializeComponent();
            componentsInitialized = true;
            colorPicker = (ColorPicker) FindName("ColorPicker");
            InitializeFontSelector();
            InitializeWindowSize();
        }

        private void InitializeFontSelector()
        {
            var sysDrawingFamilies = System.Drawing.FontFamily.Families;
            
            var commonFontFamilies =
                Fonts.SystemFontFamilies
                    .Where(
                        ff =>
                            sysDrawingFamilies
                                .Select(sdff => sdff.GetName(0)).Contains(ff.Source)
                    );
            fonts.AddRange(commonFontFamilies);
            FontSelector.ItemsSource = fonts;
        }

        private void InitializeWindowSize()
        {
            defaultToolsWidth = MainGrid.ColumnDefinitions[0].Width.Value;
            aspectRatio = (float) Screen.PrimaryScreen.Bounds.Height / Screen.PrimaryScreen.Bounds.Width;
            Width = defaultToolsWidth + (Height - SystemParameters.WindowCaptionHeight) / aspectRatio;
            MinWidth = Width;
            defaultHeight = (Width - defaultToolsWidth) * aspectRatio - SystemParameters.WindowCaptionHeight;
        }

        private void UpdateWallpaperFontSize()
        {
            if (componentsInitialized)
                WallpaperText.FontSize = WallpaperBackground.Height / defaultHeight * actualFontSize;
        }

        private void SetImage(object sender, RoutedEventArgs e)
        {
            WallpaperText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var imagePath = Path.Combine(Path.GetTempPath(), $"{DateTime.Now.ToFileTime()}.png");
            
            ImageGenerator.CreateImage(
                    WallpaperText.Text,
                    colorPicker.elementColors[ColorPicker.ElementToColor.Background],
                    colorPicker.elementColors[ColorPicker.ElementToColor.TextBox],
                    (float) ActualFontSize,
                    WallpaperText.FontFamily.Source
                )
                .Save(imagePath, ImageFormat.Png);

            Wallpaper.Set(Wallpaper.Style.Centered, imagePath);
        }

        private void SwitchColorPickerState(object sender, EventArgs e)
        {
            var buttonSender = sender as ToggleButton;
            if (!buttonSender.IsChecked.Value)
                buttonSender.IsChecked = true;
            if (buttonSender.Equals(FontColorButton))
            {
                BackgroundColorButton.IsChecked = false;
                ColorPicker.SwitchElementToColor(ColorPicker.ElementToColor.TextBox);
            }
            else
            {
                FontColorButton.IsChecked = false;
                ColorPicker.SwitchElementToColor(ColorPicker.ElementToColor.Background);
            }
        }

        private void ResizeView(object sender, SizeChangedEventArgs e)
        {
            WallpaperBackground.Height = Math.Min(ActualHeight, (e.NewSize.Width - defaultToolsWidth) * aspectRatio);
            MainGrid.ColumnDefinitions[0].Width =
                new GridLength(Math.Max(e.NewSize.Width - e.NewSize.Height / aspectRatio, defaultToolsWidth));
            UpdateWallpaperFontSize();
        }

        private void OnSliderValueChanged_ResizeFont(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ActualFontSize = minFontSize + (maxFontSize - minFontSize) * e.NewValue / ((Slider) sender).Maximum;
        }

        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            var comboBox = (ComboBox) sender;
            var fontFamily = (FontFamily) comboBox.SelectedItem;
            WallpaperText.FontFamily = fontFamily;
        }
    }
}
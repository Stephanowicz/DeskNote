//============================================================================
// DeskNote 1.x
// Copyright © 2025 Stephanowicz
// 
// <https://github.com/Stephanowicz/DeskNote>
// 
//This file is part of DeskNote.
//
//DeskNote is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//DeskNote is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with DeskNote.  If not, see <http://www.gnu.org/licenses/>.
//
//============================================================================
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DeskNote
{
    /// <summary>
    /// Interaktionslogik für settings.xaml
    /// </summary>
    public partial class settings : Window
    {
        DeskNoteWindow deskNoteForm;
        bool _bInit = true;
        public settings(DeskNoteWindow _f)
        {
            InitializeComponent();
            deskNoteForm = _f;
            if (deskNoteForm._BckgrPic)
            {
                groupBoxBckgr.IsEnabled = false;
                groupBoxBckgrPic.IsEnabled = true;
                _bckgrImage.Fill = deskNoteForm._Grid.Background;
                pMouse.X =colorPickRect.Margin.Left;
                pMouse.Y =colorPickRect.Margin.Top;
            }
            sliderOpacity.Value = (double)(deskNoteForm._BackgroundAlpha * 100 / 255);
            sliderRed.Value = deskNoteForm._BackgroundRed * 100 / 255;
            sliderGreen.Value = deskNoteForm._BackgroundGreen * 100 / 255;
            sliderBlue.Value = deskNoteForm._BackgroundBlue * 100 / 255;
            lblOpacity.Content = sliderOpacity.Value.ToString() + "%";
            lblRed.Content = sliderRed.Value.ToString() + "%";
            lblGreen.Content = sliderGreen.Value.ToString() + "%";
            lblBlue.Content = sliderBlue.Value.ToString() + "%";
            sliderRedMatrix.Value = deskNoteForm._BckgrPicRed * 10;
            sliderGreenMatrix.Value = deskNoteForm._BckgrPicGreen * 10;
            sliderBlueMatrix.Value = deskNoteForm._BckgrPicBlue * 10;
            sliderBrightnessMatrix.Value = deskNoteForm._BckgrPicBrightness * 10;
            sliderContrastMatrix.Value = deskNoteForm._BckgrPicContrast * 10;
            sliderSaturationMatrix.Value = deskNoteForm._BckgrPicSaturation * 10;
            lblRedMatrix.Content = deskNoteForm._BckgrPicRed.ToString("#.##");
            lblGreenMatrix.Content = deskNoteForm._BckgrPicGreen.ToString("#.##");
            lblBlueMatrix.Content = deskNoteForm._BckgrPicBlue.ToString("#.##");
            lblContrastMatrix.Content = deskNoteForm._BckgrPicContrast.ToString("#.##");
            lblBrightnessMatrix.Content = deskNoteForm._BckgrPicBrightness.ToString("#.##");
            lblSaturationMatrix.Content = deskNoteForm._BckgrPicSaturation.ToString("#.##");
            chkbGlass.IsChecked = deskNoteForm._blur;
            chkbImage.IsChecked = deskNoteForm._BckgrPic;
            numUpDown.Value = (int)deskNoteForm.BorderThickness.Bottom;
            colorPickerBackground.SelectedColor = deskNoteForm._cBackColor;
            colorPickerText.SelectedColor = deskNoteForm._TextColor;
            fontSizeSlider.Value = deskNoteForm._cFontSize;
            cbFontFamily.SelectedValue = deskNoteForm._cFontFamily;
            FamilyTypeface tf = new FamilyTypeface();
            tf.Stretch = deskNoteForm._cFontStretch;
            tf.Style = deskNoteForm._cFontStyle;
            tf.Weight = deskNoteForm._cFontWeight;
            cbFontStyle.SelectedValue = tf;
            numUpDownMarginLeft.Value = (int)deskNoteForm.rtbMarginLeft;
            numUpDownMarginRight.Value = (int)deskNoteForm.rtbMarginRight;
            numUpDownMarginTop.Value = (int)deskNoteForm.rtbMarginTop;
            numUpDownMarginBottom.Value = (int)deskNoteForm.rtbMarginBottom;
            listBoxFontFavourites.ItemsSource=deskNoteForm._FontFavourites;
            numUpDownShadowDepth.Value = Convert.ToInt32(deskNoteForm.dropShadow.ShadowDepth);
            numUpDownShadowOpacity.Value = Convert.ToInt32(deskNoteForm.dropShadow.Opacity * 10) ;
            numUpDownShadowDirection.Value = Convert.ToInt32(deskNoteForm.dropShadow.Direction) ;
            foreach(DeskNoteWindow.backgrPicPresets bps in deskNoteForm.bpPresets)
            {
                cboxBackgrPicPresets.Items.Add(bps.presetName);
            }
            cboxBackgrPicPresets.SelectedItem = deskNoteForm._backrPicLast;
            _bInit = false;
        }

        private void sliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_bInit)
            {
                deskNoteForm._BackgroundAlpha = Convert.ToByte(sliderOpacity.Value / 100 * 255);
                lblOpacity.Content = sliderOpacity.Value.ToString() + "%";
                if (!colorPickerBackground.IsOpen) colorPickerBackground.SelectedColor = deskNoteForm._cBackColor;
            }
        }
        private void sliderRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_bInit)
            {
                deskNoteForm._BackgroundRed = Convert.ToByte(sliderRed.Value / 100 * 255);
                lblRed.Content = sliderRed.Value.ToString() + "%";
                if (!colorPickerBackground.IsOpen) colorPickerBackground.SelectedColor = deskNoteForm._cBackColor;
            }
        }
        private void sliderGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_bInit)
            {
                deskNoteForm._BackgroundGreen = Convert.ToByte(sliderGreen.Value / 100 * 255);
                lblGreen.Content = sliderGreen.Value.ToString() + "%";
                if (!colorPickerBackground.IsOpen) colorPickerBackground.SelectedColor = deskNoteForm._cBackColor;
            }
        }
        private void sliderBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_bInit)
            {
                deskNoteForm._BackgroundBlue = Convert.ToByte(sliderBlue.Value / 100 * 255);
                lblBlue.Content = sliderBlue.Value.ToString() + "%";
                if (!colorPickerBackground.IsOpen) colorPickerBackground.SelectedColor = deskNoteForm._cBackColor;
            }
        }

        private void ColorPickerBackground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPickerBackground.IsOpen)
            {
                sliderOpacity.Value = colorPickerBackground.SelectedColor.Value.A * 100 / 255;
                sliderRed.Value = colorPickerBackground.SelectedColor.Value.R * 100 / 255;
                sliderGreen.Value = colorPickerBackground.SelectedColor.Value.G * 100 / 255;
                sliderBlue.Value = colorPickerBackground.SelectedColor.Value.B * 100 / 255;
            }
        }
        private void chkbGlass_Click(object sender, RoutedEventArgs e)
        {
            if (!_bInit) deskNoteForm._blur = ((bool)chkbGlass.IsChecked);
        }
        private void chkbImage_Click(object sender, RoutedEventArgs e)
        {
            if (!_bInit)
            {
                deskNoteForm._BckgrPic = ((bool)chkbImage.IsChecked);
                if (deskNoteForm._BckgrPic)
                {
                    groupBoxBckgr.IsEnabled = false;
                    groupBoxBckgrPic.IsEnabled = true;
                    _bckgrImage.Fill = deskNoteForm._Grid.Background;
                    pMouse.X = colorPickRect.Margin.Left;
                    pMouse.Y = colorPickRect.Margin.Top;
                }
                else
                {
                    groupBoxBckgr.IsEnabled = true;
                    groupBoxBckgrPic.IsEnabled = false;
                }

            }
        }
        private void ColorPickerText_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPickerText.IsOpen)
            {
                deskNoteForm._TextColor = (Color)colorPickerText.SelectedColor;
            }
        }
        private void sliderRedMatrix_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_bInit)
            {
                deskNoteForm._BckgrPicRed = ((float)sliderRedMatrix.Value / 10f);
                lblRedMatrix.Content = deskNoteForm._BckgrPicRed.ToString("#.##");
                deskNoteForm.SetBckgrPicColor();
                _bckgrImage.Fill = deskNoteForm._Grid.Background;
                _getColorPixel();
            }
        }
        private void sliderGreenMatrix_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_bInit)
            {
                deskNoteForm._BckgrPicGreen = ((float)sliderGreenMatrix.Value / 10f);
                lblGreenMatrix.Content = deskNoteForm._BckgrPicGreen.ToString("#.##");
                deskNoteForm.SetBckgrPicColor();
                _bckgrImage.Fill = deskNoteForm._Grid.Background;
                _getColorPixel();
            }
        }
        private void sliderBlueMatrix_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_bInit)
            {
                deskNoteForm._BckgrPicBlue = ((float)sliderBlueMatrix.Value / 10f);
                lblBlueMatrix.Content = deskNoteForm._BckgrPicBlue.ToString("#.##");
                deskNoteForm.SetBckgrPicColor();
                _bckgrImage.Fill = deskNoteForm._Grid.Background;
                _getColorPixel();
            }
        }
        private void sliderContrastMatrix_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_bInit)
            {
                deskNoteForm._BckgrPicContrast = ((float)sliderContrastMatrix.Value / 10f);
                lblContrastMatrix.Content = deskNoteForm._BckgrPicContrast.ToString("#.##");
                deskNoteForm.SetBckgrPicColor();
                _bckgrImage.Fill = deskNoteForm._Grid.Background;
                _getColorPixel();
            }
        }
        private void sliderBrightnessMatrix_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_bInit)
            {
                deskNoteForm._BckgrPicBrightness = ((float)sliderBrightnessMatrix.Value / 10f);
                lblBrightnessMatrix.Content = deskNoteForm._BckgrPicBrightness.ToString("#.##");
                deskNoteForm.SetBckgrPicColor();
                _bckgrImage.Fill = deskNoteForm._Grid.Background;
                _getColorPixel();
            }
        }
        private void sliderSaturationMatrix_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_bInit)
            {
                deskNoteForm._BckgrPicSaturation = ((float)sliderSaturationMatrix.Value / 10f);
                lblSaturationMatrix.Content = deskNoteForm._BckgrPicSaturation.ToString("#.##");
                deskNoteForm.SetBckgrPicColor();
                _bckgrImage.Fill = deskNoteForm._Grid.Background;
                _getColorPixel();
            }
        }

        private void cbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_bInit) deskNoteForm._cFontFamily = (FontFamily)cbFontFamily.SelectedValue;//deskNoteForm._richTextBox.FontFamily = (FontFamily)cbFontFamily.SelectedValue;
        }

        private void cbFontStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_bInit)
            {
                if(cbFontStyle.SelectedIndex > -1) { 
                    deskNoteForm._cFontStretch = ((FamilyTypeface)(cbFontStyle.SelectedItem)).Stretch;
                    deskNoteForm._cFontWeight = ((FamilyTypeface)(cbFontStyle.SelectedItem)).Weight;
                    deskNoteForm._cFontStyle = ((FamilyTypeface)(cbFontStyle.SelectedItem)).Style;
                    //deskNoteForm._richTextBox.FontStretch = ((FamilyTypeface)(cbFontStyle.SelectedItem)).Stretch;
                    //deskNoteForm._richTextBox.FontStyle = ((FamilyTypeface)(cbFontStyle.SelectedItem)).Style;
                    //deskNoteForm._richTextBox.FontWeight = ((FamilyTypeface)(cbFontStyle.SelectedItem)).Weight;
                }
            }
        }

        private void fontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_bInit)
            {
                //deskNoteForm._richTextBox.FontSize = fontSizeSlider.Value;
                deskNoteForm._cFontSize = fontSizeSlider.Value;
            }
        }

        private void numUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!_bInit)
            {
                deskNoteForm._dblBorderThickness = (double)numUpDown.Value;
                deskNoteForm.BorderThickness = new Thickness((double)numUpDown.Value);
            }

        }

        private void _bckgrImage_MouseMove(object sender, MouseEventArgs e)
        {

            if(e.LeftButton == MouseButtonState.Pressed)
            {
                pMouse = Mouse.GetPosition(gridBckrPic);
                _getColorPixel();
            }
        }

        private void _bckgrImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            pMouse = Mouse.GetPosition(gridBckrPic);
            _getColorPixel();
        }
        Point pMouse;
        void _getColorPixel()
        {
           // lblColorPick_Mouse.Content = "X: " + pMouse.X.ToString() + "  Y: " + pMouse.Y.ToString();

            // Use RenderTargetBitmap to get the visual, in case the image has been transformed.
            var renderTargetBitmap = new RenderTargetBitmap((int)gridBckrPic.ActualWidth,
                                                            (int)gridBckrPic.ActualHeight,
                                                            96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(gridBckrPic);

            // Make sure that the point is within the dimensions of the image.
            if ((pMouse.X < renderTargetBitmap.PixelWidth) && (pMouse.Y < renderTargetBitmap.PixelHeight))
            {
                colorPickRect.Margin = new Thickness(pMouse.X, pMouse.Y, 0, 0);
                // Create a cropped image at the supplied point coordinates.


                var croppedBitmap = new CroppedBitmap(renderTargetBitmap,
                                                        new Int32Rect((int)pMouse.X, (int)pMouse.Y, 1, 1));

                //// Copy the sampled pixel to a byte array.
                var pixels = new byte[4];
                croppedBitmap.CopyPixels(pixels, 4, 0);

                lblColorPick_R.Content = "R: " + pixels[2].ToString();
                lblColorPick_G.Content = "G: " + pixels[1].ToString();
                lblColorPick_B.Content = "B: " + pixels[0].ToString();
            }

        }
        private void btnBckgrColorReset_Click(object sender, RoutedEventArgs e)
        {
                sliderRedMatrix.Value = sliderGreenMatrix.Value = sliderBlueMatrix.Value = 10;
        }
        private void btnBckgrColorBCSReset_Click(object sender, RoutedEventArgs e)
        {
            sliderSaturationMatrix.Value = sliderContrastMatrix.Value = 10;
            sliderBrightnessMatrix.Value = 0;
        }

        private void btnBckgrPicDefault_Click(object sender, RoutedEventArgs e)
        {
            deskNoteForm.imgBack = new BitmapImage(deskNoteForm._defaultBckgrUri);
            deskNoteForm._bckgrImagePath = "";
            _bckgrImage.Fill = new ImageBrush(deskNoteForm.imgBack);
            deskNoteForm._BckgrPic = true;
            deskNoteForm.cmPresets_Init();

        }

        private void btnBckgrPicLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.bmp;*.gif)|*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            if (deskNoteForm.imgBack.UriSource != null && System.IO.File.Exists(deskNoteForm.imgBack.UriSource.AbsolutePath))
            {
                openFileDialog.InitialDirectory = System.IO.Directory.GetParent(deskNoteForm.imgBack.UriSource.AbsolutePath).FullName;
                openFileDialog.FileName = System.IO.Path.GetFileName(deskNoteForm.imgBack.UriSource.AbsolutePath);
            }
            else openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (openFileDialog.ShowDialog() == true)
            {
                deskNoteForm.imgBack = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute));
                deskNoteForm._bckgrImagePath = openFileDialog.FileName;
                _bckgrImage.Fill = new ImageBrush(deskNoteForm.imgBack);
                deskNoteForm._BckgrPic = true;
                deskNoteForm.SetBckgrPicColor();
                _getColorPixel();
                //                sliderRedMatrix.Value = sliderGreenMatrix.Value = sliderBlueMatrix.Value = 10;
            }
        }

        private void numUpDownMarginBottom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!_bInit)
                deskNoteForm.rtbMarginBottom = (double)numUpDownMarginBottom.Value;
        }

        private void numUpDownMarginRight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!_bInit)
                deskNoteForm.rtbMarginRight = (double)numUpDownMarginRight.Value;
        }

        private void numUpDownMarginTop_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!_bInit)
                deskNoteForm.rtbMarginTop = (double)numUpDownMarginTop.Value;
        }

        private void numUpDownMarginLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!_bInit)
                deskNoteForm.rtbMarginLeft = (double)numUpDownMarginLeft.Value;
        }

        private void fontList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btn_FontListAdd_Click(object sender, RoutedEventArgs e)
        {
            if (fontList.SelectedIndex > -1)
            {
                int index = listBoxFontFavourites.SelectedIndex > -1 ? listBoxFontFavourites.SelectedIndex : 0;
                FontFamily item = (FontFamily)fontList.SelectedItem;
                //listBoxFontFavourites.Items.Insert(index,item);
                ((InstalledFonts)(deskNoteForm.fontManager.fontList.ItemsSource)).Insert(index,item);
                deskNoteForm.fontManager.fontList.Items.Refresh();
                InstalledFonts.fontFamilies.Insert(index, item);
                deskNoteForm._FontFavourites.Insert(index, item);
                listBoxFontFavourites.Items.Refresh();
            }
        }

        private void btn_FontListRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxFontFavourites.SelectedIndex > -1)
            {
                //listBoxFontFavourites.Items.Remove(listBoxFontFavourites.SelectedItem);
                ((InstalledFonts)(deskNoteForm.fontManager.fontList.ItemsSource)).Remove((FontFamily)listBoxFontFavourites.SelectedItem);
                deskNoteForm.fontManager.fontList.Items.Refresh();
                InstalledFonts.fontFamilies.Remove((FontFamily)listBoxFontFavourites.SelectedItem);
                deskNoteForm._FontFavourites.Remove((FontFamily)listBoxFontFavourites.SelectedItem);
                listBoxFontFavourites.Items.Refresh();
            }

        }

        private void btn_FontListRefresh_Click(object sender, RoutedEventArgs e)
        {
            deskNoteForm.fontManager.fontList.Items.Refresh();
            listBoxFontFavourites.Items.Refresh();
        }

        private void numUpDownShadowDepth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(!_bInit)
                deskNoteForm.dropShadow.ShadowDepth = (Double)numUpDownShadowDepth.Value;
        }

        private void numUpDownShadowOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(!_bInit)
                deskNoteForm.dropShadow.Opacity = (Double)((float)numUpDownShadowOpacity.Value / 10f);

        }

        private void numUpDownShadowDirection_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(!_bInit)
                deskNoteForm.dropShadow.Direction = (Double)numUpDownShadowDirection.Value;
        }
        private void numUpDownLineHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!_bInit)
            {
                deskNoteForm._richTextBox.SetValue(Block.LineHeightProperty, Convert.ToDouble(e.NewValue));
            }
        }

        private void btnBckgrPicPresetSave_Click(object sender, RoutedEventArgs e)
        {
            if (cboxBackgrPicPresets.SelectedIndex > -1)
            {
                if(cboxBackgrPicPresets.SelectedItem.ToString() != "default" && cboxBackgrPicPresets.SelectedItem.ToString() != "last")
                {
                    deskNoteForm.bpPresets[cboxBackgrPicPresets.SelectedIndex].fRed = deskNoteForm.bpPresets[0].fRed = ((float)sliderRedMatrix.Value / 10f);
                    deskNoteForm.bpPresets[cboxBackgrPicPresets.SelectedIndex].fGreen = deskNoteForm.bpPresets[0].fGreen = ((float)sliderGreenMatrix.Value / 10f);
                    deskNoteForm.bpPresets[cboxBackgrPicPresets.SelectedIndex].fBlue = deskNoteForm.bpPresets[0].fBlue = ((float)sliderBlueMatrix.Value / 10f);
                    deskNoteForm.bpPresets[cboxBackgrPicPresets.SelectedIndex].fContrast = deskNoteForm.bpPresets[0].fContrast = ((float)sliderContrastMatrix.Value / 10f);
                    deskNoteForm.bpPresets[cboxBackgrPicPresets.SelectedIndex].fBrightness = deskNoteForm.bpPresets[0].fBrightness = ((float)sliderBrightnessMatrix.Value / 10f);
                    deskNoteForm.bpPresets[cboxBackgrPicPresets.SelectedIndex].fSaturation = deskNoteForm.bpPresets[0].fSaturation = ((float)sliderSaturationMatrix.Value / 10f);
                }
                else
                {
                    switch (cboxBackgrPicPresets.SelectedItem.ToString())
                    {
                        case "default":
                            MessageBox.Show("'default' cannot be saved", "'default' name", MessageBoxButton.OK);
                            break;
                        case "last":
                            MessageBox.Show("'last' cannot be saved", "'last' name", MessageBoxButton.OK);
                            break;
                    }

                }
            }
        }

        private void btnBckgrPicPresetNew_Click(object sender, RoutedEventArgs e)
        {
            if (tboxBackgrPicPresetsNew.Text != "" && tboxBackgrPicPresetsNew.Text != "default" && tboxBackgrPicPresetsNew.Text != "last")
            {
                if(deskNoteForm.bpPresets.Any(x => (x.presetName == tboxBackgrPicPresetsNew.Text)))
                {
                    MessageBox.Show("name already exists!\nPlease use a different name", "name exists", MessageBoxButton.OK);
                }
                else
                {
                    tboxBackgrPicPresetsNew.Text = tboxBackgrPicPresetsNew.Text.Replace(" ", "");
                    DeskNoteWindow.backgrPicPresets bps = new DeskNoteWindow.backgrPicPresets();
                    bps.fRed = ((float)sliderRedMatrix.Value / 10f);
                    bps.fGreen = ((float)sliderGreenMatrix.Value / 10f);
                    bps.fBlue = ((float)sliderBlueMatrix.Value / 10f);
                    bps.fBrightness = ((float)sliderBrightnessMatrix.Value / 10f);
                    bps.fSaturation = ((float)sliderSaturationMatrix.Value / 10f);
                    bps.fContrast = ((float)sliderContrastMatrix.Value / 10f);
                    bps.presetName = tboxBackgrPicPresetsNew.Text;

                    deskNoteForm.bpPresets = deskNoteForm.bpPresets.Append(bps).ToArray();
                    cboxBackgrPicPresets.Items.Add(tboxBackgrPicPresetsNew.Text);
                    cboxBackgrPicPresets.Items.Refresh();
                    cboxBackgrPicPresets.SelectedItem = tboxBackgrPicPresetsNew.Text;
                    deskNoteForm.cmPresets_Init();

                }

            }
            else {
                switch (tboxBackgrPicPresetsNew.Text)
                {
                    case "":
                        MessageBox.Show("name must not be empty!", "empty Name",  MessageBoxButton.OK);
                        break;
                    case "default":
                        MessageBox.Show("'default' is not allowed!", "'default' name", MessageBoxButton.OK);
                        break;
                    case "last":
                        MessageBox.Show("'last' is not allowed!", "'last' name", MessageBoxButton.OK);
                        break;
                }

            }
        }

        private void cboxBackgrPicPresets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboxBackgrPicPresets.SelectedIndex > -1 && !_bInit)
            {

                sliderRedMatrix.Value = deskNoteForm.bpPresets[cboxBackgrPicPresets.SelectedIndex].fRed * 10;
                sliderGreenMatrix.Value = deskNoteForm.bpPresets[cboxBackgrPicPresets.SelectedIndex].fGreen * 10;
                sliderBlueMatrix.Value = deskNoteForm.bpPresets[cboxBackgrPicPresets.SelectedIndex].fBlue * 10;
                sliderSaturationMatrix.Value = deskNoteForm.bpPresets[cboxBackgrPicPresets.SelectedIndex].fSaturation * 10;
                sliderContrastMatrix.Value = deskNoteForm.bpPresets[cboxBackgrPicPresets.SelectedIndex].fContrast * 10;
                sliderBrightnessMatrix.Value = deskNoteForm.bpPresets[cboxBackgrPicPresets.SelectedIndex].fBrightness * 10;
                deskNoteForm._backrPicLast = deskNoteForm.bpPresets[cboxBackgrPicPresets.SelectedIndex].presetName;
            }
        }

        private void btnBckgrPicPresetDel_Click(object sender, RoutedEventArgs e)
        {
            if (cboxBackgrPicPresets.SelectedIndex > -1)
            {
                if (cboxBackgrPicPresets.SelectedItem.ToString() != "default" && cboxBackgrPicPresets.SelectedItem.ToString() != "last")
                {
                    deskNoteForm.bpPresets = deskNoteForm.bpPresets.Where((val, idx) => idx != cboxBackgrPicPresets.SelectedIndex).ToArray();
                    cboxBackgrPicPresets.Items.Remove(cboxBackgrPicPresets.SelectedItem);
                    cboxBackgrPicPresets.Items.Refresh();
                    deskNoteForm.cmPresets_Init();
                }
                else
                {
                    switch (cboxBackgrPicPresets.SelectedItem.ToString())
                    {
                        case "default":
                            MessageBox.Show("'default' cannot be deleted", "'default' name", MessageBoxButton.OK);
                            break;
                        case "last":
                            MessageBox.Show("'last' cannot be deleted", "'last' name", MessageBoxButton.OK);
                            break;
                    }

                }
            }

        }
    }
}

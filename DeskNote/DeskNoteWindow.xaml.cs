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
//============================================================================using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Windows.Controls.Primitives;
using System.Drawing.Imaging;

namespace DeskNote
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class DeskNoteWindow : Window
    {

        string _sNote = "DeskNote";
        List<string> _lNotes = new List<string>();
        public List<FontFamily> _FontFavourites = new List<FontFamily>();
        int _iNote = 0;
        string _sName = "";
        string _sOSVersion = "";
        Point _pos;
        double _dblFormWidth, _dblFormHeight;
        double _bckgrImageWidth, _bckgrImageHeight;
        public double _dblBorderThickness;
        bool _bBckgrPic = false;
        bool _bBlur = false;
        bool _bDesktopBack = false;
        Point _pLastLocation;
        public Color _cBackColor = Color.FromArgb(60, 60, 60, 60);
        public FontFamily _cFontFamily = new System.Windows.Media.FontFamily();
        Color _cTextColor = Color.FromArgb(255, 255, 255, 255);
        Color _TextSavedColor, _BackgrSavedColor;
        public Double _cFontSize = Convert.ToDouble(12);
        public FontStyle _cFontStyle = new FontStyle();
        public FontWeight _cFontWeight = new FontWeight();
        public FontStretch _cFontStretch = new FontStretch();
        BitmapImage _imgBackgr;
        System.Timers.Timer tMouse = new System.Timers.Timer(4000);
        System.Timers.Timer tScreenshot = new System.Timers.Timer(500);
        float _fRed = 1, _fGreen = 1, _fBlue = 1, _fContrastR = 0, _fContrastG = 0, _fContrastB = 0, _fContrast = 1, _fBrightness = 0,_fSaturation=1;
        TextRange _rtbTextRange;
        Thickness _rtbMargin = new Thickness(5, 10, 5, 5);
        bool _bInit = true;
        bool _bExit = false;
        public string _bckgrImagePath = "";
        public Uri _defaultBckgrUri;
        public event RoutedEventHandler noteChanged;
        public event RoutedEventHandler noteRenamed;
        System.Windows.Media.Effects.DropShadowEffect dropShadowEffect = new System.Windows.Media.Effects.DropShadowEffect();
        double dropshadowDepth = 3, dropshadowOpacity = 0.5, dropshadowDirection = 330;
        Color dropshadowColor = Color.FromRgb(0, 0, 0);
        bool _bShadow = false;
        bool _bChanged = false;
        bool _bSetText = false;
        object wordDoNotSaveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
        public struct backgrPicPresets
        {
            public float fRed;
            public float fGreen;
            public float fBlue;
            public float fBrightness;
            public float fSaturation;
            public float fContrast;
            public String presetName;
        }
        public backgrPicPresets[] bpPresets = new backgrPicPresets[0];
        public string _backrPicLast = "default";
        public DeskNoteWindow(string sName)
        {
            InitializeComponent();
            _sName = sName;
            _init();
            navGrid.Visibility = Visibility.Hidden;
            _topGrid.Visibility = Visibility.Hidden;
            _bottomGrid.Visibility = Visibility.Hidden;
            tMouse.Elapsed += tMouse_Elapsed;
            tScreenshot.Elapsed += TScreenshot_Elapsed;
        }
        /// <summary>
        /// TScreenshot_Elapsed: 
        /// delayed execution for screenshot
        /// so the window is in a proper state without toolbar etc
        /// </summary>
        private void TScreenshot_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
           tScreenshot.Stop();
            _main.Dispatcher.Invoke(new Action(() =>
            {
                var rightTop = (this.PointToScreen(new Point(Width-5, 5)));
                var leftTop = (this.PointToScreen(new Point(5, 5)));
                var rightBottom = (this.PointToScreen(new Point(5, Height-5)));
                var width = Convert.ToInt16(rightTop.X - leftTop.X);
                var height = Convert.ToInt16(rightBottom.Y - rightTop.Y);

                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap((int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Convert.ToInt16(leftTop.X), Convert.ToInt16(leftTop.Y), 0, 0, new System.Drawing.Size((int)width, (int)height));//,System.Drawing.CopyPixelOperation.SourceCopy);
                    }
                    Clipboard.SetDataObject(bitmap, true);
                    if(bScreenshotFile)
                    {
                        Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                        saveFileDialog.DefaultExt = ".jpg";
                        saveFileDialog.AddExtension = true;
                        saveFileDialog.Filter = "JPG (.jpg)|*.jpg|PNG (.png)|*.png|BMP (.bmp)|*.bmp|TIF (.tif)|*.tif|GIF (.gif)|*.gif";
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            var extension = System.IO.Path.GetExtension(saveFileDialog.FileName);
                            switch (extension.ToLower())
                            {
                                case ".jpg":
                                    bitmap.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                                    break;
                                case ".png":
                                    bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                                    break;
                                case ".bmp":
                                    bitmap.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                                    break;
                                case ".tif":
                                    bitmap.Save(saveFileDialog.FileName, ImageFormat.Tiff);
                                    break;
                                case ".gif":
                                    bitmap.Save(saveFileDialog.FileName, ImageFormat.Gif);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(extension);
                            }
                        }

                    }
                }
            }));
        }

        /// <summary>
        /// tMouse_Elapsed: 
        /// timer - window sizing border stays visible for 3 secs
        /// this is necessary as the mouse enter event is not fired when the background is transparent
        /// </summary>
        void tMouse_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool bOutOfScope = false;
            Point p = new Point (0,0);
            _main.Dispatcher.Invoke(new Action(() => { p = Mouse.GetPosition(_main); }));
            _main.Dispatcher.Invoke(new Action(() => { if (!(p.X > 0 && p.X < _main.Width) && !(p.Y > 0 && p.Y < _main.Height) && !fontManager.isDropDownOpen && !_fontSize.IsDropDownOpen) bOutOfScope = true; }));
            if(bOutOfScope)
            {
                _main.Dispatcher.Invoke(new Action(() => { Keyboard.Focus(null); }));
             //   _main.Dispatcher.Invoke(new Action(() => { ResizeMode = System.Windows.ResizeMode.NoResize; }));
                _main.Dispatcher.Invoke(new Action(() => { navGrid.Visibility = Visibility.Hidden; }));
                _main.Dispatcher.Invoke(new Action(() => { _topGrid.Visibility = Visibility.Hidden; }));
                _main.Dispatcher.Invoke(new Action(() => { _bottomGrid.Visibility = Visibility.Hidden; }));
//                _main.Dispatcher.Invoke(()=> { BorderThickness = new Thickness(_dblBorderThickness); });

                tMouse.Stop();

                if (_bChanged)
                    _main.Dispatcher.Invoke(new Action(() => { saveXML(); }));

            }
        }
        /// <summary>
        /// _init():
        /// loading of notes and initializing of controls
        /// </summary>
        void _init()
        {
            _bInit = true;
            _rtbTextRange = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
            loadXml();
            _sOSVersion = _getOSVersion();
            if (_sOSVersion.StartsWith("6.2")) _bDesktopBack = false;
            this._main.Tag = _sName;
            cmTitle.Header = _sName;
          //  NotifyIconInit();
          //  toolStripMenuItemTitle.Text = _sName;
            _setText();
            _setNavList();
          //  notifyIcon1.Text = notifyIconText;
          //  cmLoadNotes_Init();
          //   notifyIconLoadNotes_Init();
            Background = new SolidColorBrush(_cBackColor);
            _defaultBckgrUri = new Uri(@"pack://application:,,,/Resources/Note.png", UriKind.RelativeOrAbsolute);
            if(_bckgrImagePath !="" && File.Exists(_bckgrImagePath))
                _imgBackgr = new BitmapImage(new Uri(_bckgrImagePath, UriKind.RelativeOrAbsolute));
            else
                _imgBackgr = new BitmapImage(_defaultBckgrUri);
            _BckgrPic = _bBckgrPic;
            dropShadowEffect.ShadowDepth = dropshadowDepth;
            dropShadowEffect.Direction = dropshadowDirection;
            dropShadowEffect.Color = dropshadowColor;
            dropShadowEffect.Opacity = dropshadowOpacity;
            cmPresets_Init();
            _bInit = false;
        }

        /// <summary>
        /// _main_Loaded:
        /// positioning and sizing of window
        /// </summary>
        private void _main_Loaded(object sender, RoutedEventArgs e)
        {
            location = _pos;
            _main.Width = _dblFormWidth;
            _main.Height = _dblFormHeight;
            _blur=_bBlur;
            _fontSize.ItemsSource = FontSizes;
            _richTextBox.Margin = _rtbMargin;
            ((InstalledFonts)(fontManager.fontList.ItemsSource)).InsertRange(0, _FontFavourites);
            fontManager.fontList.Items.Refresh();
            InstalledFonts.fontFamilies.InsertRange(0, _FontFavourites);
            if (_bShadow) _richTextBox.Effect = dropShadow;
            if (_bDesktopBack) _sendToBack(true,false);
        }

        private void _main_Closed(object sender, EventArgs e)
        {
            _exit();
        }
        public BitmapImage imgBack
        {
            set { _imgBackgr = value; }
            get { return _imgBackgr; }
        }
        public Color _TextColor
        {
            get { return _cTextColor; }
            set { _richTextBox.Foreground = new SolidColorBrush(_cTextColor = value); }
        }
        public Byte _BackgroundAlpha
        {
            get { return ((SolidColorBrush)Background).Color.A; }
            set { Background = new SolidColorBrush(_cBackColor = ColorExtensions.setA(_cBackColor, value));}
        }       
        public Byte _BackgroundRed
        {
            get { return ((SolidColorBrush)Background).Color.R; }
            set { Background = new SolidColorBrush(_cBackColor = ColorExtensions.setR(_cBackColor, value));}
        }
        public Byte _BackgroundGreen
        {
            get { return ((SolidColorBrush)Background).Color.G; }
            set { Background = new SolidColorBrush(_cBackColor = ColorExtensions.setG(_cBackColor, value));}
        }
        public Byte _BackgroundBlue
        {
            get { return ((SolidColorBrush)Background).Color.B; }
            set { Background = new SolidColorBrush(_cBackColor = ColorExtensions.setB(_cBackColor, value));}
        }
        public float _BckgrPicRed
        {
            get { return _fRed; }
            set { _fRed = value; }
        }
        public float _BckgrPicGreen
        {
            get { return _fGreen; }
            set { _fGreen = value; }
        }
        public float _BckgrPicBlue
        {
            get { return _fBlue; }
            set { _fBlue = value; }
        }
        public float _BckgrPicContrastRed
        {
            get { return _fContrastR; }
            set { _fContrastR = value; }
        }
        public float _BckgrPicContrastGreen
        {
            get { return _fContrastG; }
            set { _fContrastG = value; }
        }
        public float _BckgrPicContrastBlue
        {
            get { return _fContrastB; }
            set { _fContrastB = value; }
        }
        public float _BckgrPicContrast
        {
            get { return _fContrast; }
            set { _fContrast =  value; }
        }
        public float _BckgrPicSaturation
        {
            get { return _fSaturation; }
            set { _fSaturation =  value; }
        }
        public float _BckgrPicBrightness
        {
            get { return _fBrightness; }
            set { _fBrightness =  value; }
        }
        public string _noteName
        {
            get { return _sName; }
        }
        public double[] FontSizes
        {
            get
            {
                return new double[] { 
		            3.0, 4.0, 5.0, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5, 
		            10.0, 10.5, 11.0, 11.5, 12.0, 12.5, 13.0, 13.5, 14.0, 14.5, 15.0, 15.5,
		            16.0, 16.5, 17.0,  17.5, 18.0,  18.5, 19.0, 20.0, 21.0, 22.0, 23.0, 24.0, 26.0, 28.0, 30.0,
		            32.0, 34.0, 36.0, 38.0, 40.0, 44.0, 48.0, 52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0,
		            80.0, 88.0, 96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 144.0
		            };
            }
        }
        public bool _isBack
        {
            get { return _bDesktopBack; }
        }
        public string notifyIconText
        {
            get
            {
                string sNotifyIcon = ("["+_sName + "] " + _sNote).Replace("\n", "").Replace("\r", "").Replace("\r\n", " ").Replace("\t", " ");
                if (sNotifyIcon.Length > 63) sNotifyIcon = sNotifyIcon.Substring(0, 63);
                return sNotifyIcon;
            }
        }
        public Point location
        {
            get {return this.PointToScreen(new Point(0, 0));}
            set { this.Top = value.Y; this.Left = value.X; }
        }
        private Point DPIunScaling(Point point)
        {
            IntPtr windowHandle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            System.Drawing.Graphics graphics;
            graphics = System.Drawing.Graphics.FromHwnd(windowHandle);
            var dpiX = graphics.DpiX;
            var dpiY = graphics.DpiY;

            if (dpiX > 0)
                return new Point((double)((float)point.X / (dpiX / 96.0)), (double)((float)point.Y / (dpiY / 96.0)));
            else
                return point;

        }
        private Point DPIScaling(Point point)
        {
            IntPtr windowHandle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            System.Drawing.Graphics graphics;
            graphics = System.Drawing.Graphics.FromHwnd(windowHandle);
            var dpiX = graphics.DpiX;
            var dpiY = graphics.DpiY;

            if (dpiX > 0)
                return new Point((double)((float)point.X * (dpiX / 96.0)), (double)((float)point.Y * (dpiY / 96.0)));
            else
                return point;

        }

        private enum ResizeDirection { 
            Left = 61441, 
            Right = 61442, 
            Top = 61443, 
            TopLeft = 61444, 
            TopRight = 61445, 
            Bottom = 61446, 
            BottomLeft = 61447, 
            BottomRight = 61448, 
        }

        private System.Drawing.Point m_Pos;
        Point pOffset;
        private void _main_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && 
                !_richTextBox.IsMouseOver &&
                !listBoxNav.IsVisible &&
                !colorPickerTextForeground.IsOpen &&
                !canvasTextForeground.IsMouseOver &&
                !colorPickerTextBackground.IsOpen &&
                !canvasTextBackground.IsMouseOver &&
                !fontManager.isDropDownOpen &&
                !_fontSize.IsDropDownOpen &&
                !_fontSize.IsMouseOver)

                this.DragMove();

            //            {
            //                m_Pos = System.Windows.Forms.Control.MousePosition;
            //                Point p = new Point(m_Pos.X, m_Pos.Y);
            //                p = DPIunScaling(p);
            //                m_Pos.X = (int)p.X;
            //                m_Pos.Y = (int)p.Y;
            //                m_Pos.Offset(-(int)pOffset.X, -(int)pOffset.Y);

            ////                WpfScreen wpfScreen = WpfScreen.GetScreenFrom(this);
            ////                if (m_Pos.X < WpfScreen.GetScreenFrom(this).DeviceBounds.Width && m_Pos.Y < WpfScreen.GetScreenFrom(this).DeviceBounds.Height)
            //                //needs more checking - on small, scaled touch displays it sometimes happend that the window moved out of sight ;)
            //                if (m_Pos.X < System.Windows.SystemParameters.VirtualScreenWidth && m_Pos.Y <System.Windows.SystemParameters.VirtualScreenHeight) 
            //                    location = (Point)new System.Windows.Point(m_Pos.X, m_Pos.Y);
            //            }

        }
        private void _main_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && 
                !_richTextBox.IsMouseOver &&
                !listBoxNav.IsVisible &&
                !colorPickerTextForeground.IsOpen &&
                !canvasTextForeground.IsMouseOver &&
                !colorPickerTextBackground.IsOpen &&
                !canvasTextBackground.IsMouseOver &&
                !fontManager.isDropDownOpen &&
                !_fontSize.IsDropDownOpen &&
                !_fontSize.IsMouseOver)
            pOffset =  Mouse.GetPosition(this);
        }
        private void _main_MouseEnter(object sender, MouseEventArgs e)
        {
//            _main.ResizeMode = System.Windows.ResizeMode.CanResize;
//            BorderThickness = new Thickness(1);
            if (_lNotes.Count > 1)
                navGrid.Visibility = Visibility.Visible;
            _topGrid.Visibility = Visibility.Visible;
            _bottomGrid.Visibility = Visibility.Visible;
            _richTextBox.CaretBrush = null;
            rtbGrid.Margin = new Thickness(0,40,0,0);
         //   HwndSource hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
         //   winApi.SendMessage(hwndSource.Handle, 0x112, (IntPtr)61448, IntPtr.Zero);

        }
        private void _main_MouseLeave(object sender, MouseEventArgs e)
        {
            tMouse.Start();
            navGrid.Visibility = Visibility.Hidden;
            _topGrid.Visibility = Visibility.Hidden;
            _bottomGrid.Visibility = Visibility.Hidden;
            _richTextBox.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            
            rtbGrid.Margin = new Thickness(0,0,0,0);
        }
        private void _main_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_bBckgrPic && (_imgBackgr.UriSource ==_defaultBckgrUri))
            {
                double dblPadL = 35 * _Grid.ActualWidth / _bckgrImageWidth;
                double dblPadR = 65 * _Grid.ActualWidth / _bckgrImageWidth;
                double dblPadB = 90 * _Grid.ActualHeight / _bckgrImageHeight;
                _richTextBox.Margin = new Thickness(dblPadL, 20, dblPadR, dblPadB);
                dblPadL = 3 * _Grid.ActualWidth / _bckgrImageWidth;
                dblPadR = 25 * _Grid.ActualWidth / _bckgrImageWidth;
                _topGrid.Margin = new Thickness(dblPadL, -3, dblPadR, 0);

            }
            //scaling of controls when window width gets below a certain value
            if (_topGrid.ActualWidth < 242)
            {
                double scale = _topGrid.ActualWidth / 242;
                _GridTopScaleTransform.ScaleX = scale;
                _GridTopScaleTransform.ScaleY = scale;
            }
            else
            {
                _GridTopScaleTransform.ScaleX = 1;
                _GridTopScaleTransform.ScaleY = 1;
            }
            if (ActualWidth < 162)
            {
                double scale = ActualWidth / 162;
                _GridNavScaleTransform.ScaleX = scale;
                _GridNavScaleTransform.ScaleY = scale;
            }
            else
            {
                _GridNavScaleTransform.ScaleX = 1;
                _GridNavScaleTransform.ScaleY = 1;
            }

            //fontscaling with <ctrl> + windowsizing
            if (System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left)
                if(System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
                {
                    if((_dblFormHeight + _dblFormWidth +25) < (ActualHeight + ActualWidth))
                    {
                        _richTextBox.SelectAll();
                        btnIncreaseFontSize_Click(null, null);
                        _dblFormHeight = Height;
                        _dblFormWidth = Width;
                    }
                    else if ((_dblFormHeight + _dblFormWidth -25) > (ActualHeight + ActualWidth))
                    {
                        _richTextBox.SelectAll();
                        btnDecreaseFontSize_Click(null, null);
                        _dblFormHeight = Height;
                        _dblFormWidth = Width;
                    }
                }
        }
        public double rtbMarginLeft
        {
            get { return _richTextBox.Margin.Left; }
            set { _richTextBox.Margin = _rtbMargin = new Thickness(value, _richTextBox.Margin.Top, _richTextBox.Margin.Right, _richTextBox.Margin.Bottom); }
        }
        public double rtbMarginTop
        {
            get { return _richTextBox.Margin.Top; }
            set { _richTextBox.Margin = _rtbMargin = new Thickness(_richTextBox.Margin.Left, value, _richTextBox.Margin.Right, _richTextBox.Margin.Bottom); }
        }
        public double rtbMarginRight
        {
            get { return _richTextBox.Margin.Right; }
            set { _richTextBox.Margin = _rtbMargin = new Thickness(_richTextBox.Margin.Left, _richTextBox.Margin.Top, value, _richTextBox.Margin.Bottom); }
        }
        public double rtbMarginBottom
        {
            get { return _richTextBox.Margin.Bottom; }
            set { _richTextBox.Margin = _rtbMargin = new Thickness(_richTextBox.Margin.Left, _richTextBox.Margin.Top, _richTextBox.Margin.Right, value); }
        }
        /// <summary>
        /// _main_MouseDoubleClick:
        /// open settings with <ctrl> and mouse doubleclick
        /// </summary>
        private void _main_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)    
                _settings();
        }

        private void _richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_bInit)
            {
                _rtbTextRange.Select(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
//                _lNotes[_iNote] = textRangeToXAMLString(_rtbTextRange);
                _lNotes[_iNote] = textRangeToRtfString(_rtbTextRange);
                _sNote = _rtbTextRange.Text;
                listBoxNav.Items[_iNote] = _sNote.Trim() + "\r\n\r\n========= " + (_iNote + 1).ToString() + " ==========\r\n\r\n";
                //   notifyIcon1.Text = notifyIconText;
                if(!_bSetText)
                    _bChanged = true;
                _bSetText = false;
            }
        }
        private void _richTextBox_PreviewDragEnter(object sender, DragEventArgs e)
        {
           // _main.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
        }
        private void _richTextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            //if (e.ClickCount == 2)
            //{
            //    if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
            //        _settings();
            //    else
            //        _textEdit();
            //}
            
        }
        private void _richTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           // _settings();
        }
        /// <summary>
        /// _richTextBox_SelectionChanged:
        /// update toolbar and rtb contextmenue
        /// </summary>
        private void _richTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateToggleButtonState();
            UpdateColorpickerBackColor();
            UpdateSelectedFontFamily();
            UpdateSelectedFontSize();
            UpdateSelectionListType();

            if (Clipboard.ContainsText() && _richTextBox.CaretPosition.GetNextContextPosition(LogicalDirection.Backward) !=null)
                _rtbCM_Insert.IsEnabled = true;
            else
                _rtbCM_Insert.IsEnabled = false;

            _rtbCM_newURL.IsEnabled = false;
            _rtbCM_deleteURL.IsEnabled = false;
            _rtbCM_editURL.IsEnabled = false;

            if (!_richTextBox.Selection.IsEmpty && _richTextBox.Selection.Text.Trim() != "")
            {
                _rtbCM_newURL.IsEnabled = true;

                TextPointer pos = _richTextBox.Selection.Start;
                Inline dep;
                dep = pos.GetAdjacentElement(LogicalDirection.Forward) as Inline;
                if (dep is Hyperlink)
                {
                    _rtbCM_deleteURL.IsEnabled = true;
                    _rtbCM_editURL.IsEnabled = true;

                }
                if(_imageContainer != null)
                {
                    _Adorner(_imageContainer,false);
                    _imageContainer = null;
                }
            }
            else
            {
                if (_richTextBox.Selection.IsEmpty)
                {
                    Inline dep = _richTextBox.CaretPosition.Parent as Inline;
                    while (dep != null && !(dep is Hyperlink))
                    {
                        dep = dep.Parent as Inline;
                    }
                    if(dep is Hyperlink)
                    {
                        _rtbCM_editURL.IsEnabled = true;
                        _rtbCM_deleteURL.IsEnabled = true;
                    }
                    if(_imageContainer != null)
                    {
                        _Adorner(_imageContainer,false);
                        _imageContainer = null;
                    }
                }
                else
                {
                    var sel = _richTextBox.Selection.Start.GetAdjacentElement(LogicalDirection.Forward);
                    if(sel.GetType() == typeof(System.Windows.Documents.InlineUIContainer))
                    {
                        if(((InlineUIContainer)sel).Child.GetType() == typeof(System.Windows.Controls.Image))
                        {
                            _imageContainer = (InlineUIContainer)sel;
                            _Adorner(_imageContainer);
                        }
                    }
                    else if(sel.GetType() == typeof(System.Windows.Controls.Image))
                    {
                        TextRange tr = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
                        string s = textRangeToRtfString(tr);
//                        System.Windows.Forms.SendKeys.Send("{DELETE}");
                        Stream st = RtfStringToStream(s);
                        _richTextBox.Selection.Load(st,DataFormats.Rtf);
//                        _richTextBox.Selection.Load(RtfStringToStream(s),DataFormats.Rtf);
                    }
                }
            }
        }
        InlineUIContainer _imageContainer;
        private void _Adorner(InlineUIContainer container, bool add = true)
        {
            if (container != null)
            {
                var image = container.Child;
                if (image != null)
                {
                    var al = AdornerLayer.GetAdornerLayer(image);
                    if (al != null)
                    {
                        var currentAdorners = al.GetAdorners(image);
                        if (currentAdorners != null)
                        {
                            foreach (var adorner in currentAdorners)
                            {
                                al.Remove(adorner);
                            }
                        }
                        if (add)
                            al.Add(new ResizingAdorner(image));
                        else
                            _imageContainer = null;
                    }
                }
            }
        }
        private void colorPickerTextForeground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (_richTextBox.Selection.Text.Length > 0 && colorPickerTextForeground.IsOpen)
            {
                _richTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush((Color)e.NewValue));
            }
        }
        private void textColorLabel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            colorPickerTextForeground.IsOpen = true;
        }

        public System.Windows.Media.Effects.DropShadowEffect dropShadow
        {
            get { return dropShadowEffect; }
            set { dropShadowEffect = value; }
        }
        private void textShadowLabel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_richTextBox.Effect == null)
            {
                Style style = new Style(typeof(Rectangle), textShadow.Style);
                style.Setters.Add(new Setter(Rectangle.FillProperty, (Brush)Resources["ActiveItemBrush"]));
                textShadow.Style = style;
                _richTextBox.Effect = dropShadowEffect;
            }
            else
            {
                Style style = new Style(typeof(Rectangle), textShadow.Style);
                style.Setters.Add(new Setter(Rectangle.FillProperty, (Brush)Resources["Transparent"]));
                textShadow.Style = style;
                _richTextBox.Effect = null;
            }
        }
        private void ColorPickerTextBackground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (_richTextBox.Selection.Text.Length > 0 && colorPickerTextBackground.IsOpen)
            {
                _richTextBox.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush((Color)e.NewValue));
            }
        }
        private void colorPickerTextBackground_MouseUp(object sender, MouseButtonEventArgs e)
        {
            colorPickerTextBackground.IsOpen = true;
        }
        private void colorPickerTextForeground_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
        private void UpdateColorpickerBackColor()
        {
            object currentValue = _richTextBox.Selection.GetPropertyValue(TextElement.BackgroundProperty);
            if (currentValue != DependencyProperty.UnsetValue && currentValue != null)
            {
                colorPickerTextBackground.SelectedColor = ((SolidColorBrush)currentValue).Color;
            }
            else if (currentValue == null)
            {
                colorPickerTextBackground.SelectedColor = Color.FromArgb(0, 0, 0, 0);
            }

            currentValue = _richTextBox.Selection.GetPropertyValue(TextElement.ForegroundProperty);
            if (currentValue != DependencyProperty.UnsetValue && currentValue != null)
            {
                colorPickerTextForeground.SelectedColor = ((SolidColorBrush)currentValue).Color;
            }
        }
        private void UpdateToggleButtonState()
        {
            UpdateItemCheckedState(_btnBold, TextElement.FontWeightProperty, FontWeights.Bold);
            UpdateItemCheckedState(_btnItalic, TextElement.FontStyleProperty, FontStyles.Italic);
            UpdateItemCheckedState(_btnUnderline, Inline.TextDecorationsProperty, TextDecorations.Underline);
        }
        void UpdateItemCheckedState(ToggleButton button, DependencyProperty formattingProperty, object expectedValue)
        {
            object currentValue = _richTextBox.Selection.GetPropertyValue(formattingProperty);
            button.IsChecked = (currentValue == DependencyProperty.UnsetValue) ? false : currentValue != null && currentValue.Equals(expectedValue);
        }
        private void UpdateSelectionListType()
        {
            Paragraph startParagraph = _richTextBox.Selection.Start.Paragraph;
            Paragraph endParagraph = _richTextBox.Selection.End.Paragraph;
            if (startParagraph != null && endParagraph != null && (startParagraph.Parent is ListItem) && (endParagraph.Parent is ListItem) && object.ReferenceEquals(((ListItem)startParagraph.Parent).List, ((ListItem)endParagraph.Parent).List))
            {
                TextMarkerStyle markerStyle = ((ListItem)startParagraph.Parent).List.MarkerStyle;
                if (markerStyle == TextMarkerStyle.Disc) //bullets
                {
                    _btnBullets.IsChecked = true;
                }
                //else if (markerStyle == TextMarkerStyle.Decimal) //numbers
                //{
                //    _btnNumbers.IsChecked = true;
                //}
            }
            else
            {
                _btnBullets.IsChecked = false;
             //   _btnNumbers.IsChecked = false;
            }
        }

        private void FontManager_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (fontManager.selectedItem != null && !_bInit)
            {
                FontFamily editValue = (FontFamily)new FontFamilyConverter().ConvertFromString(fontManager.selectedItem);
               // editValue = Fonts.GetFontFamilies(new Uri(@"pack://application:,,,/Resources/Fonts/Calligraphy/")).Single(x => x.Source==editValue.Source);
               // editValue = (FontFamily)new FontFamilyConverter().ConvertFromString("pack://application:,,,/Resources/Fonts/Calligraphy/#AR DECODE");
                ApplyPropertyValueToSelectedText(TextElement.FontFamilyProperty, editValue);
               // _setText();
            }
        }
        private void FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((_fontSize.IsDropDownOpen || _fontSize.IsSelectionBoxHighlighted) && e.AddedItems.Count > 0 )
                ApplyPropertyValueToSelectedText(TextElement.FontSizeProperty, e.AddedItems[0]);
        }
        private void btnIncreaseFontSize_Click(object sender, RoutedEventArgs e)
        {
            if (EditingCommands.IncreaseFontSize.CanExecute(null, _richTextBox))
            {
                EditingCommands.IncreaseFontSize.Execute(null, _richTextBox);
                UpdateSelectedFontSize();
            }
        }
        private void btnDecreaseFontSize_Click(object sender, RoutedEventArgs e)
        {
            if (EditingCommands.DecreaseFontSize.CanExecute(null, _richTextBox))
            {
                EditingCommands.DecreaseFontSize.Execute(null, _richTextBox);
                UpdateSelectedFontSize();
            }
        }
        void ApplyPropertyValueToSelectedText(DependencyProperty formattingProperty, object value)
        {
            if (value == null || value == "")
                return;

            _richTextBox.Selection.ApplyPropertyValue(formattingProperty, value);
        }
        private void UpdateSelectedFontFamily()
        {
            object value = _richTextBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            FontFamily currentFontFamily = (FontFamily)((value == DependencyProperty.UnsetValue) ? null : value);
            if (currentFontFamily != null)
            {
              //  int i = System.Windows.Media.Fonts.SystemFontFamilies.ToList().IndexOf(currentFontFamily);
               // int i = InstalledFonts.fontFamilies.IndexOf(currentFontFamily);
              //  i = fontManager.indexOf(currentFontFamily);
               // fontManager.selectedIndex = i;
                fontManager.selectedIndex = fontManager.indexOf(currentFontFamily);
            }
            else
                fontManager.selectedIndex = -1;
        }
        private void UpdateSelectedFontSize()
        {
            object value = _richTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            if (value == DependencyProperty.UnsetValue)
                _fontSize.SelectedValue = null;
            else
            {
                _fontSize.SelectedValue = value;
                if (_fontSize.SelectedValue == null)
                    _fontSize.Text = value.ToString();
            }
        }
        private void _fontSize_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter && !_fontSize.IsDropDownOpen)
            {
                ApplyPropertyValueToSelectedText(TextElement.FontSizeProperty, _fontSize.Text);
            }

        }
        //private void _fontStretch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if ((_fontStretch.IsDropDownOpen || _fontStretch.IsSelectionBoxHighlighted) && e.AddedItems.Count > 0)
        //        ApplyPropertyValueToSelectedText(TextElement.FontStretchProperty, e.AddedItems[0]);
        //}

        //private void _fontStretch_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter && !_fontStretch.IsDropDownOpen)
        //    {
        //        ApplyPropertyValueToSelectedText(TextElement.FontStretchProperty, _fontStretch.Text);
        //    }
        //}

        #region send form behind desktop icons
        public void _sendToBack(bool back,bool bLocal)  //sending form behind desktop icons resulting in a completly locked state
        {
            IntPtr windowHandle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            if (back)
            {
                _pLastLocation = location; //remember last position - necessary when using multiple screens 
                _dblFormHeight = Height;
                _dblFormWidth = Width;

                // * Following code is taken from "Draw Behind Desktop Icons in Windows 8"
                // * http://www.codeproject.com/Articles/856020/Draw-behind-Desktop-Icons-in-Windows
                // * 
                // * by Gerald Degeneve (http://www.codeproject.com/script/Membership/View.aspx?mid=8529137)
                // * 
                // * Thanks a lot Gerald! Really awsome cool ;)
                // * 

                // Fetch the Progman window
                IntPtr progman = winApi.FindWindow("Progman", null);

                IntPtr result = IntPtr.Zero;

                // Send 0x052C to Progman. This message directs Progman to spawn a 
                // WorkerW behind the desktop icons. If it is already there, nothing 
                // happens.
                winApi.SendMessageTimeout(progman,
                                        0x052C,
                                        new IntPtr(0),
                                        IntPtr.Zero,
                                        SendMessageTimeoutFlags.SMTO_NORMAL,
                                        1000,
                                        out result);

                IntPtr workerw = IntPtr.Zero;

                // We enumerate all Windows, until we find one, that has the SHELLDLL_DefView 
                // as a child. 
                // If we found that window, we take its next sibling and assign it to workerw.
                winApi.EnumWindows(new winApi.EnumWindowsProc((tophandle, topparamhandle) =>
                {
                    IntPtr p = winApi.FindWindowEx(tophandle,
                                                IntPtr.Zero,
                                                "SHELLDLL_DefView",
                                                IntPtr.Zero);

                    if (p != IntPtr.Zero)
                    {
                        // Gets the WorkerW Window after the current one.
                        workerw = winApi.FindWindowEx(IntPtr.Zero,
                                                    tophandle,
                                                    "WorkerW",
                                                    IntPtr.Zero);
                    }

                    return true;
                }), IntPtr.Zero);
                // We now have the handle of the WorkerW behind the desktop icons.
                // We can use it to create a directx device to render 3d output to it, 
                // we can use the System.Drawing classes to directly draw onto it, 
                // and of course we can set it as the parent of a windows form.
                //
                // There is only one restriction. The window behind the desktop icons does
                // NOT receive any user input. So if you want to capture mouse movement, 
                // it has to be done the LowLevel way (WH_MOUSE_LL, WH_KEYBOARD_LL).

                // ************************************************************************************************

               // Visible = false;
                winApi.SetParent(windowHandle, workerw);
                winApi.MoveWindow(windowHandle, (int)(_pLastLocation.X + (_pLastLocation.X - location.X)), (int)(_pLastLocation.Y + (_pLastLocation.Y - location.Y)), (int)_dblFormWidth, (int)_dblFormHeight, true); //necessary when using multiple screens
              //  Visible = true;
                _bDesktopBack = true;

              //  sendToBackToolStripMenuItem.Checked = true;
              //  bringToFrontToolStripMenuItem.Checked = false;
              //  sendToBackToolStripMenuItem.Enabled = false;
              //  bringToFrontToolStripMenuItem.Enabled = true;

                cmSendToBack.IsChecked = true;
                cmBringToFront.IsChecked = false;
                cmSendToBack.IsEnabled = false;
                cmBringToFront.IsEnabled = true;
                Height= _dblFormHeight;
                Width = _dblFormWidth;
                //workaround to force the window to repaint content
                Width += 1;
                Width -= 1;
                if (bLocal)
                {
                    string[] sObject = new string[] { _sName, "Send to back" };
                    noteChanged(sObject, null);
                }
            }
            else
            {
                Visibility = Visibility.Hidden;
                winApi.SetParent(windowHandle, IntPtr.Zero);
                //restore wallpaper
                //  W32.SystemParametersInfo(W32.SPI_SETDESKWALLPAPER, 0, null, W32.SPIF_UPDATEINIFILE);
                winApi.MoveWindow(windowHandle, (int)_pLastLocation.X, (int)_pLastLocation.Y, (int)Width, (int)Height, true);
                Visibility = Visibility.Visible;
                _bDesktopBack = false;

                cmSendToBack.IsChecked = false;
                cmBringToFront.IsChecked = true;
                cmSendToBack.IsEnabled = true;
                cmBringToFront.IsEnabled = false;
                Height = _dblFormHeight;
                Width = _dblFormWidth;
                //workaround to force the window to repaint content
                Width += 1;
                Width -= 1;
                if (bLocal)
                {
                    string[] sObject = new string[] { _sName, "Bring to front" };
                    noteChanged(sObject, null);
                }

            }


        }
        #endregion

        #region load / save settings

        string filePath = System.IO.Directory.GetCurrentDirectory() + "\\settings.xml";

        void loadXml()
        {
            if (System.IO.File.Exists(filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("settings.xml");

                XmlNodeList list;
                XmlNode node;
                string sVal;
                if(_sName == "")
                { 
                    node = doc.SelectSingleNode("//DeskNote/LastNote");
                    if (node != null && node.InnerText != "")
                    {
                        _sName = node.InnerText;
                    }
                }
                _rtbTextRange.Text = _sName;
                node = doc.SelectSingleNode("//DeskNote/Note[@name='" + _sName + "']");
                if (node != null)
                {
                    sVal = node["FormBackgroundGlass"].InnerText;
                    if (sVal.Length > 0)
                        _bBlur = Convert.ToBoolean(sVal);
                    else
                        _bBlur = false;
                    sVal = node["BackgroundImage"].InnerText;
                    if (sVal.Length > 0)
                        _bBckgrPic = Convert.ToBoolean(sVal);
                    else
                        _bBckgrPic = true;
                    sVal = node["BackgroundImagePath"].InnerText;
                    if (sVal.Length > 0)
                        _bckgrImagePath = sVal;
                    else
                        _bckgrImagePath = "";
                    sVal = node["BackgrPictureMatrixRed"].InnerText;
                    if (sVal.Length > 0)
                        _fRed = (float)Convert.ToDouble(sVal);
                    else
                        _fRed = 1;
                    sVal = node["BackgrPictureMatrixGreen"].InnerText;
                    if (sVal.Length > 0)
                        _fGreen = (float)Convert.ToDouble(sVal);
                    else
                        _fGreen = 1;
                    sVal = node["BackgrPictureMatrixBlue"].InnerText;
                    if (sVal.Length > 0)
                        _fBlue = (float)Convert.ToDouble(sVal);
                    else
                        _fBlue = 1;
                    sVal = node["BackgrPictureMatrixBrightness"].InnerText;
                    if (sVal.Length > 0)
                        _fBrightness = (float)Convert.ToDouble(sVal);
                    else
                        _fBrightness = 0;
                    sVal = node["BackgrPictureMatrixContrast"].InnerText;
                    if (sVal.Length > 0)
                        _fContrast = (float)Convert.ToDouble(sVal);
                    else
                        _fContrast = 1;
                    sVal = node["BackgrPictureMatrixSaturation"].InnerText;
                    if (sVal.Length > 0)
                        _fSaturation = (float)Convert.ToDouble(sVal);
                    else
                        _fSaturation = 1;
                    sVal = node["BorderThickness"].InnerText;
                    if (sVal.Length > 0)
                    {
                        _dblBorderThickness = Convert.ToDouble(sVal.Split(',')[0]);
                        BorderThickness = new Thickness(_dblBorderThickness);
                    }
                    else
                        BorderThickness = new Thickness(0);
                    sVal = node["rtbMargin"].InnerText;
                    if (sVal.Length > 0)
                    {
                        string[] rtbMagin = sVal.Split(',');
                        _rtbMargin = new Thickness(Convert.ToDouble(rtbMagin[0]), Convert.ToDouble(rtbMagin[1]), Convert.ToDouble(rtbMagin[2]), Convert.ToDouble(rtbMagin[3]));
                    }
                    sVal = node["FormFixed"].InnerText;
                    if (sVal.Length > 0)
                        _bDesktopBack = Convert.ToBoolean(sVal);
                    else
                        _bDesktopBack = false;
                    sVal = node["TextColor"].InnerText;
                    if (sVal.Length > 0)
                        _TextColor = ColorExtensions.FromIntArgb(Convert.ToInt32(sVal));
                    else
                        _TextColor = Color.FromRgb(0, 0, 0);
                    sVal = node["dropShadowDepth"].InnerText;
                    if (sVal.Length > 0)
                        dropshadowDepth = Convert.ToDouble(sVal);
                    sVal = node["dropShadowOpacity"].InnerText;
                    if (sVal.Length > 0)
                        dropshadowOpacity = Convert.ToDouble(sVal);
                    sVal = node["dropShadowDirection"].InnerText;
                    if (sVal.Length > 0)
                        dropshadowDirection = Convert.ToDouble(sVal);
                    sVal = node["dropshadowColor"].InnerText;
                    if (sVal.Length > 0)
                        dropshadowColor = ColorExtensions.FromIntArgb(Convert.ToInt32(sVal));
                    sVal = node["dropShadow"].InnerText;
                    if (sVal.Length > 0)
                        _bShadow = Convert.ToBoolean(sVal);
                    sVal = node["TextFontFamily"].InnerText;
                    if (sVal.Length > 0)
                    {
                        //    _richTextBox.FontFamily = new FontFamily("Arial");
                        _cFontFamily = new FontFamily(sVal);
                        _richTextBox.FontFamily = new FontFamily(sVal);
                    }
                    else
                        _richTextBox.FontFamily = new FontFamily("Arial");
//                        _richTextBox.FontFamily = new FontFamily("Segoe Print");
                    sVal = node["TextFontSize"].InnerText;
                    if (sVal.Length > 0) {
                        _richTextBox.FontSize = Convert.ToDouble(sVal);
                        _cFontSize = Convert.ToDouble(sVal);
                    }
                    else
                    {
                        _richTextBox.FontSize = 12;
                        _cFontSize = Convert.ToDouble(12);
                    }
                    _richTextBox.FontSize = 12;
                    sVal = node["TextFontStyle"].InnerText;
                    if (sVal.Length > 0)
                    {
                        _richTextBox.FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(sVal);
                        _cFontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(sVal);
                    }
                    else
                    {
                        _richTextBox.FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString("Normal");
                        _cFontStyle = (FontStyle)new FontStyleConverter().ConvertFromString("Normal");
                    }
                    sVal = node["TextFontWeight"].InnerText;
                    if (sVal.Length > 0)
                    {
                        _richTextBox.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(sVal);
                        _cFontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(sVal);
                    }
                    else { 
                        _richTextBox.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString("Normal");
                        _cFontWeight = (FontWeight)new FontWeightConverter().ConvertFromString("Normal");
                    }
                    sVal = node["TextFontStretch"].InnerText;
                    if (sVal.Length > 0)
                    {
                        _richTextBox.FontStretch = (FontStretch)new FontStretchConverter().ConvertFromString(sVal);
                        _cFontStretch = (FontStretch)new FontStretchConverter().ConvertFromString(sVal);
                    }
                    else
                    {
                        _richTextBox.FontStretch = (FontStretch)new FontStretchConverter().ConvertFromString("Normal");
                        _cFontStretch = (FontStretch)new FontStretchConverter().ConvertFromString("Normal");
                    }
                    if (node["FontFavourites"] != null)
                    {
                        sVal = node["FontFavourites"].InnerText;
                        if (sVal.Length > 0)
                        {
                            FontFamily fItem;
                            foreach (var item in sVal.Split(','))
                            {
                                fItem = (FontFamily)new FontFamilyConverter().ConvertFromString(item);
                                _FontFavourites.Insert(0, fItem);
                               // ((InstalledFonts)(fontManager.fontList.ItemsSource)).Insert(0, fItem);
                              //  fontManager.fontList.Items.Refresh();
                              //  InstalledFonts.fontFamilies.Insert(0, fItem);
                            }
                        }
                    }
                    if (node["BackgrPicturePresetLast"] != null)
                    {
                        sVal = node["BackgrPicturePresetLast"].InnerText;
                        if (sVal.Length > 0)
                        {
                            _backrPicLast = sVal;
                        }
                    }
                    backgrPicPresets bps = new backgrPicPresets();
                    bps.fRed = _fRed;
                    bps.fGreen = _fGreen;
                    bps.fBlue = _fBlue;
                    bps.fBrightness = _fBrightness;
                    bps.fSaturation = _fSaturation;
                    bps.fContrast = _fContrast;
                    bps.presetName = "last";

                    bpPresets = bpPresets.Append(bps).ToArray();

                    bps.fRed = 1;
                    bps.fGreen = 1;
                    bps.fBlue = 1;
                    bps.fBrightness = 0;
                    bps.fSaturation = 1;
                    bps.fContrast = 1;
                    bps.presetName = "default";

                    bpPresets = bpPresets.Append(bps).ToArray();
                    if (node["BackgroundPicPresets"] != null)
                    {

                        foreach (XmlNode xmlNode in node["BackgroundPicPresets"].ChildNodes)
                        {
                            if (xmlNode.Name != "default" && xmlNode.Name != "last")
                            {
                                bps.fRed = (float)Convert.ToDouble(xmlNode["fRed"].InnerText);
                                bps.fGreen = (float)Convert.ToDouble(xmlNode["fGreen"].InnerText);
                                bps.fBlue = (float)Convert.ToDouble(xmlNode["fBlue"].InnerText);
                                bps.fBrightness = (float)Convert.ToDouble(xmlNode["fBrightness"].InnerText);
                                bps.fSaturation = (float)Convert.ToDouble(xmlNode["fSaturation"].InnerText);
                                bps.fContrast = (float)Convert.ToDouble(xmlNode["fContrast"].InnerText);
                                bps.presetName = xmlNode.Name;

                                bpPresets = bpPresets.Append(bps).ToArray();
                            }
                        }
                    }
                    sVal = node["FormBackgroundColor"].InnerText;
                    if (sVal.Length > 0)
                        _cBackColor = ColorExtensions.FromIntArgb(Convert.ToInt32(sVal));
                    else
                        _cBackColor = Color.FromArgb(0, 0, 0, 0);
                    sVal = node["posX"].InnerText;
                    if (sVal.Length > 0)
                        _pos.X = Convert.ToInt32(sVal);
                    sVal = node["posY"].InnerText;
                    if (sVal.Length > 0)
                        _pos.Y = Convert.ToInt32(sVal);

                    _lNotes.Clear();
                    list = node.SelectNodes("SubNote");
                    if (list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            sVal = list[i].InnerText;
                            if (sVal.Length > 0)
                            {
                                if (sVal.StartsWith("<Section xmlns"))
                                {
                                    TextRange tr = XAMLStringToTextRange(sVal);
                                    sVal = textRangeToRtfString(tr);
                                }
                                _lNotes.Add(sVal);
                            }
                        }

                    }
                    else
                        _lNotes.Add(textRangeToRtfString(_rtbTextRange));
//                        _lNotes.Add(textRangeToXAMLString(_rtbTextRange));
                    sVal = node["Index"].InnerText;
                    if (sVal.Length > 0)
                    {
                        _iNote = Convert.ToInt16(sVal);
                        if (_iNote >= _lNotes.Count)
                            _iNote = _lNotes.Count - 1;
                    }
                    else
                        _iNote = 0;
                    sVal = node["Width"].InnerText;
                    if (sVal.Length > 0)
                        _dblFormWidth = Convert.ToInt16(sVal);
                    else
                        _dblFormWidth = 200;
                    sVal = node["Height"].InnerText;
                    if (sVal.Length > 0)
                        _dblFormHeight = Convert.ToInt16(sVal);
                    else
                        _dblFormHeight = 300;

                    _BackgrSavedColor = _cBackColor;
                    _TextSavedColor = _TextColor;
                                        
                }
                else
                    _defaultSettings();
                
            }
            else
                _defaultSettings();

        }
        public void saveXML()
        {
            XmlNode node,subnode;
            XmlDocument doc = new XmlDocument();
            if (System.IO.File.Exists(filePath))
            {
                doc.Load("settings.xml");
            }
            else
            {
                doc.InsertBefore(doc.CreateXmlDeclaration("1.0", "iso8859-1", null), doc.DocumentElement);
                node = doc.CreateElement("DeskNote");
                doc.AppendChild(node);
            }
            node = doc.SelectSingleNode("//DeskNote/LastNote");
            if (node == null)
            {
                node = doc.SelectSingleNode("//DeskNote");
                node = node.AppendChild(doc.CreateElement("LastNote"));
            }
            node.InnerText = _sName;
            
            node = doc.SelectSingleNode("//DeskNote/Note[@name='" + _sName + "']");
            if(node == null)
            {
                XmlElement el = doc.CreateElement("Note");
                el.SetAttribute("name",_sName);
                node = doc.SelectSingleNode("//DeskNote");
                node.AppendChild(el);
                node = doc.SelectSingleNode("//DeskNote/Note[@name='" + _sName + "']");
            }
            subnode = editCreateNode(doc,node,"FormBackgroundGlass");
            subnode.InnerText = _bBlur.ToString();
            subnode = editCreateNode(doc,node,"BackgroundImage");
            subnode.InnerText = _bBckgrPic.ToString();
            subnode = editCreateNode(doc,node,"BackgroundImagePath");
            subnode.InnerText = _bckgrImagePath;
            subnode = editCreateNode(doc,node,"BorderThickness");
            subnode.InnerText = BorderThickness.ToString();
            subnode = editCreateNode(doc,node,"rtbMargin");
            subnode.InnerText = _rtbMargin.ToString();
            subnode = editCreateNode(doc,node,"FormFixed");
            subnode.InnerText = _bDesktopBack.ToString();
            subnode = editCreateNode(doc,node,"FormBackgroundColor");
            subnode.InnerText = ColorExtensions.ToIntArgb(_cBackColor).ToString();

            subnode = editCreateNode(doc,node, "dropShadowDepth");
            subnode.InnerText = dropShadowEffect.ShadowDepth.ToString();
            subnode = editCreateNode(doc,node, "dropShadowOpacity");
            subnode.InnerText = dropShadowEffect.Opacity.ToString();
            subnode = editCreateNode(doc,node, "dropShadowDirection");
            subnode.InnerText = dropShadowEffect.Direction.ToString();
            subnode = editCreateNode(doc,node, "dropshadowColor");
            subnode.InnerText = ColorExtensions.ToIntArgb(dropShadowEffect.Color).ToString();
            subnode = editCreateNode(doc,node, "dropShadow");
            subnode.InnerText = (_richTextBox.Effect != null).ToString();

            subnode = editCreateNode(doc,node,"TextColor");
            subnode.InnerText = ColorExtensions.ToIntArgb(_TextColor).ToString();
            subnode = editCreateNode(doc,node,"TextFontFamily");
//            subnode.InnerText = "Arial";//_richTextBox.FontFamily.Source;
            subnode.InnerText = _cFontFamily.Source;
            subnode = editCreateNode(doc,node,"TextFontSize");
            subnode.InnerText = Convert.ToInt16(_cFontSize).ToString();
//            subnode.InnerText = Convert.ToInt16(_richTextBox.FontSize).ToString();
            subnode = editCreateNode(doc,node,"TextFontStyle");
//            subnode.InnerText = _richTextBox.FontStyle.ToString();
            subnode.InnerText = _cFontStyle.ToString();
            subnode = editCreateNode(doc,node,"TextFontWeight");
//            subnode.InnerText = _richTextBox.FontWeight.ToString();
            subnode.InnerText = _cFontWeight.ToString();
            subnode = editCreateNode(doc,node,"TextFontStretch");
//            subnode.InnerText = _richTextBox.FontStretch.ToString();
            subnode.InnerText = _cFontStretch.ToString();
            subnode = editCreateNode(doc, node, "FontFavourites");
            subnode.InnerText = (String.Join(",", _FontFavourites.Select(x => x.Source)));
            subnode = editCreateNode(doc,node,"posX");
            subnode.InnerText = Convert.ToInt16(DPIunScaling(location).X).ToString();
            subnode = editCreateNode(doc,node,"posY");
            subnode.InnerText = Convert.ToInt16(DPIunScaling(location).Y).ToString();
            XmlNodeList notes = node.SelectNodes("SubNote");
            if (notes != null)
                foreach (XmlNode xmlnode in notes)
                    xmlnode.ParentNode.RemoveChild(xmlnode);
            for(int i = 0; i < _lNotes.Count;i++)
            {
                subnode = node.AppendChild(doc.CreateElement("SubNote"));                
                subnode.InnerText = _lNotes[i];
            }
            subnode = editCreateNode(doc,node,"Index");
            subnode.InnerText = _iNote.ToString();
            subnode = editCreateNode(doc,node,"Width");
            subnode.InnerText = Convert.ToInt16(_main.Width).ToString();
            subnode = editCreateNode(doc,node,"Height");
            subnode.InnerText = Convert.ToInt16(_main.Height).ToString();
            subnode = editCreateNode(doc,node,"BackgrPictureMatrixRed");
            subnode.InnerText = _fRed.ToString();
            subnode = editCreateNode(doc,node,"BackgrPictureMatrixGreen");
            subnode.InnerText = _fGreen.ToString();
            subnode = editCreateNode(doc,node,"BackgrPictureMatrixBlue");
            subnode.InnerText = _fBlue.ToString();
            subnode = editCreateNode(doc,node, "BackgrPictureMatrixBrightness");
            subnode.InnerText = _fBrightness.ToString();
            subnode = editCreateNode(doc,node, "BackgrPictureMatrixContrast");
            subnode.InnerText = _fContrast.ToString();
            subnode = editCreateNode(doc,node, "BackgrPictureMatrixSaturation");
            subnode.InnerText = _fSaturation.ToString();
            subnode = editCreateNode(doc,node, "BackgrPicturePresetLast");
            subnode.InnerText = _backrPicLast;

            subnode = editCreateNode(doc, node, "BackgroundPicPresets");
            foreach (backgrPicPresets bps in bpPresets)
            {
                if (bps.presetName != "default" && bps.presetName != "last") {
                    XmlNode parentsubnode = editCreateNode(doc, subnode, bps.presetName);
                    XmlNode innernode = editCreateNode(doc, parentsubnode, "fRed");
                    innernode.InnerText = bps.fRed.ToString();
                    innernode = editCreateNode(doc, parentsubnode, "fGreen");
                    innernode.InnerText = bps.fGreen.ToString();
                    innernode = editCreateNode(doc, parentsubnode, "fBlue");
                    innernode.InnerText = bps.fBlue.ToString();
                    innernode = editCreateNode(doc, parentsubnode, "fBrightness");
                    innernode.InnerText = bps.fBrightness.ToString();
                    innernode = editCreateNode(doc, parentsubnode, "fSaturation");
                    innernode.InnerText = bps.fSaturation.ToString();
                    innernode = editCreateNode(doc, parentsubnode, "fContrast");
                    innernode.InnerText = bps.fContrast.ToString();
                }
            }


            doc.Save("settings.xml");
            _bChanged = false;
        }
        void _defaultSettings()
        {
            if (_sName == "")
            {
                System.Globalization.CultureInfo cInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                _sName = DateTime.Now.ToString("G", cInfo);
            }
            _rtbTextRange.Text = _sName;
            _bBlur = false;
            _bBckgrPic = true;
            BorderThickness = new Thickness(0);
            _bDesktopBack = false;
            _TextColor = Color.FromRgb(0, 0, 0);
//            _richTextBox.FontFamily = new FontFamily("Segoe Print");
            _richTextBox.FontFamily = new FontFamily("Arial");
            _richTextBox.FontSize = 16;
            _richTextBox.FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString("Normal");
            _richTextBox.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString("Normal");
            _richTextBox.FontStretch = (FontStretch)new FontStretchConverter().ConvertFromString("Normal");
            _cBackColor= Color.FromArgb(0, 0, 0, 0);
            _lNotes.Clear();
//            _lNotes.Add(textRangeToXAMLString(_rtbTextRange));
            _lNotes.Add(textRangeToRtfString(_rtbTextRange));
            _iNote = 0;
            _dblFormWidth = 246;
            _dblFormHeight = 300;
            _BackgrSavedColor = Color.FromArgb(100, 100, 100, 100);
            _TextSavedColor = Color.FromRgb(255, 255, 255);
           // saveXML();
        }

        bool renameNoteName(string sOld,string sNew)
        {
            XmlNode node;
            XmlDocument doc = new XmlDocument();
            if (System.IO.File.Exists(filePath))
            {
                doc.Load("settings.xml");
                node = doc.SelectSingleNode("//DeskNote/Note[@name='" + sOld + "']");
                if (node != null)
                {
                    node.Attributes["name"].Value = sNew;
                    doc.Save("settings.xml");
                    return true;
                }
            }
            return false;
        }
        XmlNode editCreateNode(XmlDocument doc, XmlNode parent, string sNode)
        {
            XmlNode subNode = parent.SelectSingleNode(sNode);
            if (subNode == null)
                subNode = parent.AppendChild(doc.CreateElement(sNode));
            return subNode;
        }
        string[] noteList()
        {
            string[] sNoteList;

            if (System.IO.File.Exists(filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("settings.xml");

                XmlNodeList list;
                list = doc.SelectNodes("//DeskNote/Note");
                if (list != null)
                {
                    sNoteList = new string[list.Count];
                    for (int i = 0; i < list.Count; i++)
                    {
                        sNoteList[i] = list[i].Attributes["name"].Value;
                    }
                    return sNoteList;
                }
            }
            return null;

        }
        void deleteSubNote(int iIndex)
        {
            if (System.IO.File.Exists(filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("settings.xml");

                XmlNodeList list;
                XmlNode node;
                node = doc.SelectSingleNode("//DeskNote/Note[@name='" + _sName + "']");
                list = node.SelectNodes("SubNote");
                if (list != null && list.Count > iIndex)
                {
                    list[iIndex].ParentNode.RemoveChild(list[iIndex]);
                    doc.Save("settings.xml");
                }
            }
        }

        private byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        #endregion

        #region RTF,XAML conversions

        private string textRangeToXAMLString(TextRange tr)
        {
            using (Stream ms = new MemoryStream())
            {
                tr.Save(ms, DataFormats.Xaml);
                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        private TextRange XAMLStringToTextRange(String xaml, TextRange tr)
        {
            if(xaml == "")
            {
                tr.ClearAllProperties();
                tr.Text = "";
                return tr;
            }
            using (Stream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(xaml);
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    TextRange t = new TextRange(tr.Start, tr.End);
                    t.Load(ms, DataFormats.Xaml);
                    tr = t;
                    return t;
                }
            }
        }
        private TextRange XAMLStringToTextRange(String xaml)
        {
            using (Stream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    FlowDocument workDoc = new FlowDocument();
                    TextRange tr = new TextRange(workDoc.ContentStart, workDoc.ContentEnd);
                    sw.Write(xaml);
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    tr.Load(ms, DataFormats.Xaml);
                    return tr;
                }
            }
        }
        private String XAMLStringToText(String xaml)
        {
            FlowDocument workDoc = new FlowDocument();
            TextRange selection = new TextRange(workDoc.ContentStart, workDoc.ContentEnd);

            using (Stream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(xaml);
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    selection.Load(ms, DataFormats.Xaml);
                    return selection.Text;
                }
            }
        }

        private string textRangeToRtfString(TextRange tr)
        {
            using (Stream ms = new MemoryStream())
            {
                tr.Save(ms, DataFormats.Rtf);
                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        private TextRange RtfStreamToTextRange(Stream rtfStream, TextRange tr)
        {
                    TextRange t = new TextRange(tr.Start, tr.End);
                    t.Load(rtfStream, DataFormats.Rtf);
                    tr = t;
                    return t;
        }
        private TextRange RtfStringToTextRange(String rtf, TextRange tr)
        {
            if (rtf == "")
            {
                tr.ClearAllProperties();
                tr.Text = "";
                return tr;
            }
            using (Stream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(rtf);
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    TextRange t = new TextRange(tr.Start, tr.End);
                    t.Load(ms, DataFormats.Rtf);
                    tr = t;
                    return t;
                }
            }
        }
        private Stream RtfStringToStream(String rtf)
        {
            using (Stream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(rtf);
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    Stream x = new MemoryStream();
                    ms.CopyTo(x);
                    return x;
                }
            }
        }
       private String RtfStringToText(String rtf)
        {
            FlowDocument workDoc = new FlowDocument();
            TextRange selection = new TextRange(workDoc.ContentStart, workDoc.ContentEnd);

            using (Stream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(rtf);
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    selection.Load(ms, DataFormats.Rtf);
                    return selection.Text;
                }
            }
        }

        #endregion

        #region context menues
        //public void cmLoadNotes_Init()
        //{
        //    MenuItem mi;

        //    string[] sSections = noteList();
        //    if (sSections != null)
        //    {
        //        cmLoad.Items.Clear();
        //        for (int i = 0; i < sSections.Length; i++)
        //        {
        //            if (sSections[i] != "")
        //            {
        //                mi = new MenuItem();
        //                mi.Click += cmSettingsLoad_Click;
        //                mi.Header = mi.Tag = sSections[i];
        //                if (mi.Tag.ToString() == _sName)
        //                    mi.IsChecked = true;
        //                cmLoad.Items.Add(mi);
        //            }
        //        }
        //    }

        //}
        //Window-ContextMenue

        ////Top Submenue
        private void cmNew_Click(object sender, RoutedEventArgs e)
        {
            string sNew = _newNote();
            if (sNew != "")
            {
                _sName = sNew;
                _init();
            }
        }
        private void cmRename_Click(object sender, RoutedEventArgs e)
        {
            _rename();
        }
        private void cmSettingsLoad_Click(object sender, RoutedEventArgs e)
        {
            saveXML();
            string[] sNames = new string[2]{_sName, ((MenuItem)sender).Tag.ToString()};
            _sName = ((MenuItem)sender).Tag.ToString();
            noteChanged(sNames, null);
            _init();
            _main_Loaded(null, null);
        }
        private void cmSettingsSave_Click(object sender, RoutedEventArgs e)
        {
            saveXML();
        }

        ////Mainmenue
        private void cmInsertText_Click(object sender, RoutedEventArgs e)
        {
            _insertText();
        }
        private void cmAddText_Click(object sender, RoutedEventArgs e)
        {
            _appendClipboardText();
        }
        //
        private void cmSendToBack_Click(object sender, RoutedEventArgs e)
        {
            _sendToBack(true,true);
            winApi.SystemParametersInfo(winApi.SPI_SETDESKWALLPAPER, 0, null, winApi.SPIF_UPDATEINIFILE);
        }
        private void cmBringToFront_Click(object sender, RoutedEventArgs e)
        {
            _sendToBack(false,true);
            winApi.SystemParametersInfo(winApi.SPI_SETDESKWALLPAPER, 0, null, winApi.SPIF_UPDATEINIFILE);
        }
        private void cmShowWindow_Click(object sender, RoutedEventArgs e)
        {
            _show(true);
        }
        private void cmHideWindow_Click(object sender, RoutedEventArgs e)
        {
            _hide(true);
        }

        ////Background Submenue
        private void cmBlur_Click(object sender, RoutedEventArgs e)
        {
            bool bBlur = false;
            if (cmBlur.IsChecked)
                bBlur = true;

            _blur=bBlur;

        }
        private void cmImage_Click(object sender, RoutedEventArgs e)
        {
            bool bImage = false;
            if (cmImage.IsChecked)
            {
                bImage = true;
                cmImage.IsChecked = false;
            }
            else cmImage.IsChecked = true;

            _BckgrPic =bImage;
            cmPresets.IsEnabled = bImage;
            //string[] sObject = new string[] { _sName, "Background picture" };
            //noteChanged(sObject, null);
        }
        public void cmPresets_Init()
        {
            MenuItem mi;
            cmPresets.Items.Clear();
            foreach (backgrPicPresets bps in bpPresets)
            {
                mi = new MenuItem();
                mi.IsCheckable = true;
                mi.Click += cmPreset_Click;
                mi.Header = mi.Tag = bps.presetName;
                if (mi.Tag.ToString() == _backrPicLast)
                    mi.IsChecked = true;
                cmPresets.Items.Add(mi);
            }
            if (_BckgrPic) cmPresets.IsEnabled = true;
        }
        private void cmPreset_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)((MenuItem)sender).Parent;
            int index = mi.Items.IndexOf((MenuItem)sender);
            if(index > -1)
            {
                _fRed = bpPresets[index].fRed;
                _fGreen = bpPresets[index].fGreen;
                _fBlue = bpPresets[index].fBlue;
                _fBrightness = bpPresets[index].fBrightness;
                _fSaturation = bpPresets[index].fSaturation;
                _fContrast = bpPresets[index].fContrast;
                _backrPicLast = bpPresets[index].presetName;
                SetBckgrPicColor();
                foreach (MenuItem mx in mi.Items)
                {
                    if (mx.Header == ((MenuItem)sender).Header) mx.IsChecked = true;
                    else mx.IsChecked = false;
                }
            }
        }

        ////Mainmenue
        private void cmSettings_Click(object sender, RoutedEventArgs e)
        {
            _settings();
        }
        //
        bool bScreenshotFile = false;
        void cmScreenshot_Click(object sender, RoutedEventArgs e)
        {
            bScreenshotFile = false;
            tScreenshot.Start();
        }
        void cmScreenshotFile_Click(object sender, RoutedEventArgs e)
        {
            bScreenshotFile = true;
            tScreenshot.Start();
        }
        //
        private void cmExit_Click(object sender, RoutedEventArgs e)
        {
            _exit();
        }

        ////Textbox-ContextMenue
        private void cmPasteHTML_click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dataObject = Clipboard.GetDataObject();
                var textData = dataObject.GetData(DataFormats.Html);
                if (textData != null)
                {
                    Type wordType = Type.GetTypeFromProgID("Word.Application");
                    if (wordType != null)
                    {
                        dynamic msword = Activator.CreateInstance(wordType);
                        msword.Visible = false;
                        msword.Documents.Add();
                        msword.Windows[1].Selection.Paste();
                        msword.Windows[1].Selection.WholeStory();
                        msword.Windows[1].Selection.Copy();
                        _richTextBox.Paste();
                        msword.Quit(wordDoNotSaveChanges);
                    }
                }
            }
            catch (Exception)
            {

                ;
            }
        }
        private void cmSaveRTF_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog myDlg = new Microsoft.Win32.SaveFileDialog();
            myDlg.DefaultExt = "*.rtf";
            myDlg.Filter = "RTF Files|*.rtf";
            Nullable<bool> myResult = myDlg.ShowDialog();

            if (myResult == true)
            {
                _richTextBox.SelectAll();
                using (FileStream fs = new FileStream(myDlg.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    _richTextBox.Selection.Save(fs, DataFormats.Rtf);  
                } 
            }
        }
        private void cmLoadRTF_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog myDlg = new Microsoft.Win32.OpenFileDialog();
            myDlg.DefaultExt = "*.rtf";
            myDlg.Filter = "RTF Files|*.rtf";
            Nullable<bool> myResult = myDlg.ShowDialog();

            if (myResult == true)
            {
                /*using ( FileStream myStream = new FileStream( myDlg.FileName, FileMode.OpenOrCreate, FileAccess.Write ) ) {
                    TextRange myRange = new TextRange( rtbTraffic.Document.ContentStart, rtbTraffic.Document.ContentEnd );
                    myRange.Save( myStream, DataFormats.Rtf );
                    myStream.Close();
                }*/
                _richTextBox.SelectAll();
                using (FileStream fs = new FileStream(myDlg.FileName, FileMode.Open, FileAccess.Read))
                {
                    _richTextBox.Selection.Load(fs, DataFormats.Rtf);
                }
            }
        }


        #endregion context menues

        void _rename()
        {
            newNote nNote = new newNote();
            nNote.tbNoteName.Text = _sName;
            string sOld = _sName;
            nNote.ShowDialog();
            if (nNote.result)
            {
                _sName = nNote.tbNoteName.Text;
                renameNoteName(sOld, _sName);
                saveXML();
                string[] sNames = new string[2]{_sName, sOld};
                noteRenamed(sNames,null);
                _init();
            }

        }
        string _newNote()
        {
            newNote nNote = new newNote();
            nNote.ShowDialog();
            if (nNote.result)
            {
                if (_sName != "")
                    saveXML();
                return nNote.tbNoteName.Text;
            }
            return "";
        }
        void _appendClipboardText()
        {
            if (Clipboard.ContainsText())
            {
                DependencyObject doOld = _rtbTextRange.End.GetAdjacentElement(LogicalDirection.Backward);
                _rtbTextRange.End.InsertTextInRun(Clipboard.GetText());
                DependencyObject doNew = _rtbTextRange.End.GetAdjacentElement(LogicalDirection.Backward);
                doNew.SetValue(TextElement.FontSizeProperty, doOld.GetValue(TextElement.FontSizeProperty));
                doNew.SetValue(TextElement.FontStyleProperty, doOld.GetValue(TextElement.FontStyleProperty));
                doNew.SetValue(TextElement.FontWeightProperty, doOld.GetValue(TextElement.FontWeightProperty));
                doNew.SetValue(TextElement.BackgroundProperty, doOld.GetValue(TextElement.BackgroundProperty));
                doNew.SetValue(TextElement.ForegroundProperty, doOld.GetValue(TextElement.ForegroundProperty));
//                _lNotes[_iNote] = textRangeToXAMLString(_rtbTextRange);
                _lNotes[_iNote] = textRangeToRtfString(_rtbTextRange);
            }
		}
        void _insertText()
        {
            if (Clipboard.ContainsText() && _richTextBox.Selection != null)
            {
                TextPointer tp = _richTextBox.CaretPosition.GetNextContextPosition(LogicalDirection.Backward);
                if (tp != null)
                {
                    DependencyObject doOld = tp.GetAdjacentElement(LogicalDirection.Forward);
                    if (doOld == null)
                        doOld = tp.GetAdjacentElement(LogicalDirection.Backward);
                    if (doOld != null)
                    {
                        _richTextBox.Selection.End.InsertTextInRun(Clipboard.GetText());
                        DependencyObject doNew = _richTextBox.CaretPosition.GetNextContextPosition(LogicalDirection.Forward).GetAdjacentElement(LogicalDirection.Forward);
                        doNew.SetValue(TextElement.FontSizeProperty, doOld.GetValue(TextElement.FontSizeProperty));
                        doNew.SetValue(TextElement.FontStyleProperty, doOld.GetValue(TextElement.FontStyleProperty));
                        doNew.SetValue(TextElement.FontWeightProperty, doOld.GetValue(TextElement.FontWeightProperty));
                        doNew.SetValue(TextElement.BackgroundProperty, doOld.GetValue(TextElement.BackgroundProperty));
                        doNew.SetValue(TextElement.ForegroundProperty, doOld.GetValue(TextElement.ForegroundProperty));
 //                       _lNotes[_iNote] = textRangeToXAMLString(_rtbTextRange);
                        _lNotes[_iNote] = textRangeToRtfString(_rtbTextRange);
                    }
                }
            }
        }
        public void _show(bool bLocal)
        {
            Visibility = Visibility.Visible;
            cmShow.IsChecked = true;
            cmShow.IsEnabled = false;
            cmHide.IsChecked = false;
            cmHide.IsEnabled = true;
            if (bLocal)
            {
                string[] sObject = new string[] {_sName,"Show" };
                noteChanged(sObject, null);
            }
        }
        public void _hide(bool bLocal)
        {
            Visibility = Visibility.Hidden;
            cmShow.IsChecked = false;
            cmShow.IsEnabled = true;
            cmHide.IsChecked = true;
            cmHide.IsEnabled = false;
            if (bLocal)
            {
                string[] sObject = new string[] { _sName, "Hide" };
                noteChanged(sObject, null);
            }
        }
        public void _exit()
        {
            if (!_bExit)
            {
                _bExit = true;
                saveXML();
               base.Close();
            }
        }
        public bool _BckgrPic
        {
            set
            {
                if (value)
                {
                    if (_imgBackgr.UriSource == _defaultBckgrUri)
                    {
                        _bckgrImageWidth = _imgBackgr.PixelWidth;
                        _bckgrImageHeight = _imgBackgr.PixelHeight;
                        _Grid.Background = new ImageBrush(_imgBackgr);
                        double dblPadL = 35 * _Grid.ActualWidth / _bckgrImageWidth;
                        double dblPadR = 65 * _Grid.ActualWidth / _bckgrImageWidth;
                        double dblPadB = 90 * _Grid.ActualHeight / _bckgrImageHeight;
                        _richTextBox.Margin = new Thickness(dblPadL, 20, dblPadR, dblPadB);
                        //    imageToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
                        cmImage.IsChecked = true;
                        _bBckgrPic = true;
                        BorderThickness = new Thickness(0);
                        _BackgroundAlpha = 0;
                        dblPadL = 3 * _Grid.ActualWidth / _bckgrImageWidth;
                        dblPadR = 25 * _Grid.ActualWidth / _bckgrImageWidth;
                        _topGrid.Margin = new Thickness(dblPadL, -3, dblPadR, 0);
                    }
                    else
                    {
                        _Grid.Background = new ImageBrush(_imgBackgr);
                        cmImage.IsChecked = true;
                        _bBckgrPic = true;
                        BorderThickness = new Thickness(0);
                        _BackgroundAlpha = 0;
                        _richTextBox.Margin = _rtbMargin;//new Thickness(10,15,10,10);
                        _topGrid.Margin = new Thickness(-3, -3, -3, 0);
                    }
                }
                else
                {
                    _Grid.Background = null;
                    _richTextBox.Margin = new Thickness(5,10,5,5);
                //    imageToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Unchecked;
                    cmImage.IsChecked = false;
                    _bBckgrPic = false;
                    BorderThickness = new Thickness(_dblBorderThickness);
                    _BackgroundAlpha = _BackgrSavedColor.A == 0 ? (byte)255 : _BackgrSavedColor.A;
                    _topGrid.Margin = new Thickness(-3, -3, -3, 0);
                }

            }
            get { return _bBckgrPic; }
        }        
        public bool _blur
        {
            set
            {
                if (value)
                {
                    winApi.EnableBlur(this, AccentState.ACCENT_ENABLE_BLURBEHIND);
                 //   blurrToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
                    cmBlur.IsChecked = true;
                    _bBlur = true;
                }
                else
                {
                    winApi.EnableBlur(this, AccentState.ACCENT_DISABLED);
                 //   blurrToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Unchecked;
                    cmBlur.IsChecked = false;
                    _bBlur = false;
                }
            }
            get { return _bBlur; }
        }
        public void _settings()
        {
            settings _set = new settings(this);
            _set.Top = Top;
            if(Left < 290)
                _set.Left = Left + 290;
            else
                _set.Left = Left-290;
            _set.ShowDialog();
        }

        void _setText()
        {
            if (_lNotes[_iNote] != "")
            {
//                _rtbTextRange = XAMLStringToTextRange(_lNotes[_iNote], _rtbTextRange);
                _rtbTextRange = RtfStringToTextRange(_lNotes[_iNote], _rtbTextRange);
            }
            else
            {
                _rtbTextRange.ClearAllProperties();
                _rtbTextRange.Text = "";
            }
            _sNote = _rtbTextRange.Text;
            lblAct.Content = (_iNote + 1).ToString() + " / " + _lNotes.Count.ToString();
            _richTextBox.FontFamily = _cFontFamily;
            _richTextBox.Foreground = new SolidColorBrush(_cTextColor);
            _richTextBox.FontStretch = _cFontStretch;
            _richTextBox.FontWeight = _cFontWeight;
            _richTextBox.FontStyle = _cFontStyle;
            _richTextBox.FontSize = _cFontSize;
        }
        void _setNavList()
        {
            string s;
            for (int i = 0; i < _lNotes.Count; i++)
            {
//                s = XAMLStringToText(_lNotes[i]).Trim();
                s = RtfStringToText(_lNotes[i]).Trim();

              //  s = s.Length > 40 ? s.Substring(0, 40) : s;
                listBoxNav.Items.Add(s + "\r\n\r\n========= " + (i+1).ToString() + " =========\r\n\r\n");
            }
        }
        void _newURL(object sender, RoutedEventArgs e)
        {
            if (_richTextBox.Selection != null && !_richTextBox.Selection.IsEmpty && _richTextBox.Selection.Text != "")
            {
                newURL nURL = new newURL();
                nURL.lblLink.Content = _richTextBox.Selection.Text;
                nURL.ShowDialog();
                if (nURL.result)
                {
                    TextPointer start = _richTextBox.Selection.Start;
                    TextPointer end = _richTextBox.Selection.End;
                    Uri uri = new Uri(nURL.tbURLName.Text, UriKind.RelativeOrAbsolute);
                    if (!uri.IsAbsoluteUri)
                    {
                        uri = new Uri(@"http://" + nURL.tbURLName.Text, UriKind.Absolute);
                    }
                    if (uri != null)
                    {
                        new Hyperlink(start, end)
                        {
                            NavigateUri = uri,

                        }.Click += Hyperlink_Click;
                        _richTextBox_TextChanged(null, null);
                    }

                }
            }
        }
        void _editURL(object sender, RoutedEventArgs e)
        {
            TextPointer pos;
            Inline parent;
            string sText = "";
            if (_richTextBox.Selection.IsEmpty)
            {
                pos = _richTextBox.CaretPosition;
                parent = pos.Parent as Inline;
                while (parent != null && !(parent is Hyperlink))
                {
                    parent = parent.Parent as Inline;
                }
            }
            else
            {
                pos = _richTextBox.Selection.Start;
                parent = pos.GetAdjacentElement(LogicalDirection.Forward) as Inline;
                sText = _richTextBox.Selection.Text;
            }
            if (parent is Hyperlink)
            {
                Hyperlink hl = (Hyperlink)parent;
                newURL nURL = new newURL();
                if (sText != "")
                    nURL.lblLink.Content = sText;
                else
                    nURL.lblLink.Content = ((Run)pos.Parent).Text;
                nURL.tbURLName.Text = hl.NavigateUri.AbsoluteUri;
                nURL.ShowDialog();
                if (nURL.result)
                {
                    Uri uri = new Uri(nURL.tbURLName.Text, UriKind.RelativeOrAbsolute);
                    if (!uri.IsAbsoluteUri)
                    {
                        uri = new Uri(@"http://" + nURL.tbURLName.Text, UriKind.Absolute);
                    }
                    if (uri != null)
                    {
                        ((Hyperlink)hl).NavigateUri = uri;
                        _richTextBox_TextChanged(null, null);
                    }

                }
            }
        }
        void _deleteURL(object sender, RoutedEventArgs e)
        {
            clearHyperlink();
            _richTextBox_TextChanged(null, null);               
        }

        #region flash background when doubleclicking 
        public static readonly RoutedEvent FlashEvent = EventManager.RegisterRoutedEvent("FlashWindow",
           RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(DeskNoteWindow));

        public event RoutedEventHandler FlashWindow
        {
            add { AddHandler(FlashEvent, value); }
            remove { RemoveHandler(FlashEvent, value); }
        }
        public void _flashWindow()
        {
            _main.RaiseEvent(new RoutedEventArgs(FlashEvent));
            
        }
        #endregion
        protected override void OnRender(DrawingContext dc)
        {
            SetBckgrPicColor();
        }
        public void SetBckgrPicColor()
        {
            if(_bBckgrPic)
            {
                //System.Drawing.Bitmap ImageBack = new System.Drawing.Bitmap(Properties.Resources.Note);
                System.Drawing.Bitmap ImageBack = BitmapImage2Bitmap(_imgBackgr);

                ImageAttributes imageAttributes = new System.Drawing.Imaging.ImageAttributes();
                ColorMatrix colorMatrix = CreateColorMatrix();

                imageAttributes.SetColorMatrix(
                   colorMatrix,
                   ColorMatrixFlag.Default,
                   ColorAdjustType.Bitmap);

                System.Drawing.Rectangle BaseRectangle = new System.Drawing.Rectangle(0, 0, ImageBack.Width, ImageBack.Height);
                System.Drawing.Bitmap bmpNew = new System.Drawing.Bitmap(ImageBack.Width, ImageBack.Height);
                using(System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmpNew))
                {
                    g.DrawImage(ImageBack, BaseRectangle, 0, 0, ImageBack.Width, ImageBack.Height, System.Drawing.GraphicsUnit.Pixel, imageAttributes);
                }

                _Grid.Background = new ImageBrush(ConvertToWpfBitmap(bmpNew));
            }
        }
        private const float LumR = 0.3086f;  // or  0.2125f
        private const float LumG = 0.6094f;  // or  0.7154f
        private const float LumB = 0.0820f;  // or  0.0721f

        private ColorMatrix CreateColorMatrix()
        {
            if (_fBrightness < -1f) _fBrightness = -1f;
            if (_fBrightness > 1f) _fBrightness = 1f;
            if (_fContrast < 0f) _fContrast = 0f;
            if (_fSaturation < 0f) _fSaturation = 0f;

            float Wf = (1f - _fContrast) / 2f + _fBrightness;
            float Rf = (1f - _fSaturation) * LumR * _fContrast * _fRed;
            float Gf = (1f - _fSaturation) * LumG * _fContrast * _fGreen;
            float Bf = (1f - _fSaturation) * LumB * _fContrast * _fBlue;
            float Rf2 = (Rf + _fSaturation * _fContrast * _fRed);
            float Gf2 = (Gf + _fSaturation * _fContrast * _fGreen);
            float Bf2 = (Bf + _fSaturation * _fContrast * _fBlue);

            return (new ColorMatrix(new float[][]
            {
                new float[] {Rf2, Rf,  Rf,  0f,  0f},
                new float[] {Gf,  Gf2, Gf,  0f,  0f},
                new float[] {Bf,  Bf,  Bf2, 0f,  0f},
                new float[] {0f,  0f,  0f,  1f,  0f},
                new float[] {Wf,  Wf,  Wf,  0f,  1f}
            }));
        }
        //This method is taken from http://stackoverflow.com/questions/94456/load-a-wpf-bitmapimage-from-a-system-drawing-bitmap/6775114#6775114
        //for converting a System.Drawing.Bitmap to WPF BitmapSource
        public static BitmapSource ConvertToWpfBitmap(System.Drawing.Bitmap bitmap)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
        private System.Drawing.Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            //BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {

                PngBitmapEncoder enc = new PngBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new System.Drawing.Bitmap(bitmap);
            }
            //  return new System.Drawing.Bitmap(bitmapImage.StreamSource);
        }
        #region buttons
        private void btnDel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_lNotes.Count > 1)
            {
                BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/Del_Hover.png"));
                btnDel.Fill = new ImageBrush(image);
            }

        }
        private void btnDel_MouseLeave(object sender, MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/Del.png"));
            btnDel.Fill = new ImageBrush(image);

        }
        private void btnDel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_lNotes.Count > 1)
            {
                BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/Del_Press.png"));
                btnDel.Fill = new ImageBrush(image);
            }

        }
        private void btnDel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_lNotes.Count > 1)
            {
                BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/Del_Hover.png"));
                btnDel.Fill = new ImageBrush(image);
                _lNotes.RemoveAt(_iNote);
                listBoxNav.Items.RemoveAt(_iNote);
                deleteSubNote(_iNote);
                if (_iNote > 0)
                {
                    _iNote--;
                }
                _bSetText = true;
                _setText();
                if (_lNotes.Count == 1)
                {
                    image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/Del.png"));
                    btnDel.Fill = new ImageBrush(image);
                    navGrid.Visibility = Visibility.Hidden;
                }
                
            }
        }

        private void btnAdd_MouseEnter(object sender, MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/Add_Hover.png"));
            btnAdd.Fill = new ImageBrush(image);

        }
        private void btnAdd_MouseLeave(object sender, MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/Add.png"));
            btnAdd.Fill = new ImageBrush(image);

        }
        private void btnAdd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/Add_Press.png"));
            btnAdd.Fill = new ImageBrush(image);

        }
        private void btnAdd_MouseUp(object sender, MouseButtonEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/Add_Hover.png"));
            btnAdd.Fill = new ImageBrush(image);

            if (_iNote < _lNotes.Count - 1)
            {
                _iNote++;
                _lNotes.Insert(_iNote, "");
                listBoxNav.Items.Insert(_iNote, "");
            }
            else
            {

                _lNotes.Add("");
                _iNote++;
                listBoxNav.Items.Add("");
            }
            _bSetText = true;
            _setText();
            navGrid.Visibility = Visibility.Visible;
            if (_bChanged)
                saveXML();
        }

        private void btnLeft_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_iNote > 0)
            {
                BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/left_hover.png"));
                btnLeft.Fill = new ImageBrush(image);
            }

        }
        private void btnLeft_MouseLeave(object sender, MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/left_grey.png"));
            btnLeft.Fill = new ImageBrush(image);

        }
        private void btnLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_iNote > 0)
            {
                BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/left_press.png"));
                btnLeft.Fill = new ImageBrush(image);
            }

        }
        private void btnLeft_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_iNote > 0)
            {
                BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/left_hover.png"));
                btnLeft.Fill = new ImageBrush(image);
                if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
                    _iNote = 0;
                else
                   _iNote--;
                _bSetText = true;
                _setText();
                if (_iNote == 0)
                    btnLeft_MouseLeave(null, null);
                if (_bChanged)
                    saveXML();
            }
        }
        private void btnRight_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_iNote < _lNotes.Count - 1)
            {
                BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/right_hover.png"));
                btnRight.Fill = new ImageBrush(image);
            }

        }
        private void btnRight_MouseLeave(object sender, MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/right_grey.png"));
            btnRight.Fill = new ImageBrush(image);

        }
        private void btnRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_iNote < _lNotes.Count - 1)
            {
                BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/right_press.png"));
                btnRight.Fill = new ImageBrush(image);
            }

        }
        private void btnRight_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(_iNote < _lNotes.Count -1)
            {
                BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/navi/right_hover.png"));
                btnRight.Fill = new ImageBrush(image);
                if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
                    _iNote = _lNotes.Count - 1;
                else
                    _iNote++;
                _bSetText = true;
                _setText();
                if (_iNote == _lNotes.Count - 1)
                    btnRight_MouseLeave(null, null);
                if (_bChanged)
                    saveXML();
            }

        }

        #endregion buttons
        private void lblAct_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!listBoxNav.IsVisible)
            {
                listBoxNav.SelectedIndex = _iNote;
                listBoxNav.ScrollIntoView(listBoxNav.SelectedItem);
            }
            listBoxNav.Visibility = listBoxNav.IsVisible ? Visibility.Collapsed : Visibility.Visible;
        }
        private void listBoxNav_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(_listBoxNavSelectionChanged)
            {
                listBoxNav.Visibility = Visibility.Collapsed;
                _listBoxNavSelectionChanged = false;
            }
        }
        bool _listBoxNavSelectionChanged;


        private void listBoxNav_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxNav.SelectedIndex > -1 && listBoxNav.IsVisible)
            {
                _iNote = listBoxNav.SelectedIndex;
                _bSetText = true;
                _setText();
                _listBoxNavSelectionChanged = true;
                if (_bChanged)
                    saveXML();
            }
        }

        //----------get 'real' Windows version--------------------
        // as https://msdn.microsoft.com/en-us/library/windows/desktop/ms724832(v=vs.85).aspx states:
        // Applications not manifested for Windows 8.1 or Windows 10 will return the Windows 8 OS version value (6.2). To manifest your applications for Windows 8.1 or Windows 10, refer to Targeting your application for Windows. --> https://msdn.microsoft.com/en-us/library/windows/desktop/dn481241(v=vs.85).aspx
        string _getOSVersion()
        {
            string sVersion = "";
            string query = "SELECT Version FROM Win32_OperatingSystem";
            System.Management.ManagementObjectSearcher seeker = new System.Management.ManagementObjectSearcher(query);
            System.Management.ManagementObjectCollection oReturnCollection = seeker.Get();
            foreach (System.Management.ManagementObject m in oReturnCollection)
            {
                sVersion = m["Version"].ToString();
            }
            return sVersion;
        }

        #region hyperlinks
        //parts of this code have been taken from http://blogs.msdn.com/b/prajakta/archive/2006/10/17/autp-detecting-hyperlinks-in-richtextbox-part-i.aspx 
        static string pattern = @"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?" + 
								@"(?#Username:Password)(?:\w+:\w+@)?" + 
								@"(?#Subdomains)((?:(?:[-\w]+\.)" + 
								@"+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))|" + 
								@"((?#IP address)((25[0-5]|2[0-4][0-9]|[01]?[1-9][0-9]?)\.){1}((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){2}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)))" + 
								@"(?#Port)(?::[\d]{1,5})?" + 
								@"(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&amp;(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*" + 
								@"(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?";

        private static readonly System.Text.RegularExpressions.Regex UrlRegex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        //parts of this code have been taken from http://marcangers.com/detect-urls-add-hyperlinks-wpf-richtextbox-automatically/ 
        public static bool isHyperlink(string word)
        {
            // First check to make sure the word has at least one of the characters we need to make a hyperlink
            if (word.IndexOfAny(@":.\/".ToCharArray()) != -1)
            {
                if (Uri.IsWellFormedUriString(word, UriKind.Absolute))
                {
                    // The string is an Absolute URI
                    return true;
                }
                else if (UrlRegex.IsMatch(word))
                {
                    Uri uri = new Uri(word, UriKind.RelativeOrAbsolute);

                    if (!uri.IsAbsoluteUri)
                    {
                        // rebuild it it with http to turn it into an Absolute URI
                        try
                        {
                            uri = new Uri(@"http://" + word, UriKind.Absolute);
                        }
                        catch
                        {                            
                            return false;
                        }
                    }

                    if (uri.IsAbsoluteUri)
                    {
                        return true;
                    }
                }
                else
                {
                    try
                    {
                        Uri wordUri = new Uri(word);
                        // Check to see if URL is a network path
                        if (wordUri.IsUnc || wordUri.IsFile)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        
                        return false;
                    }

                }
            }
            
            return false;
        }
        private static bool isHyperlinkProperty(DependencyProperty dp)
        {
            return dp == Hyperlink.CommandProperty ||
                dp == Hyperlink.CommandParameterProperty ||
                dp == Hyperlink.CommandTargetProperty ||
                dp == Hyperlink.NavigateUriProperty ||
                dp.Name == "BaseUri" ||
                dp.Name == "IsHyperlinkPressed" ||
                dp == Hyperlink.TargetNameProperty;
        }
        public void scan4URL(object sender, RoutedEventArgs e)
        {
            //clearHyperlinks();
            TextRange tr = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
            TextPointer tp = tr.Start;
            bool bFound = false;
            System.Text.RegularExpressions.MatchCollection regexMatches = UrlRegex.Matches(tr.Text);
            foreach (System.Text.RegularExpressions.Match match in UrlRegex.Matches(tr.Text))
            {
                bFound = false;
                if (isHyperlink(match.Value))
                {
                    if(tp == null)
                        tp = tr.Start;
                    while (tp != null && !bFound)
                    {

                        if (tp.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                        {
                            string textRun = tp.GetTextInRun(LogicalDirection.Forward);
                            int indexInRun = textRun.IndexOf(match.Value);
                            if (indexInRun > -1)
                            {
                                bFound = true;
                                Inline parent = tp.Parent as Inline;
                                while (parent != null && !(parent is Hyperlink))
                                {
                                    parent = parent.Parent as Inline;
                                }
                                if (parent == null)
                                {

                                    TextPointer start = tp.GetPositionAtOffset(indexInRun);
                                    TextPointer end = start.GetPositionAtOffset(match.Length);
                                    Uri uri = new Uri(match.Value, UriKind.RelativeOrAbsolute);
                                    if (!uri.IsAbsoluteUri)
                                    {
                                        uri = new Uri(@"http://" + match.Value, UriKind.Absolute);
                                    }
                                    if (uri != null)
                                        new Hyperlink(start, end)
                                        {
                                            NavigateUri = uri,

                                        }.Click += Hyperlink_Click;
                                }
                            }

                        }
                        tp = tp.GetNextContextPosition(LogicalDirection.Forward);
                    }
                }
            }
            _richTextBox_TextChanged(null, null);
        }

        void clearHyperlink()
        {
            Hyperlink hyperlink = null;
            TextPointer pos;
            Inline parent;
            if (_richTextBox.Selection.IsEmpty)
            {
                pos = _richTextBox.CaretPosition;
                parent = pos.Parent as Inline;
                while (parent != null && !(parent is Hyperlink))
                {
                    parent = parent.Parent as Inline;
                }
            }
            else
            {
                pos = _richTextBox.Selection.Start;
                parent = pos.GetAdjacentElement(LogicalDirection.Forward) as Inline;
            }
            if (parent is Hyperlink)
            {
                hyperlink = parent as Hyperlink;
                InlineCollection hyperlinkChildren = hyperlink.Inlines;
                Inline[] inlines = new Inline[hyperlinkChildren.Count];
                hyperlinkChildren.CopyTo(inlines, 0);

                //Remove each child from parent hyperlink element and insert it after the hyperlink.
                for (int i = inlines.Length - 1; i >= 0; i--)
                {
                    hyperlinkChildren.Remove(inlines[i]);
                    hyperlink.SiblingInlines.InsertAfter(hyperlink, inlines[i]);
                }
                //Apply hyperlink's local formatting properties to inlines (which are now outside hyperlink scope).
                LocalValueEnumerator localProperties = hyperlink.GetLocalValueEnumerator();
                TextRange inlineRange = new TextRange(inlines[0].ContentStart, inlines[inlines.Length - 1].ContentEnd);

                while (localProperties.MoveNext())
                {
                    LocalValueEntry property = localProperties.Current;
                    DependencyProperty dp = property.Property;
                    object value = property.Value;

                    if (!dp.ReadOnly &&
                        dp != Inline.TextDecorationsProperty && // Ignore hyperlink defaults.
                        dp != TextElement.ForegroundProperty &&
                        dp.Name != "ToolTip" &&
                        !isHyperlinkProperty(dp))
                        {
                            inlineRange.ApplyPropertyValue(dp, value);
                        }
                    }
                hyperlink.SiblingInlines.Remove(hyperlink);

            }

        }


        public void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start((sender as Hyperlink).NavigateUri.AbsoluteUri);
        }
        #endregion hyperlinks
        //navigate with CTRL + left/right key
        private void _main_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control && e.Key == Key.Right)
            {
                if (_iNote < _lNotes.Count - 1)
                {
                    _iNote++;
                    _bSetText = true;
                    _setText();
                    if (_bChanged)
                        saveXML();
                }
            }
            if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control && e.Key == Key.Left)
            {
                if (_iNote > 0)
                {
                    _iNote--;
                    _bSetText = true;
                    _setText();
                    if (_bChanged)
                        saveXML();
                }
            }
        }

        //Navigate through notes with mouse wheel --> Top-Grid and bottom Nav-Grid
        private void _topGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _wheelNav(e);        
        }

        private void navGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _wheelNav(e);        
        }
        void _wheelNav(MouseWheelEventArgs e)
        {
           // if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
                if(e.Delta > 0)
                    if (_iNote < _lNotes.Count - 1)
                    {
                        _iNote++;
                        _bSetText = true;
                        _setText();
                }
            if (e.Delta < 0)
                if (_iNote > 0)
                {
                    _iNote--;
                    _bSetText = true;
                    _setText();
                }
        }
    }


    public static class ColorExtensions
    {
        public static Color FromIntArgb(int argb)
        {
            byte[] bytes = BitConverter.GetBytes(argb);
            return Color.FromArgb(bytes[3],bytes[2],bytes[1],bytes[0]);
        }
        public static int ToIntArgb(Color color)
        {
            byte[] bytes = new byte[] { color.B, color.G, color.R, color.A };
            return BitConverter.ToInt32(bytes, 0);
        }
        public static Color setA(Color color, byte A)
        {
            byte[] bytes = new byte[] { color.B, color.G, color.R, A };
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }
        public static Color setR(Color color, byte R)
        {
            byte[] bytes = new byte[] { color.B, color.G, R, color.A };
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }
        public static Color setG(Color color, byte G)
        {
            byte[] bytes = new byte[] { color.B, G, color.R, color.A };
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }
        public static Color setB(Color color, byte B)
        {
            byte[] bytes = new byte[] { B, color.G, color.R, color.A };
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

    }
    public class Font
    {

        public Font()
        {
            _family = new FontFamily("Calibri");
            _weight = new FontWeight();
            _stretch = new FontStretch();
            _style = new FontStyle();
            _size = 14;
        }


        //public FontFamily family
        //{
        //    get {return _family;}
        //    set {_family = value;}
        //}
        public FontFamily _family { get;set;}
        public FontWeight _weight { get;set;}
        public FontStretch _stretch { get;set;}
        public FontStyle _style { get;set;}
        public Double _size { get;set;}
    }
    public class WpfScreen
    {
        public static IEnumerable<WpfScreen> AllScreens()
        {
            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                yield return new WpfScreen(screen);
            }
        }

        public static WpfScreen GetScreenFrom(Window window)
        {
            System.Windows.Interop.WindowInteropHelper windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(window);
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);
            WpfScreen wpfScreen = new WpfScreen(screen);
            return wpfScreen;
        }

        public static WpfScreen GetScreenFrom(Point point)
        {
            int x = (int)Math.Round(point.X);
            int y = (int)Math.Round(point.Y);

            // are x,y device-independent-pixels ??
            System.Drawing.Point drawingPoint = new System.Drawing.Point(x, y);
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint(drawingPoint);
            WpfScreen wpfScreen = new WpfScreen(screen);

            return wpfScreen;
        }

        public static WpfScreen Primary
        {
            get { return new WpfScreen(System.Windows.Forms.Screen.PrimaryScreen); }
        }

        private readonly System.Windows.Forms.Screen screen;

        internal WpfScreen(System.Windows.Forms.Screen screen)
        {
            this.screen = screen;
        }

        public Rect DeviceBounds
        {
            get { return this.GetRect(this.screen.Bounds); }
        }

        public Rect WorkingArea
        {
            get { return this.GetRect(this.screen.WorkingArea); }
        }

        private Rect GetRect(System.Drawing.Rectangle value)
        {
            // should x, y, width, height be device-independent-pixels ??
            return new Rect
            {
                X = value.X,
                Y = value.Y,
                Width = value.Width,
                Height = value.Height
            };
        }

        public bool IsPrimary
        {
            get { return this.screen.Primary; }
        }

        public string DeviceName
        {
            get { return this.screen.DeviceName; }
        }
    }
    public class FontsWrapper
    {
        static ICollection<FontWeight> fontWeights;
        static ICollection<FontStyle> fontStyles;
        static ICollection<FontFamily> fontFamilies;
        static ICollection<FontStretch> fontStretches;

        public static ICollection<FontStyle> GetFontStyles()
        {
            return fontStyles ?? (fontStyles = new List<FontStyle>() { System.Windows.FontStyles.Italic, System.Windows.FontStyles.Normal, System.Windows.FontStyles.Oblique });//TODO:Get by reflection
        }

        public static ICollection<FontFamily> GetFontFamilies()
        {
            return fontFamilies ?? (fontFamilies = Fonts.SystemFontFamilies);
        }

        public static ICollection<FontWeight> GetFontWeights()
        {
            if (fontWeights == null)
                fontWeights = new List<FontWeight>();
            else
                return fontWeights;

            var type = typeof(FontWeights);
            foreach (var p in type.GetProperties().Where(s => s.PropertyType == typeof(FontWeight)))
            {
                fontWeights.Add((FontWeight)p.GetValue(null, null));
            }
            return fontWeights;
        }

        public static ICollection<FontStretch> GetFontStretches()
        {
            if (fontStretches == null)
                fontStretches = new List<FontStretch>();
            else
                return fontStretches;

            var type = typeof(FontStretches);
            foreach (var p in type.GetProperties().Where(s => s.PropertyType == typeof(FontStretch)))
            {
                fontStretches.Add((FontStretch)p.GetValue(null, null));
            }
            return fontStretches;
        }

        public static ICollection<FontWeight> FontWeights
        {
            get { return fontWeights ?? (fontWeights = GetFontWeights()); }
        }
        public static ICollection<FontStyle> FontStyles
        {
            get { return fontStyles ?? (fontStyles = GetFontStyles()); }
        }

        public static ICollection<FontFamily> FontFamilies
        {
            get { return fontFamilies ?? (fontFamilies = GetFontFamilies()); }
        }
        public static ICollection<FontStretch> FontStretches
        {
            get { return fontStretches ?? (fontStretches = GetFontStretches()); }
        }

    }

    public class ResizingAdorner : Adorner
    {
        // Resizing adorner uses Thumbs for visual elements.   
        // The Thumbs have built-in mouse input handling. 
        Thumb topLeft, topRight, bottomLeft, bottomRight;

        // To store and manage the adorner's visual children. 
        VisualCollection visualChildren;

        // Initialize the ResizingAdorner. 
        public ResizingAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            // Call a helper method to initialize the Thumbs 
            // with a customized cursors. 
            BuildAdornerCorner(ref topLeft, Cursors.SizeNWSE);
            BuildAdornerCorner(ref topRight, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomLeft, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomRight, Cursors.SizeNWSE);

            // Add handlers for resizing. 
            bottomLeft.DragDelta += new DragDeltaEventHandler(HandleBottomLeft);
            bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
            topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeft);
            topRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);
        }

        // Handler for resizing from the bottom-right. 
        void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize. 
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger  
            // than the width or height of an adorner, respectively. 
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the bottom-left. 
        void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize. 
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger  
            // than the width or height of an adorner, respectively. 
            adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-right. 
        void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize. 
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger  
            // than the width or height of an adorner, respectively. 
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-left. 
        void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize. 
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger  
            // than the width or height of an adorner, respectively. 
            adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
        }

        // Arrange the Adorners. 
        protected override Size ArrangeOverride(Size finalSize)
        {
            // desiredWidth and desiredHeight are the width and height of the element that's being adorned.   
            // These will be used to place the ResizingAdorner at the corners of the adorned element.   
            double desiredWidth = AdornedElement.DesiredSize.Width;
            double desiredHeight = AdornedElement.DesiredSize.Height;
            // adornerWidth & adornerHeight are used for placement as well. 
            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;

            topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            topRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            bottomLeft.Arrange(new Rect(-adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            bottomRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));

            // Return the final size. 
            return finalSize;
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property,  
        // set some appearance properties, and add the elements to the visual tree. 
        void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics. 
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 10;
            cornerThumb.Opacity = 0.40;
            cornerThumb.Background = new SolidColorBrush(Colors.MediumBlue);

           visualChildren.Add(cornerThumb);
        }

        // This method ensures that the Widths and Heights are initialized.  Sizing to content produces 
        // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height 
        // need to be set first.  It also sets the maximum size of the adorned element. 
        void EnforceSize(FrameworkElement adornedElement)
        {
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = adornedElement.DesiredSize.Width;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = adornedElement.DesiredSize.Height;

            FrameworkElement parent = adornedElement.Parent as FrameworkElement;
            if (parent != null)
            {
                adornedElement.MaxHeight = parent.ActualHeight;
                adornedElement.MaxWidth = parent.ActualWidth;
            }
        }
        // Override the VisualChildrenCount and GetVisualChild properties to interface with  
        // the adorner's visual collection. 
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }
}
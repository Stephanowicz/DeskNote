using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeskNote
{
    /// <summary>
    /// Interaktionslogik für FontManager.xaml
    /// </summary>
    public partial class FontManager : UserControl
    {
        public bool isDropDownOpen
        {
            get { return fontList.IsDropDownOpen; }
        }
        public string selectedItem
        {
            get { return fontList.SelectedIndex > -1 ? fontList.SelectedValue.ToString() : null; }
        }
        public int selectedIndex
        {
            get { return fontList.SelectedIndex; }
            set { fontList.SelectedIndex = value; }
        }

        public int indexOf(FontFamily f)
        {
            return fontList.Items.IndexOf(f);
        }
        public FontManager()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler SelectionChanged;

        private void fontList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged.Invoke(sender, e);
        }


    }
    public class InstalledFonts : List<FontFamily>
    {
        static List<FontFamily> _fontFamilies = Fonts.SystemFontFamilies.ToList();
        //static List<FontFamily> _myInitialFonts = Fonts.GetFontFamilies(new Uri(@"pack://application:,,,/Resources/Fonts/Initials/"), "/Resources/Fonts/Initials/").ToList();
        //static List<FontFamily> _myCalligraphyFonts = Fonts.GetFontFamilies(new Uri(@"pack://application:,,,/Resources/Fonts/Calligraphy/"), "/Resources/Fonts/Calligraphy/").ToList();

        public InstalledFonts()
        {
            _fontFamilies = _fontFamilies.OrderBy(p => p.Source).ToList();
            //_fontFamilies.InsertRange(0, _myCalligraphyFonts);
            //_fontFamilies.InsertRange(0, _myInitialFonts);
            //AddRange(_myInitialFonts);
            //AddRange(_myCalligraphyFonts);
            this.AddRange(_fontFamilies);
        }

        public static List<FontFamily> fontFamilies
        {
            get { return _fontFamilies; }
            set { _fontFamilies = value; }
        }

    }

}

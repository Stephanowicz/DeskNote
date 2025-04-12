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
using System.Windows.Shapes;
using System.Xml;

namespace DeskNote
{
    /// <summary>
    /// Interaktionslogik für newNote.xaml
    /// </summary>
    public partial class newURL : Window
    {
        public bool result { get; set; }
        public string sURLName { get { return tbURLName.Text; } }

        public newURL()
        {
            result = false;
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!isHyperlink(tbURLName.Text))
                MessageBox.Show("This is not a valid address");
            else
            {
                result = true;
                Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            result = false;
            Close();
        }

        private void tbURLName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbURLName.Text.Length > 0)
                btnOK.IsEnabled = true;
            else
                btnOK.IsEnabled = false;
        }

        private void tbURLName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && tbURLName.Text.Length > 0)
                btnOK_Click(null, null);
            if (e.Key == Key.Escape)
                btnCancel_Click(null, null);

        }
        //parts of this code have been taken from http://blogs.msdn.com/b/prajakta/archive/2006/10/17/autp-detecting-hyperlinks-in-richtextbox-part-i.aspx 
        private static readonly System.Text.RegularExpressions.Regex UrlRegex = new System.Text.RegularExpressions.Regex(@"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&amp;(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");
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

    }
}

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
    public partial class newNote : Window
    {
        public bool result { get; set; }
        public string sNoteName { get { return tbNoteName.Text; } }

        public newNote()
        {
            result = false;
            InitializeComponent();
            System.Globalization.CultureInfo cInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            tbNoteName.Text = DateTime.Now.ToString("G", cInfo);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (noteExists(tbNoteName.Text))
                MessageBox.Show("This Note already exists!" + Environment.NewLine + "Choose another name");
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

        private void tbNoteName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbNoteName.Text.Length > 0)
                btnOK.IsEnabled = true;
            else
                btnOK.IsEnabled = false;
        }

        private void tbNoteName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && tbNoteName.Text.Length > 0)
                btnOK_Click(null, null);
            if (e.Key == Key.Escape)
                btnCancel_Click(null, null);

        }
        public bool noteExists(string sName)
        {
            string filePath = System.IO.Directory.GetCurrentDirectory() + "\\settings.xml";
            XmlNode node;
            XmlDocument doc = new XmlDocument();
            if (System.IO.File.Exists(filePath))
            {
                doc.Load("settings.xml");
                node = doc.SelectSingleNode("//DeskNote/Note[@name='" + sName + "']");
                if (node != null)
                {
                    return true;
                }
            }
            return false;
        }

    }
}

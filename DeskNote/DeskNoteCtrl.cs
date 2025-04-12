using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace DeskNote
{
    public partial class DeskNoteCtrl : Form
    {

        //As http://stackoverflow.com/questions/3751554/application-openforms-count-0-always 
        //says, <Application.OpenForms> is buggy in certain circumstances
        //So this is a workaround with managing the open forms in lists
        List<string> _openWindowsNames = new List<string>();
        List<System.Windows.Window> _openWindows = new List<System.Windows.Window>();

       public DeskNoteCtrl()
        {
            InitializeComponent();
            loadSettings();
        }

        #region toolstrip
        /// <summary>
        /// addNoteToolStripMenuItems: 
        /// add a note item to toolstrip when a new note is created
        /// </summary>
        void addNoteToolStripMenuItems(string sName)
        {
            if (cmStrip.Items.Find(sName, false).Length == 0)
            {
                int index = cmStrip.Items.IndexOf(tsSeparator2);
                ToolStripMenuItem tsmi = new ToolStripMenuItem(sName);
                tsmi.Name = sName;
                tsmi.DropDownItems.AddRange(tsCreateSubMenuItems());
                tsmi.DoubleClick += tsmi_DoubleClick;
                tsmi.DoubleClickEnabled = true;
                cmStrip.Items.Insert(index, tsmi);
                setState_ToolStripItems(tsmi, sName);
            }
        }

        void tsmi_DoubleClick(object sender, EventArgs e)
        {
            DeskNoteWindow win;
            String sName = ((ToolStripMenuItem)(sender)).Name;
            if (_openWindowsNames.IndexOf(sName) > -1)
            {
                win = (DeskNoteWindow)_openWindows[_openWindowsNames.IndexOf(sName)];
                win._flashWindow();
            }
        }
        /// <summary>
        /// tsCreateSubMenuItems: 
        /// create items for the notes toolstrip submenue
        /// </summary>
        ToolStripItem[] tsCreateSubMenuItems()
        {
            ToolStripMenuItem tsmiOpen, tsmiClose, tsmiDelete, tsmiShow, tsmiHide, tsmiFront, tsmiBack, tsmiSaveSettings, tsmiSettings, tsmiBackgroundPic;
            ToolStripSeparator tsSep1, tsSep2, tsSep3;
            ToolStripItem[] tSub;

            tsmiOpen = new ToolStripMenuItem("Open", null, loadNoteToolStripMenuItem_Click, "Open");
            tsmiClose = new ToolStripMenuItem("Close", null, closeNoteToolStripMenuItem_Click, "Close");
            tsmiDelete = new ToolStripMenuItem("Delete", null, deleteNoteToolStripMenuItem_Click, "Delete");
            tsmiShow = new ToolStripMenuItem("Show", null, showNoteToolStripMenuItem_Click, "Show");
            tsmiHide = new ToolStripMenuItem("Hide", null, hideNoteToolStripMenuItem_Click, "Hide");
            tsmiFront = new ToolStripMenuItem("Bring to front", null, frontNoteToolStripMenuItem_Click, "Bring to front");
            tsmiBack = new ToolStripMenuItem("Send to back", null, backNoteToolStripMenuItem_Click, "Send to back");
            tsmiSettings = new ToolStripMenuItem("Settings", null, settingsNoteToolStripMenuItem_Click, "Settings");
            tsmiSaveSettings = new ToolStripMenuItem("Save settings", null, saveSettingsNoteToolStripMenuItem_Click, "Save settings");
            tsmiBackgroundPic = new ToolStripMenuItem("Background picture", null, backgroundPicNoteToolStripMenuItem_Click, "Background picture");

            tsSep1 = new ToolStripSeparator();
            tsSep2 = new ToolStripSeparator();
            tsSep3 = new ToolStripSeparator();
            tSub = new ToolStripItem[] { 
                tsmiOpen,       
                tsmiClose,      
                tsmiDelete,     
                tsSep1,         
                tsmiFront,     
                tsmiBack,     
                tsmiShow,      
                tsmiHide,       
                tsSep2,
            //    tsmiBackgroundPic,
                tsmiSaveSettings,   
                tsmiSettings   
            };

            return tSub;
        }
        /// <summary>
        /// removeToolStripItems: 
        /// remove note items from toolstrip when notes where deleted
        /// </summary>
        void removeToolStripItems(string sName)
        {
            ToolStripItem[] tsi;
            tsi = cmStrip.Items.Find(sName, true);
            if (tsi.Length != 0)
                cmStrip.Items.Remove(tsi[0]);
        }
        /// <summary>
        /// setState_ToolStripItems: 
        /// set the enabled / diabled state of submenue items when note is open or close
        /// </summary>
        void setState_ToolStripItems(ToolStripMenuItem tsmi, string sName)
        {
            ToolStripItem tSub = new ToolStripMenuItem();
            if (findOpenNotes(sName))
            {
                tsmi.Checked = true;
                ((ToolStripMenuItem)(tsmi.DropDownItems["Open"])).Enabled = false;
                ((ToolStripMenuItem)(tsmi.DropDownItems["Close"])).Enabled = true;
                for(int i = 3; i < tsmi.DropDownItems.Count;i++)
                {
                    tsmi.DropDownItems[i].Visible = true;
                    checkState_NoteSettings(sName, tsmi.DropDownItems[i].Name, tsmi);
                }
                DeskNoteWindow win = getWin(sName);
                tsmi.ToolTipText = win.notifyIconText;
            }
            else
            {
                tsmi.Checked = false;
                ((ToolStripMenuItem)(tsmi.DropDownItems["Open"])).Enabled = true;
                ((ToolStripMenuItem)(tsmi.DropDownItems["Close"])).Enabled = false;
                for(int i = 3; i < tsmi.DropDownItems.Count;i++)
                {
                    tsmi.DropDownItems[i].Visible = false;
                }
            }

        }
        void setDoubleState_ToolStripItems(ToolStripMenuItem tsmi, int iIndex, bool bState)
        {
            ((ToolStripMenuItem)(tsmi.DropDownItems[iIndex])).Enabled = bState;
            ((ToolStripMenuItem)(tsmi.DropDownItems[iIndex + 1])).Enabled = !bState;
            ((ToolStripMenuItem)(tsmi.DropDownItems[iIndex])).Checked = !bState;
            ((ToolStripMenuItem)(tsmi.DropDownItems[iIndex + 1])).Checked = bState;
        }
        void checkState_NoteSettings(string sNote,string sState,ToolStripMenuItem tsmi)
        {
            DeskNoteWindow win = getWin(sNote);
            bool bState = false;
            if (win != null)
            {
                switch (sState)
                {
                    case "Show":
                        bState = win.Visibility == System.Windows.Visibility.Visible ? true : false;
                        ((ToolStripMenuItem)tsmi.DropDownItems[sState]).Checked = bState;
                        ((ToolStripMenuItem)tsmi.DropDownItems[sState]).Enabled = !bState;
                        break;
                    case "Hide":
                        bState = win.Visibility == System.Windows.Visibility.Visible ? true : false;
                        ((ToolStripMenuItem)tsmi.DropDownItems[sState]).Checked = !bState;
                        ((ToolStripMenuItem)tsmi.DropDownItems[sState]).Enabled = bState;
                        break;
                    case "Bring to front":
                        bState = win._isBack;
                        ((ToolStripMenuItem)tsmi.DropDownItems[sState]).Checked = !bState;
                        ((ToolStripMenuItem)tsmi.DropDownItems[sState]).Enabled = bState;
                        break;
                    case "Send to back":
                        bState = win._isBack;
                        ((ToolStripMenuItem)tsmi.DropDownItems[sState]).Checked = bState;
                        ((ToolStripMenuItem)tsmi.DropDownItems[sState]).Enabled = !bState;
                        break;
                    case "Background picture":
                        bState = win._BckgrPic;
                        ((ToolStripMenuItem)tsmi.DropDownItems[sState]).Checked = bState;
                        break;
                    default:
                        break;
                }
            }
        }
        int getIndex_ToolStripItems(ToolStripMenuItem tsmi)
        {           
            string tsmiName = tsmi.Text;
            ToolStrip ts = (ToolStrip)(tsmi.Owner);
            int iIndex = ts.Items.IndexOf(tsmi);
            return iIndex;
        }
        /// <summary>
        /// getNoteItem: 
        /// return the toolstrip item for the note
        /// </summary>
        public ToolStripMenuItem getNoteItem(string sName)
        {
            if (cmStrip.Items.Find(sName, false).Length == 1)
                return (ToolStripMenuItem)cmStrip.Items.Find(sName, false)[0];
            else
                return null;
        }

        private void addNewNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newNote nNote = new newNote();
                nNote.ShowDialog();
                if (nNote.result)
                {
                    loadNote(nNote.sNoteName);
                    addNoteToolStripMenuItems(nNote.sNoteName);
                }
        }
        private void loadNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sName = ((ToolStripMenuItem)sender).OwnerItem.Text;
            loadNote(sName);
            setState_ToolStripItems(((ToolStripMenuItem)((ToolStripMenuItem)sender).OwnerItem), sName);
        }
        private void closeNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sName = ((ToolStripMenuItem)sender).OwnerItem.Text;
            closeNote(sName);
            setState_ToolStripItems(((ToolStripMenuItem)((ToolStripMenuItem)sender).OwnerItem), sName);
        }
        private void showNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sNote = ((ToolStripMenuItem)sender).OwnerItem.Text;
            showNote(sNote);
            int iIndex = getIndex_ToolStripItems((ToolStripMenuItem)sender);
            setDoubleState_ToolStripItems(((ToolStripMenuItem)((ToolStripMenuItem)sender).OwnerItem), iIndex, false);
        }
        private void hideNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sName = ((ToolStripMenuItem)sender).OwnerItem.Text;
            hideNote(sName);
            int iIndex = getIndex_ToolStripItems((ToolStripMenuItem)sender);
            setDoubleState_ToolStripItems(((ToolStripMenuItem)((ToolStripMenuItem)sender).OwnerItem), iIndex-1, true);
        }
        private void frontNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sName = ((ToolStripMenuItem)sender).OwnerItem.Text;
            NoteBringToFront(sName);
            int iIndex = getIndex_ToolStripItems((ToolStripMenuItem)sender);
            setDoubleState_ToolStripItems(((ToolStripMenuItem)((ToolStripMenuItem)sender).OwnerItem), iIndex, false);
        }
        private void backNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sName = ((ToolStripMenuItem)sender).OwnerItem.Text;
            NoteSendToBack(sName);
            int iIndex = getIndex_ToolStripItems((ToolStripMenuItem)sender);
            setDoubleState_ToolStripItems(((ToolStripMenuItem)((ToolStripMenuItem)sender).OwnerItem), iIndex-1, true);
        }
        private void settingsNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sName = ((ToolStripMenuItem)sender).OwnerItem.Text;
            NoteSettings(sName);
        }
        private void saveSettingsNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sName = ((ToolStripMenuItem)sender).OwnerItem.Text;
            NoteSaveSettings(sName);
        }
        private void backgroundPicNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sName = ((ToolStripMenuItem)sender).OwnerItem.Text;
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;

            bool bSet = false;
            if (!tsmi.Checked)
                bSet = true;
            NoteBackgroundPic(sName,bSet);
            tsmi.Checked = bSet;
        }
        private void deleteNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sName = ((ToolStripMenuItem)sender).OwnerItem.Text;
            deleteNote(sName);
            removeToolStripItems(sName);
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _exit();
        }
        #endregion toolstrip


        DeskNoteWindow getWin(string sName)
        {
            DeskNoteWindow win;
            if (_openWindowsNames.IndexOf(sName) > -1)
            {
                win = (DeskNoteWindow)_openWindows[_openWindowsNames.IndexOf(sName)];
                return win;
            }
            return null;
        }
        bool _bExit = false;
        private void _exit()
        {
            if (!_bExit)
            {
                _bExit = true;
                saveSettings();
                System.Windows.Window win;

                while (_openWindows.Count > 0)
                {
                    win = _openWindows[0];
                    closeNote(((DeskNoteWindow)win)._noteName);
                    System.Threading.Thread.Sleep(100);
                    win.Close();
                    System.Threading.Thread.Sleep(100);
                }
                Dispose(true);
                Close();
            }
        }
        /// <summary>
        /// NoteExists: search for note in toolstrip menue
        /// </summary>
        public bool tsNoteExists(string sName)
        {
            if (cmStrip.Items.Find(sName, false).Length == 1)
                return true;
            else
                return false;
        }
        /// <summary>
        /// findOpenNotes: 
        /// check if note is open
        /// </summary>
        public bool findOpenNotes(string sName)
        {
            if (_openWindowsNames.IndexOf(sName) > -1)
                return true;
            
            return false;
        }

        /// <summary>
        /// loadNote: 
        /// open a note with the given name
        /// if exist, saved settings will be loaded
        /// </summary>
        void loadNote(string sName)
        {
            DeskNoteWindow _wDeskNoteWindow = new DeskNoteWindow(sName);
            _openWindows.Add(_wDeskNoteWindow);
            _openWindowsNames.Add(sName);
            _wDeskNoteWindow.Closed += DeskNote_WindowClosed;
            _wDeskNoteWindow.noteChanged += DeskNote_NoteChanged;
            _wDeskNoteWindow.noteRenamed += DeskNote_NoteRenamed;
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(_wDeskNoteWindow);
            _wDeskNoteWindow.Show();
        }

        /// <summary>
        /// DeskNote_NoteChanged:
        /// routed event from open note 
        /// a different note was loaded by the contextmenue of the former note
        /// the local lists and toolstrip menues get updated
        /// </summary>
        void DeskNote_NoteChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            string[] arr = ((IEnumerable<object>)sender).Cast<object>()
                                 .Select(x => x.ToString())
                                 .ToArray();

            ToolStripMenuItem tsmi = getNoteItem(arr[0]);
            if (tsmi != null)
            {
               // DeskNoteWindow win = getWin(arr[0]);
               // bool bState = false;
               // if (win != null)
                {
                    switch (arr[1])
                    {
                        case "Show":
                            ((ToolStripMenuItem)tsmi.DropDownItems["Show"]).Checked = true;
                            ((ToolStripMenuItem)tsmi.DropDownItems["Show"]).Enabled = false;
                            ((ToolStripMenuItem)tsmi.DropDownItems["Hide"]).Checked = false;
                            ((ToolStripMenuItem)tsmi.DropDownItems["Hide"]).Enabled = true;
                            break;
                        case "Hide":
                            ((ToolStripMenuItem)tsmi.DropDownItems["Hide"]).Checked = true;
                            ((ToolStripMenuItem)tsmi.DropDownItems["Hide"]).Enabled = false;
                            ((ToolStripMenuItem)tsmi.DropDownItems["Show"]).Checked = false;
                            ((ToolStripMenuItem)tsmi.DropDownItems["Show"]).Enabled = true;
                            break;
                        case "Bring to front":
                            ((ToolStripMenuItem)tsmi.DropDownItems["Bring to front"]).Checked = true;
                            ((ToolStripMenuItem)tsmi.DropDownItems["Bring to front"]).Enabled = false;
                            ((ToolStripMenuItem)tsmi.DropDownItems["Send to back"]).Checked = false;
                            ((ToolStripMenuItem)tsmi.DropDownItems["Send to back"]).Enabled = true;
                            break;
                        case "Send to back":
                            ((ToolStripMenuItem)tsmi.DropDownItems["Send to back"]).Checked = true;
                            ((ToolStripMenuItem)tsmi.DropDownItems["Send to back"]).Enabled = false;
                            ((ToolStripMenuItem)tsmi.DropDownItems["Bring to front"]).Checked = false;
                            ((ToolStripMenuItem)tsmi.DropDownItems["Bring to front"]).Enabled = true;
                            break;
                        //case "Background picture":
                        //    bState = win._BckgrPic;
                        //    ((ToolStripMenuItem)tsmi.DropDownItems["Background picture"]).Checked = bState;
                        //    break;
                        default:
                            break;
                    }
                }

            }
                setState_ToolStripItems(tsmi, arr[0]);
            tsmi = getNoteItem(arr[1]);
            if (tsmi != null)
                setState_ToolStripItems(tsmi, arr[1]);
        }
        /// <summary>
        /// DeskNote_NoteRenameded:
        /// routed event from open note 
        /// the note was renamed
        /// the local lists and toolstrip menues get updated
        /// </summary>
        void DeskNote_NoteRenamed(object sender, System.Windows.RoutedEventArgs e)
        {
            string[] arr = ((IEnumerable<object>)sender).Cast<object>()
                                 .Select(x => x.ToString())
                                 .ToArray();

            _openWindowsNames[_openWindowsNames.IndexOf(arr[1])] = arr[0];
            ToolStripMenuItem tms = getNoteItem(arr[1]);
            if (tms != null)
                tms.Name = tms.Text = arr[0];
            //foreach (System.Windows.Window win in _openWindows)
            //{
            //    if (((DeskNoteWindow)(win))._noteName != arr[0])
            //    {
            //        ((DeskNoteWindow)(win)).cmLoadNotes_Init();
            //        ((DeskNoteWindow)(win)).notifyIconLoadNotes_Init();
            //    }
            //}
        }

        void showNote(string sName)
        {
            DeskNoteWindow win = getWin(sName);
            if (win != null)
                win._show(false);            
        }
        void hideNote(string sName)
        {
            DeskNoteWindow win = getWin(sName);
            if (win != null)
                win._hide(false);          
        }
        void NoteBringToFront(string sName)
        {
            DeskNoteWindow win = getWin(sName);
            if (win != null)
                win._sendToBack(false,false);
        }
        void NoteSendToBack(string sName)
        {
            DeskNoteWindow win = getWin(sName);
            if (win != null)
                win._sendToBack(true,false);
        }
        void NoteSettings(string sName)
        {
            DeskNoteWindow win = getWin(sName);
            if (win != null)
                win._settings();            
        }
        void NoteSaveSettings(string sName)
        {
            DeskNoteWindow win = getWin(sName);
            if (win != null)
                win.saveXML();
        }
        void NoteBackgroundPic(string sName,bool bSet)
        {
            DeskNoteWindow win = getWin(sName);
            if (win != null)
                win._BckgrPic = bSet;
        }
        /// <summary>
        /// closeNote: 
        /// close the note
        /// settings will be saved
        /// </summary>
        void closeNote(string sName)
        {
            DeskNoteWindow win = getWin(sName);
            if (win != null)
                win._exit();
        }

        /// <summary>
        /// deleteNote: 
        /// close the note - if open
        /// settings in xml file will be deleted
        /// contextmenues of open notes are updated
        /// </summary>
        void deleteNote(string sName)
        {
            closeNote(sName);
            deleteXMLNote(sName);

            //update contextmenues of open notes
            //foreach (System.Windows.Window win in _openWindows)
            //{
            //    ((DeskNoteWindow)(win)).cmLoadNotes_Init();
            //    ((DeskNoteWindow)(win)).notifyIconLoadNotes_Init();
            //}
        }

        /// <summary>
        /// DeskNote_WindowClosed: 
        /// update the openWindows list
        /// update toolstrip items
        /// </summary>
        void DeskNote_WindowClosed(object sender, EventArgs e)
        {
            string sName = ((DeskNoteWindow)(sender))._noteName;
            _openWindowsNames.Remove(sName);
            _openWindows[_openWindows.FindIndex(item => item == (DeskNoteWindow)(sender))]= null;
            _openWindows.Remove(null);

            ToolStripMenuItem tms = getNoteItem(sName);
            if (tms != null)
                setState_ToolStripItems(tms, sName);

            ((DeskNoteWindow)(sender)).Closed -= DeskNote_WindowClosed;
            ((DeskNoteWindow)(sender)).noteChanged -= DeskNote_NoteChanged;
            ((DeskNoteWindow)(sender)).noteRenamed -= DeskNote_NoteRenamed;
            sender = null;
           // if(!_bExit)
                //update contextmenues of open notes
                //foreach (System.Windows.Window win in _openWindows)
                //{
                //    ((DeskNoteWindow)(win)).cmLoadNotes_Init();
                //    ((DeskNoteWindow)(win)).notifyIconLoadNotes_Init();
                //}

        }


        /// <summary>
        /// notifyIcon1_DoubleClick: 
        /// open a new note when double clicking the icon in the tray
        /// </summary>
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            addNewNoteToolStripMenuItem_Click(null, null);
        }

        /// <summary>
        /// OnShown: 
        /// override the system function
        /// in order to hide the programm from the tasklist
        /// it has to be done here or later
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Visible = false;
        }

        #region load/save Settings
        
        string filePath = System.IO.Directory.GetCurrentDirectory() + "\\settings.xml";
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
        void loadSettings()
        {
            XmlNodeList list;
            XmlNode node;
            XmlDocument doc = new XmlDocument();
            string sVal;
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
            node = doc.SelectSingleNode("//DeskNote/OpenNotes");
            if (node == null)
            {
                node = doc.SelectSingleNode("//DeskNote");
                node = node.AppendChild(doc.CreateElement("OpenNotes"));
                node.InnerText = "";
            }
            else
            {
                list = node.SelectNodes("OpenNote");
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        sVal = list[i].InnerText;
                        if (sVal.Length > 0)
                            loadNote(sVal);
                    }
                }

            }
            string[] sSections = noteList();
            if (sSections != null)
            {
                for (int i = 0; i < sSections.Length; i++)
                {
                    if (sSections[i] != "")
                    {
                        addNoteToolStripMenuItems(sSections[i]);
                    }
                }
            }
            doc.Save("settings.xml");
        }
        void deleteXMLNote(string sName)
        {
            if (System.IO.File.Exists(filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("settings.xml");

                XmlNode node;
                node = doc.SelectSingleNode("//DeskNote/Note[@name='" + sName + "']");
                if (node != null)
                {
                    node.ParentNode.RemoveChild(node);
                    doc.Save("settings.xml");
                }
            }
        }
        void saveSettings()
        {
            if (System.IO.File.Exists(filePath))
            {
                XmlNode node, subnode;
                XmlNodeList list;
                XmlDocument doc = new XmlDocument();
                doc.Load("settings.xml");
                node = doc.SelectSingleNode("//DeskNote/OpenNotes");
                list = node.SelectNodes("OpenNote");
                if(list != null && list.Count > 0)
                    foreach (XmlNode xmlnode in list)
                        xmlnode.ParentNode.RemoveChild(xmlnode);
                for (int i = 0; i < _openWindowsNames.Count; i++)
                {
                    subnode = node.AppendChild(doc.CreateElement("OpenNote"));
                    subnode.InnerText = _openWindowsNames[i];                   
                }
                doc.Save("settings.xml");
            }
        }

        #endregion

        private void DeskNoteCtrl_FormClosed(object sender, FormClosedEventArgs e)
        {
            _exit();
        }
    }
}

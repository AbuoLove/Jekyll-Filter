﻿using RunBatch;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilterWF
{
    public partial class MDIParent1 : Form
    {
        
        private int childFormNumber = 0;

        public static MDIParent1  mdiparent =null;

        public MDIParent1()
        {
            InitializeComponent();
            mdiparent = this;
        }

        //New
        private void ShowNewForm(object sender, EventArgs e)
        {
            var form1 = LoadForm1();
            form1.tbUrl_Text = "";
            form1.tbHtmlTitle_Text = "";
            form1.Show();
            form1.WindowState = FormWindowState.Maximized;
        }

        
        //New
        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Select Markdown file to filter";
            fdlg.InitialDirectory = Program.BlogSiteRoot;
            if (!Directory.Exists(Program.BlogSiteRoot))
                fdlg.InitialDirectory = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments),        "");
            Form1 form1;
          
            
            // fdlg.InitialDirectory = @"c:\";
            fdlg.Filter = "Markdown (*.md)|*.md|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                form1 = LoadForm1();
                form1.Text = fdlg.FileName;
                form1.srcFilePath = fdlg.FileName;
                form1.tbSrcFilename_Text = Path.GetFileName(form1.srcFilePath);
                form1.tbSrcFolder_Text = Path.GetFullPath(form1.srcFilePath).Replace(form1.tbSrcFilename_Text, "");

                form1.tbUrl_Text = "";
                form1.LoadFile(false);
                form1.Show();
            }
        }

        //No change
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form1;
            if (this.ActiveMdiChild is Form1)
            {
                form1 = (Form1)this.ActiveMdiChild;
                if (form1 != null)
                {
                    form1.SaveAs_Click(sender, e);
                }
            }
            
        }

        internal static void Form3Close(DialogResult dialogResult)
        {
            mdiparent.form3Close(dialogResult);
        }

        private Form3 form3=null;
        private void form3Close(DialogResult dialogResult)
        {
            if (form3 != null)
            {
                {
                    Form3 testDialog = form3;

                    if (dialogResult == DialogResult.OK)
                    {
                        // Read the raw contents of testDialog's TextBox.
                        string newTopic = testDialog.Topic;
                        string newCategoryName = testDialog.Cat;
                        string newTags = testDialog.Tags;
                        string newSubTopics = testDialog.SubTopics;
                        DateTime newDate = testDialog.Date;

                        string layout = testDialog.layout.ToLower();

                        string excerpt_separator = testDialog.Except_Eeperator;
                        
                        bool isDisqus = testDialog.IsDisqus;
                        bool topicAndSubTopics = testDialog.TopicAndSubTopics;
                        bool headingsAndBlurbs = testDialog.HeadingsAndBlurbs;
                        Program.tbSeperator_Text = testDialog.Seperator;

                        //Process it
                        string topic = newTopic.Trim();
                        string CategoryName = newCategoryName;
                        string catz = "";
                        var abbrev = from n in Program.Categorys where n.Name == CategoryName select n;
                        if (abbrev.Count() == 1)
                            catz = abbrev.First().Abbrev;
                        string[] tags = newTags.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries); //This would have trimmed
                        string[] subTopics;
                        if (!topicAndSubTopics)
                        {
                            topic = "";
                            subTopics = newSubTopics.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        }
                        else
                        {
                            subTopics = newSubTopics.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        }

                        for (int i = 0; i < subTopics.Length; i++)
                            subTopics[i] = subTopics[i].Trim();


                        DateTime date = newDate;
                        string blurb = "The quick brown fox jumps over the lazy dog";
                        string subTopicOrHeadingBlurbSep = Program.tbSeperator_Text;

                        string postslPath = Path.Combine(Program.BlogSiteRoot, "_posts");
                        for (int i = 0; i < subTopics.Length; i++)
                        {
                            string subtopic = subTopics[i];
                            if (headingsAndBlurbs)
                            {
                                //A fix so app doesn't crash (I typed @@ instaed of @@@ once)
                                if (!subtopic.Contains(subTopicOrHeadingBlurbSep))
                                    subtopic += subTopicOrHeadingBlurbSep;
                                string[] headnblurb = subtopic.Split(new string[] { subTopicOrHeadingBlurbSep }, StringSplitOptions.RemoveEmptyEntries);
                                blurb = subtopic.Substring(headnblurb[0].Length + subTopicOrHeadingBlurbSep.Length);
                                subtopic = headnblurb[0];
                                blurb = blurb.Trim();
                                subtopic = subtopic.Trim();
                            }


                            string filename = date.ToString("yyyy-MM-dd") + "-" + CategoryName + "-" + topic + "-" + subtopic + ".md";
                            date = date.AddDays(1);
                            filename = filename.Replace(" ", "-");
                            string path = Path.Combine(postslPath, filename);
                            string header = "";
                            header = "---\r\n";

                            if (topic != "")
                            {
                                header += "title: " + topic + "\r\n";
                                if (subtopic != "")
                                    header += "subtitle: " + subtopic + "\r\n";
                            }
                            else
                            {
                                header += "title: " + subtopic + "\r\n";
                            }
                            header += "layout: " + layout + "\r\n";
                            if (newTags != "")
                                header += "tags: " + newTags + "\r\n";
                            //if (CategoriesComboBox.SelectedIndex != -1)
                            header += "category: " + catz + "\r\n";
                            if (isDisqus)
                                header += "disqus: " + "1" + "\r\n";
                            else
                                header += "disqus: " + "0" + "\r\n";
                            header += "date: " + date + "\r\n";
                            if (excerpt_separator != "")
                                header += "excerpt_separator: " + excerpt_separator + "\r\n";
                            header += "---\r\n\r\n";
                            header += blurb + "\r\n";
                            if (excerpt_separator != "")
                                header += excerpt_separator + "\r\n";
                            using (var writer = new StreamWriter(path))
                            {
                                writer.Write(header);
                            }
                        }

                    }
                    else
                    {

                    }
                }
                form3.Close();
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            if (childFormNumber != 0)
            {
                if (MessageBox.Show("Do you wish to close all files and lose any unsaved data?", "Close", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    foreach (Form childForm in MdiChildren)
                    {
                        childForm.Close();
                    }
                    this.Close();
                }
            }
            else
                this.Close(); 
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form1;
            if (this.ActiveMdiChild is Form)
            {
                form1 = (Form)this.ActiveMdiChild;
                if (form1 != null)
                {
                    if (form1.ActiveControl is TextBox)
                    {
                        TextBox tb = (TextBox)form1.ActiveControl;
                        if (tb != null)
                        {
                            // Ensure that text is currently selected in the text box.   
                            if (tb.SelectedText != "")
                                // Cut the selected text in the control and paste it into the Clipboard.
                                tb.Cut();
                        }
                    }
                }
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form1;
            if (this.ActiveMdiChild is Form)
            {
                form1 = (Form)this.ActiveMdiChild;
                if (form1 != null)
                {
                    if (form1.ActiveControl is TextBox)
                    {
                        TextBox tb = (TextBox)form1.ActiveControl;
                        if (tb != null)
                        {
                            // Ensure that text is selected in the text box.   
                            if (tb.SelectionLength > 0)
                                // Copy the selected text to the Clipboard.
                                tb.Copy();
                        }
                    }
                }
            }
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form1;
            if (this.ActiveMdiChild is Form)
            {
                form1 = (Form)this.ActiveMdiChild;
                if (form1 != null)
                {
                    if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
                    {
                        if (form1.ActiveControl is TextBox)
                        {
                            TextBox tb = (TextBox)form1.ActiveControl;
                            if (tb != null)
                            {
                                // Determine if any text is selected in the text box.
                                if (tb.SelectionLength > 0)
                                {
                                    // Ask user if they want to paste over currently selected text.
                                    if (MessageBox.Show("Do you want to paste over current selection?", "Cut Example", MessageBoxButtons.YesNo) == DialogResult.No)
                                        // Move selection to the point after the current selection and paste.
                                        tb.SelectionStart = tb.SelectionStart + tb.SelectionLength;
                                }
                                // Paste current text in Clipboard into text box.
                                tb.Paste();
                            }
                        }
                    };
                }
            }
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (childFormNumber != 0)
            {
                if (MessageBox.Show("Do you wish to close all files and lose any unsaved data?", "Close", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    foreach (Form childForm in MdiChildren)
                    {
                        childForm.Close();
                    }
                }
            }
        }

        //Existing
        private void reloadStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you wish to undo all unsaved changes to this file?", "Reload file", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Form1 form1;
                if (this.ActiveMdiChild is Form1)
                {
                    form1 = (Form1)this.ActiveMdiChild;
                    if (form1 != null)
                    {
                        form1.srcFilePath = Path.Combine(form1.tbSrcFolder_Text, form1.tbSrcFilename_Text);
                        form1.LoadFile(false);
                        form1.tbUrl_Text = "";
                        form1.tbHtmlTitle_Text = "";
                    }
                }
            }
        }

        //New New
        private Form1 LoadForm1()
        {
            //if (form1 == null)
            //{
            Form1 newForm1 = new Form1();
            newForm1.MdiParent = this;
            newForm1.Text = "New MD File " + childFormNumber++;
            newForm1.WindowState = FormWindowState.Maximized;
            return newForm1;
            //}
        }

        //New
        private void Wrd2MD_Click(object sender, EventArgs e)
        {
           

                var fileContent = string.Empty;
            var filePath = string.Empty;
            string file = "";
            Form1 form1;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Word files (*.docx)|*.docx|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    

                    string targetPath = Path.Combine(Program.BlogSiteRoot, "_drafts");
                    if (!Directory.Exists(targetPath))
                        Directory.CreateDirectory(targetPath);

                    var srcPath = openFileDialog.FileName;
                    string srcfilename = Path.GetFileName(srcPath);
                    string srcfilebase = Path.GetFileNameWithoutExtension(srcfilename);
                    string srcfilebasedotted = srcfilebase.Replace(" ", "-");
                    targetPath = Path.Combine(targetPath, srcfilebasedotted + ".md");
                    PandocUtil.MD2Html(Program.BlogSiteRoot, Program.WorkingDirectory, srcPath, targetPath, srcfilebase);

                    //Cleanup image urls
                    string txt = "";
                    using (StreamReader sr = File.OpenText(targetPath))
                        txt = sr.ReadToEnd();
                    string crud = Program.BlogSiteRoot + "/media/";
                    txt = txt.Replace(crud, "/media/");
                    File.WriteAllText(targetPath, txt);

                    form1 = LoadForm1();
                    form1.Text = openFileDialog.FileName;
                    form1.srcFilePath = targetPath;
                    form1.tbSrcFilename_Text = srcfilebasedotted + ".md";
                    form1.tbSrcFolder_Text = Path.Combine(Program.BlogSiteRoot, "_draft");

                    form1.tbUrl_Text = "";
                    form1.tbHtmlTitle_Text = "";


                    form1.LoadFile(true);
                    form1.Show();


                    form1.chkJustrDoneConversion_Checked = true;
                    form1.Text = targetPath + " " + childFormNumber++;
                }

            }
        }

        //New
        private void Html2MD_Click(object sender, EventArgs e)
        {


            var fileContent = string.Empty;
            var filePath = string.Empty;
            string file = "";
            Form1 form1;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Word files (*.html)|*.html|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {


                    string targetPath = Path.Combine(Program.BlogSiteRoot, "_drafts");
                    if (!Directory.Exists(targetPath))
                        Directory.CreateDirectory(targetPath);

                    var srcPath = openFileDialog.FileName;
                    string srcfilename = Path.GetFileName(srcPath);
                    string srcfilebase = Path.GetFileNameWithoutExtension(srcfilename);
                    string srcfilebasedotted = srcfilebase.Replace(" ", "-");
                    targetPath = Path.Combine(targetPath, srcfilebasedotted + ".md");


                    string mediaFolder = Path.Combine(Program.BlogSiteRoot, "media");
                    PandocUtil.MD2Html(mediaFolder, Program.WorkingDirectory, srcPath, targetPath, srcfilebase);

                    string txt = "";
                    using (StreamReader sr = File.OpenText(targetPath))
                        txt = sr.ReadToEnd();
                    string crud = Program.BlogSiteRoot + "\\media/";
                    txt = txt.Replace(crud, "/media/");
                    File.WriteAllText(targetPath, txt);


                    form1 = LoadForm1();
                    form1.Text = openFileDialog.FileName;
                    form1.srcFilePath = targetPath;
                    form1.tbSrcFilename_Text = srcfilebasedotted + ".md";
                    form1.tbSrcFolder_Text = Path.Combine(Program.BlogSiteRoot, "_draft");

                    form1.tbUrl_Text = "";
                    form1.tbHtmlTitle_Text = "";

                    form1.LoadFile(true);
                    form1.Show();



                    form1.chkJustrDoneConversion_Checked = true;
                    form1.Text = targetPath + " " + childFormNumber++;
                }

            }


        }



        private void Http2MD_Click(object sender, EventArgs e)
        {
            frmGetUrl testDialog = new frmGetUrl();
            testDialog.Url = "";

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (testDialog.ShowDialog(this) == DialogResult.OK)
            {
                string url = testDialog.Url;
                string title = testDialog.Title;


                bool isUrl = (Uri.IsWellFormedUriString(url, UriKind.Absolute));
                if (!isUrl)
                {
                    MessageBox.Show("Download Url to MD", "Invalid Url", MessageBoxButtons.OK);
                    return;
                }

                if (title == "")
                {
                    MessageBox.Show("Download Url to MD", "Need a page title ", MessageBoxButtons.OK);
                    return;
                }

                string filefolder = Path.Combine(Program.BlogSiteRoot, "_drafts");
                if (!Directory.Exists(filefolder))
                    Directory.CreateDirectory(filefolder);
                string targetPath = Path.Combine(filefolder, title + ".md");

                string mediafolder = Path.Combine(Program.BlogSiteRoot, "media");
                if (!Directory.Exists(mediafolder))
                    Directory.CreateDirectory(mediafolder);

                PandocUtil.Http2MD(Program.WorkingDirectory, mediafolder, url, targetPath);


                Form1 form1 = LoadForm1();

                form1.srcFilePath = targetPath;
                form1.tbSrcFilename_Text = Path.GetFileName(targetPath);
                form1.tbSrcFolder_Text = Path.GetFullPath(form1.srcFilePath).Replace(form1.tbSrcFilename_Text, "");
                form1.Url = url;
                form1.Title = title;



                form1.LoadFile(true);

                form1.chkJustrDoneConversion_Checked = true;
                form1.Text = title + " " + childFormNumber++;
                form1.Show();
                form1.WindowState = FormWindowState.Maximized;
            }
            else
            {

            }
            testDialog.Dispose();

        }

        //Existing
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form1;
            if (this.ActiveMdiChild is Form1)
            {
                form1 = (Form1)this.ActiveMdiChild;
                if (form1 != null)
                {
                    form1.button3_saveexisting_Click(sender, e);
                }
            }
            
        }


        //None
        private void clearwordFolderStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1.butto_clear_9word_Click(sender, e);
        }

        //None
        private void clearhtmlFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1.button_clear_html_Click(sender, e);
        }


        //None
        private void clearAllWorkFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1.button_clear_all_Click(sender, e);
        }

        private void cleardraftFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1.button_clear_drafts_Click(sender, e);
        }


        //None
        private void autoGenerateATopiscPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var cats = from c in Program.Categorys select c.Name;
            Form3.Categories = cats.ToArray();


            Form3 testDialog = new Form3();
            form3 = testDialog;
            testDialog.Cat = null;
            testDialog.IsPost = true;
            testDialog.IsDisqus = true;
            testDialog.Seperator = Program.tbSeperator_Text;

            testDialog.MdiParent = this;
            testDialog.Text = "Autogen Topic " + childFormNumber++;
            testDialog.Show();
            testDialog.WindowState = FormWindowState.Maximized;
        }

        //Existing
        private void applyFiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form1;
            if (this.ActiveMdiChild is Form1)
            {
                form1 = (Form1)this.ActiveMdiChild;
                if (form1 != null)
                {
                    form1.button2_Click(sender, e);
                }
            }
           
        }

        //Existing
        private void addACategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form1;
            if (this.ActiveMdiChild is Form1)
            {
                form1 = (Form1)this.ActiveMdiChild;
                if (form1 != null)
                {
                    form1.cmdAddCategory_Click(sender, e);
                }
            }
            
        }

        //Existing
        private void removeACategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form1;
            if (this.ActiveMdiChild is Form1)
            {
                form1 = (Form1)this.ActiveMdiChild;
                if (form1 != null)
                {
                    form1.button4_Click(sender, e);
                }
            }
            
        }

        //Existing
        private void updateYamlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form1;
            if (this.ActiveMdiChild is Form1)
            {
                form1 = (Form1)this.ActiveMdiChild;
                if (form1 != null)
                {
                    form1.button5_Click_1(sender, e);
                }
            }
            
        }


        //Dialog
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutDialog = new AboutBox1();
            aboutDialog.ShowDialog();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form1;
            if (this.ActiveMdiChild is Form1)
            {
                form1 = (Form1)this.ActiveMdiChild;
                if (form1 != null)
                {
                    form1.Close();
                }
            }
        }

        internal static void DecrementCount()
        {
            if (mdiparent != null)
                mdiparent.childFormNumber--;
        }

        private void toggleFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form1;
            if (this.ActiveMdiChild is Form1)
            {
                form1 = (Form1)this.ActiveMdiChild;
                if (form1 != null)
                {
                    applyFiltersToolStripMenuItem.Enabled = form1.ToggleFilter();
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmSettings frmSettings = new frmSettings();
            frmSettings.ShowDialog();
        }

        private void previewPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form1;
            if (this.ActiveMdiChild is Form1)
            {
                form1 = (Form1)this.ActiveMdiChild;
                if (form1 != null)
                {
                    string srcPath = form1.filepath;
                    string targetPath = Path.Combine(Program.BlogSiteRoot, "_drafts");
                    if (!Directory.Exists(targetPath))
                        Directory.CreateDirectory(targetPath);
                    targetPath = Path.Combine(targetPath,"temp.html");

                    string title = Path.GetFileName(srcPath);
                    SetStatus("Busy: Converting MD to Html.");
                    PandocUtil.MD2Html(Program.BlogSiteRoot, Program.WorkingDirectory, srcPath, targetPath, title);
                    frmwebBrowser wb = new frmwebBrowser();
                    wb.MdiParent = this;
                    wb.Text = srcPath + childFormNumber++;
                    wb.WindowState = FormWindowState.Maximized;
                    wb.PathStr = targetPath;
                    SetStatus("Busy: Rendering Preview.");
                    wb.Show();

                }
            }
        }

        public static void SetStatus(string msg)
        {
            mdiparent.statusStrip.Text = msg;

        }

        public static void ClrStatus()
        {
            mdiparent.statusStrip.Text = "";

        }



        private void applyMetaInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form1;
            if (this.ActiveMdiChild is Form1)
            {
                form1 = (Form1)this.ActiveMdiChild;
                if (form1 != null)
                {
                    form1.AddMetaInfo();
                }
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form1;
            if (this.ActiveMdiChild is Form)
            {
                form1 = (Form)this.ActiveMdiChild;
                if (form1 != null)
                {
                    if (form1.ActiveControl is TextBox)
                    {
                        TextBox tb = (TextBox)form1.ActiveControl;
                        if (tb != null)
                        {
                            tb.SelectAll();
                        }
                        }
                }
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Form form1;
            if (this.ActiveMdiChild is Form)
            {
                form1 = (Form)this.ActiveMdiChild;
                if (form1 != null)
                {
                    if (form1.ActiveControl is TextBox)
                    {
                        TextBox tb = (TextBox)form1.ActiveControl;
                        if (tb != null)
                            // Determine if last operation can be undone in text box.   
                            if (tb.CanUndo == true)
                            {
                                // Undo the last operation.
                                tb.Undo();
                                // Clear the undo buffer to prevent last action from being redone.
                                tb.ClearUndo();
                            }
                    }
                }
            }

        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form1;
            if (this.ActiveMdiChild is Form)
            {
                form1 = (Form)this.ActiveMdiChild;
                if (form1 != null)
                {
                    if (form1.ActiveControl is TextBox)
                    {
                        TextBox tb = (TextBox)form1.ActiveControl;
                        if (tb != null)
                        {
                            MessageBox.Show("Sorry, Redo not avaiable.");
                            // Determine if last operation can be undone in text box.   
                            //if (tb.CanUndo == true)
                            //{
                            //    // Undo the last operation.
                            //    tb.Undo();
                            //    // Clear the undo buffer to prevent last action from being redone.
                            //    tb.ClearUndo();
                            //}
                        }
                    }
                }

            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);
        }

        private void printPreviewToolStripButton_Click(object sender, EventArgs e)
        {
            applyMetaInfoToolStripMenuItem_Click(sender, e);
        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void broswerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmwebBrowser wb = new frmwebBrowser();
            wb.MdiParent = this;
            wb.WindowState = FormWindowState.Maximized;
            SetStatus("Busy: Rendering Preview.");
            wb.Show();
        }

        private void saveAsPostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form1;
            if (this.ActiveMdiChild is Form1)
            {
                form1 = (Form1)this.ActiveMdiChild;
                if (form1 != null)
                {
                    form1.SaveAsPost_Click(sender, e);
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Http2MD_Click(sender, e);
        }
    }    
}

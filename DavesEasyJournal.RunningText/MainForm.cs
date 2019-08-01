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

namespace DavesEasyJournal.RunningText
{
    public partial class MainForm : Form
    {
        public const string ENTRY_SEPARATOR = "====================================";
        public const string JOURNAL_FILE_FILTERS = "Journals|*.txt";
        public const string JOURNAL_FILE_EXTENSION = "txt";

        bool _unsavedChanges = false;
        string _currentJournalFilename = string.Empty;

        public MainForm()
        {
            InitializeComponent();
        }

        #region Form update functions

        void NewJournal()
        {
            CheckUnsavedChanges();

            txtJournalContent.Clear();
            _unsavedChanges = false;
            _currentJournalFilename = string.Empty;
        }

        void AddEntry()
        {
            if (!string.IsNullOrWhiteSpace(txtJournalInput.Text))
            {
                AddEntry(txtJournalInput.Text, DateTime.Now);
                txtJournalInput.Clear();
                txtJournalInput.Focus(); // Put the cursor back into the entry field
            }

            txtJournalInput.Clear();
        }

        void AddEntry(string content, DateTime timestamp)
        {
            var sb = new StringBuilder();
            sb.AppendLine(ENTRY_SEPARATOR);
            sb.AppendLine(timestamp.ToString());
            sb.AppendLine(content);

            txtJournalContent.AppendText(sb.ToString());

            _unsavedChanges = true;
        }

        void SaveJournal()
        {
            // Commit the last uncomitted journal entry
            if (!string.IsNullOrWhiteSpace(txtJournalInput.Text))
            {
                AddEntry();
            }

            if (string.IsNullOrEmpty(_currentJournalFilename))
            {
                SaveJournalFileDialog();
            }

            if (!string.IsNullOrEmpty(_currentJournalFilename))
            {
                var content = txtJournalContent.Text;
                File.WriteAllText(_currentJournalFilename, content);
                _unsavedChanges = false;
            }
        }

        void SaveJournalFileDialog()
        {
            var sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.OverwritePrompt = true;
            sfd.DefaultExt = JOURNAL_FILE_EXTENSION;
            if (!string.IsNullOrEmpty(_currentJournalFilename)) sfd.FileName = _currentJournalFilename;
            sfd.Filter = JOURNAL_FILE_FILTERS;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _currentJournalFilename = sfd.FileName;
            }
        }

        void OpenJournalFileDialog()
        {
            CheckUnsavedChanges();

            var ofd = new OpenFileDialog();
            ofd.Filter = JOURNAL_FILE_FILTERS;
            ofd.Multiselect = false;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(ofd.FileName))
                {
                    MessageBox.Show(
                        "That file does not exist, you doofus.",
                        "File does not exist",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var fileContent = File.ReadAllText(ofd.FileName);

                txtJournalContent.Text = fileContent;
            }
        }

        void CheckUnsavedChanges()
        {
            if (_unsavedChanges || !string.IsNullOrEmpty(txtJournalInput.Text))
            {
                if (MessageBox.Show(
                    "There are unsaved changes, did you want to save them before losing them?",
                    "Unsaved changes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveJournal();
                }
            }
        }

        #endregion

        private void btnAddEntry_Click(object sender, EventArgs e)
        {
            AddEntry();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewJournal();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveJournal();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveJournalFileDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CheckUnsavedChanges();
        }
    }
}

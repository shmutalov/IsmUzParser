using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IsmUzParser
{
    public partial class MainWindow : Form
    {
        IsmUzBackgroundParser parser;
        List<IsmModel> parsedList;

        public MainWindow()
        {
            InitializeComponent();
            //parser = new IsmUzBackgroundParser("http://ism.uz", true, "192.168.0.2:3128");
            parser = new IsmUzBackgroundParser("http://ism.uz", true, true);
        }

        private void AddStatusMessage(string message, WORKER_STATE_STATUS status)
        {
            ListViewItem li = MessageList.Items.Add(message);
            switch (status)
            {
                case WORKER_STATE_STATUS.WARNING:
                    li.ForeColor = Color.Yellow;
                    break;
                case WORKER_STATE_STATUS.ERROR:
                    li.ForeColor = Color.Red;
                    break;
                case WORKER_STATE_STATUS.NORMAL:
                default:
                    li.ForeColor = Color.White;
                    break;
            }

            li.EnsureVisible();
        }

        private void UpdateNamesTable(IList<IsmModel> ismList)
        {
            NamesTable.Items.Clear();

            if (ismList != null)
            { 
                foreach (IsmModel ism in ismList)
                {
                    ListViewItem li = NamesTable.Items.Add(ism.Letter);
                    li.SubItems.Add(ism.Gender == GENDER.MALE ? "male" : "female");
                    li.SubItems.Add(ism.Name);
                    li.SubItems.Add(ism.Meaning);
                    li.SubItems.Add(ism.Origin);

                    li.BackColor = (ism.Gender == GENDER.MALE) ? Color.LightBlue : Color.LightPink;
                }
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            IsmUzBackgroundParser parser = e.Argument as IsmUzBackgroundParser;

            if (parser == null)
                return;

            parser.DoWork(worker, e);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            StatusProgress.Value = e.ProgressPercentage;
            StatusProgressLabel.Text = e.ProgressPercentage.ToString() + "%";

            if (StatusProgress.Value == 100)
                ParseButton.Enabled = true;

            WorkerState state = e.UserState as WorkerState;

            if (state == null)
                return;
            else
            {
                AddStatusMessage(state.Info, state.Status);
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ParseButton.Enabled = true;

            IsmUzBackgroundParser parser = e.Result as IsmUzBackgroundParser;

            if (parser == null)
                return;
            else
            {
                parsedList = (List<IsmModel>)parser.GetIsmList();
                UpdateNamesTable(parsedList);
            }
        }

        private void ParseButton_Click(object sender, EventArgs e)
        {
            ParseButton.Enabled = false;
            worker.RunWorkerAsync(parser);
        }

        private void FindButton_Click(object sender, EventArgs e)
        {
            if (parsedList == null)
                return;

            List<IsmModel> filteredList = null;

            if (FilterText.Text.Length == 0)
            {
                filteredList = (List<IsmModel>)parsedList.Select(a => a).ToList();
            }
            else
            {
                filteredList = (List<IsmModel>)parsedList.Where(a => a.Name.Contains(FilterText.Text)).ToList();
            }

            UpdateNamesTable(filteredList);
        }

        private void FilterText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                FindButton_Click(this, e);
        }
    }
}

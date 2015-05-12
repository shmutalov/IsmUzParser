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

        public MainWindow()
        {
            InitializeComponent();
            parser = new IsmUzBackgroundParser("http://ism.uz", true, "192.168.0.1:3128");
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
        }

        private void ParseButton_Click(object sender, EventArgs e)
        {
            ParseButton.Enabled = false;
            worker.RunWorkerAsync(parser);
        }
    }
}

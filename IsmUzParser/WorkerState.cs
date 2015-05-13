using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsmUzParser
{
    public enum WORKER_STATE_STATUS
    {
        NORMAL
        , WARNING
        , ERROR
    }

    public class WorkerState
    {
        private TextWriter log;
        private BackgroundWorker worker;

        public String Info { get; set; }
        public WORKER_STATE_STATUS Status { get; set; }

        public WorkerState(TextWriter log, BackgroundWorker worker)
        {
            this.log = log;
            this.worker = worker;
        }

        public void SetState(int progressPercentage, String info, WORKER_STATE_STATUS status)
        {
            Info = info;
            Status = status;

            if (log != null)
            {
                try
                {
                    log.WriteLine(info);
                }
                catch (Exception ex)
                { }
            }

            if (worker != null && worker.WorkerReportsProgress)
            {
                worker.ReportProgress(progressPercentage, this);
                System.Threading.Thread.Sleep(1); // TODO fix this
            }
        }
    }
}

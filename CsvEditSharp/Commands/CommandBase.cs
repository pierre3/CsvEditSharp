using CsvEditSharp.Interfaces;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace CsvEditSharp.Commands
{
    public class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        [Dependency] public IUnityContainer IocContainer { get; set; }

        [Dependency] public IMainViewModel ViewModel { get; set; }

        [Dependency] public StartupEventArgs StartupArgs { get; set; }

        [Dependency] public IViewServiceProvider ViewService { get; set; }


        public virtual bool CanExecute(object parameter) { return true; }

        public virtual void Execute(object parameter) { }

        public void InvokeCommand(string commandName, object para)
        {
            var runConfigCmd = IocContainer.Resolve<ICommand>(commandName);
            runConfigCmd.Execute(para);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Background worker support - reference RunConfigCommand for usage 

        private int _backgroundProgressCounter = 1;
        private BackgroundWorker _backgroundWorker;

        public void StartBackgroundWork(object parameter)
        {
            _backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _backgroundWorker.DoWork += BackgroundWork;

            //For the display of operation progress to UI.    
            _backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;

            //After the completation of operation.    
            _backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.RunWorkerAsync(parameter); // Start the background process

            // Kick off a timer that will update the progress display every second
            // When the background process is done the timer will be killed
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimerReportProgress);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
        }

        private void OnTimerReportProgress(object source, ElapsedEventArgs e)
        {
            // As long as background worker is running - report progress
            if (_backgroundWorker.IsBusy)
                _backgroundWorker.ReportProgress(_backgroundProgressCounter++);
            else
                // Otherwise kill the timer - we're done
                ((System.Timers.Timer)source).Stop();
        }

        protected virtual void BackgroundWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                if (_backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                //backgroundWorker.ReportProgress(i);
                Thread.Sleep(1000);
                e.Result = 1000;
            }
        }

        protected virtual void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Debug.WriteLine($"Progress update #{e.ProgressPercentage}");
        }

        protected void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                Debug.WriteLine("Operation Cancelled");
            else if (e.Error != null)
                Debug.WriteLine("Error in Process :" + e.Error);
            else
                Debug.WriteLine("Operation Completed :" + e.Result);
        }

        #endregion 
    }
}

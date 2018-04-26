using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using KOR_Updater_UI_WPF_Example.Database;
using KOR.Updater.Core;
using KOR.Updater.Core.Feed;
using System.Diagnostics;
using KOR.Updater.Core.Helpers;

namespace KOR_Updater_UI_WPF_Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Define timer
        private DispatcherTimer Timer;
        // define Database Process class
        private DatabaseProcesses dbprocs = new DatabaseProcesses();

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            Timer.Tick += new EventHandler(Timer_Tick);
        }

        // Timer for following download progress
        void Timer_Tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DownloadSizes.Text = DownloaderProgress.DownlaodedSize + " Kb / " + DownloaderProgress.TotalSize + " Kb";
                RemainingTime.Text = DownloaderProgress.RemainingTime + " sn / " + DownloaderProgress.Speed + " Kbs";

                Progress.Text = "%" + Math.Round(DownloaderProgress.Progress, 0);
                Progrress.Value = DownloaderProgress.Progress;
                Status.Text = "File Downloading...";
            });
        }

        /// <summary>
        /// Default window loaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Basic API definitaions

            // api key for app authorization
            Api.API_KEY = "12B697B4037EB4BC4E5CC1C0E2E6A90BEEDBE6EC7FFE34A1065E14EE6DE753B44116306874FDEC3D0841FCF5DC8EFA4224EA7399CC513E983852D4EE";
            // api secret for client authorization
            Api.API_SECRET = "daed110249b5a1f88054ca021499e12f12edb21891ed851fd1fdee5a85afa0d075839a236d11728db9213c3dde7e5b1d491590e84c1e420553fca8f2d45bd815";
            // select output type
            Api.OutputType = Api.OutputTypes.Json;

            #endregion

            // define Updater class and set credentials
            Updater updater = new Updater
            {
                // app versions stores sqlite database
                AppVersion = dbprocs.GetAppVersion(),
                // listing multi results off
                MultiResult = false
            };

            // check update
            var checkresult = updater.CheckUpdate();

            // if there is update/s
            if ((bool)checkresult == true)
            {
                // if there is listed update/s
                if (updater.Updates.Count > 0)
                {
                    // create new thread and start extracting file
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;

                        foreach (var update in updater.Updates)
                        {
                            #region Backup Block

                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                Status.Text = "Starting backup process...";
                            }));
                            Thread.Sleep(1500);

                            // prepare backup
                            Backup backup = new Backup
                            {
                                // maximum compression
                                // might be hard some CPUs
                                CompressionLevel = 9,
                                // set backup store folder path
                                BackupStoreFolder = AppDomain.CurrentDomain.BaseDirectory + "backup",
                                // which folder and contains includes
                                BackupFolder = AppDomain.CurrentDomain.BaseDirectory
                            };

                            // ignore backup "/backup" folder
                            backup.IgnoreDirs.Add(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "backup"));
                            // ignore backup downloaded update
                            //backup.IgnoreFiles.Add(new FileInfo(download.DownloadFilePath));

                            // start backup
                            backup.CreateBackup();

                            if (backup.BackupState)
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    Status.Text = "Backup created successfully. Now starting download new updated.";
                                }));
                                Thread.Sleep(1500);
                            }
                            #endregion

                            #region Download Block

                            // prepare downloading
                            Download download = new Download
                            {
                                // set download url
                                DownloadUrl = new Uri(update.DownloadUrl),
                                DownloadFileName = Path.GetFileName(update.DownloadUrl),
                                DownloadFilePath = AppDomain.CurrentDomain.BaseDirectory + Path.GetFileName(update.DownloadUrl),
                            };

                            // time needs download progress
                            Timer.Start();

                            // start downloading
                            download.DownloadUpdate();

                            // when download finished
                            if (download.DownloadFinished)
                            {
                                Timer.Stop();
                            }

                            #endregion

                            #region Extract Block

                            // if downloaded file is .zip file
                            if (Path.GetExtension(download.DownloadFileName) == ".zip")
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    Status.Text = "Extracting update...";
                                }));
                                Thread.Sleep(1500);

                                // prepare extraction
                                Extract extract = new Extract
                                {
                                    FilePath = download.DownloadFilePath,
                                    OutputFolder = AppDomain.CurrentDomain.BaseDirectory
                                };

                                // extract file
                                extract.ExtractZipFile();
                            }

                            #endregion

                            #region Final Block

                            // send operation results
                            Reporter reporter = new Reporter
                            {
                                Downloaded = true,
                                Success = true
                            };

                            // send report
                            reporter.SendReport(update.UpdateId);

                            // update version info new version value and save last update info
                            dbprocs.UpdateVersionString(update.AppVersion, update.UpdateId, update.AddedFeatures, update.RemovedFeatures, update.ReasonCode, update.ReasonTitle);
                            #endregion

                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                Status.Text = "Your application updated!";
                                // and then go next step, what you want

                                // start main app and close this updated
                                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "KOR Updater Demo Main WPF App");
                                Environment.Exit(Environment.ExitCode);
                            }));
                        }

                    }).Start();
                    // loop in updates
                }
            }
            else if ((bool)checkresult == false)
            {
                // if there is not update
                MessageBox.Show("You application is up to date.");

                if (!string.IsNullOrEmpty(updater.UpdaterResponse.ResponseAPIErrorMessage))
                {
                    MessageBox.Show(updater.UpdaterResponse.ResponseAPIErrorMessage, "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (!string.IsNullOrEmpty(updater.UpdaterResponse.ResponseAPIWarningMessage))
                {
                    MessageBox.Show(updater.UpdaterResponse.ResponseAPIWarningMessage, "Warning:", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (checkresult == null)
            {
                // possible no internet connection
                MessageBox.Show("There is no internet connection.");
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            // force exiting with all threads
            Environment.Exit(Environment.ExitCode);
        }
    }
}

using KOR.Updater.Core;
using KOR_Updater_UI_WPF_Example.Database;
using System;
using System.Diagnostics;
using System.Windows;

namespace KOR_Updater_Demo_Main_WPF_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // define Database Process class (original source in Update UI)
        private DatabaseProcesses dbprocs = new DatabaseProcesses();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // get last updates
            LastUpdatesList.ItemsSource = dbprocs.GetLastUpdates();

            // api key for app authorization
            Api.API_KEY = "API_KEY";
            // api secret for client authorization
            Api.API_SECRET = "API_SECRET";
            // select output type
            Api.OutputType = Api.OutputTypes.Json;


            // show current version
            CurrentVersion.Text = "Current Version: " + dbprocs.GetAppVersion();

            // if there is uncommented update feed show feed window
            string updateid = dbprocs.GetLastUpdateId();
            if (!string.IsNullOrEmpty(updateid))
            {
                FeedbackWindow feedwindow = new FeedbackWindow();
                feedwindow.Owner = GetWindow(this);
                feedwindow.UpdateId = updateid;
                feedwindow.Show();
            }

            #region Error Message Example

            // send error message example
            //if (!string.IsNullOrEmpty(updateid))
            //{
            //    Error error = new Error
            //    {
            //        UpdateId = updateid,
            //        ErrorTitle = "Null referance error",
            //        ErrorMessage = "This is an error message",
            //        ErrorTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            //    };

            //    if (error.SendErrorInfo())
            //    {
            //        MessageBox.Show("Error sended.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //    else
            //    {
            //        if (!string.IsNullOrEmpty(error.ErrorResponse.ResponseAPIErrorMessage))
            //        {
            //            MessageBox.Show(error.ErrorResponse.ResponseAPIErrorMessage, "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
            //        }
            //        else if (!string.IsNullOrEmpty(error.ErrorResponse.ResponseAPIWarningMessage))
            //        {
            //            MessageBox.Show(error.ErrorResponse.ResponseAPIWarningMessage, "Warning:", MessageBoxButton.OK, MessageBoxImage.Warning);
            //        }
            //    }
            //}

            #endregion
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
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

            // if there is update
            if ((bool)checkresult == true)
            {
                GetUpdates.Visibility = Visibility.Visible;
                // show new version
                NewVersion.Text = "New Version: " + updater.Updates[0].AppVersion;
            }
            else 
            {
                NewVersion.Text = "Your app up to date";

                if (!string.IsNullOrEmpty(updater.UpdaterResponse.ResponseAPIErrorMessage))
                {
                    MessageBox.Show(updater.UpdaterResponse.ResponseAPIErrorMessage, "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
                } else if (!string.IsNullOrEmpty(updater.UpdaterResponse.ResponseAPIWarningMessage))
                {
                    MessageBox.Show(updater.UpdaterResponse.ResponseAPIWarningMessage, "Warning:", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void GetUpdates_Click(object sender, RoutedEventArgs e)
        {
            // ask to user would you like download this update/s
            if (MessageBox.Show("Do you want to update application?", "There is an update", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // start updater and close this
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "KOR Updater UI WPF Example.exe");
                Environment.Exit(Environment.ExitCode);
            }
        }
    }
}

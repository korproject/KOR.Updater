using KOR.Updater.Core.Feed;
using KOR_Updater_UI_WPF_Example.Database;
using System.Windows;

namespace KOR_Updater_Demo_Main_WPF_App
{
    /// <summary>
    /// Interaction logic for FeedbackWindow.xaml
    /// </summary>
    public partial class FeedbackWindow : Window
    {
        public FeedbackWindow()
        {
            InitializeComponent();
        }

        // define Database Process class (original source in Update UI)
        private DatabaseProcesses dbprocs = new DatabaseProcesses();
        // update id for unique feedback
        public string UpdateId { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(UpdateId))
            {
                Feedback feedback = new Feedback
                {
                    Comment = Comment.Text,
                    Point = Point.Value,
                    UpdateId = UpdateId
                };

                if (feedback.SendFeedback())
                {
                    dbprocs.UpdatedFeedback();
                    MessageBox.Show("Feedback sended.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                } else
                {
                    if (!string.IsNullOrEmpty(feedback.FeedbackResponse.ResponseAPIErrorMessage))
                    {
                        MessageBox.Show(feedback.FeedbackResponse.ResponseAPIErrorMessage, "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (!string.IsNullOrEmpty(feedback.FeedbackResponse.ResponseAPIWarningMessage))
                    {
                        MessageBox.Show(feedback.FeedbackResponse.ResponseAPIWarningMessage, "Warning:", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }
    }
}

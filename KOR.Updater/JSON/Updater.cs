namespace KOR.Updater.JSON
{
    public class RootobjectforUpdater
    {
        public int code { get; set; }
        public Result[] result { get; set; }
        public Messages messages { get; set; }
        public Dev_Tips dev_tips { get; set; }
    }

    public class Messages
    {
        public bool error { get; set; }
        public object error_message { get; set; }
        public bool warning { get; set; }
        public object warning_message { get; set; }
    }

    public class Dev_Tips
    {
        public object tip { get; set; }
        public object link { get; set; }
    }

    public class Result
    {
        public string appid { get; set; }
        public string app_version { get; set; }
        public string updateid { get; set; }
        public string client_version { get; set; }
        public string reason_code { get; set; }
        public string reason_title { get; set; }
        public string added_date { get; set; }
        public string message_title { get; set; }
        public string message_content { get; set; }
        public string download_url { get; set; }
        public string added_features { get; set; }
        public string removed_features { get; set; }
    }
}

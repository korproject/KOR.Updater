namespace KOR.Updater.Core.Helpers
{
    public class DownloaderProgress
    {
        /// <summary>
        /// Downlaod progress value by percent
        /// </summary>
        public static float Progress { get; set; }

        /// <summary>
        /// Downlaod speed by KiB/sec.
        /// </summary>
        public static int Speed { get; set; }

        /// <summary>
        /// Download remaining time by sec
        /// </summary>
        public static long RemainingTime { get; set; }

        /// <summary>
        /// Downloaded file size of KiB
        /// </summary>
        public static long DownlaodedSize { get; set; }

        /// <summary>
        /// Downloading file total size
        /// </summary>
        public static long TotalSize { get; set; }
    }
}

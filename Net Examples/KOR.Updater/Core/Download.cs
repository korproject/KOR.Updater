using Toqe.Downloader.Business.Contract;
using Toqe.Downloader.Business.Contract.Events;
using Toqe.Downloader.Business.Download;
using Toqe.Downloader.Business.DownloadBuilder;
using Toqe.Downloader.Business.Observer;
using Toqe.Downloader.Business.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using KOR.Updater.Core.Helpers;

namespace KOR.Updater.Core
{
    public class Download
    {
        #region Downloader Contstructors

        /// <summary>
        /// Finished value
        /// </summary>
        public Uri DownloadUrl { get; set; }

        /// <summary>
        /// Download file path
        /// </summary>
        public string DownloadFilePath { get; set; }

        /// <summary>
        /// Download file name
        /// </summary>
        public string DownloadFileName { get; set; }

        /// <summary>
        /// Finished value
        /// </summary>
        public bool DownloadFinished { get; set; }

        #endregion

        #region Downloader

        /// <summary>
        /// Update Downlaoder
        /// </summary>
        /// <param name="downloadurl">file url to download</param>
        public void DownloadUpdate()
        {
            bool useDownloadSpeedThrottling = false;

            var file = new FileInfo(Path.GetFileName(DownloadUrl.ToString()));
            var requestBuilder = new SimpleWebRequestBuilder();
            var dlChecker = new DownloadChecker();
            var httpDlBuilder = new SimpleDownloadBuilder(requestBuilder, dlChecker);
            var timeForHeartbeat = 3000;
            var timeToRetry = 5000;
            var maxRetries = 25;
            var resumingDlBuilder = new ResumingDownloadBuilder(timeForHeartbeat, timeToRetry, maxRetries, httpDlBuilder);
            List<DownloadRange> alreadyDownloadedRanges = null;
            var bufferSize = 4096;
            var numberOfParts = 4;
            var download = new MultiPartDownload(DownloadUrl, bufferSize, numberOfParts, resumingDlBuilder, requestBuilder, dlChecker, alreadyDownloadedRanges);
            var speedMonitor = new DownloadSpeedMonitor(maxSampleCount: 50000);
            speedMonitor.Attach(download);
            var progressMonitor = new DownloadProgressMonitor();
            progressMonitor.Attach(download);

            if (useDownloadSpeedThrottling)
            {
                var downloadThrottling = new DownloadThrottling(maxBytesPerSecond: 200 * 10024, maxSampleCount: 1288);
                downloadThrottling.Attach(download);
            }

            var dlSaver = new DownloadToFileSaver(file);
            dlSaver.Attach(download);
            download.DownloadCompleted += OnCompleted;
            download.Start();

            while (!DownloadFinished)
            {
                var alreadyDownloadedSizeInBytes = progressMonitor.GetCurrentProgressInBytes(download);
                var totalDownloadSizeInBytes = progressMonitor.GetTotalFilesizeInBytes(download);
                var currentSpeedInBytesPerSecond = speedMonitor.GetCurrentBytesPerSecond();

                var currentProgressInPercent = progressMonitor.GetCurrentProgressPercentage(download) * 100;
                var alreadyDownloadedSizeInKiB = (alreadyDownloadedSizeInBytes / 1024);
                var totalDownloadSizeInKiB = (totalDownloadSizeInBytes / 1024);
                var currentSpeedInKiBPerSecond = (currentSpeedInBytesPerSecond / 1024);
                var remainingTimeInSeconds = currentSpeedInBytesPerSecond == 0 ? 0 : (totalDownloadSizeInBytes - alreadyDownloadedSizeInBytes) / currentSpeedInBytesPerSecond;

                DownloaderProgress.Progress = currentProgressInPercent;
                DownloaderProgress.DownlaodedSize = alreadyDownloadedSizeInKiB;
                DownloaderProgress.TotalSize = totalDownloadSizeInKiB;
                DownloaderProgress.Speed = currentSpeedInKiBPerSecond;
                DownloaderProgress.RemainingTime = remainingTimeInSeconds;
            }
        }

        /// <summary>
        /// When download comleted
        /// </summary>
        /// <param name="args"></param>
        void OnCompleted(DownloadEventArgs args)
        {
            args.Download.DetachAllHandlers();

            if (DownloaderProgress.DownlaodedSize == DownloaderProgress.TotalSize && File.Exists(DownloadFilePath))
            {
                DownloadFinished = true;
            }
        }

        #endregion
    }
}

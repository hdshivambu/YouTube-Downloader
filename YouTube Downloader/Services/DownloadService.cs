﻿namespace YouTube.Downloader.Services
{
    using System;
    using System.Collections.Generic;

    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.Services.Interfaces;

    internal class DownloadService : IDownloadService
    {
        private const int MaxConcurrentDownloads = 3;

        private readonly Queue<DownloadProcess> _downloadQueue = new Queue<DownloadProcess>();

        private readonly List<DownloadProcess> _currentDownloads = new List<DownloadProcess>();

        public void QueueDownloads(IEnumerable<DownloadProcess> downloads)
        {
            foreach (DownloadProcess download in downloads)
            {
                _downloadQueue.Enqueue(download);
                download.DownloadStatus.DownloadState = DownloadState.Queued;
            }

            while (_currentDownloads.Count < MaxConcurrentDownloads && _downloadQueue.Count > 0)
            {
                DownloadNext();
            }
        }

        private void DownloadNext()
        {
            while (true)
            {
                if (_downloadQueue.Count == 0)
                {
                    return;
                }

                DownloadProcess downloadProcess = _downloadQueue.Dequeue();
                downloadProcess.DownloadStatus.DownloadState = DownloadState.Downloading;

                if (downloadProcess.HasExited)
                {
                    continue;
                }

                _currentDownloads.Add(downloadProcess);

                void DownloadFinished(object sender, EventArgs e)
                {
                    downloadProcess.Exited -= DownloadFinished;

                    _currentDownloads.Remove(downloadProcess);

                    DownloadNext();
                }

                downloadProcess.Exited += DownloadFinished;

                downloadProcess.Start();

                break;
            }
        }
    }
}
﻿namespace YouTube.Downloader.ViewModels.Interfaces.Process
{
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models.Download;

    internal interface IDownloadProcessViewModel : IActiveProcessViewModel
    {
        DownloadProgress DownloadProgress { get; }

        void Initialise(IVideoViewModel videoViewModel, DownloadProcess process, DownloadProgress downloadProgress);
    }
}
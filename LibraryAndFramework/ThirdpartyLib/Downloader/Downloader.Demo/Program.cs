using System.Net;

namespace Downloader.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var downloadOpt = new DownloadConfiguration()
            {
                // usually, hosts support max to 8000 bytes, default value is 8000
                BufferBlockSize = 10240,
                // file parts to download, the default value is 1
                ChunkCount = 8,
                // download speed limited to 2MB/s, default values is zero or unlimited
                MaximumBytesPerSecond = 1024 * 1024 * 2,
                // the maximum number of times to fail
                MaxTryAgainOnFailover = 5,
                // release memory buffer after each 50 MB
                MaximumMemoryBufferBytes = 1024 * 1024 * 50,
                // download parts of the file as parallel or not. The default value is false
                ParallelDownload = true,
                // number of parallel downloads. The default value is the same as the chunk count
                ParallelCount = 4,
                // timeout (millisecond) per stream block reader, default values is 1000
                Timeout = 1000,
                // set true if you want to download just a specific range of bytes of a large file
                RangeDownload = false,
                // floor offset of download range of a large file
                RangeLow = 0,
                // ceiling offset of download range of a large file
                RangeHigh = 0,
                // clear package chunks data when download completed with failure, default value is false
                ClearPackageOnCompletionWithFailure = true,
                // minimum size of chunking to download a file in multiple parts, the default value is 512
                MinimumSizeOfChunking = 1024,
                // Before starting the download, reserve the storage space of the file as file size, the default value is false
                ReserveStorageSpaceBeforeStartingDownload = true,
                // Get on demand downloaded data with ReceivedBytes on downloadProgressChanged event 
                EnableLiveStreaming = false,
            };

            var downloader = new DownloadService(downloadOpt);

            downloader.DownloadStarted += Downloader_DownloadStarted; ;
            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged; ;

        }

        private static void Downloader_DownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine($"DownloadProgressChanged---->{e.ProgressPercentage}");
        }

        private static void Downloader_DownloadStarted(object? sender, DownloadStartedEventArgs e)
        {
            Console.WriteLine("Downloader_DownloadStarted~");
        }


    }
}

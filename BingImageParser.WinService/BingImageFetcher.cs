using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using BingImageParser.Core;

namespace BingImageParser.WinService
{
    internal partial class BingImageFetcher : ServiceBase
    {
        private static CancellationTokenSource _cts;
        
        public BingImageFetcher()
        {
            InitializeComponent();
            this.ServiceName = "BingImageFetcherService";
            
            _cts = new CancellationTokenSource();
            this.CanShutdown = true;
            this.AutoLog = false;
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            
        }

        protected override async void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.

            await GetWallpaper(_cts);
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            _cts.Cancel();
        }


        private async Task GetWallpaper(CancellationTokenSource token)
        {
            while (!token.IsCancellationRequested)
                try
                {
                    if (WallpaperHelper.TodaysWallpaperAlreadySet)
                    {                        
                        await Task.Delay(TimeSpan.FromSeconds(20), token.Token);
                        continue;
                    }
                    
                    await WallpaperHelper.GetWallpaper();
                    await Task.Delay(TimeSpan.FromSeconds(20), token.Token);
                }

                catch (Exception exception)
                {
                    EventLog.WriteEntry($"Bing parser encoutered an error. Exception says: {exception.Message}",
                        EventLogEntryType.Warning);
                    await Task.Delay(TimeSpan.FromMinutes(1), token.Token);
                }
        }
    }
}
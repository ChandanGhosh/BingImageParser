using BingImageParser.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BingImageParser.WindowsService
{
    public class Processor
    {
        private bool _running;
        private Task _myLoop;
        private static CancellationTokenSource _cts = new CancellationTokenSource();

        public async Task Start()
        {
            //Probably do some async data loading here
            await Task.Delay(1);
            _running = true;
            //_myLoop = Loop();

            //using (_cts = new CancellationTokenSource())
            //{
            //    //System.Console.CancelKeyPress += (s, e) =>
            //    //{
            //    //    if (e.SpecialKey == ConsoleSpecialKey.ControlC)
            //    //        _cts?.Cancel();
            //    //};


            //    //_myLoop = GetWallpaper(_cts);
            //}
            //_cts = new CancellationTokenSource();
            _myLoop = GetWallpaper(_cts);
        }

        public async Task Stop()
        {
            //_running = false;
            _cts.Cancel();
            await _myLoop.ContinueWith(t=> { _cts.Dispose(); });
           
        }

        public async Task Loop()
        {
            while (_running)
            {
                await Task.Delay(5000);
                System.Console.WriteLine("Everything is fine.");
            }
        }

        private static async Task GetWallpaper(CancellationTokenSource token)
        {
            while (!token.IsCancellationRequested)
                try
                {

                    if (WallpaperHelper.TodaysWallpaperAlreadySet)
                    {
                        System.Console.WriteLine("Todays wallpaper is already set.");
                        Trace.WriteLine("Todays wallpaper is already set.");
                        await Task.Delay(TimeSpan.FromSeconds(5), token.Token);
                        continue;
                    }
                    System.Console.WriteLine("Get images in json from bing.com");
                    await WallpaperHelper.GetWallpaper();
                    System.Console.WriteLine("Image downloaded and set as wallpaper from bing");
                    await Task.Delay(TimeSpan.FromSeconds(20), token.Token);
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception.Message);
                    System.Console.WriteLine($"Exception occured: {exception.Message}");
                    await Task.Delay(TimeSpan.FromMinutes(1), token.Token);
                }
        }
    }
}

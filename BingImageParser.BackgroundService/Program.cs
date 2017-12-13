using BingImageParser.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingImageParser.BackgroundService
{
    class Program
    {
        private static CancellationTokenSource _cts;

        private static void Main(string[] args)
        {

            using (_cts = new CancellationTokenSource())
            {
                System.Console.CancelKeyPress += (s, e) =>
                {
                    if (e.SpecialKey == ConsoleSpecialKey.ControlC)
                        _cts?.Cancel();
                };


                GetWallpaper(_cts).Wait(_cts.Token);
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
                    System.Console.WriteLine($"Exception occured: {exception.Message}");
                    await Task.Delay(TimeSpan.FromMinutes(1), token.Token);
                }
        }
    }
}

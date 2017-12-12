using System;
using System.Threading;
using System.Threading.Tasks;
using BingImageParser.Core;

namespace BingImageParser
{
    internal static class Program
    {
        private static CancellationTokenSource _cts;

        private static void Main(string[] args)
        {
            using (_cts = new CancellationTokenSource())
            {
                Console.CancelKeyPress += (s, e) =>
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
                    Console.WriteLine("Get images in json from bing.com");
                    await WallpaperHelper.GetWallpaper();
                    Console.WriteLine("Image downloaded and set as wallpaper from bing");
                    await Task.Delay(TimeSpan.FromSeconds(20), token.Token);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Exception occured: {exception.Message}");
                    await Task.Delay(30, token.Token);
                }
        }
    }
}
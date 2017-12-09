using System;

namespace BingImageParser
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Get images in json from bing.com");
            new WallpaperHelper().GetWallpaper();
            Console.WriteLine("Image downloaded from bing");
        }
    }
}

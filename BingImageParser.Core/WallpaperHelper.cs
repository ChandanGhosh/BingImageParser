using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BingImageParser.Core
{
    public static class WallpaperHelper
    {
        private const int SpiSetdeskwallpaper = 20;
        private const int SpifUpdateinifile = 1;
        private const int SpifSendwininichange = 2;
        private const string BaseUri = "https://www.bing.com";
        private const string ServiceUri = "/HPImageArchive.aspx?format=hp&idx=0&n=1";

        private static readonly string WallpaperFolderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "temp");


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public static void SetWallPaper(string folderPath, string imageName, Style style)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                SetWallpaperForWindows(folderPath, imageName, style);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                SetWallpaperForOsx(folderPath, imageName, style);
        }

        //ToDo: Implement for mac os
        private static void SetWallpaperForOsx(string folderPath, string imageName, Style style)
        {
        }

        private static void SetWallpaperForWindows(string folderPath, string imageName, Style style)
        {
            var lpvParam = Path.Combine(folderPath, imageName);

            //using (var img = System.Drawing.Image.FromFile(Path.Combine(folderPath, imageName)))
            //{
            //    img.Save(lpvParam, ImageFormat.Bmp);
            //}

            using (var registryKey1 = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true))
            {
                int num;
                const string name1 = "WallpaperStyle";
                const string name2 = "TileWallpaper";
                switch (style)
                {
                    case Style.Fill:
                    {
                        var registryKey2 = registryKey1;
                        num = 10;
                        var str1 = num.ToString();
                        registryKey2.SetValue(name1, str1);
                        var registryKey3 = registryKey1;
                        num = 0;
                        var str2 = num.ToString();
                        registryKey3.SetValue(name2, str2);
                        break;
                    }
                    case Style.Fit:
                    {
                        var registryKey2 = registryKey1;

                        num = 6;
                        var str1 = num.ToString();
                        registryKey2.SetValue(name1, str1);
                        var registryKey3 = registryKey1;

                        num = 0;
                        var str2 = num.ToString();
                        registryKey3.SetValue(name2, str2);
                        break;
                    }
                    case Style.Span:
                    {
                        var registryKey2 = registryKey1;

                        num = 22;
                        var str1 = num.ToString();
                        registryKey2.SetValue(name1, str1);
                        var registryKey3 = registryKey1;

                        num = 0;
                        var str2 = num.ToString();
                        registryKey3.SetValue(name2, str2);
                        break;
                    }
                    case Style.Stretch:
                    {
                        var registryKey2 = registryKey1;

                        num = 2;
                        var str1 = num.ToString();
                        registryKey2.SetValue(name1, str1);
                        var registryKey3 = registryKey1;

                        num = 0;
                        var str2 = num.ToString();
                        registryKey3.SetValue(name2, str2);
                        break;
                    }
                    case Style.Tile:
                    {
                        var registryKey2 = registryKey1;

                        num = 0;
                        var str1 = num.ToString();
                        registryKey2.SetValue(name1, str1);
                        var registryKey3 = registryKey1;

                        num = 1;
                        var str2 = num.ToString();
                        registryKey3.SetValue(name2, str2);
                        break;
                    }
                    case Style.Center:
                    {
                        var registryKey2 = registryKey1;

                        num = 0;
                        var str1 = num.ToString();
                        registryKey2.SetValue(name1, str1);
                        var registryKey3 = registryKey1;

                        num = 0;
                        var str2 = num.ToString();
                        registryKey3.SetValue(name2, str2);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(style), style, null);
                }

                SystemParametersInfo(SpiSetdeskwallpaper, 0, lpvParam, SpifSendwininichange | SpifUpdateinifile);
            }
        }

        public static async Task GetWallpaper()
        {
            using (var client = new HttpClient())
            {
                var str1 = await client.GetStringAsync(new Uri($"{BaseUri}{ServiceUri}", UriKind.RelativeOrAbsolute));
                var str2 = str1.Substring(str1.IndexOf('{'));
                var rootObject =
                    JsonConvert.DeserializeObject<RootObject>(str2.Substring(0, str2.LastIndexOf('}') + 1));
                if (rootObject.images.FirstOrDefault() == null)
                    return;
                await SaveWallpaper(rootObject.images.First().url);
            }
        }

        public static async Task SaveWallpaper(string wallpaperUrl)
        {
            if (!Directory.Exists(WallpaperFolderPath))
                Directory.CreateDirectory(WallpaperFolderPath);
            var imageName = wallpaperUrl.Split('/').LastOrDefault()?.Replace(".jpg", ".bmp");
            using (var client = new HttpClient())
            {
                if (!File.Exists(Path.Combine(WallpaperFolderPath, imageName)))
                    using (var imageStream = await client.GetStreamAsync(new Uri($"{BaseUri}{wallpaperUrl}")))
                    {
                        System.Drawing.Image.FromStream(imageStream).Save(Path.Combine(WallpaperFolderPath, imageName),
                            ImageFormat.Bmp);
                    }
            }
            SetWallPaper(WallpaperFolderPath, imageName, Style.Fill);

            DeleteOldWallpapers(WallpaperFolderPath, imageName);
        }

        private static void DeleteOldWallpapers(string wallpaperFolderPath, string imageName)
        {
            if (Directory.Exists(wallpaperFolderPath))
                foreach (var fileName in Directory.EnumerateFiles(wallpaperFolderPath))
                    if (fileName.Contains(imageName) == false)
                        File.Delete(fileName);
        }
    }
}
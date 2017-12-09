using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BingImageParser
{
    internal sealed class WallpaperHelper
    {
        private const int SpiSetdeskwallpaper = 20;
        private const int SpifUpdateinifile = 1;
        private const int SpifSendwininichange = 2;
        private const string BaseUri = "https://www.bing.com";
        private const string ServiceUri = "/HPImageArchive.aspx?format=hp&idx=0&n=1";
        private readonly WebClient _client;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public static string WallpaperFolderPath { get; set; }

        public static string ImageName { get; private set; }

        public WallpaperHelper()
        {

            WallpaperFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "temp");
            _client = new WebClient();
        }

        public void SetWallPaper(string folderPath, string imageName, Style style)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SetWallpaperForWindows(folderPath, imageName, style);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                SetWallpaperForOsx(folderPath, imageName, style);
            }
        }

        private void SetWallpaperForOsx(string folderPath, string imageName, Style style)
        {

        }

        private static void SetWallpaperForWindows(string folderPath, string imageName, Style style)
        {
            var lpvParam = Path.Combine(folderPath, $"{ImageName}.bmp");

            System.Drawing.Image.FromFile(Path.Combine(folderPath, imageName)).Save(lpvParam, ImageFormat.Bmp);
            var registryKey1 = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
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

        public void GetWallpaper()
        {
            using (_client)
            {
                var str1 = _client.DownloadString(new Uri(string.Format("{0}{1}", BaseUri, ServiceUri), UriKind.RelativeOrAbsolute));
                var str2 = str1.Substring(str1.IndexOf('{'));
                var rootObject = JsonConvert.DeserializeObject<RootObject>(str2.Substring(0, str2.LastIndexOf('}') + 1));
                if (rootObject.images.FirstOrDefault() == null)
                    return;
                SaveWallpaper(rootObject.images.First().url);
            }
        }

        public void SaveWallpaper(string wallpaperUrl)
        {
            if (!Directory.Exists(WallpaperFolderPath))
                Directory.CreateDirectory(WallpaperFolderPath);
            ImageName = wallpaperUrl.Split('/').LastOrDefault();
            using (_client)
            {
                if (!File.Exists(Path.Combine(WallpaperFolderPath, ImageName)))
                    _client.DownloadFile(new Uri($"{BaseUri}{wallpaperUrl}"), Path.Combine(WallpaperFolderPath, ImageName));
            }
            SetWallPaper(WallpaperFolderPath, ImageName, Style.Fill);
        }
    }
}

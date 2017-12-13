using System;
using Topshelf;

namespace BingImageParser.WindowsService
{
    class Program
    {/// <summary>
     /// The main entry point for the application.
     /// </summary>
        public static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<Processor>(s =>
                {
                    s.ConstructUsing(() => Create());
                    s.WhenStarted(p => p.Start().Wait());
                    s.WhenStopped(p => p.Stop().Wait());
                });
                //x.RunAsLocalSystem();
                x.RunAsPrompt();
                x.StartAutomatically();
                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(0);
                    r.RestartService(1);
                    r.RestartService(2);

                    r.OnCrashOnly();

                    //number of days until the error count resets
                    r.SetResetPeriod(1);
                });

                x.SetDescription("Bing image fetcher");
                x.SetDisplayName("Bing image fetcher service");
                x.SetServiceName("BingImageFetcherService");
                
            });
        }

        public static Processor Create()
        {
            //Allow parameterised configuration here.
            return new Processor();
        }
    }
}

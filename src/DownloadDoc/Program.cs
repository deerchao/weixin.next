using System.IO;

namespace DownloadDoc
{
    class Program
    {
        static void Main(string[] args)
        {
            var folder = args.Length == 1
                ? args[0]
                : "../../../../docs";

            var di = new DirectoryInfo(folder);
            if(!di.Exists)
                di.Create();

            new DocDownloader(di.FullName).Download().Wait();
        }
    }
}

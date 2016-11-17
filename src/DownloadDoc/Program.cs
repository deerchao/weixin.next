using System.IO;
using System.Threading.Tasks;

namespace DownloadDoc
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = args.Length == 1
                ? args[0]
                : "../../../../docs";

            var di = new DirectoryInfo(root);
            if (!di.Exists)
                di.Create();

            root = di.FullName;

            var tasks = new[]
            {
             //   new MPDocDownloader(Path.Combine(root, "mp")).Download(),
                new PayDocDownloader(Path.Combine(root, "pay")).Download(),
            };
            Task.WaitAll(tasks);
        }
    }
}

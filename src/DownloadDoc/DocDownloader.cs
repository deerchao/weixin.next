using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DownloadDoc
{
    class DocDownloader
    {
        private readonly string _folder;

        public DocDownloader(string folder)
        {
            _folder = folder;
        }


        public async Task Download()
        {
            Clear();

            var menu = await GetMenu();

            foreach (var item in menu)
            {
                await DownloadContent(_folder, item);
            }

            GenerateIndex(menu);
        }

        private void Clear()
        {
            foreach (var folder in Directory.GetDirectories(_folder))
            {
                Directory.Delete(folder, true);
            }

            File.Delete(Path.Combine(_folder, "index.html"));
        }

        void GenerateIndex(menuitem[] items)
        {
            var html = @"<html>
<head>
<meta content=""text/html; charset=utf-8"" http-equiv=""content-type"" />
</head>
<body>
<ul>
";

            foreach (var item in items)
            {
                html += GenerateMenuHtml(item, "") + "\r\n";
            }

            html += @"
</ul>
</body>
</html>";

            var file = Path.Combine(_folder, "index.html");
            File.WriteAllText(file, html, Encoding.UTF8);
        }

        string GenerateMenuHtml(menuitem item, string prefix)
        {
            if (item.leaf == "1")
            {
                return $"<li><a href=\"{prefix.Trim('\\')}\\{item.name}.html\">{item.name}</a></li>";
            }

            var html = $"<li>{item.name}\r\n<ul>\r\n";
            foreach (var child in item.children)
            {
                html += GenerateMenuHtml(child, prefix + "\\" + item.name) + "\r\n";
            }
            html += "</ul>\r\n</li>";
            return html;
        }

        async Task DownloadContent(string folder, menuitem item)
        {
            Console.WriteLine($"处理 {item.name}");

            if (item.leaf == "1")
            {
                var path = Path.Combine(folder, $"{item.name}.html");
                var url = $"https://mp.weixin.qq.com/wiki?action=doc&id={item.id}&lang=zh_CN";
                var html = await GetHtml(url);
                File.WriteAllText(path, TidyHtml(html), Encoding.UTF8);
            }
            else
            {
                folder = Path.Combine(folder, item.name);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                foreach (var child in item.children)
                {
                    await DownloadContent(folder, child);
                }
            }
        }

        private string TidyHtml(string html)
        {
            //开始标签前加换行
            html = Regex.Replace(html, @"<(?!/|a|span|strong|th|td|img|em)([^>]+)>\s*(?=<)", "\r\n<$1>");
            //结束标签后加换行
            html = Regex.Replace(html, @"<(?!/a|/span|/strong|/th|/td|/img|/em)(/[^>]+)>\s*(?=<)", "<$1>\r\n");
            return html;
        }

        async Task<menuitem[]> GetMenu()
        {
            var html = await GetHtml("https://mp.weixin.qq.com/wiki");
            var r = new Regex(@"\bcgiData\s*=\s*({.+?});", RegexOptions.Singleline);
            var cgi = r.Match(html).Groups[1].Value;

            var o = JsonConvert.DeserializeObject<cgidata>(cgi);
            return o.list.list;
        }

        Task<string> GetHtml(string url)
        {
            for (var i = 0; i < 3; i++)
            {
                try
                {
                    return new HttpClient().GetStringAsync(url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误 {ex.Message}");
                    Thread.Sleep(1000);
                }
            }

            throw new Exception("无法获取内容");
        }

        // ReSharper disable InconsistentNaming
        class cgidata
        {
            public string id { get; set; }
            public string anchor { get; set; }
            public menulist list { get; set; }
        }

        class menulist
        {
            public menuitem[] list { get; set; }
        }

        class menuitem
        {
            public string id { get; set; }
            public string father { get; set; }
            public string leaf { get; set; }
            public string name { get; set; }
            public menuitem[] children { get; set; }
            public int expanded { get; set; }
            public bool is_online { get; set; }
        }
    }
}

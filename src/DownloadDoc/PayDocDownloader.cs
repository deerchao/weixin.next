using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DownloadDoc
{
    class PayDocDownloader
    {
        private readonly string _folder;

        public PayDocDownloader(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            _folder = folder;
        }


        public async Task Download()
        {
            Clear();

            var menu = await GetMenu();

            foreach (var item in menu)
            {
                await DownloadContent(item);
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
            string category = null;
            foreach (var item in items)
            {
                if (category != item.category)
                {
                    if (category != null)
                        html += "</ul></li>";
                    html += "<li>" + category + "<ul>";
                    html += GenerateMenuHtml(item) + "\r\n";
                    category = item.category;
                }
                else
                {
                    html += GenerateMenuHtml(item) + "\r\n";
                }
            }

            html += @"
</ul>
</li>
</ul>
</body>
</html>";

            var file = Path.Combine(_folder, "index.html");
            File.WriteAllText(file, html, Encoding.UTF8);
        }

        string GenerateMenuHtml(menuitem item)
        {
            return $"<li><a href=\"{item.category}\\{item.name}.html\">{item.name}</a></li>";
        }

        async Task DownloadContent(menuitem item)
        {
            Console.WriteLine($"处理 {item.name}");

            var folder = Path.Combine(_folder, item.category);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, $"{item.name}.html");
            var html = await GetHtml(item.url);
            File.WriteAllText(path, TidyHtml(html), Encoding.UTF8);
        }

        private string TidyHtml(string html)
        {
            var start = html.IndexOf("<!-- 主区域 [[ -->", StringComparison.InvariantCulture);

            var end = html.IndexOf("<!-- 主区域 ]] -->", StringComparison.InvariantCulture);

            if (start >= 0 && end >= 0)
                html = html.Substring(start, end - start);

            return html;
        }

        async Task<menuitem[]> GetMenu()
        {
            var url = new Uri("https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=9_1");
            var html = await GetHtml(url);
            var r = new Regex(@"<dl\sid=""\d+"">\s*
<dt><a\shref=""\#""><i\sclass=""ico-arrows""></i>(.+?)</i></a></dt>\s*
(?:<dd><a\shref=""([^""]+)"">(.*?)</a></dd>\s*)+
</dl>", RegexOptions.IgnorePatternWhitespace);

            var matches = r.Matches(html);
            var list = new List<menuitem>();
            foreach (Match m in matches)
            {
                for (var i = 0; i < m.Groups[2].Captures.Count; i++)
                {
                    list.Add(new menuitem
                    {
                        category = m.Groups[1].Value,
                        url = new Uri(url, m.Groups[2].Captures[i].Value),
                        name = m.Groups[3].Captures[i].Value,
                    });
                }
            }
            return list.ToArray();
        }

        Task<string> GetHtml(Uri url)
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

        class menuitem
        {
            public string category { get; set; }
            public Uri url { get; set; }
            public string name { get; set; }
        }
    }
}

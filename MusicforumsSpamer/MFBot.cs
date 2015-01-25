using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MusicforumsSpamer
{
    class MFBot
    {

        public static void LoadSectionsLinks() 
        {
            IList<string> links = new List<string>();
            string content;
            string host = "http://www.musicforums.ru";
            using (WebClient wc = new WebClient())
            {
                wc.Headers["User-Agent"] = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
             content=  wc.DownloadString("http://www.musicforums.ru/");
            }
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            var node = doc.DocumentNode.SelectSingleNode("//div[@class='pad-tb corn-c']");
            var nodesUl = node.SelectNodes("ul");
            if (nodesUl.Count > 3) 
            {
                for (int i = 0; i < 3; i++)
                {
                    var anodes = nodesUl[i].SelectNodes("li/a");
                    foreach (var item in anodes)
                    {
                        string href = item.GetAttributeValue("href", "");
                        if (!string.IsNullOrEmpty(href)) 
                        {
                            links.Add(host + href);
                        }
                    }
                     anodes = nodesUl[i].SelectNodes("li/h1/a");
                    foreach (var item in anodes)
                    {
                        string href = item.GetAttributeValue("href", "");
                        if (!string.IsNullOrEmpty(href))
                        {
                            links.Add(host + href);
                        }
                    }

                }
            }

            File.WriteAllLines("SectionLinks.txt", links);
        }

        public static CookieContainer GetCookieAuth(string user, string password) 
        {
            string postString = String.Format("site=mfor&bn=mfor_buysell&loginform=1&loginuser={0}&loginpassword={1}&keepalive=on&btnlogin=%C2%EE%E9%F2%E8", user, password);
            CookieContainer cookie = new CookieContainer();
            string urlRefer = "http://www.musicforums.ru/buysell/login.php?bn=mfor_buysell";
            string url = "http://www.musicforums.ru/buysell/login.php";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Accept = "text/html, application/xhtml+xml, */*";
            req.AllowAutoRedirect = true;
            req.Headers["Accept-Language"] = "en-US,en;q=0.7,ru;q=0.3";
            req.Headers["Accept-Encoding"] = "gzip, deflate";
            req.CookieContainer = cookie;
            req.Method = "POST";
            req.Referer = urlRefer;
            byte[] ByteArr = System.Text.Encoding.UTF8.GetBytes(postString);
            req.ContentLength = ByteArr.Length;
            req.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            return cookie;
        }
        
    }
}

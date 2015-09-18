using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System;

namespace MusicforumsSpamer
{
  class MFBot
  {
    public static void LoadSectionsLinks()
    {
      Logger.LogMessage("Loading section links...");
      IList<string> links = new List<string>();
      string content;
      var host = "http://www.musicforums.ru";
      using (var wc = new WebClient())
      {
        wc.Headers["User-Agent"] = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
        content = wc.DownloadString(host);
      }
      var doc = new HtmlDocument();
      doc.LoadHtml(content);
      var node = doc.DocumentNode.SelectSingleNode("//div[@class='pad-tb corn-c']");
      if (node == null)
        throw new Exception("node is null");
      var nodesUl = node.SelectNodes("ul");
      if (nodesUl == null)
        throw new Exception("nodesUl is null");
      if (nodesUl.Count > 3)
      {
        for (var i = 0; i < 3; i++)
        {
          var anodes = nodesUl[i].SelectNodes("li/a");
          if (anodes == null)
            continue;
          foreach (var item in anodes)
          {
            var href = item.GetAttributeValue("href", "");
            if (!string.IsNullOrEmpty(href))
              links.Add(host + href);
          }
          anodes = nodesUl[i].SelectNodes("li/h1/a");
          foreach (var item in anodes)
          {
            var href = item.GetAttributeValue("href", "");
            if (!string.IsNullOrEmpty(href))
              links.Add(host + href);
          }
        }
      }
      Logger.LogMessage(string.Format("Load {0} section links", links.Count));
      File.WriteAllLines("SectionLinks.txt", links);
      Logger.LogSuccess("Loading section links... SUCCESS");
    }

    public static CookieContainer GetCookieAuth(string user, string password)
    {
      var postString = string.Format("site=mfor&bn=mfor_buysell&loginform=1&loginuser={0}&loginpassword={1}&keepalive=on&btnlogin=%C2%EE%E9%F2%E8", user, password);
      var cookie = new CookieContainer();
      var urlRefer = "http://www.musicforums.ru/buysell/login.php?bn=mfor_buysell";
      var url = "http://www.musicforums.ru/buysell/login.php";

      var req = WebRequest.Create(url) as HttpWebRequest;
      req.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
      req.ContentType = "application/x-www-form-urlencoded";
      req.Accept = "text/html, application/xhtml+xml, */*";
      req.AllowAutoRedirect = true;
      req.Headers["Accept-Language"] = "en-US,en;q=0.7,ru;q=0.3";
      req.Headers["Accept-Encoding"] = "gzip, deflate";
      req.CookieContainer = cookie;
      req.Method = "POST";
      req.Referer = urlRefer;
      var ByteArr = Encoding.UTF8.GetBytes(postString);
      req.ContentLength = ByteArr.Length;
      req.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);

      if ((req.GetResponse() as HttpWebResponse).StatusCode == HttpStatusCode.OK)
        return cookie;
      else
        throw new Exception("Authentication failed");
    }
  }
}

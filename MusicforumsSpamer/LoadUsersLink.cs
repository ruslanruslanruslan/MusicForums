using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MusicforumsSpamer
{
  class LoadUsersLink
  {
    private string link;
    private int countRepeats = 0;
    private bool next = true;
    public string Link
    {
      get { return link; }
      set { link = value; }
    }
    private HtmlDocument doc;
    private string content;
    public CookieContainer Cookie { get; set; }
    private HashSet<string> linksOnUser;

    private IList<string> numbersPages = new List<string>();
    private IList<string> newLinks = new List<string>();

    public IList<string> NewLinks
    {
      get { return newLinks; }
      set { newLinks = value; }
    }

    public HashSet<string> LinksOnUser
    {
      get { return linksOnUser; }
    }

    public LoadUsersLink(string link)
    {
      this.link = link;
      LoadContent(link);
      linksOnUser = new HashSet<string>(new UserUniqueComparer());
    }
    public LoadUsersLink(IEnumerable<string> links)
    {
      linksOnUser = new HashSet<string>(links, new UserUniqueComparer());
    }
    public LoadUsersLink(string link, IEnumerable<string> links)
    {
      this.link = link;
      LoadContent(link);
      linksOnUser = new HashSet<string>(links, new UserUniqueComparer());
    }

    public void LoadContent(string link)
    {
      Thread.Sleep(500);
      var req = WebRequest.Create(link) as HttpWebRequest;
      req.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
      req.Timeout = 7000;
      req.AllowAutoRedirect = true;
      var res = req.GetResponse() as HttpWebResponse;
      var resStream = res.GetResponseStream();
      using (var st = new StreamReader(resStream))
      {
        content = st.ReadToEnd();
      }
      /*
                  using (WebClient wc = new WebClient())
                  {
                      wc.Headers["User-Agent"] = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                      content = wc.DownloadString(link);

                  }
       */
      if (string.IsNullOrEmpty(content))
        throw new WebException("Content have not Downloaded");

      doc = new HtmlDocument();
      doc.LoadHtml(content);
    }

    public void LoadLinkFromPage()
    {
      var trNode = doc.DocumentNode.SelectNodes("//div[@class='tbl-striped mrg-header blue']/table/tr[@id]");
      if (trNode == null)
        return;
      foreach (var item in trNode)
      {
        if (!next)
          break;
        var pinTopic = item.SelectSingleNode("td[@class='th-nobrd']/div[@title='Прикреплённая тема']");
        if (pinTopic == null)
        {
          var aNode = item.SelectSingleNode("td/a");
          if (aNode != null)
          {
            var href = aNode.GetAttributeValue("href", "");
            if (!href.Equals(string.Empty))
            {
              var linkAd = link + href;
              var linkOnUser = GetUserMessageLink(linkAd);
              if (!string.IsNullOrEmpty(linkOnUser))
              {
                var result = linksOnUser.Add(linkOnUser);
                if (result)
                {
                  newLinks.Add(linkOnUser);
                  countRepeats = 0;
                  using (var tw = new StreamWriter("Links.txt", true))
                  {
                    tw.WriteLine(linkOnUser);
                  }
                }
                else
                {
                  countRepeats++;
                  if (countRepeats > 20)
                    next = false;
                }
              }
            }
          }
        }
      }
    }

    public void ParserPagesLinks()
    {
      var pattern = " href=(\\d+) title='Страница";
      var pages = Regex.Matches(content, pattern);
      numbersPages.Clear();
      foreach (Match item in pages)
      {
        var number = item.Groups[1].Value;
        if (!string.IsNullOrEmpty(number))
          numbersPages.Add(number);
      }
    }

    public void LoadLinkFromAllPage()
    {
      foreach (var item in numbersPages)
      {
        if (!next)
          break;
        var linkPage = link + item;
        LoadContent(linkPage);
        LoadLinkFromPage();
      }
    }

    private string GetUserMessageLink(string adLink)
    {
      Thread.Sleep(500);

      var host = Regex.Match(adLink, "http://www.musicforums.ru/.+?/").Value;
      string contentAd;
      string linkOnUser = null;
      var req = WebRequest.Create(adLink) as HttpWebRequest;
      req.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
      req.Accept = "text/html, application/xhtml+xml, */*";
      req.CookieContainer = Cookie;
      HttpWebResponse res;
      try
      {
        res = req.GetResponse() as HttpWebResponse;
      }
      catch (WebException)
      {
        return null;
      }
      var streamResponce = res.GetResponseStream();

      using (var sr = new StreamReader(streamResponce, Encoding.GetEncoding("windows-1251")))
      {
        contentAd = sr.ReadToEnd();
      }
      var docAd = new HtmlDocument();
      docAd.LoadHtml(contentAd);
      var aNode = docAd.DocumentNode.SelectSingleNode("//a[@title='Отправить сообщение']");
      var href = aNode.GetAttributeValue("href", string.Empty);
      if (!string.IsNullOrEmpty(href))
        linkOnUser = host + href;
      return linkOnUser;
    }
  }
}

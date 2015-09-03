using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MusicforumsSpamer
{
  class Messenger
  {
    public Func<string, bool> IsNewUser { get; set; }
    private IEnumerable<string> linksUsers;
    private Message message;
    public CookieContainer Cookie { get; set; }
    private static readonly string urlSend = "http://www.musicforums.ru/agora/messages_backend.php";
    public IList<string> Users { get; set; }
    public Messenger(IEnumerable<string> linksUsers, Message message)
    {
      this.linksUsers = linksUsers;
      this.message = message;
    }

    public void ClearRepeats()
    {
      IList<string> list = new List<string>();
      foreach (var item in linksUsers)
      {
        var to = Regex.Match(item, "&to=(.+)").Groups[1].Value;
        if (!Users.Contains(to))
          list.Add(item);
      }
      linksUsers = list;
    }

    public void SendAll()
    {
      foreach (var item in linksUsers)
      {
        Send(item);
        Thread.Sleep(1000);
      }
    }

    public void Send(string link)
    {
      var to = Regex.Match(link, "&to=(.+)").Groups[1].Value;
      var bn = Regex.Match(link, "\\?bn=(.+?)&").Groups[1].Value;
      var postString = string.Format("do_post=1&actiontype=insert&bn={0}&to={1}&email=&subject={2}&icon=icon1.gif&img_url=&body={3}&save=1", bn, to, message.Title, message.Body);
      var req = WebRequest.Create(urlSend) as HttpWebRequest;
      req.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
      req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
      req.Accept = "application/json, text/javascript, */*; q=0.01";
      req.AllowAutoRedirect = true;
      req.Headers["X-Requested-With"] = "XMLHttpRequest";
      req.Headers["Accept-Language"] = "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3";
      req.Headers["Accept-Encoding"] = "gzip, deflate";
      req.CookieContainer = Cookie;
      req.Method = "POST";
      //   req.Referer = link;
      req.AutomaticDecompression = DecompressionMethods.GZip;
      var ByteArr = Encoding.UTF8.GetBytes(postString);
      req.ContentLength = ByteArr.Length;
      req.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);

      var res = req.GetResponse();
      var str = res.GetResponseStream();
      string content;
      using (var sr = new StreamReader(str))
      {
        content = sr.ReadToEnd();
      }
      var isGood = Regex.IsMatch(content, "\\{\"error\":0");
      if (!isGood)
      {
        using (var sw = new StreamWriter("errorLinks.txt", true))
        {
          sw.WriteLine(link);
        }
      }
      else
      {
        using (var sw = new StreamWriter("users.txt", true))
        {
          sw.WriteLine(to);
        }
      }
    }
  }

  struct Message
  {
    public string Title { get; set; }
    public string Body { get; set; }
  }
}

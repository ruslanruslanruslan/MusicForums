using System.Collections.Generic;
using System.IO;

namespace MusicforumsSpamer
{
  class Main
  {
    public Message Message_ { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }

    public void RewriteSectionLinks()
    {
      MFBot.LoadSectionsLinks();
    }

    public void Start()
    {
      var linksSection = File.ReadAllLines("SectionLinks.txt");
      var linkOnUser = File.ReadAllLines("Links.txt");
      var cookie = MFBot.GetCookieAuth(Login, Password);
      var loaderLink = new LoadUsersLink(linkOnUser);
      loaderLink.Cookie = cookie;

      foreach (var item in linksSection)
      {
        loaderLink.Link = item;
        loaderLink.LoadContent(item);

        loaderLink.LoadLinkFromPage();
        loaderLink.ParserPagesLinks();
        loaderLink.LoadLinkFromAllPage();
      }
      var newLinks = loaderLink.NewLinks;
      var allLinks = loaderLink.LinksOnUser;
      if (allLinks.Count > 0)
      {
        var ms = new Messenger(allLinks, Message_);
        ms.Users = new List<string>(File.ReadAllLines("users.txt"));
        ms.Cookie = cookie;
        ms.ClearRepeats();
        ms.SendAll();
      }
    }
  }
}

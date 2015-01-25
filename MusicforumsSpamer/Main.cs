using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            string[] linksSection = File.ReadAllLines("SectionLinks.txt");
            string[] linkOnUser = File.ReadAllLines("Links.txt");
            var cookie = MFBot.GetCookieAuth(Login, Password);
            LoadUsersLink loaderLink = new LoadUsersLink(linkOnUser);
            loaderLink.Cookie=cookie;

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
                Messenger ms = new Messenger(allLinks, Message_);
                ms.Users = new List<string>(File.ReadAllLines("users.txt"));
                ms.Cookie = cookie;
                ms.ClearRepeats();
                ms.SendAll();
            }
        }
    }
}

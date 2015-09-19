using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

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
      string[] linksSection = null;
      try
      {
        Logger.LogMessage("Reading SectionLinks.txt...");
        linksSection = File.ReadAllLines("SectionLinks.txt");
      }
      catch (Exception ex)
      {
        Logger.LogFailed("Reading SectionLinks.txt... FAILED: " + ex.Message);
        return;
      }
      if (linksSection == null)
      {
        Logger.LogFailed("Reading SectionLinks.txt... FAILED");
        return;
      }
      Logger.LogSuccess("Reading SectionLinks.txt... SUCCESS");

      string[] linkOnUser = null;
      try
      {
        Logger.LogMessage("Reading Links.txt...");
        linkOnUser = File.ReadAllLines("Links.txt");
      }
      catch (Exception ex)
      {
        Logger.LogFailed("Reading Links.txt... FAILED" + ex.Message);
        return;
      }
      if (linkOnUser == null)
      {
        Logger.LogFailed("Reading Links.txt... FAILED");
        return;
      }
      Logger.LogSuccess("Reading Links.txt... SUCCESS");

      CookieContainer cookie = null;
      try
      {
        Logger.LogMessage("Authentication...");
        cookie = MFBot.GetCookieAuth(Login, Password);
      }
      catch (Exception ex)
      {
        Logger.LogFailed("Authentication... FAILED: " + ex.Message);
        return;
      }
      if (cookie == null)
      {
        Logger.LogFailed("Authentication... FAILED");
        return;
      }
      Logger.LogSuccess("Authentication... SUCCESS");

      LoadUsersLink loaderLink = null;
      try
      {
        Logger.LogMessage("Load user links...");
        loaderLink = new LoadUsersLink(linkOnUser);
      }
      catch (Exception ex)
      {
        Logger.LogFailed("Load user links... FAILED: " + ex.Message);
        return;
      }
      if (loaderLink == null)
      {
        Logger.LogFailed("Load user links... FAILED");
        return;
      }
      Logger.LogSuccess("Load user links... SUCCESS");

      loaderLink.Cookie = cookie;

      foreach (var item in linksSection)
      {
        Logger.LogMessage("Loading " + item + "...");
        try
        {
          loaderLink.Link = item;
          loaderLink.LoadContent(item);

          Logger.LogMessage("Parsed " + loaderLink.LoadLinkFromPage() + " new items");
          loaderLink.ParserPagesLinks();
          loaderLink.LoadLinkFromAllPage();
        }
        catch(Exception ex)
        {
          Logger.LogFailed("Loading " + item + "... FAILED: " + ex.Message);
          continue;
        }
        Logger.LogSuccess("Loading " + item + "... SUCCESS");
      }
      var allLinks = loaderLink.LinksOnUser;
      if (allLinks != null && allLinks.Count > 0)
      {
        var ms = new Messenger(allLinks, Message_);
        if (ms == null)
        {
          Logger.LogFailed("Can't create Messenger");
          return;
        }
        try
        {
          Logger.LogMessage("Loading users.txt...");
          ms.Users = new List<string>(File.ReadAllLines("users.txt"));
        }
        catch (Exception ex)
        {
          Logger.LogFailed("Loading users.txt... FAILED: " + ex.Message);
          return;
        }
        Logger.LogSuccess("Loading users.txt... SUCCESS");
        ms.Cookie = cookie;
        ms.ClearRepeats();
        ms.SendAll();
      }
    }
  }
}

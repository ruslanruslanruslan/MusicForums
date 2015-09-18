using System;
using System.IO;
using System.Text;

namespace MusicforumsSpamer
{
  public class UserInfo2
  {

    public string Login { get; set; }
    public string Password { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
  }

  class UserInfoReader2
  {
    private static UserInfo2 userInfo;
    public static UserInfo2 Info
    {
      get
      {
        try
        {
          Logger.LogMessage("Reading user info...");
          if (userInfo == null)
            userInfo = ReadUserInfo("info.txt");
        }
        catch(Exception ex)
        {
          Logger.LogFailed("Reading user info... FAILED: " + ex.Message);
          return null;
        }
        Logger.LogSuccess("Reading user info... SUCCESS");
        return userInfo;
      }
    }
    public static UserInfo2 ReadUserInfo(string path)
    {
      string text;
      try
      {
        using (var reader = new StreamReader(path, Encoding.UTF8))
        {
          text = reader.ReadToEnd();
        }
      }
      catch(Exception ex)
      {
        throw new Exception("Can't read file " + path, ex);
      }
      var user = new UserInfo2();

      user.Login = GetValueForName(text, "login");
      user.Password = GetValueForName(text, "password");
      user.Title = GetValueForName(text, "title");
      user.Body = GetValueForName(text, "body");
      return user;
    }

    private static string GetValueForName(string text, string nameField)
    {
      return InfoPage.GetDatafromText(text, "" + nameField + "=\"(.+?)\"", 1);
    }
  }
}

namespace MusicforumsSpamer
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length > 0)
        if (args[0].Equals("clear"))
          MFBot.LoadSectionsLinks();
      var user = UserInfoReader2.Info;
      var m = new Main()
      {
        Login = user.Login,
        Password = user.Password,
        Message_ = new Message { Body = user.Body, Title = user.Title }
      };
      m.Start();
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicforumsSpamer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0) 
            {
                if (args[0].Equals("clear")) 
                {
                    MFBot.LoadSectionsLinks();
                }
            }
            var user = UserInfoReader2.Info;
            Main m = new Main() { Login = user.Login, 
                Password = user.Password,
                Message_ = new Message { Body = user.Body, Title = user.Title } };
            m.Start();

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MusicforumsSpamer
{
    class UserUniqueComparer:IEqualityComparer<string>
    {

        public bool Equals(string x, string y)
        {
            string pattern="&to=(.+)";
            string nickX = Regex.Match(x, pattern).Groups[1].Value;
            string nickY = Regex.Match(y, pattern).Groups[1].Value;
            return nickX.Equals(nickY);
        }

        public int GetHashCode(string obj)
        {
            string pattern = "&to=(.+)";
            string nickX = Regex.Match(obj, pattern).Groups[1].Value;
            return nickX.Length;
        }
    }
}

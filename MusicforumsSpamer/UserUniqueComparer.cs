using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MusicforumsSpamer
{
  class UserUniqueComparer : IEqualityComparer<string>
  {
    public bool Equals(string x, string y)
    {
      var pattern = "&to=(.+)";
      var nickX = Regex.Match(x, pattern).Groups[1].Value;
      var nickY = Regex.Match(y, pattern).Groups[1].Value;
      return nickX.Equals(nickY);
    }

    public int GetHashCode(string obj)
    {
      return Regex.Match(obj, "&to=(.+)").Groups[1].Value.Length;
    }
  }
}

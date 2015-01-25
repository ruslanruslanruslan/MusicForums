using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;


public class InfoPage
 {

      public static string GetDatafromText(string text, string pattern)
        {
               Match mc = Regex.Match(text, pattern);
               return mc.Value;
        }

        public static string GetDatafromText(string text, string pattern, int fGroup)
        {
                Match mc = Regex.Match(text, pattern);
                string textout = mc.Groups[fGroup].Value;
                return textout;
        }
       
        public static string Replace(string text,string pattern,string newValue) 
        {
            return Regex.Replace(text, pattern, newValue);
        }
        public static void WriteSerObjectToFile(string path, Object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (var fStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fStream, obj);
            }

        }
        public static object ReadSerObjectToFile(string path) 
        {
            object obj;
            BinaryFormatter formatter = new BinaryFormatter();
            using (var fStream = File.OpenRead(path))
            {
                 obj = formatter.Deserialize(fStream);
            }
            return obj;
        }

        public static string GetPage(string url)
        {
            using (var wc = new System.Net.WebClient())
            {
                wc.Encoding = Encoding.Default;//UTF8Encoding.UTF8;

                return wc.DownloadString(url); ;
            }
          
        } 
    
}


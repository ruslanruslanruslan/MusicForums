using System.IO;
using System;

namespace MusicforumsSpamer
{
  public static class Logger
  {
    public static void LogMessage(string message, ConsoleColor color, string filename, bool bFile, bool bConsole)
    {
      if (bFile)
        LogMessageToFile(message, filename);
      if (bConsole)
        LogMessageToConsole(message, color);
    }

    public static void LogMessageToFile(string message, string filename)
    {
      using (var file = new StreamWriter(filename, true))
      {
        file.WriteLine(message);
      }
    }

    public static void LogMessageToConsole(string message, ConsoleColor color)
    {
      Console.ForegroundColor = color;
      Console.WriteLine(message);
      Console.ResetColor();
    }

    public static void LogMessage(string message, ConsoleColor color, string filename)
    {
      LogMessage(message, color, filename, true, true);
    }

    public static void LogMessage(string message, ConsoleColor color)
    {
      LogMessage(message, color, AppDomain.CurrentDomain.FriendlyName + ".log");
    }

    public static void LogMessage(string message)
    {
      LogMessage(message, Console.ForegroundColor);
    }

    public static void LogFailed(string message)
    {
      LogMessage(message, ConsoleColor.Red);
    }

    public static void LogSuccess(string message)
    {
      LogMessage(message, ConsoleColor.Green);
    }
  }
}

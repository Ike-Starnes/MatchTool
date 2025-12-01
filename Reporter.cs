using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchTool
{
   internal class Reporter
   {
      private string _reportFile = string.Empty;
      public string ReportFile { get => _reportFile; }

      public Reporter()
      {
      }

      public Reporter(string reportFile)
      {
         _reportFile = reportFile;
         if (File.Exists(_reportFile)) { File.Delete(_reportFile); }
      }

      public void Report(string message)
      {
         Console.WriteLine(message);
         if (!String.IsNullOrEmpty(_reportFile))
         {
            File.AppendAllText(_reportFile, message);
            File.AppendAllText(_reportFile, Environment.NewLine);
         }
      }

      public void Display()
      {
         if (File.Exists(_reportFile)) { Process.Start("notepad.exe", _reportFile); }
      }
   }
}

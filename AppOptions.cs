using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchTool
{
   internal class AppOptions
   {
      private string _dataFolder = string.Empty;
      private int _topPercentToPay;
      private double _prizeMoney;
      private int _surrenderPercent;
      private string _reportFile = string.Empty;

      [Option('d', "datafolder", Required = true, HelpText = "Data folder.")]
      public string DataFolder { get => _dataFolder; set => _dataFolder = value; }

      [Option('t', "toppercenttopay", Default = 10, Required = false, HelpText = "Top percent to pay.")]
      public int TopPercentToPay { get => _topPercentToPay; set => _topPercentToPay = value; }

      [Option('p', "prizemoney", Default = 10000.0, Required = true, HelpText = "Total prize money.")]
      public double PrizeMoney { get => _prizeMoney; set => _prizeMoney = value; }

      [Option('s', "surrenderpercent", Default = 66, Required = false, HelpText = "Division / Class surrender percentage.")]
      public int SurrenderPercent { get => _surrenderPercent; set => _surrenderPercent = value; }

      [Option('r', "reportfile", Default = "", Required = false, HelpText = "Report file.")]
      public string ReportFile { get => _reportFile; set => _reportFile = value; }
   }
}

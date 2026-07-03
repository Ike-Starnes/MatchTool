using CommandLine;

namespace MatchTool
{
   internal class AppOptions
   {
      private string _dataFolder = string.Empty;
      private int _topPercentToPay;
      private double _prizeMoney;
      private int _surrenderPercent;
      private string _reportFile = string.Empty;
      private int _minimumShooters;
      private bool _hoaOnly;
      private bool _htmlOutput;

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

      [Option('m', "minshooters", Default = 5, Required = false, HelpText = "Minimum shooters required for payout.")]
      public int MinimumShooters { get => _minimumShooters; set => _minimumShooters = value; }

      [Option('h', "hoaonly", Default = false, Required = false, HelpText = "Only pay HOA for each division.")]
      public bool HOAOnly { get => _hoaOnly; set => _hoaOnly = value; }

      [Option('l', "htmloutput", Default = true, Required = false, HelpText = "Output as HTML page.")]
      public bool HTMLOutput { get => _htmlOutput; set => _htmlOutput = value; }
   }
}

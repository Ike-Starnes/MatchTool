namespace MatchTool
{
   internal class MatchOptions
   {
      private string _dataFolder = "";
      private int _topPercentToPay;
      private double _prizeMoney;
      private int _surrenderPercent;
      private int _minimumShooters = 5;
      private bool _hoaOnly = false;
      private bool _htmlOutput = false;

      public string DataFolder { get => _dataFolder; set => _dataFolder = value; }

      public int TopPercentToPay { get => _topPercentToPay; set => _topPercentToPay = value; }

      public double PrizeMoney { get => _prizeMoney; set => _prizeMoney = value; }

      public int SurrenderPercent { get => _surrenderPercent; set => _surrenderPercent = value; }

      public int MinimumShooters { get => _minimumShooters; set => _minimumShooters = value; }

      public bool HOAOnly { get => _hoaOnly; set => _hoaOnly = value; }

      public bool HTMLOutput { get => _htmlOutput; set => _htmlOutput = value; }

      public MatchOptions(AppOptions options)
      {
         _dataFolder = options.DataFolder;
         _topPercentToPay = options.TopPercentToPay;
         _prizeMoney = options.PrizeMoney;
         _surrenderPercent = options.SurrenderPercent;
         _minimumShooters = options.MinimumShooters;
         _hoaOnly = options.HOAOnly;
         _htmlOutput = options.HTMLOutput;
      }
   }
}

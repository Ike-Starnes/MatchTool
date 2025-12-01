using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchTool
{
   internal class MatchOptions
   {
      private string _dataFolder = "";
      private int _topPercentToPay;
      private double _prizeMoney;
      private int _surrenderPercent;

      public string DataFolder { get => _dataFolder; set => _dataFolder = value; }

      public int TopPercentToPay { get => _topPercentToPay; set => _topPercentToPay = value; }

      public double PrizeMoney { get => _prizeMoney; set => _prizeMoney = value; }

      public int SurrenderPercent { get => _surrenderPercent; set => _surrenderPercent = value; }

      public MatchOptions(AppOptions options)
      {
         _dataFolder = options.DataFolder;
         _topPercentToPay = options.TopPercentToPay;
         _prizeMoney = options.PrizeMoney;
         _surrenderPercent = options.SurrenderPercent;
      }

      public MatchOptions(string dataFolder, int topPercentToPay, double prizeMoney, int surrenderPercent)
      {
         _dataFolder = dataFolder;
         _topPercentToPay = topPercentToPay;
         _prizeMoney = prizeMoney;
         _surrenderPercent = surrenderPercent;
      }
   }
}

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MatchTool
{
   public enum Divisions
   {
      CarryOptics,
      LimitedOptics,
      Open,
      Limited,
      SingleStack,
      PCC,
      Production,
      Revolover,
   };

   public enum Classifications
   {
      GM,
      M,
      A,
      B,
      C,
      D,
      U,
   };

   internal class MatchInfo
   {
      private string _dataFolder;
      public string DataFolder { get => _dataFolder; }

      private string _name;
      public string Name { get => _name; }

      private string _date;
      public string Date { get => _date; }

      private string _club;
      public string Club { get => _club; }

      public List<MatchResults> Results { get => _results; }

      private List<MatchResults> _results;

      private double _prizeMoney;
      public double PrizeMoney { get => _prizeMoney; }

      private double _surrenderPercent;
      public double SurrenderPercent { get => _surrenderPercent; }

      private double _topPercentToPay;
      public double TopPercentToPay { get => _topPercentToPay; }


      private int _totalShooters;
      public int TotalShooters { get => _totalShooters; set => _totalShooters = value; }

      public MatchInfo(string dataFolder, double prizeMoney, double surrenderPercent, double topPercentToPay)
      {
         _name = _date = _club = String.Empty;

         if (!Directory.Exists(dataFolder)) throw new DirectoryNotFoundException(dataFolder);
         _dataFolder = dataFolder;

         _prizeMoney = prizeMoney;
         _surrenderPercent = surrenderPercent;
         _topPercentToPay = topPercentToPay;

         string matchFile = Path.Combine(DataFolder, $"Main.html");
         if (!File.Exists(matchFile)) throw new FileNotFoundException(matchFile);

         HtmlDocument doc = new HtmlDocument();
         string html = File.ReadAllText(matchFile);
         doc.LoadHtml(html);

         HtmlNode tableNode = doc.DocumentNode.SelectNodes("//table")[0];
         HtmlNodeCollection tableRows = tableNode.SelectNodes(".//tr");
         foreach (HtmlNode tableRow in tableRows)
         {
            HtmlNodeCollection tableColumns = tableRow.SelectNodes(".//td");
            if (tableColumns[0].InnerText.Contains("Match Name")) _name = tableColumns[1].InnerText;
            if (tableColumns[0].InnerText.Contains("Match Date")) _date = tableColumns[1].InnerText;
            if (tableColumns[0].InnerText.Contains("Club ID")) _club = tableColumns[1].InnerText;
         }

         _totalShooters = 0;

         _results = new List<MatchResults>();
         GetResults();
      }

      private void GetResults()
      {
         _results.Clear();

         foreach (Divisions division in Enum.GetValues(typeof(Divisions)))
         {
            try
            {
               MatchResults matchResults = GetResults(division);
               _results.Add(matchResults);
               _totalShooters += matchResults.TotalShooters;
            }
            catch (FileNotFoundException /*e*/) { System.Diagnostics.Debug.WriteLine($"No results file for division: {division}"); }
         }
      }
      private MatchResults GetResults(Divisions division)
      {
         string resultsFile = Path.Combine(DataFolder, $"{division}.html");
         if (!File.Exists(resultsFile))
         {
            string resultsFile2 = resultsFile.Replace(" ", "");
            if (!File.Exists(resultsFile2)) throw new FileNotFoundException(resultsFile);
            resultsFile = resultsFile2;
         }

         HtmlDocument doc = new HtmlDocument();
         string html = File.ReadAllText(resultsFile);
         doc.LoadHtml(html);

         HtmlNode resultsTable = doc.DocumentNode.SelectNodes("//table")[0];
         MatchResults matchResults = new MatchResults(division, resultsTable);

         return matchResults;
      }
   }
}

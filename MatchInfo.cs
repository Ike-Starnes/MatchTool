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
      Revolver,
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
      HOA,
   };

   internal class MatchInfo
   {
      private MatchOptions _options;
      internal MatchOptions Options { get => _options; set => _options = value; }


      private string _name;
      public string Name { get => _name; }

      public List<MatchResults> Results { get => _results; }

      private List<MatchResults> _results;


      private int _totalShooters;
      public int TotalShooters { get => _totalShooters; set => _totalShooters = value; }

      public MatchInfo(MatchOptions options)
      {
         if(options == null) throw new ArgumentNullException(nameof(options));

         _name = String.Empty;
         _options = options;
         _results = new List<MatchResults>();
      }

      private void GetMatchInfo(String matchFile)
      {
         HtmlDocument doc = new HtmlDocument();
         string html = File.ReadAllText(matchFile);
         doc.LoadHtml(html);

         HtmlNode matchNode = doc.DocumentNode.SelectSingleNode("//h3");
         _name = matchNode.InnerText.Trim();
      }

      public void GetResults()
      {
         // To get the files, go to Practiscore.com, find the match results, then scroll down to "Old style results"
         // From that page, save as HTML all the overall results pages "Combined", "Carry Optics", ...
         // Ex:
         // https://practiscore.com/results/html/e8bcf627-e274-4a86-b67a-860f94430c49?page=overall-combined
         // https://practiscore.com/results/html/e8bcf627-e274-4a86-b67a-860f94430c49?page=overall-carryoptics
         // https://practiscore.com/results/html/e8bcf627-e274-4a86-b67a-860f94430c49?page=overall-limited
         // ...

         _results.Clear();
         _totalShooters = 0;

         if (!Directory.Exists(_options.DataFolder)) throw new DirectoryNotFoundException(_options.DataFolder);

         string matchFile = Path.Combine(_options.DataFolder, $"Main.html");
         if (!File.Exists(matchFile)) throw new FileNotFoundException(matchFile);

         GetMatchInfo(matchFile);

         GetAllMatchResults();
      }

      private void GetAllMatchResults()
      {
         foreach (Divisions division in Enum.GetValues(typeof(Divisions)))
         {
            try
            {
               MatchResults matchResults = GetDivisionResults(division);
               if (matchResults.TotalShooters >= _options.MinimumShooters)
                  _results.Add(matchResults);
               else
                  Console.WriteLine($"Excluding {division} from results.");

               _totalShooters += matchResults.TotalShooters;
            }
            catch (FileNotFoundException /*e*/) { System.Diagnostics.Debug.WriteLine($"No results file for division: {division}"); }
         }
      }

      private MatchResults GetDivisionResults(Divisions division)
      {
         string resultsFile = Path.Combine(Options.DataFolder, $"{division}.html");
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

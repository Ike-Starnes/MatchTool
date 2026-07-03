using System.Diagnostics;

namespace MatchTool
{
   internal class MatchReporter
   {
      private string _reportFile = string.Empty;
      public string ReportFile { get => _reportFile; }

      private MatchInfo _matchInfo;

      public MatchReporter(MatchInfo matchInfo, string reportFile)
      {
         _matchInfo = matchInfo;
         _reportFile = reportFile;
         if (File.Exists(_reportFile)) { File.Delete(_reportFile); }
      }

      private void Report(string message)
      {
         Console.WriteLine(message);
         if (!String.IsNullOrEmpty(_reportFile))
         {
            File.AppendAllText(_reportFile, message);
            File.AppendAllText(_reportFile, Environment.NewLine);
         }
      }

      private void ReportThickHorizontalRule()
      {
         Report($"==================================================================================================================");
      }

      private void ReportThinHorizontalRule()
      {
         Report($"------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
      }

      private void ReportMainHeader()
      {
         ReportThickHorizontalRule();
         Report($"Match Name:\t\t{_matchInfo.Name}");
         Report($"Total Shooters:\t\t{_matchInfo.TotalShooters}");
         Report($"Prize Money:\t\t${_matchInfo.Options.PrizeMoney:F2}");
         Report($"Surrender %:\t\t{_matchInfo.Options.SurrenderPercent}%");
         Report($"Top % to pay:\t\t{_matchInfo.Options.TopPercentToPay}%");
         Report($"Required min shooters:\t{_matchInfo.Options.MinimumShooters}");
         ReportThickHorizontalRule();
      }

      public void ReportResults()
      {
         ReportMainHeader();

         Report(Environment.NewLine);

         // Report results for each division (and class within each division)
         foreach (MatchResults results in _matchInfo.Results)
         {
            ReportDivisionResults(results);
         }
      }


      private void ReportDivisionHeader(MatchResults results)
      {
         // Division header
         string report = "Division".PadRight(16);
         report += "Prize Pool".PadRight(16);
         foreach (Classifications classification in Enum.GetValues(typeof(Classifications)))
            report += classification.ToString().PadRight(16);
         report += "Totals".PadRight(16);
         Report(report);
         ReportThinHorizontalRule();
      }

      private PrizePool ReportDivisionPrizePool(MatchResults results)
      {
         // Get the prize pool for this division (and classes within)
         double percentage = (double)results.TotalShooters / (double)_matchInfo.TotalShooters;
         double poolMoney = _matchInfo.Options.PrizeMoney * percentage;
         PrizePool prizePool = new PrizePool(poolMoney, results, _matchInfo.Options);
         Report(prizePool.ToString());
         ReportThinHorizontalRule();
         return prizePool;
      }

      private void ReportClassHeader(DivClassPool divClassPool, List<double> payouts)
      {
         string classification = divClassPool.Classification.ToString();
         string displayClassification = classification;
         if (classification == "GM") displayClassification = "HOA";
         Report($"{divClassPool.Division} {displayClassification} (Paying {payouts.Count} of {divClassPool.Count})");
      }

      private void ReportWinners(List<string> winners, List<double> payouts)
      {
         for (int i = 0; i < payouts.Count; i++)
         {
            Report($"{i + 1}\t{winners[i],-30}\t${payouts[i]:F2}");
         }
         Report(Environment.NewLine);
      }

      private void ReportClassResults(DivClassPool divClassPool)
      {
         List<double> payouts = divClassPool.GenerateSmoothPayouts(divClassPool.Count, _matchInfo.Options.TopPercentToPay);
         List<string> winners = divClassPool.Results.GetWinners(divClassPool.Classification, divClassPool.Count);

         if (divClassPool.PrizeMoney > 0.0)
         {
            ReportClassHeader(divClassPool, payouts);
            ReportWinners(winners, payouts);
         }
      }

      private void ReportDivisionResults(MatchResults results)
      {
         ReportDivisionHeader(results);
         PrizePool prizePool = ReportDivisionPrizePool(results);

         // Report results for each class within this division
         foreach (DivClassPool divClassPool in prizePool.Pools)
         {
            ReportClassResults(divClassPool);
         }
      }

      public void DisplayResults()
      {
         if (File.Exists(_reportFile)) { Process.Start("notepad.exe", _reportFile); }
      }
   }
}

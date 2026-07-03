using System.Diagnostics;
using System.Text;

namespace MatchTool
{
   public enum ReportOutputMode
   {
      Text,
      Html
   }


   internal class MatchReporter
   {
      private readonly ReportOutputMode _mode;
      private readonly StringBuilder _html = new StringBuilder();

      private string _reportFile = string.Empty;
      public string ReportFile { get => _reportFile; }

      private MatchInfo _matchInfo;

      public MatchReporter(MatchInfo matchInfo, AppOptions appOptions)
      {
         _matchInfo = matchInfo;
         _mode = appOptions.HTMLOutput ? ReportOutputMode.Html : ReportOutputMode.Text;
         _reportFile = appOptions.ReportFile;
         if (_mode == ReportOutputMode.Html) { _reportFile = _reportFile.ToLower().Replace(".txt", ".html"); }

         if (File.Exists(_reportFile)) { File.Delete(_reportFile); }

         if (_mode == ReportOutputMode.Html)
         {
            _html.AppendLine("<html><head><style>");
            _html.AppendLine("body { font-family: Consolas, monospace; }");
            _html.AppendLine("table { border-collapse: collapse; width: 100%; }");
            _html.AppendLine("th, td { border: 1px solid #444; padding: 6px; }");
            _html.AppendLine("h2 { margin-top: 40px; }");
            _html.AppendLine("</style></head><body>");
         }
      }

      private void Report(string message)
      {
         Console.WriteLine(message);

         if (_mode == ReportOutputMode.Text)
         {
            File.AppendAllText(_reportFile, message + Environment.NewLine);
         }
         else
         {
            _html.AppendLine($"<div>{System.Net.WebUtility.HtmlEncode(message)}</div>");
         }
      }

      private void ReportThickHorizontalRule()
      {
         if (_mode == ReportOutputMode.Html)
            _html.AppendLine("<hr style='border: 3px solid black;'>");
         else
            Report("==================================================================================================================");
      }

      private void ReportThinHorizontalRule()
      {
         if (_mode == ReportOutputMode.Html)
            _html.AppendLine("<hr style='border: 1px solid #999;'>");
         else
            Report("------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
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

         if (_mode == ReportOutputMode.Html)
         {
            _html.AppendLine("</body></html>");
            File.WriteAllText(_reportFile, _html.ToString());
         }
      }

      private void ReportDivisionHeader(MatchResults results)
      {
         if (_mode == ReportOutputMode.Html)
         {
            _html.AppendLine("<h2>Division Results</h2>");
            _html.AppendLine("<table><tr>");
            _html.AppendLine("<th>Division</th><th>Prize Pool</th>");

            if (_matchInfo.Options.HOAOnly)
            {
               _html.AppendLine($"<th>{Classifications.HOA.ToString()}</th>");
            }
            else
            {
               foreach (Classifications c in Enum.GetValues(typeof(Classifications)))
                  _html.AppendLine($"<th>{c}</th>");
            }

            _html.AppendLine("<th>Totals</th></tr>");
         }
         else
         {
            string report = "Division".PadRight(16);
            report += "Prize Pool".PadRight(16);
            foreach (Classifications classification in Enum.GetValues(typeof(Classifications)))
               report += classification.ToString().PadRight(16);
            report += "Totals".PadRight(16);
            Report(report);
            ReportThinHorizontalRule();
         }
      }

      private PrizePool ReportDivisionPrizePool(MatchResults results)
      {
         double percentage = (double)results.TotalShooters / (double)_matchInfo.TotalShooters;
         double poolMoney = _matchInfo.Options.PrizeMoney * percentage;
         PrizePool prizePool = new PrizePool(poolMoney, results, _matchInfo.Options);

         if (_mode == ReportOutputMode.Html)
         {
            _html.AppendLine("<tr>");
            _html.AppendLine($"<td>{results.Division}</td>");
            _html.AppendLine($"<td>${poolMoney:F2}</td>");

            foreach (var p in prizePool.Pools)
               _html.AppendLine($"<td>${p.PrizeMoney:F2}</td>");

            _html.AppendLine($"<td>{results.TotalShooters}</td>");
            _html.AppendLine("</tr>");
         }
         else
         {
            Report(prizePool.ToString());
            ReportThinHorizontalRule();
         }

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
         if (_mode == ReportOutputMode.Html)
         {
            _html.AppendLine("<table>");
            _html.AppendLine("<tr><th>Place</th><th>Name</th><th>Payout</th></tr>");

            for (int i = 0; i < payouts.Count; i++)
            {
               _html.AppendLine(
                   $"<tr><td>{i + 1}</td><td>{System.Net.WebUtility.HtmlEncode(winners[i])}</td><td>${payouts[i]:F2}</td></tr>");
            }

            _html.AppendLine("</table><br>");
         }
         else
         {
            for (int i = 0; i < payouts.Count; i++)
               Report($"{i + 1}\t{winners[i],-30}\t${payouts[i]:F2}");

            Report(Environment.NewLine);
         }
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
         if (File.Exists(_reportFile))
         {
            if (_mode == ReportOutputMode.Html)
               Process.Start(new ProcessStartInfo(_reportFile) { UseShellExecute = true });
            else
               Process.Start("notepad.exe", _reportFile);
         }
      }
   }
}

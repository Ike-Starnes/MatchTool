using CommandLine;
using HtmlAgilityPack;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

// Input files must be saved from Practiscore HTML results pages such as:
// https://practiscore.com/results/html/e8bcf627-e274-4a86-b67a-860f94430c49?page=overall-carryoptics
// https://practiscore.com/results/html/e8bcf627-e274-4a86-b67a-860f94430c49?page=overall-pcc

// Example command line: 
// MatchTool.exe -d C:\Temp\CarolinaClassic -p 9555.00 -s 75 -r C:\Temp\CarolinaClassic\report.txt

namespace MatchTool
{
   internal class Program
   {
      static void RunOptions(AppOptions opts)
      {
         if (opts.HOAOnly)
            opts.SurrenderPercent = 0;
      }

      static void HandleParseError(IEnumerable<CommandLine.Error> errs)
      {
         Usage();
      }

      private static void Usage()
      {
         //Console.WriteLine("For command line help:");
         //Console.WriteLine("   MatchTool.exe --help");
      }

      static int Main(string[] args)
      {
         // Parse command line
         ParserResult<AppOptions> parseResult = Parser.Default.ParseArguments<AppOptions>(args)
           .WithParsed(RunOptions)
           .WithNotParsed(HandleParseError);

         // Get app options from command line
         if (parseResult.Errors.Any()) { return 1; }
         AppOptions appOptions = parseResult.Value;

         // Construct match options class from app options
         MatchOptions matchOptions = new MatchOptions(appOptions);

         // Get match info
         MatchInfo matchInfo = new MatchInfo(matchOptions);

         // Get match results
         matchInfo.GetResults();

         // Report the match prize results
         MatchReporter reporter = new MatchReporter(appOptions.ReportFile);

         reporter.Report($"==================================================================================================================");
         reporter.Report($"Match Name:\t\t{matchInfo.Name}");
         reporter.Report($"Total Shooters:\t\t{matchInfo.TotalShooters}");
         reporter.Report($"Prize Money:\t\t${matchInfo.Options.PrizeMoney:F2}");
         reporter.Report($"Surrender %:\t\t{matchInfo.Options.SurrenderPercent}%");
         reporter.Report($"Top % to pay:\t\t{matchInfo.Options.TopPercentToPay}%");
         reporter.Report($"Required min shooters:\t{matchInfo.Options.MinimumShooters}");
         reporter.Report($"==================================================================================================================");

         reporter.Report(Environment.NewLine);

         foreach (MatchResults results in matchInfo.Results)
         {
            string report = "Division".PadRight(16);
            report += "Prize Pool".PadRight(16);
            foreach (Classifications classification in Enum.GetValues(typeof(Classifications)))
               report += classification.ToString().PadRight(16);
            report += "Totals".PadRight(16);
            //report += Environment.NewLine;
            reporter.Report(report);

            reporter.Report($"------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

            double percentage = (double)results.TotalShooters / (double)matchInfo.TotalShooters;
            double poolMoney = matchInfo.Options.PrizeMoney * percentage;

            PrizePool prizePool = new PrizePool(poolMoney, results, matchOptions);
            reporter.Report(prizePool.ToString());

            reporter.Report($"************************************************************************************************************************************************************************");

            foreach (DivClassPool divClassPool in prizePool.Pools)
            {
               //List<double> payouts = divClassPool.CalculatePayouts(matchInfo.Options.TopPercentToPay);
               List<double> payouts = divClassPool.GenerateSmoothPayouts(divClassPool.Count, matchInfo.Options.TopPercentToPay);
               List<string> winners = results.GetWinners(divClassPool.Classification, divClassPool.Count);

               if (divClassPool.PrizeMoney > 0.0)
               {
                  string classification = divClassPool.Classification.ToString();

                  string displayClassification = classification;
                  if (classification == "GM") displayClassification = "HOA";
                  reporter.Report($"{divClassPool.Division} {displayClassification} (Paying {payouts.Count} of {divClassPool.Count})");
                  for (int i = 0; i < payouts.Count; i++)
                  {
                     reporter.Report($"{i+1}\t{winners[i],-30}\t${payouts[i]:F2}");
                  }
                  reporter.Report(string.Empty);
               }
            }
         }
         reporter.Display();
         return 0;
      }
   }
}

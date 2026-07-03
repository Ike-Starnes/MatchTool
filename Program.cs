using CommandLine;

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

         // Calculate and report the match prize results
         MatchReporter reporter = new MatchReporter(matchInfo, appOptions);
         reporter.ReportResults();

         // Display the results file
         reporter.DisplayResults();

         return 0;
      }
   }
}

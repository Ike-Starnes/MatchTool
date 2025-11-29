using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

// Input files must be saved from Practiscore HTML results pages such as:
// https://practiscore.com/results/html/e8bcf627-e274-4a86-b67a-860f94430c49?page=overall-carryoptics
// https://practiscore.com/results/html/e8bcf627-e274-4a86-b67a-860f94430c49?page=overall-pcc

namespace MatchTool
{
   internal class Program
   {
      static void Main(string[] args)
      {
         double topPercentToPay = 0.1;

         double prizeMoney = double.Parse(args[1].Replace("$", ""));
         double surrenderPercent = double.Parse(args[2].Replace("%", "")) / 100.0;
         MatchInfo matchInfo = new MatchInfo(args[0], prizeMoney, surrenderPercent, topPercentToPay);

         Console.WriteLine($"=========================================================");
         Console.WriteLine($"Match Name:\t{matchInfo.Name}");
         Console.WriteLine($"Match Date:\t{matchInfo.Date}");
         Console.WriteLine($"Club ID:\t{matchInfo.Club}");
         Console.WriteLine($"Total Shooters:\t{matchInfo.TotalShooters}");
         Console.WriteLine($"Prize Money:\t${matchInfo.PrizeMoney:F2}");
         Console.WriteLine($"Surrender %:\t{matchInfo.SurrenderPercent * 100}%");
         Console.WriteLine($"Top % to pay:\t{matchInfo.TopPercentToPay * 100}%");
         Console.WriteLine($"=========================================================");

         Console.WriteLine(Environment.NewLine);

         Console.Write("Division".PadRight(16));
         Console.Write("Prize Pool".PadRight(16));
         foreach (Classifications classification in Enum.GetValues(typeof(Classifications)))
            Console.Write(classification.ToString().PadRight(16));
         Console.Write("Totals".PadRight(16));
         Console.Write(Environment.NewLine);

         Console.WriteLine($"---------------------------------------------------------------------------------------------------------------------------------------------------------------------");

         foreach (MatchResults results in matchInfo.Results)
         {
            double percentage = (double)results.TotalShooters / (double)matchInfo.TotalShooters;
            double poolMoney = prizeMoney * percentage;

            PrizePool prizePool = new PrizePool(poolMoney, surrenderPercent, results);
            Console.WriteLine(prizePool.ToString());

            Console.WriteLine($"*********************************************************************");

            foreach (DivClassPool divClassPool in prizePool.Pools)
            {
               List<double> payouts = divClassPool.CalculatePayouts(matchInfo.TopPercentToPay);

               if(divClassPool.PrizeMoney > 0.0)
               {
                  string classification = divClassPool.Classification.ToString();
                  if (classification == "GM") classification = "HOA";
                  Console.WriteLine($"{divClassPool.Division} {classification} (Paying {payouts.Count} of {divClassPool.Count})");

                  double totalPayouts = 0.0;
                  for (int i = 0; i < payouts.Count; i++)
                  {
                     Console.WriteLine($"${payouts[i]:F2}");
                     totalPayouts += payouts[i];
                  }
               }
            }
         }
      }
   }
}

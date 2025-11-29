using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchTool
{
   internal class DivClassPool
   {
      public Divisions Division;
      public Classifications Classification;
      public int Count;
      public double PrizeMoney;

      private List<double> GeneratePayoutPercentages(int totalWinners, double exponent = 1.5)
      {
         List<double> percentages = new List<double>();

         for (int rank = 1; rank <= totalWinners; rank++)
         {
            // Higher ranks get more using a power law: 1 / rank^exponent
            percentages.Add(1 / Math.Pow(rank, exponent));
         }

         // Normalize so that sum of percentages = 1 (100% of payout pool)
         double sum = percentages.Sum();
         for (int i = 0; i < percentages.Count; i++)
         {
            percentages[i] /= sum;
         }

         return percentages;
      }

      // Method to calculate actual payouts based on total prize pool
      public List<double> CalculatePayouts(double topPercentToPay = 0.1)
      {
         int winners = (int)Math.Ceiling(this.Count * topPercentToPay);
         var payoutPercentages = GeneratePayoutPercentages(winners);

         List<double> payouts = payoutPercentages.Select(p => Math.Round(p * this.PrizeMoney, 2)).ToList();
         return payouts;
      }
   }
}

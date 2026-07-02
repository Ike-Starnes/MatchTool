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

      public List<double> GenerateSmoothPayouts(int shooterCount, int topPercentToPay = 10)
      {
         double pool = PrizeMoney;
         List<double> payouts = new List<double>();

         if (pool <= 0 || shooterCount < 1)
            return payouts;

         double topPercent = (double)topPercentToPay / 100.0;

         // Pay at least 3 places so the curve can smooth out
         int paidPlaces = Math.Max(3, (int)(shooterCount * topPercent));

         // Gentle curve: smooth but still rewards 1st place
         double alpha = 1.15;

         double[] weights = new double[paidPlaces];

         for (int i = 0; i < paidPlaces; i++)
            weights[i] = 1.0 / Math.Pow(i + 1, alpha);

         double sum = weights.Sum();

         for (int i = 0; i < paidPlaces; i++)
            payouts.Add(pool * (weights[i] / sum));

         return payouts;
      }

      //private List<double> GeneratePayoutPercentages(int totalWinners, double exponent = 1.5)
      //{
      //   List<double> percentages = new List<double>();

      //   for (int rank = 1; rank <= totalWinners; rank++)
      //   {
      //      // Higher ranks get more using a power law: 1 / rank^exponent
      //      percentages.Add(1 / Math.Pow(rank, exponent));
      //   }

      //   // Normalize so that sum of percentages = 1 (100% of payout pool)
      //   double sum = percentages.Sum();
      //   for (int i = 0; i < percentages.Count; i++)
      //   {
      //      percentages[i] /= sum;
      //   }

      //   return percentages;
      //}

      //// Method to calculate actual payouts based on total prize pool
      //public List<double> CalculatePayouts(int topPercentToPay = 10)
      //{
      //   int winners = (int)Math.Ceiling(this.Count * ((double)topPercentToPay / 100.0));
      //   var payoutPercentages = GeneratePayoutPercentages(winners);

      //   List<double> payouts = payoutPercentages.Select(p => Math.Round(p * this.PrizeMoney, 2)).ToList();
      //   return payouts;
      //}
   }
}

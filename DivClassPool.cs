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
      public MatchResults Results;

      public DivClassPool(MatchResults results)
      {
         if (results == null) throw new ArgumentNullException(nameof(results));
         Results = results;
      }

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
   }
}

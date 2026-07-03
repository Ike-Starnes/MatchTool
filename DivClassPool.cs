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

      //public List<double> GenerateSmoothPayouts(int shooterCount, int topPercentToPay = 10)
      //{
      //   double pool = PrizeMoney;
      //   List<double> payouts = new List<double>();

      //   if (pool <= 0 || shooterCount < 1)
      //      return payouts;

      //   double topPercent = (double)topPercentToPay / 100.0;

      //   // Pay at least 3 places so the curve can smooth out
      //   int paidPlaces = Math.Max(3, (int)(shooterCount * topPercent));

      //   // Gentle curve: smooth but still rewards 1st place
      //   double alpha = 1.15;

      //   double[] weights = new double[paidPlaces];

      //   for (int i = 0; i < paidPlaces; i++)
      //      weights[i] = 1.0 / Math.Pow(i + 1, alpha);

      //   double sum = weights.Sum();

      //   for (int i = 0; i < paidPlaces; i++)
      //      payouts.Add(pool * (weights[i] / sum));

      //   return payouts;
      //}

      public List<double> GenerateSmoothPayouts(int shooterCount, int topPercentToPay = 10)
      {
         double pool = PrizeMoney;
         List<double> payouts = new List<double>();

         if (pool <= 0 || shooterCount < 1)
            return payouts;

         double topPercent = (double)topPercentToPay / 100.0;

         // Pay at least 4 places for smoother curve
         int paidPlaces = Math.Max(4, (int)(shooterCount * topPercent));

         double alpha = 1.05; // flatter curve

         double[] weights = new double[paidPlaces];

         for (int i = 0; i < paidPlaces; i++)
         {
            double linear = (paidPlaces - i);
            double expo = 1.0 / Math.Pow(i + 1, alpha);

            // Hybrid weighting: 35% linear, 65% exponential
            weights[i] = (linear * 0.35) + (expo * 0.65);
         }

         double sum = weights.Sum();

         for (int i = 0; i < paidPlaces; i++)
            payouts.Add(pool * (weights[i] / sum));

         return payouts;
      }

      public List<double> GenerateWSOPStylePayouts(int shooterCount, int topPercentToPay = 10)
      {
         double pool = PrizeMoney;
         List<double> payouts = new List<double>();

         if (pool <= 0 || shooterCount < 1)
            return payouts;

         double topPercent = (double)topPercentToPay / 100.0;

         // WSOP pays ~15% of field, but we scale to your percent
         int paidPlaces = Math.Max(5, (int)(shooterCount * topPercent));

         double[] weights = new double[paidPlaces];

         for (int i = 0; i < paidPlaces; i++)
         {
            int place = i + 1;

            // --- WSOP payout tiers ---
            if (place == 1)
            {
               // Winner gets ~12% of total pool in WSOP Main Event
               weights[i] = 12.0;
            }
            else if (place == 2)
            {
               // 2nd gets ~7%
               weights[i] = 7.0;
            }
            else if (place == 3)
            {
               // 3rd gets ~5%
               weights[i] = 5.0;
            }
            else if (place <= 9)
            {
               // Final table taper (WSOP uses ~4%, 3%, 2.5%, 2%, etc.)
               weights[i] = 4.0 / Math.Pow(place - 2, 0.7);
            }
            else
            {
               // Bubble + min-cash zone (flat payouts)
               weights[i] = 1.0;
            }
         }

         // Normalize weights
         double sum = weights.Sum();

         for (int i = 0; i < paidPlaces; i++)
            payouts.Add(pool * (weights[i] / sum));

         return payouts;
      }
   }
}

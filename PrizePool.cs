using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchTool
{
   internal class PrizePool
   {
      private double _total;
      public double Total { get => _total; }
      public MatchResults Results { get; set; }

      private int _surrender;

      private List<DivClassPool> _pools;
      internal List<DivClassPool> Pools { get => _pools; }

      public double GetClassificationPrize(Classifications classification)
      {
         double ret = 0.0;
         foreach (DivClassPool pool in Pools)
         {
            if (pool.Classification == classification)
               ret = pool.PrizeMoney;
         }
         return ret;
      }

      public PrizePool(double poolMoney, MatchResults results, MatchOptions options)
      {
         _total = poolMoney;
         _surrender = options.SurrenderPercent;
         Results = results;

         _pools = new List<DivClassPool>();

         if (!options.HOAOnly)
         {
            DivClassPool? gmPool = null;
            double classMoneySum = 0.0;
            int classSum = 0;
            foreach (Classifications classification in Enum.GetValues(typeof(Classifications)))
            {
               DivClassPool classPool = new DivClassPool(results);
               Pools.Add(classPool);
               classPool.Division = results.Division;
               classPool.Classification = classification;
               if (classPool.Classification == Classifications.GM)
                  gmPool = classPool;
               classPool.Count = Results.GetClassificationCount(classification);

               if ((classPool.Classification == Classifications.GM) || (classPool.Classification == Classifications.U) || (classPool.Count < 5))
               {
                  classPool.PrizeMoney = 0.0;
               }
               else
               {
                  if (classPool.Classification != Classifications.GM)
                  {
                     classPool.PrizeMoney = _total * ((double)classPool.Count / (double)Results.TotalShooters); // without accounting for surrender amount
                     double surrenderAmount = classPool.PrizeMoney * ((double)_surrender / 100.0);
                     classPool.PrizeMoney -= surrenderAmount;
                  }
               }
               classMoneySum += classPool.PrizeMoney;
            }

            gmPool.PrizeMoney = poolMoney - classMoneySum;
            gmPool.Count = results.TotalShooters - classSum;
         }
         else
         {
            DivClassPool hoaPool = new DivClassPool(results);
            Pools.Add(hoaPool);
            hoaPool.Division = results.Division;
            hoaPool.Classification = Classifications.HOA;
            hoaPool.Count = Results.TotalShooters;
            hoaPool.PrizeMoney = poolMoney;
         }
      }

      public override string ToString()
      {
         string ret = string.Empty;

         // line #1
         string division = Results.Division.ToString().PadRight(16);
         ret += division;
         ret += $"${_total:F2}".PadRight(16);

         foreach (Classifications classification in Enum.GetValues(typeof(Classifications)))
         {
            ret += $"{Results.GetClassificationCount(classification)}".PadRight(16);
         }
         ret += $"{Results.TotalShooters}{Environment.NewLine}";

         // line #2
         ret += "Class Pools".PadLeft(16).PadRight(32);
         foreach (Classifications classification in Enum.GetValues(typeof(Classifications)))
         {
            string classPrize = $"${GetClassificationPrize(classification):F2}".PadRight(16);
            ret += classPrize;
         }

         double totalPrize = 0.0;
         foreach (DivClassPool pool in Pools)
         {
            totalPrize += pool.PrizeMoney;
         }

         ret += $"${totalPrize:F2}";

         return ret;
      }
   }
}

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

      private int _surrender;

      private MatchResults _results;

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

      public PrizePool(double totalMoney, int surrender, MatchResults results)
      {
         _total = totalMoney;
         _surrender = surrender;
         _results = results;

         _pools = new List<DivClassPool>();

         DivClassPool? gmPool = null;

         double classMoneySum = 0.0;
         int classSum = 0;

         foreach (Classifications classification in Enum.GetValues(typeof(Classifications)))
         {
            DivClassPool pool = new DivClassPool();
            Pools.Add(pool);
            pool.Division = results.Division;
            pool.Classification = classification;
            if (pool.Classification == Classifications.GM)
               gmPool = pool;
            pool.Count = _results.GetClassificationCount(classification);

            if ((pool.Classification == Classifications.GM) || (pool.Classification == Classifications.U) || (pool.Count < 5))
            {
               pool.PrizeMoney = 0.0;
            }
            else
            {
               if (pool.Classification != Classifications.GM)
               {
                  pool.PrizeMoney = _total * ((double)pool.Count / (double)_results.TotalShooters); // without accounting for surrender amount
                  double surrenderAmount = pool.PrizeMoney * ((double)_surrender/100.0);
                  pool.PrizeMoney -= surrenderAmount;
               }
            }
            classMoneySum += pool.PrizeMoney;
         }

         gmPool.PrizeMoney = totalMoney - classMoneySum;
         gmPool.Count = results.TotalShooters - classSum;
      }

      public override string ToString()
      {
         string ret = string.Empty;

         // line #1
         string division = _results.Division.ToString().PadRight(16);
         ret += division;
         ret += $"${_total:F2}".PadRight(16);

         foreach (Classifications classification in Enum.GetValues(typeof(Classifications)))
         {
            ret += $"{_results.GetClassificationCount(classification)}".PadRight(16);
         }
         ret += $"{_results.TotalShooters}{Environment.NewLine}";

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

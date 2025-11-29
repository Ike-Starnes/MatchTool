using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchTool
{
   internal class MatchResults
   {
      // table row
      // col 0: Placement
      // col 1: Name
      // col 2: USPSA #
      // col 3: Classification
      private HtmlNode _results;

      private Divisions _division;
      public Divisions Division { get => _division; set => _division = value; }

      public MatchResults(Divisions division, HtmlNode html)
      {
         this._division = division;
         this._results = html;
      }

      public int TotalShooters
      {
         get
         {
            HtmlNodeCollection resultsRows = _results.SelectNodes(".//tr");
            return resultsRows.Count - 2; // don't count division head row and header row
         }
      }

      public override string ToString()
      {
         string ret = $"Division: {_division}";
         ret += Environment.NewLine;
         ret += $"Total Shooters: {TotalShooters}";
         ret += Environment.NewLine;
         foreach (Classifications classification in Enum.GetValues(typeof(Classifications)))
         {
            ret += $"{classification} Class:\t{GetClassificationCount(classification)}";
            if(classification != Classifications.U) ret += Environment.NewLine;
         }
         return ret;
      }

      public int GetClassificationCount(Classifications classification)
      {
         int count = 0;
         HtmlNodeCollection resultsRows = _results.SelectNodes(".//tr");
         for (int i = 2; i < TotalShooters + 2; i++)
         {
            HtmlNodeCollection cols = resultsRows[i].SelectNodes(".//td");
            if (cols[3].InnerText.ToLower() == classification.ToString().ToLower())
            {
               count++;
            }
         }
         return count;
      }
   }
}

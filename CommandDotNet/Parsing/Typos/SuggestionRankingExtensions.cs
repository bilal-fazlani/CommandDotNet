using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.Parsing.Typos
{
    internal static class SuggestionRankingExtensions
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        internal static IEnumerable<string> RankAndTrimSuggestions(
            this IEnumerable<string> suggestions, string typo, int maxSuggestionCount)
        {
            var tuples = suggestions
                .Where(n => !n.IsNullOrWhitespace())
                .Select(name =>
                {
                    var rearranged = GetRearranged(typo).ToList();

                    var distance = -Levenshtein.ComputeDistance(typo, name);
                    var startsWith = GetStartsWithLength(typo, name);
                    var sameness = rearranged.Max(n => GetSamenessLength(n, name));
                    sameness = sameness == typo.Length 
                        ? sameness * 2 //found possible match of compound words out-of-order
                        : Math.Max(sameness, GetContainsLength(typo, name));
                    return (name, distance, startsWith, sameness, score: distance + startsWith + sameness);
                })
                .OrderByDescending(v => v.score)
                .ThenBy(v => v.startsWith)
                .ThenBy(v => v.sameness)
                .ThenBy(v => v.name)
                .ToList();

            if (Log.IsDebugEnabled())
            {
                var withScores = tuples
                    .Select(v => $"  {v.name}  - distance:{v.distance} startsWith:{v.startsWith} sameness:{v.sameness} score:{v.score}");
                Log.Debug($"scores:{Environment.NewLine}{withScores.ToCsv(Environment.NewLine)}");
            }

            bool IsSimilarEnough((string name, int distance, int startsWith, int sameness, int score) valueTuple)
            {
                return !(valueTuple.sameness == 0 && valueTuple.startsWith == 0 & valueTuple.distance < -3);
            }

            return tuples
                .Where(v => v.score > -3)
                .Where(IsSimilarEnough)
                .Select(s => s.name)
                .Take(maxSuggestionCount);
        }

        private static int GetStartsWithLength(string first, string second)
        {
            for (int i = 0; i < Math.Min(first.Length, second.Length); i++)
            {
                if (first[i] != second[i])
                    return i;
            }

            return first.Length;
        }

        private static int GetSamenessLength(string first, string second)
        {
            int same = 0;
            for (int i = 0; i < Math.Min(first.Length, second.Length); i++)
            {
                if (first[i] == second[i])
                    same++;
            }

            return same;
        }

        private static int GetContainsLength(string first, string second)
        {
            var (small, large) = first.Length > second.Length ? (second, first) : (first, second);
            return large.ToLower().Contains(small.ToLower()) ? small.Length : 0;
        }

        private static IEnumerable<string> GetRearranged(string typo)
        {
            for (int i = 0; i < typo.Length; i++)
            {
                string name = "";

                if (i < typo.Length - 1)
                {
                    name += typo.Substring(i);
                }
                if (i > 0)
                {
                    name += typo.Substring(0, i);
                }

                yield return name;
            }
        }
    }
}
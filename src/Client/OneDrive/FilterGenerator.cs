using System;
using System.Collections.Generic;
using System.Linq;

namespace PassiveEyes.SDK.OneDrive
{
    public class FilterGenerator
    {
        public string GenerateFilters(IEnumerable<Tuple<string, string, string>> filters)
            => '?' + string.Join(
                " and ",
                filters.Select(filter => $"{filter.Item1} {filter.Item2} {filter.Item3}"));

        public string GenerateFilters(Tuple<string, string, string> filter)
            => this.GenerateFilters(new[] { filter });

        public string GenerateFilters(string[][] filters)
            => this.GenerateFilters(
                filters.Select(
                    filter => new Tuple<string, string, string>(
                        filter[0], filter[1], filter[2])));

        public string GenerateFilters(string[] filter) => this.GenerateFilters(new[] { filter });
    }
}
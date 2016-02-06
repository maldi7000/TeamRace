using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace TeamRace
{
    [DelimitedRecord(";")]
    // Ignore header
    [IgnoreFirst]
    class Racer : IComparable<Racer>
    {
        public int Bib;
        public string Name;
        public int Category;
        public string TeamName;
        [FieldConverter(ConverterKind.Double, ",")]
        public double Time;
        public char Gender;

        public int CompareTo(Racer other)
        {
            return Time.CompareTo(other.Time);
        }
    }
}

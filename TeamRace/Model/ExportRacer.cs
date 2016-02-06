using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamRace
{
    [DelimitedRecord(";")]
    class ExportRacer : Racer
    {
        [FieldConverter(ConverterKind.Double, ",")]
        public double? TeamTime;

        public ExportRacer(Racer racer)
        {
            Bib = racer.Bib;
            Name = racer.Name;
            Category = racer.Category;
            TeamName = racer.TeamName;
            Time = racer.Time;
            Gender = racer.Gender;
        }

        private ExportRacer() {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamRace
{
    class TeamControllerToLatexController
    {
        private TeamController teamController;
        private bool isTrophy;
        private string title;

        public TeamControllerToLatexController(TeamController tc)
        {
            teamController = tc;
            isTrophy = false;
        }

        public void setTitle(string title)
        {
            this.title = title;
        }

        public void setIsTrophy(bool isTrophy)
        {
            this.isTrophy = isTrophy;
        }

        public String GenerateLatex()
        {
            StringBuilder latex = new StringBuilder(@"\raceClass{" + title + @"}
\begin{TRtable}"); // using commands defined in main.tex

            for (int i = 0; i < teamController.Teams.Count(); i++)
            {
                latex.Append(ToLatexSnippet(teamController.Teams[i], i + 1));
            }

            latex.Append(@"\end{TRtable}");
            return latex.ToString();
        }

        private String ToLatexSnippet(Team t, int rank)
        {
            // create multirows for rank and team time
            int multiRowSize = t.Skiers.Count() + 1;
            StringBuilder snippet = new StringBuilder(makeMultiRow(multiRowSize, rank.ToString()));
            snippet.Append(" & ");
            snippet.Append(makeMultiRow(multiRowSize, t.Name.Replace("&", "\\&")));
            snippet.Append(ToLatexSnippet(t.Sledger, true));

            if (!isTrophy)
            {
                snippet.Append(makeMultiRow(multiRowSize, t.TeamTime + "\\,s")); // add small space between time and unit
            }
            else
            {
                snippet.Append(makeMultiRow(multiRowSize, t.TrophyTime + "\\,s")); // add small space between time and unit
            }
            snippet.Append("\\\\ \n"); // add newline for readability

            foreach (Racer r in t.Skiers)
            {
                snippet.Append(ToLatexSnippet(r, false));
            }
            snippet.Append("\\hline \n"); // add newline for readability
            return snippet.ToString();
        }

        private String ToLatexSnippet(Racer r, bool isFirst)
        {
            StringBuilder snippet = new StringBuilder(" & ");
            if (!isFirst)
            {
                snippet.Append(" & ");
            }

            snippet.Append(r.Name);
            snippet.Append(" & ");
            snippet.Append(r.Bib);
            snippet.Append(" & ");
            snippet.Append(r.Time + "\\,s & "); // add small space between time and unit
            if (!isFirst)
            {
                snippet.Append("\\\\ \n");
            }
            return snippet.ToString();
        }

        private static String makeMultiRow(int rows, String content)
        {
            return @"\mr{" + rows + "}{" + content + "}";
        }
    }
}

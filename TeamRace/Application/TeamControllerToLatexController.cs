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
            StringBuilder latex = new StringBuilder(@"\section*{Rangliste " + title + @"}
\begin{longtabu} to \linewidth {| c | l | l | c | l | l |}
\hline 
Rang & Team & Name & Startnummer & Zeit & Teamzeit \\ \hline
\endhead");

            for (int i = 0; i < teamController.Teams.Count(); i++)
            {
                latex.Append(ToLatexSnippet(teamController.Teams[i], i + 1));
            }

            latex.Append(@"\end{longtabu}");
            return latex.ToString();
        }

        private String ToLatexSnippet(Team t, int rank)
        {
            // create multirows for rank and team time
            int multiRowSize = t.Skiers.Count() + 1;
            StringBuilder snippet = new StringBuilder(makeMultiRow(multiRowSize, rank.ToString()));
            snippet.Append(" & ");
            snippet.Append(makeMultiRow(multiRowSize, t.Name.Replace("&", "\\&")));
            snippet.Append(" & ");
            snippet.Append(ToLatexSnippet(t.Sledger, true));

            if (!isTrophy)
            {
                snippet.Append(makeMultiRow(multiRowSize, t.TeamTime + "s"));
            }
            else
            {
                snippet.Append(makeMultiRow(multiRowSize, t.TrophyTime + "s"));
            }
            snippet.Append(@"\\");

            foreach (Racer r in t.Skiers)
            {
                snippet.Append(ToLatexSnippet(r, false));
            }
            snippet.Append(@"\hline");
            return snippet.ToString();
        }

        private String ToLatexSnippet(Racer r, bool isFirst)
        {
            StringBuilder snippet = new StringBuilder();
            if (!isFirst)
            {
                snippet.Append("& &");
            }

            snippet.Append(r.Name);
            snippet.Append(" & ");
            snippet.Append(r.Bib);
            snippet.Append(" & ");
            snippet.Append(r.Time + "s & ");
            if (!isFirst)
            {
                snippet.Append("\\\\ \n");
            }
            return snippet.ToString();
        }

        private static String makeMultiRow(int rows, String content)
        {
            return @"\multirow{" + rows + "}{*}{" + content + "}";
        }
    }
}

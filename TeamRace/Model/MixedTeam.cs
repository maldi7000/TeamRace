using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamRace
{
    class MixedTeam : Team
    {
        public MixedTeam(string name) : base(name) {
        }

        public override void computeTeamTime()
        {
            if (Sledger.Gender == 'w')
            {
                base.computeTeamTime();
                return;
            }

            else
            {
                // tempTeamTime to avoid raising INotifyPropertyChanged events
                double tempTeamTime = Sledger.Time + Skiers[0].Time;

                if (Skiers[0].Gender == 'm')
                {
                    for (int i = 1; i < Skiers.Count(); i++)
                    {
                        if (Skiers[i].Gender == 'w')
                        {
                            tempTeamTime += Skiers[i].Time;
                            TeamTime = tempTeamTime;
                            return;
                        }
                    }
                }

                tempTeamTime += Skiers[1].Time;
                TeamTime = tempTeamTime;
                return;
            }
        }
    }
}

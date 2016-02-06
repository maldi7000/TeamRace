using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamRace
{
    class TeamController : INotifyPropertyChanged
    {
        public ObservableCollection<Team> Teams {
            get
            {
                return teams;
            }
            set
            {
                teams = value;
                OnPropertyChanged("Teams");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        private ObservableCollection<Team> teams;
        public bool IsMixed { get; private set; }

        public TeamController(bool isMixed)
        {
            IsMixed = isMixed;
            Teams = new ObservableCollection<Team>();
        }

        public TeamController()
        {
            IsMixed = false;
            Teams = new ObservableCollection<Team>();
        }

        public TeamController(List<Team> teams)
        {
            IsMixed = false;
            Teams = new ObservableCollection<Team>(teams);
        }
        private bool hasTeam(string teamName) {
            foreach (Team t in Teams)
            {
                if (t.Name.Equals(teamName))
                {
                    return true;
                }
            }

            return false;
        }

        private Team getTeamByName(string name)
        {
            foreach (Team t in Teams)
            {
                if (t.Name.Equals(name))
                {
                    return t;
                }
            }

            return null;
        }

        public void addRacer(Racer racer)
        {
            if (!hasTeam(racer.TeamName))
            {
                if (IsMixed)
                {
                    Teams.Add(new MixedTeam(racer.TeamName)); 
                }

                else
                {
                    Teams.Add(new Team(racer.TeamName));
                }
            }

            Team team = getTeamByName(racer.TeamName);
            team.addRacer(racer);
            team.TrophyTime += racer.Time;
        }
    }
}

using FileHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TeamRace
{
    class Team : IComparable<Team>, INotifyPropertyChanged
    {
        public string Name { get; set; }
        public List<Racer> Skiers { get; set; }
        public Racer Sledger { get; set; }
        public int Category { get; set; }
        public double TeamTime
        {
            get
            {
                return teamTime;
            }
            set
            {
                OnPropertyChanged("TeamTime");
                teamTime = value;
            }
        }

        public double TrophyTime
        {
            get
            {
                return trophyTime;
            }

            set
            {
                OnPropertyChanged("TrophyTime");
                trophyTime = value;
            }
        }
        private double teamTime;
        private double trophyTime;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        
        public Team()
        {
            Skiers = new List<Racer>();
        }

        public Team(string name)
        {
            Name = name;
            Skiers = new List<Racer>();
        }

        public void addRacer(Racer racer)
        {
            if (racer.Name.Contains('®'))
            {
                Sledger = racer;
            }

            else
            {
                Skiers.Add(racer);
            }
        }

        public virtual void computeTeamTime()
        {
            Skiers.Sort();
            TeamTime = Sledger.Time + Skiers[0].Time + Skiers[1].Time;
        }

        public int CompareTo(Team other)
        {
            return TeamTime.CompareTo(other.TeamTime);
        }
    }
}

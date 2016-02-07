using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using FileHelpers;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace TeamRace
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<int, TeamController> TeamControllers { get; set; }
        Double TrophyTime { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            TeamControllers = new Dictionary<int, TeamController>();
        }

        private void openFileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog() { Filter = "CSV Dateien (.csv)|*.csv", Multiselect = false };
            var result = fd.ShowDialog();


            if (result.HasValue && result.Value)
            {
                string fileName = fd.FileName;
                fileTextBox.Text = fileName;
                var engine = new FileHelperAsyncEngine<Racer>();
                engine.ErrorManager.ErrorMode = ErrorMode.IgnoreAndContinue;
                int numberOfRacers = 0;

                using (engine.BeginReadFile(fileName))
                {
                    foreach (Racer r in engine)
                    {
                        TrophyTime += r.Time;
                        ++numberOfRacers;
                        if (!TeamControllers.ContainsKey(r.Category))
                        {
                            bool mixed = askForMixed(r.Category);
                            TeamControllers[r.Category] = new TeamController(mixed);
                        }

                        TeamControllers[r.Category].addRacer(r);
                    }
                }

                TrophyTime = 4 * TrophyTime / numberOfRacers;
                trophyTimeTextBox.Text = TrophyTime.ToString("N") + "s";
                computeTeamTimes();
                displayTeamsInTabControl();
            }
        }

        private bool askForMixed(int category)
        {
            string dialogText = "Ist diese Klasse mixed?";
            string dialogTitle = "Klasse " + category;

            MessageBoxButton dialogButtons = MessageBoxButton.YesNo;
            MessageBoxImage dialogIcon = MessageBoxImage.Question;

            MessageBoxResult dialogResult = System.Windows.MessageBox.Show(dialogText, dialogTitle, dialogButtons, dialogIcon);

            switch (dialogResult)
            {
                case MessageBoxResult.Yes:
                    return true;

                case MessageBoxResult.No:
                    return false;
            }

            return false;
        }

        private void displayTeamsInTabControl()
        {
            foreach (int k in TeamControllers.Keys)
            {
                var tab = new TabItem();
                tab.Header = k;
                teamTabControl.Items.Add(tab);
                var teamList = new System.Windows.Controls.ListView();
                // initalize listview
                var gridView = new GridView();
                teamList.View = gridView;

                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "Team",
                    DisplayMemberBinding = new Binding("Name")
                });

                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "Zeit [s]",
                    DisplayMemberBinding = new Binding("TeamTime")
                });

                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "Zeit (Trophy-Wertung) [s]",
                    DisplayMemberBinding = new Binding("TrophyTime")
                });

                tab.Content = teamList;

                Binding tabBinding = new Binding();
                tabBinding.Source = TeamControllers[k];
                tabBinding.Path = new PropertyPath("Teams");
                tabBinding.Mode = BindingMode.OneWay;
                tabBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(teamList, System.Windows.Controls.ListView.ItemsSourceProperty, tabBinding);
            }
        }

        private void exportButtonClick(object sender, RoutedEventArgs e)
        {
            var folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;
            CommonFileDialogResult result = folderDialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string folderPath = folderDialog.FileName;
                if (File.Exists(folderPath + "\\main.tex"))
                {
                    File.Delete(folderPath + "\\main.tex");
                }
                var mainTex = Properties.Resources.main;
                File.WriteAllBytes(folderPath + "\\main.tex", mainTex);
                var engine = new FileHelperAsyncEngine<ExportRacer>();
                engine.ErrorManager.ErrorMode = ErrorMode.IgnoreAndContinue;

                List<Team> trophyTeamList = new List<Team>();
                foreach (int key in TeamControllers.Keys)
                {
                    var teamController = TeamControllers[key];
                    createPdf(folderPath, "Klasse " + key, teamController, false);

                    trophyTeamList.AddRange(teamController.Teams);
                    using (engine.BeginWriteFile(folderPath + "\\Ergebnis Klasse " + key + ".csv"))
                    {
                        ObservableCollection<Team> teamList = teamController.Teams;

                        foreach (Team t in teamList)
                        {
                            foreach (Racer r in t.Skiers)
                            {
                                ExportRacer w = new ExportRacer(r);
                                engine.WriteNext(w);
                            }
                            ExportRacer sledger = new ExportRacer(t.Sledger);
                            sledger.TeamTime = t.TeamTime;
                            engine.WriteNext(sledger);
                        }
                    }
                }

                trophyTeamList.Sort(delegate(Team x, Team y)
                {
                    double xDifferenceToTrophyTime = Math.Abs(TrophyTime - x.TrophyTime);
                    double yDifferenceToTrophyTime = Math.Abs(TrophyTime - y.TrophyTime);

                    return xDifferenceToTrophyTime.CompareTo(yDifferenceToTrophyTime);
                });

                TeamController trophyController = new TeamController(trophyTeamList);
                createPdf(folderPath, "Trophy-Wertung", trophyController, true);

                using (engine.BeginWriteFile(folderPath + "\\Ergebnis Trophy.csv"))
                {
                    foreach (Team t in trophyTeamList)
                    {
                        foreach (Racer r in t.Skiers)
                        {
                            ExportRacer w = new ExportRacer(r);
                            engine.WriteNext(w);
                        }
                        ExportRacer sledger = new ExportRacer(t.Sledger);
                        sledger.TeamTime = t.TrophyTime;
                        engine.WriteNext(sledger);
                    }
                }

                // clean up latex overhead
                Process cleanUp = new Process();
                cleanUp.StartInfo.FileName = ("cmd");
                cleanUp.StartInfo.Arguments = ("/c del \"" + folderPath + "\\main.*\" \"" + folderPath + "\\table.*\"");

                cleanUp.Start();
            }

        }

        private void createPdf(string folderPath, string title, TeamController teamController, bool isTrophy)
        {
            TeamControllerToLatexController latexWriter = new TeamControllerToLatexController(teamController);
            latexWriter.setTitle(title);
            latexWriter.setIsTrophy(isTrophy);

            File.WriteAllText(folderPath + "\\table.tex", latexWriter.GenerateLatex());
            
            Process latexCompile = new Process();
            latexCompile.StartInfo.FileName = "pdflatex";
            latexCompile.StartInfo.Arguments = "-output-directory=\"" + folderPath + "\" \"" + folderPath + "\\main.tex\"";

            latexCompile.Start();
            latexCompile.WaitForExit();

            latexCompile.Start();
            latexCompile.WaitForExit();
            // TODO: remove overhead from LaTeX compilation like .log, .out, etc...
            if (File.Exists(folderPath + "\\" + title + ".pdf"))
            {
                File.Delete(folderPath + "\\" + title + ".pdf");
            }
            File.Move(folderPath + "\\main.pdf", folderPath + "\\" + title + ".pdf");
        }

        private void computeTeamTimes()
        {
            foreach (var teamController in TeamControllers.Values)
            {
                foreach (Team t in teamController.Teams)
                {
                    t.computeTeamTime();
                }
                teamController.Teams.Sort();
            }
        }
    }

    static class Extensions
    {
        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable<Team>
        {
            List<T> sorted = collection.OrderBy(x => x).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }
}

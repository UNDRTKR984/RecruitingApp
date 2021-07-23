using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SQLite;
using System.Collections.ObjectModel;
using RecruitingApp.Models;
using Xamarin.Forms.Xaml;

namespace RecruitingApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportsPage : ContentPage
    {
        private List<AgeGroup> ageGroupList;
        private List<Team> teamList;
        private List<Player> playerList;
        public ReportsPage()
        {
            InitializeComponent();
        }

        // used to reload database everytime page appears it comes from the ContentPage class
        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                ageGroupList = conn.Table<AgeGroup>().ToList();
                teamList = conn.Table<Team>().ToList();
                playerList = conn.Table<Player>().ToList();
            }

            ageGroupList = ageGroupList.OrderBy(age => age.Name).ToList();
            teamList = teamList.OrderBy(team => team.Name.ToUpper()).ToList();
            playerList = playerList.OrderBy(player => player.Number).ToList();

            if (playersCreated.IsChecked)
            {
                var lastCreated = playerList.OrderBy(player => player.Created).Take(100).ToList();
                foreach (var player in lastCreated)
                {
                    player.Created = player.Created.ToLocalTime();
                }
                reportSection.IsVisible = false;
                teamSearchEntry.IsVisible = false;
                teamGrid.IsVisible = false;
                last100.IsVisible = true;
                last100.ItemsSource = lastCreated;
            }
            if (teamSearch.IsChecked)
            {
                teamGrid.ItemsSource = teamList.Where(team => team.Name.ToUpper().StartsWith(teamSearchEntry.Text.ToUpper())).ToList();
            }

        }

        // queryButton_Clicked
        //      runs the query or allows the user to search
        private void queryButton_Clicked(object sender, EventArgs e)
        {
            reportSection.Text = "";
            teamSearchEntry.Text = "";
            if (playersPerAge.IsChecked)
            {
                teamGrid.IsVisible= false;
                last100.IsVisible = false;
                reportSection.IsVisible = true;

                reportSection.Text += "\tHere are the number of recruits for each Age Group:\n\n";

                foreach (var age in ageGroupList)
                {
                    var count = 0;
                    reportSection.Text += $"\t{age.Name, -5}: ";

                    var tempTeams = teamList.Where(team => team.AgeGroupID == age.ID);
                    foreach (var team in tempTeams)
                    {
                        count += playerList.Where(player => player.TeamID == team.ID).Count();
                    }
                    reportSection.Text += $"{count, 2}\n";
                }
                             
            }
            if (playersCreated.IsChecked)
            {
                var lastCreated = playerList.OrderByDescending(player => player.Created).Take(100).ToList();
                foreach (var player in lastCreated)
                {
                    player.Created = player.Created.ToLocalTime();
                }
                reportSection.IsVisible = false;
                teamSearchEntry.IsVisible = false;
                teamGrid.IsVisible = false;
                last100.IsVisible = true;
                last100.ItemsSource = lastCreated;


            }
            if (teamSearch.IsChecked)
            {
                last100.IsVisible = false;
                reportSection.IsVisible = false;
                teamSearchEntry.IsVisible = true;
                teamSearchEntry.Focus();
                teamGrid.IsVisible = true;
            }
        }

        private void teamSearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            teamGrid.ItemsSource = teamList.Where(team => team.Name.ToUpper().StartsWith(e.NewTextValue.ToUpper())).ToList();
        }

        async private void teamGrid_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as Team;
            MainPage.selectedAgeGroup = item.AgeGroupID;
            MainPage.selectedTeam = item.ID;

            await Navigation.PushAsync(new EditTeamPage());
        }

        async private void last100_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as Player;
            MainPage.selectedPlayer = item.ID;
            MainPage.selectedTeam = item.TeamID;
            var tempTeam = teamList.Where(team => team.ID == item.TeamID).FirstOrDefault();
            MainPage.selectedAgeGroup = tempTeam.AgeGroupID;

            await Navigation.PushAsync(new EditPlayerPage());
        }
    }
}
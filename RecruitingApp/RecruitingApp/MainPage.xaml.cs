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

namespace RecruitingApp
{
    public partial class MainPage : ContentPage
    {
        // used to keep track of Age Groups, Teams, and Players throughout the app
        public static List<AgeGroup> ageGroupList;
        public static int selectedAgeGroup;
        public static int selectedTeam;
        public static int selectedPlayer;
        public static bool deletedAllAgeGroups;

       

        public MainPage()
        {
            InitializeComponent();
            deletedAllAgeGroups = false;

        }

        // used to reload database everytime page appears it comes from the ContentPage class
        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<AgeGroup>();
                conn.CreateTable<Team>();
                conn.CreateTable<Player>();
                var ages = conn.Table<AgeGroup>().ToList();
                var teams = conn.Table<Team>().ToList();
                var players = conn.Table<Player>().ToList();

                // if the ages table is empty (no data) insert some dummy data to be used to test the application -- comment out if using for real life purposes
                if (ages.Count == 0 && deletedAllAgeGroups == false)
                {
                    conn.Insert(new AgeGroup
                    {
                        Year = new DateTime(2003, 12, 31),
                        Created = DateTime.UtcNow
                    });
                    conn.Insert(new AgeGroup
                    {
                        Year = new DateTime(2004, 12, 31),
                        Created = DateTime.UtcNow
                    });
                    conn.Insert(new AgeGroup
                    {
                        Year = new DateTime(2005, 12, 31),
                        Created = DateTime.UtcNow
                    });

                    ages = conn.Table<AgeGroup>().ToList();

                    conn.Insert(new Team
                    {
                        Name = "Richmond FC",
                        Level = "Gold",
                        CoachName = "Frank Franky",
                        CoachPhone = "123456789",
                        CoachEmail = "Frank@franky.com",
                        Created = DateTime.UtcNow,
                        AgeGroupID = ages[0].ID,
                        AgeGroupString = "2003"
                    });
                    conn.Insert(new Team
                    {
                        Name = "Liverpool",
                        Level = "Gold",
                        CoachName = "Bill Shankly",
                        CoachPhone = "123556789",
                        CoachEmail = "Bill@liverpool.com",
                        Created = DateTime.UtcNow,
                        AgeGroupID = ages[1].ID,
                        AgeGroupString = "2004"
                    });
                    conn.Insert(new Team
                    {
                        Name = "Lexington FC",
                        Level = "Gold",
                        CoachName = "Mike Kelly",
                        CoachPhone = "523456789",
                        CoachEmail = "MKelly@amazingCoach.com",
                        Created = DateTime.UtcNow,
                        AgeGroupID = ages[2].ID,
                        AgeGroupString = "2005"
                    });

                    teams = conn.Table<Team>().ToList();

                    conn.Insert(new Player
                    {
                        Number = "11",
                        FirstName = "Abra",
                        LastName = "Cadabra",
                        Rating = "5",
                        Position = "Center Back",
                        Email = "amazing@amazon.com",
                        Cell = 12343421,
                        Address = "123 Amazing Player Blvd",
                        City = "Lexington",
                        State = "KY",
                        ZIP = "40456",
                        Notes = "So fast",
                        ActivelyRecruiting = true,
                        Created = DateTime.UtcNow,
                        TeamID = teams[0].ID
                    });
                    conn.Insert(new Player
                    {
                        Number = "8",
                        FirstName = "Zinedine",
                        LastName = "Zidane",
                        Rating = "5",
                        Position = "Center Mid",
                        Email = "zizoug@amazon.com",
                        Cell = 14343421,
                        Address = "55 Amazing Player Blvd",
                        City = "Lexington",
                        State = "KY",
                        ZIP = "40456",
                        Notes = "So skilled",
                        ActivelyRecruiting = true,
                        Created = DateTime.UtcNow,
                        TeamID = teams[0].ID
                    });
                    conn.Insert(new Player
                    {
                        Number = "7",
                        FirstName = "Cristiano",
                        LastName = "Ronaldo",
                        Rating = "5",
                        Position = "Winger/FWD",
                        Email = "CR7@amazon.com",
                        Cell = 777777777,
                        Address = "777 Amazing Player Blvd",
                        City = "Lexington",
                        State = "KY",
                        ZIP = "40456",
                        Notes = "So skilled and fast",
                        ActivelyRecruiting = true,
                        Created = DateTime.UtcNow,
                        TeamID = teams[1].ID
                    });
                }
                

                // get the age groups from the age groups table and assign it to the ageGroupView item source
                ages = conn.Table<AgeGroup>().ToList();
                ageGroupList = ages.OrderBy(age => age.Year).ToList();

                ageGroupView.ItemsSource = ageGroupList;

                
            }


        }

        // ageGroupView_ItemTapped
        //      finds the age group and opens a EditAgeGroupPage with the selected age group in focus
        async private void ageGroupView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var termSelected = e.Item as AgeGroup;
            selectedAgeGroup = termSelected.ID;

            await Navigation.PushAsync(new EditAgeGroupPage());
        }

        // addAgeGroup_Clicked
        //      executes when the addAgeGroup button is clicked and opens up an add age group page
        async private void addAgeGroup_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddAgeGroupPage());
        }

        // reportsButton_Clicked
        //      navigates the user to the Reports page
        async private void reportsButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReportsPage());
        }
    }
}

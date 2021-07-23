using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using RecruitingApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Mail;
using Xamarin.Essentials;

namespace RecruitingApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPlayerPage : ContentPage
    {
        // used to keep track of information related to this player
        private Team currentTeam;
        private long cellPhoneNumber;

        private List<Player> teamPlayers;

        public AddPlayerPage()
        {
            InitializeComponent();
            playerRating.SelectedIndex = 0;            
        }

        // used to reload the database, it comes from the ContentPage class
        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Team>();
                var teamList = conn.Table<Team>().ToList();
                currentTeam = teamList.Where(team => team.ID == MainPage.selectedTeam).FirstOrDefault();
                teamPlayers = conn.Table<Player>().Where(player => player.TeamID == MainPage.selectedTeam).ToList();
            }
            BindingContext = currentTeam;

        }

        // SavePlayerButton_Clicked
        //      validates input and then saves the player
        async private void SavePlayerButton_Clicked(object sender, EventArgs e)
        {
            playerNumber.BackgroundColor = Color.Transparent;
            email.BackgroundColor = Color.Transparent;
            cellPhone.BackgroundColor = Color.Transparent;

            string errorMessages = "";
            bool errorFound = false;
            if (playerNumber.Text == null || playerNumber.Text == "")
            {
                errorMessages += "Please enter a number for this player.\n";
                errorFound = true;
                playerNumber.BackgroundColor = Color.FromHex("#f8a5c2");
            }
            
            if (cellPhone.Text != null)
            {
                if (cellPhone.Text != "")
                {
                    try
                    {
                        cellPhoneNumber = long.Parse(cellPhone.Text);
                    }
                    catch
                    {
                        errorMessages += "Please use numbers only for the player's telephone.\n";
                        errorFound = true;
                        cellPhone.BackgroundColor = Color.FromHex("#f8a5c2");
                    }
                }
            }
            if (email.Text != null)
            {
                if (email.Text != "")
                {
                    try
                    {
                        new MailAddress(email.Text);
                    }
                    catch
                    {
                        errorMessages += "Please specify a correct email.\n";
                        errorFound = true;
                        email.BackgroundColor = Color.FromHex("#f8a5c2");
                    }
                }
            }

            if (errorFound)
            {
                await DisplayAlert("Cannot Add Player", errorMessages, "OK");
                return;
            }

            // check to see if a player on the team already has this number, sometimes teams have two players with the same number but make sure user is still ok with it
            foreach (var player in teamPlayers)
            {
                if (player.Number == playerNumber.Text)
                {
                    bool answer = await DisplayAlert("Alert", $"There is already a player on this team with the same number.  Add anyway?", "Yes", "Cancel");
                    if (!answer)
                    {
                        playerNumber.BackgroundColor = Color.FromHex("#f8a5c2");
                        return;
                    }
                    
                }
            }

            // needed so there is no error when doing the search functionality on the player search feature
            if (playerFirstName.Text == null)
            {
                playerFirstName.Text = "";
            }
            if (playerLastName.Text == null)
            {
                playerLastName.Text = "";
            }

            
            Player playerToAdd = new Player
            {
                Number = playerNumber.Text,
                FirstName = playerFirstName.Text,
                LastName = playerLastName.Text,
                Rating = playerRating.Items[playerRating.SelectedIndex],
                Position = position.Text,
                Email = email.Text,
                Cell = cellPhoneNumber,
                Address = address.Text,
                City = city.Text,
                State = state.Text,
                ZIP = zip.Text,
                Notes = notes.Text,
                ActivelyRecruiting = activelyRecruiting.IsChecked,
                Created = DateTime.UtcNow,
                TeamID = currentTeam.ID
            };
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                Console.WriteLine(playerToAdd);
                conn.Insert(playerToAdd);
            }

            await Navigation.PopAsync();
        }
    }
}
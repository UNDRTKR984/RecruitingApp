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
    public partial class EditPlayerPage : ContentPage
    {
        private Team currentTeam;
        private AgeGroup currentAgeGroup;
        private List<Player> associatedPlayers;
        private long cellPhoneNumber;
        private Player currentPlayer;


        public EditPlayerPage()
        {
            InitializeComponent();
            
        }

        // used to reload the database, it comes from the ContentPage class
        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Team>();
                var playerList = conn.Table<Team>().ToList();
                currentTeam = playerList.Where(course => course.ID == MainPage.selectedTeam).FirstOrDefault();
                associatedPlayers = conn.Table<Player>().Where(assessment => assessment.TeamID == MainPage.selectedTeam).ToList();
                currentPlayer = associatedPlayers.Where(assessment => assessment.ID == MainPage.selectedPlayer).FirstOrDefault();
                currentAgeGroup = conn.Table<AgeGroup>().Where(AgeGroup => AgeGroup.ID == MainPage.selectedAgeGroup).FirstOrDefault();
            }

            playerNumber.Text = currentPlayer.Number;
            playerFirstName.Text = currentPlayer.FirstName;
            playerLastName.Text = currentPlayer.LastName;
            position.Text = currentPlayer.Position;            
            foreach (var index in playerRating.Items)
            {
                if (index == currentPlayer.Rating)
                {
                    playerRating.SelectedItem = index;
                }
            }
            email.Text = currentPlayer.Email;
            cellPhone.Text = currentPlayer.Cell.ToString();
            address.Text = currentPlayer.Address;
            city.Text = currentPlayer.City;
            zip.Text = currentPlayer.ZIP;
            activelyRecruiting.IsChecked = currentPlayer.ActivelyRecruiting;
            notes.Text = currentPlayer.Notes;


            BindingContext = currentTeam;

        }

        // SavePlayerButton_Clicked
        //      validates input and then saves/updates the player
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
                await DisplayAlert("Cannot Update Player", errorMessages, "OK");
                return;
            }

            // check to see if another player on the team already has this number, sometimes teams have two players with the same number but make sure user is still ok with it
            foreach (var player in associatedPlayers)
            {
                if (player.ID == currentPlayer.ID)
                {
                    continue;
                }
                if (player.Number == playerNumber.Text)
                {
                    bool answer = await DisplayAlert("Alert", $"There is already another player on this team with the same number.  Save anyway?", "Yes", "Cancel");
                    if (!answer)
                    {
                        playerNumber.BackgroundColor = Color.FromHex("#f8a5c2");
                        return;
                    }

                }
            }


            currentPlayer.Number = playerNumber.Text;
            currentPlayer.FirstName = playerFirstName.Text;
            currentPlayer.LastName = playerLastName.Text;
            currentPlayer.Rating = playerRating.Items[playerRating.SelectedIndex];
            currentPlayer.Position = position.Text;
            currentPlayer.Email = email.Text;
            currentPlayer.Cell = cellPhoneNumber;
            currentPlayer.Address = address.Text;
            currentPlayer.City = city.Text;
            currentPlayer.State = state.Text;
            currentPlayer.ZIP = zip.Text;
            currentPlayer.Notes = notes.Text;
            currentPlayer.LastUpdated = DateTime.UtcNow;
            currentPlayer.ActivelyRecruiting = activelyRecruiting.IsChecked;
            currentPlayer.TeamID = currentTeam.ID;
           
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.Update(currentPlayer);
            }

            await Navigation.PopAsync();
        }

        // DeletePlayerButton_Clicked
        //      checks to make sure user wants to delete, and if so, deletes the player
        async private void DeletePlayerButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Alert", $"Are you sure you want to delete {currentPlayer.FirstName} {currentPlayer.LastName} from {currentTeam.Name}?", "Yes", "Cancel");
            if (answer)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {

                    conn.Delete(currentPlayer);

                }
                await Navigation.PopAsync();
            }
        }

        // emailButton_Clicked
        //      sends notes and player information to the email of choice
        async private void emailButton_Clicked(object sender, EventArgs e)
        {
            string errorMessages = "";
            bool errorFound = false;
            if ((playerNumber.Text == null || playerNumber.Text == "") && (playerFirstName.Text == null || playerFirstName.Text == "") && (playerLastName.Text == null || playerLastName.Text == ""))
            {
                errorMessages += "Please specify the player's number, first name, or last name.\n";
                errorFound = true;
            }
            if (notes.Text == null || notes.Text == "")
            {
                errorMessages += "There aren't any notes to send.\n";
                errorFound = true;
            }
            if (errorFound)
            {
                await DisplayAlert("Can't Send Notes", errorMessages, "OK");
                return;
            }

            // save progress before mailing
            currentPlayer.Number = playerNumber.Text;
            currentPlayer.FirstName = playerFirstName.Text;
            currentPlayer.LastName = playerLastName.Text;
            currentPlayer.Rating = playerRating.Items[playerRating.SelectedIndex];
            currentPlayer.Position = position.Text;
            currentPlayer.Email = email.Text;
            currentPlayer.Cell = cellPhoneNumber;
            currentPlayer.Address = address.Text;
            currentPlayer.City = city.Text;
            currentPlayer.State = state.Text;
            currentPlayer.ZIP = zip.Text;
            currentPlayer.Notes = notes.Text;
            currentPlayer.LastUpdated = DateTime.UtcNow;
            currentPlayer.ActivelyRecruiting = activelyRecruiting.IsChecked;
            currentPlayer.TeamID = currentTeam.ID;

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.Update(currentPlayer);
            }

            string result = await DisplayPromptAsync("Email Notes", "What email address should this be sent to?", "OK", "Cancel", "Email");

            if (result != null)
            {
                await Email.ComposeAsync($"#{playerNumber.Text} {playerFirstName.Text} {playerLastName.Text} from {currentTeam.Name} - {currentAgeGroup.Name}", notes.Text, result);
            }
        }
    }
}

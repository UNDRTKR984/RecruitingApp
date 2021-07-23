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
    public partial class EditTeamPage : ContentPage
    {
        // keeps track of the current Course and everything that corresponds with it
        private Team currentTeam;
        private AgeGroup curentAgeGroup;
        private List<Player> players;

        public string TitleText
        {

            get
            {
                return string.Format("{0} - {1:yyyy}", currentTeam.Name, curentAgeGroup.Year);
            }
        }

        public EditTeamPage()
        {
            InitializeComponent();


        }

        // used to reload the database and information about the course everytime page appears, it comes from the ContentPage class
        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                currentTeam = conn.Table<Team>().Where(team => team.ID == MainPage.selectedTeam).FirstOrDefault();
                curentAgeGroup = conn.Table<AgeGroup>().Where(ageGroup => ageGroup.ID == currentTeam.AgeGroupID).FirstOrDefault();
                players = conn.Table<Player>().Where(player => player.TeamID == MainPage.selectedTeam).OrderBy(player => player.Number).ToList();

            }

            TeamName.Text = currentTeam.Name;
            
            foreach (var index in level.Items)
            {
                if (index == currentTeam.Level)
                {
                    level.SelectedItem = index;
                }
            }
            CoachName.Text = currentTeam.CoachName;
            CoachPhone.Text = currentTeam.CoachPhone;
            CoachEmail.Text = currentTeam.CoachEmail;
            
            Title = TitleText;
            playersListView.ItemsSource = players;
        }

        // saveCourseButton_Clicked
        //      validates input and then updates this course in the table
        async private void saveTeamButton_Clicked(object sender, EventArgs e)
        {
            TeamName.BackgroundColor = Color.Transparent;
            CoachName.BackgroundColor = Color.Transparent;
            CoachEmail.BackgroundColor = Color.Transparent;
            CoachPhone.BackgroundColor = Color.Transparent;


            string errorMessages = "";
            bool errorFound = false;
            if (TeamName.Text == null || TeamName.Text == "")
            {
                errorMessages += "Please specify a Team name.\n";
                errorFound = true;
                TeamName.BackgroundColor = Color.FromHex("#f8a5c2");

            }

            if (CoachPhone.Text != null)
            {
                if (CoachPhone.Text != "")
                {
                    try
                    {
                        long.Parse(CoachPhone.Text);
                    }
                    catch
                    {
                        errorMessages += "Please use numbers only for the Coach's telephone.\n";
                        errorFound = true;
                        CoachPhone.BackgroundColor = Color.FromHex("#f8a5c2");
                    }
                }

            }
            if (CoachEmail.Text != null)
            {
                if (CoachEmail.Text != "")
                {
                    try
                    {
                        new MailAddress(CoachEmail.Text);
                    }
                    catch
                    {
                        errorMessages += "Please specify a correct email.\n";
                        errorFound = true;
                        CoachEmail.BackgroundColor = Color.FromHex("#f8a5c2");
                    }
                }
            }



            if (errorFound)
            {
                await DisplayAlert("Cannot Add Team", errorMessages, "OK");
                return;
            }
            
            currentTeam.Name = TeamName.Text;
            currentTeam.Level = level.Items[level.SelectedIndex];
            currentTeam.CoachName = CoachName.Text;
            currentTeam.CoachPhone = CoachPhone.Text;
            currentTeam.CoachEmail = CoachEmail.Text;
            currentTeam.LastUpdated = DateTime.UtcNow;



            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.Update(currentTeam);
            }

            await Navigation.PopAsync();
        }

        // deleteCourseButton_Clicked
        //      checks to see if the user really wants to delete, and if so, deletes this course and all associated assessments
        async private void deleteTeamButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Alert", $"Are you sure you want to delete the {currentTeam.Name} team?", "Yes", "Cancel");

            if (answer)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    // delete all associated assessments with this course
                    conn.Table<Player>().Where(player => player.TeamID == currentTeam.ID).Delete();
                    // delete this course
                    conn.Delete(currentTeam);

                }

                await Navigation.PopAsync();
            }

        }

        // AddPlayerButton_Clicked
        //      navigates the user to an Add Player Page
        async private void AddPlayerButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddPlayerPage());
        }

        // playerListView_ItemTapped
        //      Navigates the user to an EditPlayerPage with the corresponding player info loaded into the page
        async private void playerListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as Player;
            MainPage.selectedPlayer = item.ID;
            await Navigation.PushAsync(new EditPlayerPage());
        }

        private void playerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            playersListView.ItemsSource = players
                .Where(player => player.FirstName.ToUpper().StartsWith(e.NewTextValue.ToUpper()) 
                || player.LastName.StartsWith(e.NewTextValue.ToUpper()) 
                || player.Number.StartsWith(e.NewTextValue))
                .ToList();
        }

        // emailButton_Clicked
        //      allows the user to send the notes via an email -- requires certain fields to be filled out
        /*async private void emailButton_Clicked(object sender, EventArgs e)
        {
            string errorMessages = "";
            bool errorFound = false;
            if (CourseName.Text == null || CourseName.Text == "")
            {
                errorMessages += "Please specify a Course name.\n";
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

            string result = await DisplayPromptAsync("Email Notes", "What email address should this be sent to?", "OK", "Cancel", "Email");

            if (result != null)
            {
                await Email.ComposeAsync($"{currentTeam.Name} Course Notes", notes.Text, result);
            }

        }*/
    }
}
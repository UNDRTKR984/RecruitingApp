using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using RecruitingApp.Models;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace RecruitingApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTeamPage : ContentPage
    {
        // used to keep track of information related to this team
        private AgeGroup currentAge;
        public AddTeamPage()
        {
            InitializeComponent();
        }

        // used to reload database everytime page appears, it comes from the ContentPage class
        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<AgeGroup>();
                var termList = conn.Table<AgeGroup>().ToList();
                currentAge = termList.Where(term => term.ID == MainPage.selectedAgeGroup).FirstOrDefault();
            }

            
            this.BindingContext = currentAge;
        }

        // addTeamButton_Clicked
        //      validates input and adds the new team associated with the age group
        async private void addTeamButton_Clicked(object sender, EventArgs e)
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
                if(CoachPhone.Text != "")
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
            var tempTeam = new Team
            {
                Name = TeamName.Text,
                Level = level.Items[level.SelectedIndex],
                CoachName = CoachName.Text,
                CoachPhone = CoachPhone.Text,
                CoachEmail = CoachEmail.Text,
                Created = DateTime.UtcNow,
                AgeGroupID = MainPage.selectedAgeGroup,
                AgeGroupString = currentAge.Name
            };

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Team>();
                conn.Insert(tempTeam);
            }

            await Navigation.PopAsync();

        }

        // emailButton_Clicked
        //      allows the user to send the notes via an email -- requires certain fields to be filled out
       /* async private void emailButton_Clicked(object sender, EventArgs e)
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
                await Email.ComposeAsync($"{CourseName.Text} Course Notes", notes.Text, result);
            }

        }*/
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecruitingApp.Models;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecruitingApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAgeGroupPage : ContentPage
    {
        public List<Team> associatedTeams;
        private AgeGroup currentAge;

        public EditAgeGroupPage()
        {
            InitializeComponent();
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

                var teamList = conn.Table<Team>().OrderBy(team => team.Name, StringComparer.OrdinalIgnoreCase).ToList();
                associatedTeams = teamList.Where(course => course.AgeGroupID == MainPage.selectedAgeGroup).ToList();
                var tempAge = conn.Table<AgeGroup>().ToList();
                currentAge = tempAge.Where(term => term.ID == MainPage.selectedAgeGroup).FirstOrDefault();
                int tempIndex = 0;

                foreach (var year in yearTime.Items)
                {
                    if (currentAge.Year == new DateTime(Convert.ToInt32(year), 12, 31))
                    {
                        yearTime.SelectedIndex = tempIndex;
                        break;
                    }
                    tempIndex++;
                }

                this.BindingContext = currentAge;
                associatedTeamsListView.ItemsSource = associatedTeams;
            }
        }

        // saveButton_Clicked
        //      validates input and then updates the age group in the database
        async private void saveButton_Clicked(object sender, EventArgs e)
        {
            year.BackgroundColor = Color.Transparent;
            string errorMessages = "";
            bool errorFound = false;
            if (year.Text == null || year.Text == "")
            {
                errorMessages += "Please select a year.\n";
                errorFound = true;
                year.BackgroundColor = Color.FromHex("#f8a5c2");
            }
            try
            {
                currentAge.Year = new DateTime(Convert.ToInt32(yearTime.Items[yearTime.SelectedIndex]), 12, 31);

            }
            catch
            {
                errorMessages += "The selection is not a valid date.\n";
                errorFound = true;
                year.BackgroundColor = Color.FromHex("#f8a5c2");
            }

            foreach (var date in MainPage.ageGroupList)
            {
                if (date.ID == MainPage.selectedAgeGroup)
                {
                    continue;
                }
                if (date.Year == new DateTime(Convert.ToInt32(yearTime.Items[yearTime.SelectedIndex]), 12, 31))
                {
                    errorMessages += "This age group is already in the system.";
                    errorFound = true;
                    yearTime.BackgroundColor = Color.FromHex("#f8a5c2");
                }
            }

            if (errorFound)
            {
                await DisplayAlert("Cannot Add Age Group", errorMessages, "OK");
                return;
            }

            currentAge.LastUpdated = DateTime.UtcNow;

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.Update(currentAge);
            }

            await Navigation.PopAsync();
        }

        // deleteButton_Clicked
        //      checks to make sure the user wants to delete, and if so, deletes the Age Group from the database, along with all associated teams and playerss
        async private void deleteButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Alert", $"Are you sure you want to delete the {currentAge.Name} age group?", "Yes", "Cancel");
            if (answer)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    // delete all associated assessments with each course associated with this term
                    foreach (var teamToDelete in associatedTeams)
                    {
                        conn.Table<Player>().Where(assessment => assessment.TeamID == teamToDelete.ID).Delete();
                    }
                    // delete all courses associated with this term
                    conn.Table<Team>().Where(course => course.AgeGroupID == currentAge.ID).Delete();
                    // delete this term
                    conn.Delete(currentAge);

                    // used so that the load function on the MainPage doesn't reload dummy data on the same session
                    MainPage.deletedAllAgeGroups = conn.Table<Player>().Count() == 0 ? true : false;

                }
                await Navigation.PopAsync();
            }

        }

        // AddTeamButton_Clicked
        //      navigates to the AddTeamPage
        async private void AddTeamButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTeamPage());
        }

        // associatedCoursesTeamsView_ItemTapped
        //      finds the corresponding team that the user taps on and opens up an EditTeamPage
        async private void associatedTeamsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as Team;
            MainPage.selectedTeam = item.ID;
            await Navigation.PushAsync(new EditTeamPage());
        }

        // searchTeams_TextChanged
        //      searches through the teams and filters the teams beginning with what is entered into the entry box
        private void searchTeams_TextChanged(object sender, TextChangedEventArgs e)
        {
            associatedTeamsListView.ItemsSource = associatedTeams.Where(team => team.Name.ToUpper().StartsWith(e.NewTextValue.ToUpper())).ToList();
        }
    }
}
using RecruitingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecruitingApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAgeGroupPage : ContentPage
    {
        // keeps track of information related to the Term
        public static int termID;
        public static List<Team> courses;


        private DateTime tempYear;

        public AddAgeGroupPage()
        {
            InitializeComponent();
        }

        // saveButton_Clicked
        //      validates information before saving the Age Group
        async private void saveButton_Clicked(object sender, EventArgs e)
        {
            yearTime.BackgroundColor = Color.Transparent;
            string errorMessages = "";
            bool errorFound = false;
            if (yearTime.SelectedItem == null || yearTime.SelectedItem.ToString() == "")
            {
                errorMessages += "Please select a year.\n";
                errorFound = true;
                yearTime.BackgroundColor = Color.FromHex("#f8a5c2");
            }
            try
            {
                tempYear = new DateTime(Convert.ToInt32(yearTime.Items[yearTime.SelectedIndex]), 12, 31);

            }
            catch
            {
                errorMessages += "The selection is not a valid date.\n";
                errorFound = true;
                yearTime.BackgroundColor = Color.FromHex("#f8a5c2");
            }
            foreach (var date in MainPage.ageGroupList)
            {
                if (date.Year == tempYear)
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

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<AgeGroup>();
                conn.Insert(new AgeGroup
                {
                    Year = new DateTime(Convert.ToInt32(yearTime.Items[yearTime.SelectedIndex]), 12, 31),
                    Created = DateTime.UtcNow
                });
            }
            await Navigation.PopAsync();
        }
    }
}
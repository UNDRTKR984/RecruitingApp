using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecruitingApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private string errorText;
        public LoginPage()
        {
            InitializeComponent();
        }


        // incompleteEntry
        //      checks to see if any of the text box fields are missing information
        private bool incompleteEntry()
        {
            bool incomplete = false;

            Username.BackgroundColor = Color.Transparent;
            Password.BackgroundColor = Color.Transparent;

            if (Username.Text == null || Username.Text == "")
            {                           
               errorText += "Please specify a username.\n";

               Username.BackgroundColor = Color.FromHex("#f8a5c2");
               incomplete = true;
            }
            if (Password.Text == null || Username.Text == "")
            {            
                errorText += "Please specify a password.\n";
      
                Password.BackgroundColor = Color.FromHex("#f8a5c2");
                incomplete = true;
            }

            return incomplete;
        }

        async private void loginButton_Clicked(object sender, EventArgs e)
        {
            errorText = "";
            bool error = false;

            if (incompleteEntry())
            {
                await DisplayAlert("Login Failed", errorText, "OK");
                return;
            }

            // check input for correctness
            if (Username.Text.ToLower() != "test")
            {
                error = true;

                errorText += "The username that was entered is incorrect, please try again.\n";


                Username.BackgroundColor = Color.FromHex("#f8a5c2");
            }
            if (Password.Text.ToLower() != "test")
            {
                error = true;

                errorText += "The passord that was entered is incorrect, please try again.\n";


                Password.BackgroundColor = Color.FromHex("#f8a5c2");
            }
            if (Username.Text.ToLower() != Password.Text.ToLower())
            {
                error = true;

                errorText += "The username and password do not match, please try again.\n";

            }

            if (error)
            {
                await DisplayAlert("Login Failed", errorText, "OK");
                return;
            }

            App.Current.MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.FromHex("#9D2235")
            };
        }
    }
}
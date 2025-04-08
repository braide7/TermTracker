using System.Text.RegularExpressions;
using TermTrackerApp.Core.Services;

namespace TermTrackerApp.Views;


public partial class LoginPage : ContentPage
{
    private readonly IDatabaseService _databaseService;
    private readonly AuthService _authService;
    public LoginPage(IDatabaseService databaseService, AuthService authService)
	{
       
        InitializeComponent();
        BindingContext = this;
        _databaseService = databaseService;
        _authService = authService;
    }


    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Username and Password cannot be empty", "OK");
            return;
        }
        try
        {
            if (IsValidPassword(password))
            {
                if (await _authService.LoginUser(username, password))
                {
                    await DisplayAlert("Success", "Login Successful!", "OK");
                    Application.Current.MainPage = new NavigationPage(new TermList(_databaseService, _authService));
                }
                else
                {
                    await DisplayAlert("Error", "Invalid Username or Password", "OK");
                }
            }
        }
        catch (Exception ex) 
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }

        

       
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Username and Password cannot be empty", "OK");
            return;
        }

        if (IsValidPassword(password)) 
        {
            try
            {
                bool isRegistered = await _authService.RegisterUser(username, password);
                if (isRegistered)
                {
                    await DisplayAlert("Success", "Registration Successful!", "OK");
                    Application.Current.MainPage = new NavigationPage(new TermList(_databaseService, _authService));
                }
                else
                {
                    await DisplayAlert("Error", "Username already exists", "OK");
                }
            }
            catch (Exception exc) 
            {
                await DisplayAlert("Error", $"An error occurred: {exc.Message}", "OK");
            }
        }


    }

    private bool IsValidPassword(string pass) 
    {
        string pattern = @"^(?=.*\d)(?=.*[!@#$%^&*]).{8,}$";

        // Check if password is null, empty, or doesn't match the pattern
        if (string.IsNullOrEmpty(pass) || !Regex.IsMatch(pass, pattern))
        {
            Password_Feedback.IsVisible = true;
            Password_Feedback.Text = "Password must contain 8 characters, one digit, and one special character.";
            return false;
        }

        Password_Feedback.IsVisible = false;
        Password_Feedback.Text = "";
        return true;
    }
}
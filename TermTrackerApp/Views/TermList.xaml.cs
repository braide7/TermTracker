using Plugin.LocalNotification;
using SQLite;
using System.Collections.ObjectModel;
using TermTrackerApp.Core.Models;
using TermTrackerApp.Core.Services;

namespace TermTrackerApp.Views;

public partial class TermList : ContentPage
{
    private readonly AuthService _authService;

    private readonly IDatabaseService _databaseService;
    public ObservableCollection<Term> Terms { get; set; } = new ObservableCollection<Term>();

    private int _userId = -1;
    public TermList(IDatabaseService databaseService, AuthService authService)
    {
        InitializeComponent();
        BindingContext = this;
        _databaseService = databaseService;
        _authService = authService;
        if (_authService.CurrentUserId.HasValue)
        {
            _userId = (int)_authService.CurrentUserId;
        }
        LoadTerms();
        

    }


    public async void NotificationHandler()
    {

        
        if (await Plugin.LocalNotification.LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await Plugin.LocalNotification.LocalNotificationCenter.Current.RequestNotificationPermission();
        }
    }





    protected override void OnAppearing()
    {
        
        base.OnAppearing();
        NotificationHandler();
        LoadTerms();
    }

    private async void LoadTerms()
    {
        var terms = await _databaseService.GetTermsAsync(_userId);
        Terms.Clear();
        if (terms.Any())
        {
            Empty_Terms.IsVisible = false;
        }
        else
        {
            Empty_Terms.IsVisible = true;
        }


        foreach (var term in terms)
        {
            Terms.Add(term);
        }

    }

    private async void OnTermSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            var selectedTerm = (Term)e.CurrentSelection[0];
            await Navigation.PushAsync(new TermPage(selectedTerm, _databaseService, _authService));
        }
    }

    private void OnAddTermClicked(object sender, EventArgs e)
    {
        // Navigate to an empty term detail page for adding a new term
        Navigation.PushAsync(new TermPage(new Term(), _databaseService, _authService));
    }

    private async void onLogoutClicked(object sender, EventArgs e)
    {
        if (await _authService.LogoutUser()) 
        {
            Application.Current.MainPage = new NavigationPage(new LoginPage(_databaseService, _authService));
        }
        else
        {
            DisplayAlert("Error", "Logout Failed", "OK");
        }
    }

    private async void ReportButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ReportPage(_databaseService, _authService));
    }
}
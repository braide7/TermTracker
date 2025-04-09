using Microsoft.Maui.Controls.Compatibility;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using TermTrackerApp.Core.Services;
using TermTrackerApp.Core.Models;



namespace TermTrackerApp.Views;

public partial class TermPage : ContentPage
{
    private Term _term;

    private readonly AuthService _authService;

    private readonly IDatabaseService _databaseService;

    private bool _isEditing = false;

    private bool userInput = false;
    private int _userId = -1;


    public ObservableCollection<Course> CourseList { get; set; } = new ObservableCollection<Course>();

    public TermPage(Term term, IDatabaseService databaseService, AuthService authService)
    {
        InitializeComponent();
        _term = term;
        _databaseService = databaseService;
        _authService = authService;
        if (_authService.CurrentUserId.HasValue)
        {
            _userId = (int)_authService.CurrentUserId;
        }


        if (_term.Title == null)
        {
            _isEditing = true;
            isEditing(_isEditing);

            AddCourseButton.IsVisible = false;
            AddCourseButton.IsEnabled = false;

            Courses_label.IsVisible = false;

            EditButton.IsVisible = false;
            EditButton.IsEnabled = false;

            DateTime beginningDate = new DateTime(2024, 1, 1);
            DateTime endingDate = new DateTime(2024, 6, 30);
            StartDatePicker.MinimumDate = beginningDate;
            EndDatePicker.MinimumDate = endingDate;


        }



        BindingContext = this;
        LoadTermDetails();

    }




    private void LoadTermDetails()
    {
        if (_term.Title != null)
        {
            TitleEntry.Text = _term.Title;
        }

        StartDatePicker.Date = _term.StartDate;
        EndDatePicker.Date = _term.EndDate;
        LoadCourses();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CoursesList.SelectedItem = null;
        LoadCourses();
    }

    private async void LoadCourses()
    {
        try
        {

            var courses = await _databaseService.GetCoursesAsync(_term.Id, _userId);
            CourseList.Clear();

            if (courses.Any())
            {
                Empty_Courses.IsVisible = false;
            }
            else
            {
                Empty_Courses.IsVisible = true;
            }

            if (courses != null)
            {
                

                if ( _term.Id > 0)
                {
                    AddCourseButton.IsVisible = true;
                    AddCourseButton.IsEnabled = true;
                }
                else
                {
                    AddCourseButton.IsVisible = false;
                    AddCourseButton.IsEnabled = false;
                }
                foreach (var course in courses)
                {
                    CourseList.Add(course);
                }
            }



            OnPropertyChanged(nameof(CourseList));
        }
        catch (Exception ex)
        {
            await DisplayAlert($"Error: {ex.Message}", "An error occurred while loading courses", "OK");
        }



    }

    private void isEditing(bool editing)
    {
        TitleEntry.IsEnabled = editing;
        StartDatePicker.IsEnabled = editing;
        EndDatePicker.IsEnabled = editing;


        EditButton.IsVisible = !editing;
        EditButton.IsEnabled = !editing;

        validateEntries();

    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        _isEditing = !_isEditing;

        isEditing(_isEditing);

    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {

        _term.Title = TitleEntry.Text.Trim();
        _term.StartDate = StartDatePicker.Date;
        _term.EndDate = EndDatePicker.Date;
       


        try
        {
            _term.UserId = _userId;
            if (_term.Id > 0)
            {
                await _databaseService.UpdateTermAsync(_term);
            }
            else
            {
                int id = await _databaseService.SaveTermAsync(_term);
                _term.Id = id; 
            }



            _isEditing = false;
            isEditing(_isEditing);
            SaveButton.IsEnabled = false;
            SaveButton.IsVisible = false;

            AddCourseButton.IsEnabled = true;
            AddCourseButton.IsVisible = true;

            bool confirm = await DisplayAlert("Term Saved", $"{_term.Title} has been saved", $"Back to Term List", "Okay");
            if (confirm)
            {
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert($"Error: {ex.Message}", "An error occurred while saving the term", "OK");
        }




    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Delete", "Are you sure you want to delete this term?", "Yes", "No");
        if (confirm)
        {

            await _databaseService.DeleteTermAsync(_term);
            await Navigation.PopAsync();
        }
    }

    private void TitleEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        userInput = true;
        validateEntries();

    }

    private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        validateEntries();

    }

    private void EndDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        validateEntries();

    }

    private void validateEntries()
    {
        bool valid = true;
        if (userInput)
        {
            if (TitleEntry.Text == null || TitleEntry.Text == "")
            {
                TitleEntry_FeedbackLabel.Text = "Please enter a term name";
                TitleEntry_FeedbackLabel.IsVisible = true;
                valid = false;
            }

            if (StartDatePicker.Date >= EndDatePicker.Date)
            {

                Date_FeedbackLabel.IsVisible = true;
                Date_FeedbackLabel.Text = "Term Start Date Must Be Before End Date";
                valid = false;
            }

            if (valid)
            {
                if (_isEditing)
                {
                    SaveButton.IsEnabled = true;
                    SaveButton.IsVisible = true;
                }

                Date_FeedbackLabel.IsVisible = false;
                TitleEntry_FeedbackLabel.IsVisible = false;
            }
            else
            {
                SaveButton.IsEnabled = false;
                SaveButton.IsVisible = false;
            }
        }

    }

    private void AddCourseButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CoursePage(new Course(), _term, _databaseService, _authService));
    }

    private async void CoursesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {

            Course selectedCourse = (Course)e.CurrentSelection[0];

            // Navigate to the course page
            await Navigation.PushAsync(new CoursePage(selectedCourse, _term, _databaseService, _authService));


            CoursesList.SelectedItem = null;

        }
    }

    private async void OnHome_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
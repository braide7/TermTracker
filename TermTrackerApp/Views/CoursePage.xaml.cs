using Plugin.LocalNotification;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using TermTrackerApp.Core.Models;
using TermTrackerApp.Core.Services;


namespace TermTrackerApp.Views;

public partial class CoursePage : ContentPage
{
    private Course _course;

    private bool userInput = false;

    private readonly AuthService _authService;

    private readonly IDatabaseService _databaseService;


    private INotifyService _notificationService = new NotificationService(new LocalNotificationCenterAdapter());

    private bool _isEditing = false;
    private Term _term;
    private int? _existingAssessment = null;
    private int _userId = -1;

    public ObservableCollection<Assessment> AssessmentList { get; set; } = new ObservableCollection<Assessment>();
    public CoursePage(Course course, Term term, IDatabaseService databaseService, AuthService authService)
    {
        _course = course;
        _term = term;
        _course.TermId = _term.Id;



        InitializeComponent();
        _databaseService = databaseService;
        _authService = authService;
        if (_authService.CurrentUserId.HasValue)
        {
            _userId = (int)_authService.CurrentUserId;
        }

        DateTime beginningDate = _term.StartDate.Date;
        DateTime endingDate = _term.EndDate.Date;
        StartDatePicker.MinimumDate = beginningDate;
        StartDatePicker.MaximumDate = endingDate;
        EndDatePicker.MinimumDate = beginningDate.AddDays(1);
        EndDatePicker.MaximumDate = endingDate;

        if (_course.Name == null)
        {
            _isEditing = true;
            isEditing(_isEditing);

            AddAssessmentButton.IsVisible = false;
            AddAssessmentButton.IsEnabled = false;

            Assessment_label.IsVisible = false;

            EditButton.IsVisible = false;
            EditButton.IsEnabled = false;

            StartDatePicker.Date = beginningDate.Date;
            EndDatePicker.Date = endingDate.Date;

        }
        BindingContext = this;
        LoadCourseDetails();
       
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AssessmentsList.SelectedItem = null;
        LoadAssessments();
    }

    private async void LoadAssessments()
    {

        try
        {

            var assessments = await _databaseService.GetAssessmentsAsync(_course.Id, _userId);
            AssessmentList.Clear();

            if (assessments == null || assessments.Count() == 0)
            {
                Empty_Assessments.IsVisible = true;
            }
            else
            {
                Empty_Assessments.IsVisible = false;
            }

            if (assessments != null)
            {
                
                foreach (var assessment in assessments)
                {
                    AssessmentList.Add(assessment);
                    if (assessment is ObjectiveAssessment)
                    {
                        _existingAssessment = 0;
                    }
                    else
                    {
                        _existingAssessment = 1;
                    }
                }
                if (assessments.Count() < 2 && _course.Id > 0)
                {
                    AddAssessmentButton.IsVisible = true;
                    AddAssessmentButton.IsEnabled = true;
                }
                else
                {
                    AddAssessmentButton.IsVisible = false;
                    AddAssessmentButton.IsEnabled = false;
                    if (assessments.Count() == 2)
                    {
                        _existingAssessment = 2;
                    }
                    else 
                    {
                        _existingAssessment = null;
                    }
                }



            }
            else 
            {
                _existingAssessment = -1;
                
            }

            OnPropertyChanged(nameof(AssessmentList));
        }
        catch (Exception ex)
        {
            await DisplayAlert($"Error: {ex.Message}", "An error occurred while loading assessments", "OK");
        }


    }

    private void LoadCourseDetails()
    {
        CourseNameEntry.Text = _course.Name;
        StartDatePicker.Date = _course.StartDate;
        EndDatePicker.Date = _course.EndDate;
        InstructorNameEntry.Text = _course.InstructorName;
        InstructorEmailEntry.Text = _course.InstructorEmail;
        InstructorNumberEntry.Text = _course.InstructorPhone;
        CourseStatusPicker.SelectedItem = _course.Status;
        startSwitch.IsToggled = _course.StartNotification;
        endSwitch.IsToggled = _course.EndNotification;
        NotesEditor.Text = _course.Notes;

        LoadAssessments();
    }

    private void isEditing(bool editing)
    {
        CourseNameEntry.IsEnabled = editing;
        StartDatePicker.IsEnabled = editing;
        EndDatePicker.IsEnabled = editing;
        InstructorNameEntry.IsEnabled = editing;
        InstructorEmailEntry.IsEnabled = editing;
        InstructorNumberEntry.IsEnabled = editing;
        CourseStatusPicker.IsEnabled = editing;
        startSwitch.IsEnabled = editing;
        endSwitch.IsEnabled = editing;
        NotesEditor.IsEnabled = editing;

        EditButton.IsVisible = !editing;
        EditButton.IsEnabled = !editing;



        validateEntries();

    }

    private void EditButton_Clicked(object sender, EventArgs e)
    {
        _isEditing = !_isEditing;


        isEditing(_isEditing);


    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        _course.Name = CourseNameEntry.Text.Trim();
        _course.StartDate = StartDatePicker.Date;
        _course.EndDate = EndDatePicker.Date.Date;
        _course.InstructorName = InstructorNameEntry.Text.Trim();
        _course.InstructorPhone = InstructorNumberEntry.Text.Trim();
        _course.InstructorEmail = InstructorEmailEntry.Text.Trim();
        _course.StatusIndex = CourseStatusPicker.SelectedIndex;
        _course.Status = GetStatusFromIndex(_course.StatusIndex);
        _course.StartNotification = startSwitch.IsToggled;
        _course.EndNotification = endSwitch.IsToggled;
        if (string.IsNullOrWhiteSpace(NotesEditor.Text))
        {
            _course.Notes = "";
        }
        else
        {
            _course.Notes = NotesEditor.Text.Trim();
        }



        try
        {
            _course.UserId = _userId;

            if (_course.Id > 0)
            {

                await _databaseService.UpdateCourseAsync(_course);
            }
            else
            {
                
                int id= await _databaseService.SaveCourseAsync(_course);
                _course.Id = id; 
                
            }
            if (_course.StartNotification)
            {
                int id = 100 + _course.Id;
                await _notificationService.SetNotification(_course.StartDate, id, "Course Start", $"Course {_course.Name} starts today");
            }
            else 
            {
                int id = 100 + _course.Id;
                await _notificationService.CancelNotification(id);
            }

            if (_course.EndNotification)
            {
                int id = 500 + _course.Id;
                await _notificationService.SetNotification(_course.EndDate, id, "Course End", $"Course {_course.Name} ends today");
            }
            else 
            {
                int id = 500 + _course.Id;
                await _notificationService.CancelNotification(id);
            }
            _term.Courses.Add(_course);
            await _databaseService.UpdateTermAsync(_term);

            _isEditing = false;
            isEditing(_isEditing);
            SaveButton.IsEnabled = false;
            SaveButton.IsVisible = false;

            LoadAssessments();
            //scrolls to bottom
            await CourseScrollView.ScrollToAsync(0, CourseScrollView.ContentSize.Height, true);

            bool confirm = await DisplayAlert("Course Saved", $"{_course.Name} has been saved", $"Back to {_term.Title}", "Okay");
            if (confirm)
            {
                await Navigation.PopAsync();
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert($"Error: {ex.Message}", "An error occurred while saving the course", "OK");
        }


    }


    private string GetStatusFromIndex(int statusIndex)
    {
        switch (statusIndex)
        {
            case 0:
                return "In Progress";
            case 1:
                return "Completed";
            case 2:
                return "Dropped";
            case 3:
                return "Plan to Take";
            default:
                return "Unknown";
        }
    }
    private async void OnDeleteButton_Clicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Delete", "Are you sure you want to delete this course?", "Yes", "No");
        if (confirm)
        {

            await _databaseService.DeleteCourseAsync(_course.Id);
            await Navigation.PopAsync();
        }
    }

    private void CourseNameEntry_TextChanged(object sender, TextChangedEventArgs e)
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

    private void AddAssessmentButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AssessmentPage(_course, _databaseService, _authService, null, null, _existingAssessment));
    }

    private void InstructorNameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        userInput = true;
        validateEntries();
    }

    private void InstructorNumberEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        userInput = true;
        validateEntries();
    }

    private void InstructorEmailEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        userInput = true;
        validateEntries();
    }



    private void CourseStatusPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        userInput = true;
        validateEntries();
    }

    private async void ShareButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NotesEditor.Text))
            {
                await DisplayAlert("Error", "No notes to share", "OK");
                return;
            }
            else
            {
                string fn = $"{_course.Name}Notes.txt";
                string file = Path.Combine(FileSystem.CacheDirectory, fn);

                File.WriteAllText(file, NotesEditor.Text);

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Share Course Notes",
                    File = new ShareFile(file)
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }


    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidNumber(string number) {
        if (!number.Any(c => char.IsLetter(c)) && number.Length >= 10)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    private void validateEntries()
    {
        bool valid = true;
        if (userInput)
        {
            if (string.IsNullOrWhiteSpace(CourseNameEntry.Text))
            {
                CourseName_FeedbackLabel.Text = "Please enter a course name";
                CourseName_FeedbackLabel.IsVisible = true;
                valid = false;
            }
            else
            {
                CourseName_FeedbackLabel.Text = "";
                CourseName_FeedbackLabel.IsVisible = false;
            }

            if (StartDatePicker.Date >= EndDatePicker.Date)
            {

                Date_FeedbackLabel.IsVisible = true;
                Date_FeedbackLabel.Text = "Course Start Date Must Be Before End Date";
                valid = false;
            }
            else
            {
                Date_FeedbackLabel.IsVisible = false;
                Date_FeedbackLabel.Text = "";
            }

            if (string.IsNullOrWhiteSpace(InstructorNameEntry.Text))
            {
                InstructorName_FeedbackLabel.Text = "Please enter an instructor name";
                InstructorName_FeedbackLabel.IsVisible = true;
                valid = false;
            }
            else
            {
                InstructorName_FeedbackLabel.Text = "";
                InstructorName_FeedbackLabel.IsVisible = false;
            }

            if (string.IsNullOrWhiteSpace(InstructorEmailEntry.Text))
            {
                InstructorEmail_FeedbackLabel.Text = "Please enter a valid email";
                InstructorEmail_FeedbackLabel.IsVisible = true;
                valid = false;
            } else if(!IsValidEmail(InstructorEmailEntry.Text.Trim())) 
            {
                InstructorEmail_FeedbackLabel.Text = "Please enter a valid email";
                InstructorEmail_FeedbackLabel.IsVisible = true;
                valid = false;
            }
            else
            {
                InstructorEmail_FeedbackLabel.Text = "";
                InstructorEmail_FeedbackLabel.IsVisible = false;
            }

            if (string.IsNullOrWhiteSpace(InstructorNumberEntry.Text) )
            {
                InstructorNumber_FeedbackLabel.Text = "Please enter a valid phone number - 10 digits & numbers only";
                InstructorNumber_FeedbackLabel.IsVisible = true;
                valid = false;
            }else if(!IsValidNumber(InstructorNumberEntry.Text.Trim()))
            {
                InstructorNumber_FeedbackLabel.Text = "Please enter a valid phone number - 10 digits & numbers only";
                InstructorNumber_FeedbackLabel.IsVisible = true;
                valid = false;
            }
            else
            {
                InstructorNumber_FeedbackLabel.Text = "";
                InstructorNumber_FeedbackLabel.IsVisible = false;
            }

            if (CourseStatusPicker.SelectedIndex == -1)
            {
                CourseStatus_FeedbackLabel.Text = "Please select a course status";
                CourseStatus_FeedbackLabel.IsVisible = true;
                valid = false;
            }
            else
            {
                CourseStatus_FeedbackLabel.Text = "";
                CourseStatus_FeedbackLabel.IsVisible = false;
            }


            if (valid)
            {
                if (_isEditing)
                {
                    SaveButton.IsEnabled = true;
                    SaveButton.IsVisible = true;
                }

            }
            else
            {
                SaveButton.IsEnabled = false;
                SaveButton.IsVisible = false;
            }
        }

    }

    private async void AssessmentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {

            Assessment selectedAssessment = (Assessment)e.CurrentSelection[0];
            int assessID = selectedAssessment.Id;
            int assessType = selectedAssessment.TypeIndex;

            // Navigate to the course page
            await Navigation.PushAsync(new AssessmentPage(_course, _databaseService, _authService, assessID, assessType, _existingAssessment));


            AssessmentsList.SelectedItem = null;

        }
    }

    private async void OnHome_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }

}
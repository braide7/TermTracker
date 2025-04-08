using System;
using System.Collections.ObjectModel;
using System.Drawing;
using TermTrackerApp.Core.Models;
using TermTrackerApp.Core.Services;


namespace TermTrackerApp.Views;

public partial class AssessmentPage : ContentPage
{
    private Course _course;
    private readonly AuthService _authService;
    private readonly IDatabaseService _databaseService;
    private bool _isEditing = false;
    private int _assessmentID;
    private int _assessmentType;
    private int? _existingAssessment;
    private INotifyService _notificationService = new NotificationService(new LocalNotificationCenterAdapter());
    private int _userId = -1;

    private List<Assessment> _assessment = new List<Assessment>();



    public AssessmentPage(Course course, IDatabaseService databaseService, AuthService authService, int? assessmentID = null, int? assessmentType = null, int? existingAssessment = null)
    {
        InitializeComponent();
        BindingContext = this;
        _course = course;

        _existingAssessment = existingAssessment;
        _databaseService = databaseService;
        _authService = authService;
        if (_authService.CurrentUserId.HasValue) 
        {
            _userId = (int)_authService.CurrentUserId;
        }

        DateTime beginningDate = _course.StartDate.Date;
        DateTime endingDate = _course.EndDate.Date;
        StartDatePicker.MinimumDate = beginningDate;
        StartDatePicker.MaximumDate = endingDate;

        EndDatePicker.MinimumDate = beginningDate;
        EndDatePicker.MaximumDate = endingDate;

        if (!assessmentID.HasValue || !assessmentType.HasValue)
        {
            _isEditing = true;
            isEditing(_isEditing);


            StartDatePicker.Date = beginningDate;
            EndDatePicker.Date = beginningDate.Date.AddDays(1);

        }
        else
        {
            _assessmentID = assessmentID.Value;
            _assessmentType = assessmentType.Value;
            LoadExistingAssessment();
        }

        validate();
    }

    private async void LoadExistingAssessment()
    {
        // Load polymorphically
        var assessment = await _databaseService.GetAssessmentAsync(_assessmentID, _userId);
        if (assessment == null)
        {
            await DisplayAlert("Error", "Assessment not found", "OK");
            await Navigation.PopAsync();
            return;
        }

        _assessment.Add(assessment);

        // Set common properties
        AssessmentNameEntry.Text = assessment.Name;
        StartDatePicker.Date = assessment.StartDate.Date;
        EndDatePicker.Date = assessment.EndDate.Date;
        startSwitch.IsToggled = assessment.StartNotification;
        endSwitch.IsToggled = assessment.EndNotification;

        // Set type-specific properties
        if (assessment is ObjectiveAssessment oa)
        {
            AssessmentPicker.SelectedIndex = 0;
            OAEntry.Text = oa.NumberOfQuestions.ToString();
            OAEntry.IsEnabled = true;
            OAEntry.IsVisible = true;
            PAEntry.IsEnabled = false;
            PAEntry.IsVisible = false;
        }
        else if (assessment is PerformanceAssessment pa)
        {
            AssessmentPicker.SelectedIndex = 1;
            PAEntry.SelectedIndex = pa.TaskTypeIndex;
            PAEntry.IsEnabled = true;
            PAEntry.IsVisible = true;
            OAEntry.IsEnabled = false;
            OAEntry.IsVisible = false;
        }

    }

    private void validate()
    {
        bool valid = true;
        if (AssessmentPicker.SelectedIndex == -1)
        {
            AssessmentType_FeedbackLabel.IsVisible = true;
            AssessmentType_FeedbackLabel.Text = "Please select the assessment type";
        }
        else
        {
            AssessmentType_FeedbackLabel.IsVisible = false;
            AssessmentType_FeedbackLabel.Text = "";
        }
        if (_existingAssessment.HasValue)
        {
            if (_existingAssessment == AssessmentPicker.SelectedIndex && _assessmentType != AssessmentPicker.SelectedIndex)
            {

                valid = false;
                AssessmentType_FeedbackLabel.IsVisible = true;
                AssessmentType_FeedbackLabel.Text = "You can only have one OA and PA per course";



            }
            else if (AssessmentPicker.SelectedIndex == -1)
            {
                AssessmentType_FeedbackLabel.IsVisible = true;
                AssessmentType_FeedbackLabel.Text = "Please select the assessment type";

            }
            else if (_existingAssessment == 2 && AssessmentPicker.SelectedIndex != _assessmentType) 
            {
                valid = false;
                AssessmentType_FeedbackLabel.IsVisible = true;
                AssessmentType_FeedbackLabel.Text = "You can only have one OA and PA per course";
            }
            else
            {
                AssessmentType_FeedbackLabel.IsVisible = false;
                AssessmentType_FeedbackLabel.Text = "";
            }



        }



        if (string.IsNullOrWhiteSpace(AssessmentNameEntry.Text))
        {
            AssessmentName_FeedbackLabel.Text = "Please enter an assessment name";
            AssessmentName_FeedbackLabel.IsVisible = true;
            valid = false;
        }
        else
        {
            AssessmentName_FeedbackLabel.Text = "";
            AssessmentName_FeedbackLabel.IsVisible = false;
        }




        if (StartDatePicker.Date >= EndDatePicker.Date)
        {

            Date_FeedbackLabel.IsVisible = true;
            Date_FeedbackLabel.Text = "Assessment Start Date Must Be Before End Date";
            valid = false;
        }
        else
        {
            Date_FeedbackLabel.IsVisible = false;
            Date_FeedbackLabel.Text = "";
        }
        if (PAEntry.IsVisible && PAEntry.SelectedIndex == -1)
        {
            PAType_FeedbackLabel.IsVisible = true;

            PAType_FeedbackLabel.Text = "Please select type";
            valid = false;
        }
        else
        {
            PAType_FeedbackLabel.IsVisible = false;
            PAType_FeedbackLabel.Text = "";
        }

        if (OAEntry.IsVisible && string.IsNullOrWhiteSpace(OAEntry.Text))
        {
            OANum_FeedbackLabel.IsVisible = true;
            OANum_FeedbackLabel.Text = "Please enter number of questions";
            valid = false;
        }
        else
        {
            OANum_FeedbackLabel.IsVisible = false;
            OANum_FeedbackLabel.Text = "";
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

    private void isEditing(bool editing)
    {
        AssessmentNameEntry.IsEnabled = editing;
        StartDatePicker.IsEnabled = editing;
        EndDatePicker.IsEnabled = editing;
        AssessmentPicker.IsEnabled = editing;

        startSwitch.IsEnabled = editing;
        endSwitch.IsEnabled = editing;


        EditButton.IsVisible = !editing;
        EditButton.IsEnabled = !editing;

        SaveButton.IsVisible = editing;
        SaveButton.IsEnabled = editing;





        validate();

    }

    private Assessment CreateAssessment()
    {
        Assessment assessment;

        if (AssessmentPicker.SelectedIndex == 0)
        {
            assessment = new ObjectiveAssessment
            {
                Name = AssessmentNameEntry.Text,
                StartDate = StartDatePicker.Date,
                EndDate = EndDatePicker.Date,
                CourseId = _course.Id,
                Type = "Objective Assessment",
                TypeIndex = 0,
                StartNotification = startSwitch.IsToggled,
                EndNotification = endSwitch.IsToggled,
                UserId = _userId,
                NumberOfQuestions = int.Parse(OAEntry.Text)
            };
        }
        else
        {
            assessment = new PerformanceAssessment
            {
                Name = AssessmentNameEntry.Text,
                StartDate = StartDatePicker.Date,
                EndDate = EndDatePicker.Date,
                CourseId = _course.Id,
                Type = "Performance Assessment",
                TypeIndex = 1,
                StartNotification = startSwitch.IsToggled,
                EndNotification = endSwitch.IsToggled,
                UserId = _userId,
                TaskTypeIndex = PAEntry.SelectedIndex,
                TaskType = PAEntry.SelectedItem?.ToString()
            };
        }

        return assessment;
    }

    private void AssessmentPicker_SelectedIndexChanged(object sender, EventArgs e)
    {

        if (AssessmentPicker.SelectedIndex == 0)
        {
            OALabel.IsVisible = true;
            OALabel.Text = "Number of Questions on OA";
            OAEntry.IsVisible = true;
            OAEntry.IsEnabled = true;

            PAEntry.IsVisible = false;
            PAEntry.IsEnabled = false;
            PALabel.IsVisible = false;
        }
        else if (AssessmentPicker.SelectedIndex == 1)
        {
            PALabel.IsVisible = true;
            PALabel.Text = "Task type for PA";
            PAEntry.IsVisible = true;
            PAEntry.IsEnabled = true;

            OALabel.IsVisible = false;
            OAEntry.IsVisible = false;
            OAEntry.IsEnabled = false;
        }
        else
        {
            OALabel.IsVisible = false;
            OAEntry.IsVisible = false;
            OAEntry.IsEnabled = false;

            PALabel.IsVisible = false;
            PAEntry.IsVisible = false;
            PAEntry.IsEnabled = false;
        }

        validate();
    }

    private void AssessmentNameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        validate();
    }

    private async void OnDeleteButton_Clicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Delete", "Are you sure you want to delete this assessment?", "Yes", "No");
        if (confirm)
        {
            var assessment = await _databaseService.GetAssessmentAsync(_assessmentID, _userId);
            if (assessment != null)
            {
                // Cancel notifications
                int startNotificationId = assessment is ObjectiveAssessment ? 1000 : 2000;
                int endNotificationId = assessment is ObjectiveAssessment ? 5000 : 7000;

                await _notificationService.CancelNotification(startNotificationId + _assessmentID);
                await _notificationService.CancelNotification(endNotificationId + _assessmentID);

                // Delete assessment polymorphically
                await _databaseService.DeleteAssessmentAsync(assessment);

                // Remove from course
                var assessmentToRemove = _course.Assessments.FirstOrDefault(a => a.Id == _assessmentID);
                if (assessmentToRemove != null)
                {
                    _course.Assessments.Remove(assessmentToRemove);
                    await _databaseService.UpdateCourseAsync(_course);
                }
            }

            await Navigation.PopAsync();
        }
    }

    private async void SetupNotifications(Assessment assessment)
    {
        int startNotificationId = assessment is ObjectiveAssessment ? 1000 : 2000;
        int endNotificationId = assessment is ObjectiveAssessment ? 5000 : 7000;

        if (assessment.StartNotification)
        {
            int id = startNotificationId + assessment.Id;
            await _notificationService.SetNotification(
                assessment.StartDate,
                id,
                "Assessment Start",
                $"Assessment {assessment.Name} starts today"
            );
        }

        if (assessment.EndNotification)
        {
            int id = endNotificationId + assessment.Id;
            await _notificationService.SetNotification(
                assessment.EndDate,
                id,
                "Assessment End",
                $"{assessment.Name} ends today"
            );
        }
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {

        try
        {
            // Create assessment using our helper method
            Assessment assessment = CreateAssessment();

            // Handle previous notifications if editing
            if (_assessmentID > 0)
            {
                // Cancel existing notifications
                int startNotificationId = assessment is ObjectiveAssessment ? 1000 : 2000;
                int endNotificationId = assessment is ObjectiveAssessment ? 5000 : 7000;

                await _notificationService.CancelNotification(startNotificationId + _assessmentID);
                await _notificationService.CancelNotification(endNotificationId + _assessmentID);

                // Delete the old assessment
                var oldAssessment = await _databaseService.GetAssessmentAsync(_assessmentID, _userId);
                await _databaseService.DeleteAssessmentAsync(oldAssessment);
            }

            // Save the assessment polymorphically
            assessment.Id = await _databaseService.SaveAssessmentAsync(assessment);

            // Set up notifications
            SetupNotifications(assessment);

            // Update course
            _course.Assessments.Add(assessment);
            await _databaseService.UpdateCourseAsync(_course);

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }


    }

    private void EditButton_Clicked(object sender, EventArgs e)
    {
        _isEditing = !_isEditing;


        isEditing(_isEditing);
    }

    private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        validate();
    }

    private void EndDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        validate();
    }



    private void OAEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        validate();
    }

    private void PAEntry_SelectedIndexChanged(object sender, EventArgs e)
    {
        validate();
    }

    private async void OnHome_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}
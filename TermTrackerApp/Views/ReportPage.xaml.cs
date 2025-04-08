
using System.Collections.ObjectModel;
using TermTrackerApp.Core.Services;
using Newtonsoft.Json;
using TermTrackerApp.Core.Models;



namespace TermTrackerApp.Views;

public partial class ReportPage : ContentPage
{

    private readonly AuthService _authService;

    private readonly IDatabaseService _databaseService;

    private int _userId = -1;

    IEnumerable<Course> courses = new List<Course>();
    public ObservableCollection<Course> CourseList { get; set; } = new ObservableCollection<Course>();
    public ReportPage(IDatabaseService databaseService, AuthService authService)
    {
        InitializeComponent();
        BindingContext = this;
        _databaseService = databaseService;
        _authService = authService;
        if (_authService.CurrentUserId.HasValue)
        {
            _userId = (int)_authService.CurrentUserId;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CoursesList.SelectedItem = null;
        CourseList.Clear();
        LoadCourses();

    }

    private async void LoadCourses()
    {
        Report_Label.IsVisible = false;
        try
        {
            
            if (SearchByName.IsChecked)
            {
                string textIn = SearchEntry.Text.Trim();
                courses = await _databaseService.GetCoursesByNameAsync(textIn, _userId);

                Report_Label.Text = "Courses By Name";

            }
            else if (SearchByDate.IsChecked)
            {
                courses = await _databaseService.GetCoursesByDateAsync(StartDatePicker.Date, EndDatePicker.Date, _userId);
                Report_Label.Text = "Courses By Date Range";

            }

            CourseList.Clear();



            if (courses != null && courses.Any())
            {

                foreach (var course in courses)
                {
                    CourseList.Add(course);
                }
                Report_Label.IsVisible = true;
                Report_Frame.IsVisible = true;
                ExportReportButton.IsVisible = true;
                Line1.IsVisible = true;
                Line2.IsVisible = true;
            }



            OnPropertyChanged(nameof(CourseList));
        }
        catch (Exception ex)
        {
            await DisplayAlert($"Error: {ex.Message}", "An error occurred while loading courses", "OK");
        }



    }

    private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        DateEntry_Feedback.IsVisible = false;
    }

    private void EndDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        DateEntry_Feedback.IsVisible = false;
    }

    private async void OnHome_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Report_Label.IsVisible = false;
        SearchEntry_Feedback.IsVisible = false;
        DateEntry_Feedback.IsVisible = false;
        CourseList.Clear();
        SearchEntry.Text = "";
        Report_Frame.IsVisible = false;

        ExportReportButton.IsVisible = false;
        Line1.IsVisible = false;
        Line2.IsVisible = false;

        if (SearchByDate.IsChecked)
        {
            DateLayout.IsVisible = true;
            SearchEntry.IsVisible = false;
            RunReportButton.IsVisible = true;
        }
        else if (SearchByName.IsChecked)
        {
            DateLayout.IsVisible = false;
            SearchEntry.IsVisible = true;
            RunReportButton.IsVisible = true;
        }
        else
        {
            DateLayout.IsVisible = false;
            SearchEntry.IsVisible = false;
            RunReportButton.IsVisible = false;
        }
    }

    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchEntry_Feedback.IsVisible = false;
    }

    private void RunReportButton_Clicked(object sender, EventArgs e)
    {
        if (validate())
        {
            LoadCourses();
        }

    }

    private bool validate()
    {
        bool valid = true;
        if (SearchByName.IsChecked && string.IsNullOrWhiteSpace(SearchEntry.Text))
        {
            SearchEntry_Feedback.IsVisible = true;
            valid = false;
        }
        else
        {
            SearchEntry_Feedback.IsVisible = false;
        }

        if (SearchByDate.IsChecked && StartDatePicker.Date > EndDatePicker.Date)
        {
            DateEntry_Feedback.IsVisible = true;
            valid = false;
        }
        else
        {
            DateEntry_Feedback.IsVisible = false;
        }

        return valid;
    }

    private async void CoursesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {

            Course selectedCourse = (Course)e.CurrentSelection[0];

            Term term = await _databaseService.GetTermByIDAsync(selectedCourse.TermId, _userId);

            // Navigate to the course page
            await Navigation.PushAsync(new CoursePage(selectedCourse, term, _databaseService, _authService));


            CoursesList.SelectedItem = null;

        }
    }

    private async void ExportReportButton_Clicked(object sender, EventArgs e)
    {

        try
        {
            // Create the report data with timestamp
            var reportData = new
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                CourseData = courses
            };

            // Serialize to JSON
            string jsonString = JsonConvert.SerializeObject(reportData);
            string fileName = "CourseReport.txt";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            // Write the file and ensure it's properly saved
            await File.WriteAllTextAsync(filePath, jsonString); // Using async version

            // Verify the file exists before sharing
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The report file was not created.", filePath);
            }

            // Share the file
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Share Course Report",
                File = new ShareFile(filePath)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to export report: {ex.Message}", "OK");
        }
    }
}
using TermTrackerApp.Views;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using TermTrackerApp.Core.Services;

namespace TermTrackerApp
{
    public partial class App : Application
    {
        private readonly IDatabaseService _databaseService;
        private readonly AuthService _authService;


        public App(IDatabaseService databaseService, AuthService authService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _authService = authService;


            //var termListPage = new TermList();
            var loginPage = new LoginPage(_databaseService, _authService);
   
            MainPage = loginPage;
            

            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;


        }

        

        private void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            if (e.IsDismissed)
            {

                return;
            }
            if (e.IsTapped)
            {
                // Dismiss Notification
                return;
            }

        }
    }
}

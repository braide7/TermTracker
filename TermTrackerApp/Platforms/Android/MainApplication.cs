using Android.App;
using Android.Runtime;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification;

namespace TermTrackerApp.Platforms.Android
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(nint handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public async override void OnCreate()
        {
            base.OnCreate();

            // Create a notification channel for Android 8.0+
            var channel = new NotificationChannelRequest
            {
                Id = "TermTrackerChannel", // Unique ID for the channel
                Name = "Term Tracker Notifications", // User-visible name
                Description = "Notifications for term tracking events", // User-visible description
                Importance = Plugin.LocalNotification.AndroidOption.AndroidImportance.High // Priority level (High triggers sound/vibration)
            };
            var channelList = new List<NotificationChannelRequest>
            {
                channel
            };
            LocalNotificationCenter.CreateNotificationChannels(channelList);
        }
    }
}

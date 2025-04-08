using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using TermTrackerApp.Core.Services;

namespace TermTrackerApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "Terms.db");

            builder.Services.AddSingleton<IDatabaseService>(s => new DatabaseService(dbPath));
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            builder.Services.AddSingleton<AuthService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

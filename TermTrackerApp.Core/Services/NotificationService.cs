using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TermTrackerApp.Core.Models;

namespace TermTrackerApp.Core.Services
{
    public class NotificationService : INotifyService
    {
        private readonly ILocalNotificationCenter _notificationCenter;

        public NotificationService(ILocalNotificationCenter notificationCenter = null)
        {
            _notificationCenter = notificationCenter ?? new LocalNotificationCenterAdapter();
        }

        public virtual async Task SetNotification(DateTime date, int id, string title, string description)
        {
            DateTime now = DateTime.Now.AddSeconds(10);
            DateTime notify;
            if (date.Date == now.Date)
            {
                notify = new DateTime
                (
                        date.Date.Year,
                        date.Date.Month,
                        date.Date.Day,
                        now.Hour,
                        now.Minute,
                        now.Second
                    );
            }
            else
            {
                notify = new DateTime
                    (
                        date.Date.Year,
                        date.Date.Month,
                        date.Date.Day,
                        7,
                        0,
                        0
                    );
            }
            var notification = new NotificationRequest
            {
                NotificationId = id,
                Title = title,
                Description = description,
                ReturningData = "Test data",
                Schedule =
                {
                NotifyTime = notify
                },
                Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions 
                {
                    ChannelId = "TermTrackerChannel", // Use the same channel ID as created in MainApplication
                }
            };
            await _notificationCenter.Show(notification);
        }

        public virtual async Task CancelNotification(int id)
        {
            var pendingNotifications = await _notificationCenter.GetPendingNotificationList();
            var notification = pendingNotifications.FirstOrDefault(n => n.NotificationId == id);

            if (notification != null)
            {
                bool wasCanceled = await Task.Run(() => _notificationCenter.Cancel(notification.NotificationId));
            }
        }
    }

    public class LocalNotificationCenterAdapter : ILocalNotificationCenter
    {
        public async Task<IList<NotificationRequest>> GetPendingNotificationList()
        {
            return await LocalNotificationCenter.Current.GetPendingNotificationList();
        }

        public async Task<bool> Show(NotificationRequest request)
        {
            return await LocalNotificationCenter.Current.Show(request);
        }

        public Task<bool> Cancel(int notificationId)
        {
            return Task.FromResult(LocalNotificationCenter.Current.Cancel(notificationId));
        }
    }

}

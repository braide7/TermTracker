using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermTrackerApp.Core.Services
{
    public interface ILocalNotificationCenter
    {
        Task<IList<NotificationRequest>> GetPendingNotificationList();
        Task<bool> Show(NotificationRequest request);
        Task<bool> Cancel(int notificationId);
    }
}

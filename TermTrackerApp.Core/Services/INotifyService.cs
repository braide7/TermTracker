using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermTrackerApp.Core.Services
{
    public interface INotifyService
    {
        Task SetNotification(DateTime date, int id, string title, string description);
        Task CancelNotification(int id);
    }
}

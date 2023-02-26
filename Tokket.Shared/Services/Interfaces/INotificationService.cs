using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models.Notification;
using Tokket.Core;
using Tokket.Core.Tools;

namespace Tokket.Shared.Services.Interfaces
{
    public interface INotificationService
    {
        Task<TokkepediaNotificationSet> GetNotificationsAsync(string id, NotficationQueryValues values = null);
        Task<bool> MarkAsReadAsync(string id, string pk);
        Task<bool> RemoveNotificationsAsync(string id, string pk);
        Task<ResultData<TokkepediaNotificationNew>> GetNotif(string id);
    }
}

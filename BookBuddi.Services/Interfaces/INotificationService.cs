using BookBuddi.Services.ServiceModels;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Services.Interfaces
{
    public interface INotificationService
    {
        IEnumerable<NotificationViewModel> GetAllNotifications();
        NotificationViewModel? GetNotificationById(int notificationId);
        IEnumerable<NotificationViewModel> GetNotificationsByMember(int memberId);
        IEnumerable<NotificationViewModel> GetUnreadNotificationsByMember(int memberId);
        int GetUnreadCount(int memberId);
        void AddNotification(NotificationViewModel model, string createdBy);
        void MarkAsRead(int notificationId);
        void MarkAllAsRead(int memberId);
        void CreateDueReminderNotification(int memberId, int transactionId, DateTime dueDate);
        void CreateOverdueAlertNotification(int memberId, int transactionId);
        void CreateFineIssuedNotification(int memberId, int fineId, decimal amount);
        void CreateRequestUpdateNotification(int memberId, int requestId, RequestStatus newStatus);
    }
}

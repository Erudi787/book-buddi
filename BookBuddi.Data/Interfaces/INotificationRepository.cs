using BookBuddi.Data.Models;

namespace BookBuddi.Data.Interfaces
{
    public interface INotificationRepository
    {
        IQueryable<Notification> GetNotifications();
        Notification? GetNotificationById(int notificationId);
        IEnumerable<Notification> GetNotificationsByMember(int memberId);
        IEnumerable<Notification> GetUnreadNotificationsByMember(int memberId);
        int GetUnreadCount(int memberId);
        void AddNotification(Notification notification);
        void UpdateNotification(Notification notification);
        void DeleteNotification(Notification notification);
        void MarkAsRead(int notificationId);
        void MarkAllAsRead(int memberId);
    }
}

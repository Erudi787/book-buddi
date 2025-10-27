using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetAllNotificationsAsync();
        Task<Notification?> GetNotificationByIdAsync(int notificationId);
        Task<IEnumerable<Notification>> GetNotificationsByMemberIdAsync(int memberId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsByMemberIdAsync(int memberId);
        Task<IEnumerable<Notification>> GetNotificationsByTypeAsync(NotificationType type);
        Task<int> GetUnreadCountForMemberAsync(int memberId);

        Task<Notification> AddNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(int notificationId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(int memberId);
    }
}
using BookBuddi.Interfaces;
using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Services
{
    public class NotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IEnumerable<Notification>> GetMemberNotificationsAsync(int memberId)
        {
            return await _notificationRepository.GetNotificationsByMemberIdAsync(memberId);
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int memberId)
        {
            return await _notificationRepository.GetUnreadNotificationsByMemberIdAsync(memberId);
        }

        public async Task<int> GetUnreadCountAsync(int memberId)
        {
            return await _notificationRepository.GetUnreadCountForMemberAsync(memberId);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            await _notificationRepository.MarkAsReadAsync(notificationId);
        }

        public async Task MarkAllAsReadAsync(int memberId)
        {
            await _notificationRepository.MarkAllAsReadAsync(memberId);
        }

        public async Task<Notification> CreateNotificationAsync(
            int memberId,
            NotificationType type,
            string title,
            string message,
            int? relatedEntityId = null,
            string? relatedEntityType = null)
        {
            var notification = new Notification
            {
                MemberId = memberId,
                Type = type,
                Title = title,
                Message = message,
                RelatedEntityId = relatedEntityId,
                RelatedEntityType = relatedEntityType,
                DateCreated = DateTime.Now,
                IsRead = false
            };

            return await _notificationRepository.AddNotificationAsync(notification);
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            await _notificationRepository.DeleteNotificationAsync(notificationId);
        }
    }
}

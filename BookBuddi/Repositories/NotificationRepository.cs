using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using BookBuddi.Interfaces;
using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await _context.Notifications
                .Include(n => n.Member)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();
        }

        public async Task<Notification?> GetNotificationByIdAsync(int notificationId)
        {
            return await _context.Notifications
                .Include(n => n.Member)
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByMemberIdAsync(int memberId)
        {
            return await _context.Notifications
                .Where(n => n.MemberId == memberId)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsByMemberIdAsync(int memberId)
        {
            return await _context.Notifications
                .Where(n => n.MemberId == memberId && !n.IsRead)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByTypeAsync(NotificationType type)
        {
            return await _context.Notifications
                .Include(n => n.Member)
                .Where(n => n.Type == type)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountForMemberAsync(int memberId)
        {
            return await _context.Notifications
                .CountAsync(n => n.MemberId == memberId && !n.IsRead);
        }

        public async Task<Notification> AddNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.DateRead = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int memberId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.MemberId == memberId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.DateRead = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
    }
}

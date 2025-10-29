using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Data.Repositories
{
    public class NotificationRepository : BaseRepository, INotificationRepository
    {
        public NotificationRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Notification> GetNotifications()
        {
            return this.GetDbSet<Notification>().OrderByDescending(n => n.DateCreated);
        }

        public Notification? GetNotificationById(int notificationId)
        {
            return this.GetDbSet<Notification>()
                .FirstOrDefault(n => n.NotificationId == notificationId);
        }

        public IEnumerable<Notification> GetNotificationsByMember(int memberId)
        {
            return this.GetDbSet<Notification>()
                .Where(n => n.MemberId == memberId)
                .OrderByDescending(n => n.DateCreated)
                .ToList();
        }

        public IEnumerable<Notification> GetUnreadNotificationsByMember(int memberId)
        {
            return this.GetDbSet<Notification>()
                .Where(n => n.MemberId == memberId && !n.IsRead)
                .OrderByDescending(n => n.DateCreated)
                .ToList();
        }

        public int GetUnreadCount(int memberId)
        {
            return this.GetDbSet<Notification>()
                .Count(n => n.MemberId == memberId && !n.IsRead);
        }

        public void AddNotification(Notification notification)
        {
            this.GetDbSet<Notification>().Add(notification);
            UnitOfWork.SaveChanges();
        }

        public void UpdateNotification(Notification notification)
        {
            SetEntityState(notification, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }

        public void DeleteNotification(Notification notification)
        {
            this.GetDbSet<Notification>().Remove(notification);
            UnitOfWork.SaveChanges();
        }

        public void MarkAsRead(int notificationId)
        {
            var notification = GetNotificationById(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.DateRead = DateTime.Now;
                UpdateNotification(notification);
            }
        }

        public void MarkAllAsRead(int memberId)
        {
            var notifications = GetUnreadNotificationsByMember(memberId);
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.DateRead = DateTime.Now;
                SetEntityState(notification, EntityState.Modified);
            }
            UnitOfWork.SaveChanges();
        }
    }
}

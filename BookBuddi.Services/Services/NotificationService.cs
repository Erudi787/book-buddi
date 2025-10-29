using AutoMapper;
using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public IEnumerable<NotificationViewModel> GetAllNotifications()
        {
            var notifications = _notificationRepository.GetNotifications().ToList();
            return _mapper.Map<IEnumerable<NotificationViewModel>>(notifications);
        }

        public NotificationViewModel? GetNotificationById(int notificationId)
        {
            var notification = _notificationRepository.GetNotificationById(notificationId);
            return notification != null ? _mapper.Map<NotificationViewModel>(notification) : null;
        }

        public IEnumerable<NotificationViewModel> GetNotificationsByMember(int memberId)
        {
            var notifications = _notificationRepository.GetNotificationsByMember(memberId);
            return _mapper.Map<IEnumerable<NotificationViewModel>>(notifications);
        }

        public IEnumerable<NotificationViewModel> GetUnreadNotificationsByMember(int memberId)
        {
            var notifications = _notificationRepository.GetUnreadNotificationsByMember(memberId);
            return _mapper.Map<IEnumerable<NotificationViewModel>>(notifications);
        }

        public int GetUnreadCount(int memberId)
        {
            return _notificationRepository.GetUnreadCount(memberId);
        }

        public void AddNotification(NotificationViewModel model, string createdBy)
        {
            var notification = _mapper.Map<Notification>(model);
            notification.DateCreated = DateTime.Now;
            notification.IsRead = false;
            notification.CreatedBy = createdBy;
            notification.CreatedTime = DateTime.Now;
            notification.UpdatedBy = createdBy;
            notification.UpdatedTime = DateTime.Now;

            _notificationRepository.AddNotification(notification);
        }

        public void MarkAsRead(int notificationId)
        {
            _notificationRepository.MarkAsRead(notificationId);
        }

        public void MarkAllAsRead(int memberId)
        {
            _notificationRepository.MarkAllAsRead(memberId);
        }

        public void CreateDueReminderNotification(int memberId, int transactionId, DateTime dueDate)
        {
            var notification = new Notification
            {
                MemberId = memberId,
                Type = NotificationType.DueReminder,
                Title = "Book Due Date Reminder",
                Message = $"Your borrowed book is due on {dueDate:MMM dd, yyyy}. Please return it on time to avoid fines.",
                DateCreated = DateTime.Now,
                IsRead = false,
                RelatedEntityId = transactionId,
                RelatedEntityType = "Transaction",
                CreatedBy = "System",
                CreatedTime = DateTime.Now,
                UpdatedBy = "System",
                UpdatedTime = DateTime.Now
            };

            _notificationRepository.AddNotification(notification);
        }

        public void CreateOverdueAlertNotification(int memberId, int transactionId)
        {
            var notification = new Notification
            {
                MemberId = memberId,
                Type = NotificationType.OverdueAlert,
                Title = "Overdue Book Alert",
                Message = "You have an overdue book. Please return it as soon as possible to avoid additional fines.",
                DateCreated = DateTime.Now,
                IsRead = false,
                RelatedEntityId = transactionId,
                RelatedEntityType = "Transaction",
                CreatedBy = "System",
                CreatedTime = DateTime.Now,
                UpdatedBy = "System",
                UpdatedTime = DateTime.Now
            };

            _notificationRepository.AddNotification(notification);
        }

        public void CreateFineIssuedNotification(int memberId, int fineId, decimal amount)
        {
            var notification = new Notification
            {
                MemberId = memberId,
                Type = NotificationType.FineIssued,
                Title = "Fine Issued",
                Message = $"A fine of ${amount:F2} has been issued to your account for an overdue book. Please pay it at your earliest convenience.",
                DateCreated = DateTime.Now,
                IsRead = false,
                RelatedEntityId = fineId,
                RelatedEntityType = "Fine",
                CreatedBy = "System",
                CreatedTime = DateTime.Now,
                UpdatedBy = "System",
                UpdatedTime = DateTime.Now
            };

            _notificationRepository.AddNotification(notification);
        }

        public void CreateRequestUpdateNotification(int memberId, int requestId, RequestStatus newStatus)
        {
            var statusText = newStatus switch
            {
                RequestStatus.Approved => "approved",
                RequestStatus.Rejected => "rejected",
                RequestStatus.Fulfilled => "fulfilled",
                _ => "updated"
            };

            var notification = new Notification
            {
                MemberId = memberId,
                Type = NotificationType.RequestUpdate,
                Title = "Book Request Update",
                Message = $"Your book request has been {statusText}.",
                DateCreated = DateTime.Now,
                IsRead = false,
                RelatedEntityId = requestId,
                RelatedEntityType = "BookRequest",
                CreatedBy = "System",
                CreatedTime = DateTime.Now,
                UpdatedBy = "System",
                UpdatedTime = DateTime.Now
            };

            _notificationRepository.AddNotification(notification);
        }
    }
}

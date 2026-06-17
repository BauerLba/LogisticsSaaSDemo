namespace LogisticsSaaS.Core.Application.Services;

public record Notification(
    Guid Id,
    string Title,
    string Message,
    string Type,
    DateTime CreatedAt,
    bool IsRead = false,
    string? ActionUrl = null
);

public class NotificationService
{
    private static Dictionary<string, List<Notification>> _userNotifications = new();
    private static int _maxPerUser = 50;

    public void AddNotification(string userId, string title, string message, string type = "info", string? actionUrl = null)
    {
        if (!_userNotifications.ContainsKey(userId))
        {
            _userNotifications[userId] = new List<Notification>();
        }

        var notification = new Notification(
            Guid.NewGuid(),
            title,
            message,
            type,
            DateTime.UtcNow,
            false,
            actionUrl
        );

        _userNotifications[userId].Insert(0, notification);

        if (_userNotifications[userId].Count > _maxPerUser)
        {
            _userNotifications[userId] = _userNotifications[userId].Take(_maxPerUser).ToList();
        }
    }

    public IEnumerable<Notification> GetNotifications(string userId, int limit = 20, bool unreadOnly = false)
    {
        if (!_userNotifications.ContainsKey(userId))
            return Enumerable.Empty<Notification>();

        var notifications = _userNotifications[userId];

        if (unreadOnly)
        {
            notifications = notifications.Where(n => !n.IsRead).ToList();
        }

        return notifications.Take(limit);
    }

    public int GetUnreadCount(string userId)
    {
        if (!_userNotifications.ContainsKey(userId))
            return 0;

        return _userNotifications[userId].Count(n => !n.IsRead);
    }

    public void MarkAsRead(string userId, Guid notificationId)
    {
        if (!_userNotifications.ContainsKey(userId))
            return;

        var notification = _userNotifications[userId].FirstOrDefault(n => n.Id == notificationId);
        if (notification != null)
        {
            var index = _userNotifications[userId].IndexOf(notification);
            _userNotifications[userId][index] = notification with { IsRead = true };
        }
    }

    public void MarkAllAsRead(string userId)
    {
        if (!_userNotifications.ContainsKey(userId))
            return;

        _userNotifications[userId] = _userNotifications[userId]
            .Select(n => n with { IsRead = true })
            .ToList();
    }

    public void DeleteNotification(string userId, Guid notificationId)
    {
        if (!_userNotifications.ContainsKey(userId))
            return;

        _userNotifications[userId].RemoveAll(n => n.Id == notificationId);
    }

    public void ClearAll(string userId)
    {
        if (_userNotifications.ContainsKey(userId))
        {
            _userNotifications[userId].Clear();
        }
    }
}

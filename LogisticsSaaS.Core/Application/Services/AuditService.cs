namespace LogisticsSaaS.Core.Application.Services;

public record AuditLog(
    Guid Id,
    DateTime Timestamp,
    string Action,
    string EntityType,
    string? UserId,
    string? Changes
);

public class AuditService
{
    private static List<AuditLog> _logs = new();

    public void Log(string action, string entityType, string? userId = null, string? changes = null)
    {
        var log = new AuditLog(
            Guid.NewGuid(),
            DateTime.UtcNow,
            action,
            entityType,
            userId,
            changes
        );
        _logs.Add(log);

        if (_logs.Count > 1000)
        {
            _logs = _logs.TakeLast(500).ToList();
        }
    }

    public IEnumerable<AuditLog> GetLogs(int limit = 100)
    {
        return _logs.OrderByDescending(l => l.Timestamp).Take(limit);
    }

    public IEnumerable<AuditLog> GetLogsByEntity(string entityType, int limit = 50)
    {
        return _logs
            .Where(l => l.EntityType == entityType)
            .OrderByDescending(l => l.Timestamp)
            .Take(limit);
    }

    public IEnumerable<AuditLog> GetLogsByAction(string action, int limit = 50)
    {
        return _logs
            .Where(l => l.Action == action)
            .OrderByDescending(l => l.Timestamp)
            .Take(limit);
    }

    public void ClearOldLogs(int daysOld = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
        _logs = _logs.Where(l => l.Timestamp > cutoffDate).ToList();
    }
}

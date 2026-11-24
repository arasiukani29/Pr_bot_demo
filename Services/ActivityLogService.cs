namespace DummyLoginApi.Services;

using System.Collections.Concurrent;
using DummyLoginApi.Models;

public interface IActivityLogService
{
    IReadOnlyList<ActivityLogResponseDto> GetAll(string? entityName = null, string? severity = null);
    ActivityLogResponseDto? GetById(Guid id);
    ActivityLogResponseDto Log(CreateActivityLogRequestDto dto, string? ipAddress = null);
    int ClearOlderThan(DateTime cutoff);
}

public class ActivityLogService : IActivityLogService
{
    private static readonly ConcurrentDictionary<Guid, ActivityLogItem> _logs = new();

    public IReadOnlyList<ActivityLogResponseDto> GetAll(string? entityName = null, string? severity = null)
    {
        var query = _logs.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(entityName))
        {
            query = query.Where(l => string.Equals(l.EntityName, entityName.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(severity))
        {
            query = query.Where(l => string.Equals(l.Severity, severity.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderByDescending(l => l.Timestamp)
                    .Select(MapToDto)
                    .ToList();
    }

    public ActivityLogResponseDto? GetById(Guid id)
    {
        if (_logs.TryGetValue(id, out var log))
        {
            return MapToDto(log);
        }

        return null;
    }

    public ActivityLogResponseDto Log(CreateActivityLogRequestDto dto, string? ipAddress = null)
    {
        var item = new ActivityLogItem
        {
            Id = Guid.NewGuid(),
            Action = dto.Action.Trim(),
            EntityName = dto.EntityName.Trim(),
            EntityId = dto.EntityId?.Trim(),
            Details = dto.Details?.Trim(),
            Severity = string.IsNullOrWhiteSpace(dto.Severity) ? "Info" : dto.Severity.Trim(),
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow
        };

        _logs[item.Id] = item;
        return MapToDto(item);
    }

    public int ClearOlderThan(DateTime cutoff)
    {
        var keysToRemove = _logs.Values
            .Where(l => l.Timestamp < cutoff)
            .Select(l => l.Id)
            .ToList();

        var removedCount = 0;
        foreach (var key in keysToRemove)
        {
            if (_logs.TryRemove(key, out _))
            {
                removedCount++;
            }
        }

        return removedCount;
    }

    private static ActivityLogResponseDto MapToDto(ActivityLogItem item) => new()
    {
        Id = item.Id,
        Action = item.Action,
        EntityName = item.EntityName,
        EntityId = item.EntityId,
        Details = item.Details,
        Severity = item.Severity,
        IpAddress = item.IpAddress,
        Timestamp = item.Timestamp
    };
}
